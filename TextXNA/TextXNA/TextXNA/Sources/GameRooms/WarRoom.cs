using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Surface;
using Microsoft.Surface.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using TestXNA.Sources;
using TestXNA.Sources.GameData;
using TestXNA.Sources.DialogBoxes;
using TestXNA.Sources.NodeJSClient;
using TestXNA.Sources.NodeJSClient.MessageData;



namespace TestXNA.Sources.GameRooms
{
    class WarRoom : IRoom
    {


        //My data types
        private class PopupStr
        {
            public PopupStr() { }

            public Vector2 position;
            public float scale;
            public float alpha;
            public float livedTime;
            public float angleInRadians;
        }


        //Useful Tag Values :
        //      C0 = User Position : for test
        //      C1 = Arrow Target
        //      C2 = Arrow Start
        //      D0 <-> D7   Commander Tags

        #region variables

        private Texture2D _popupImage;
        private Texture2D _iconImage;
        private Texture2D _arrowImage;
        private Texture2D _mapBackground;
        private Texture2D _mapOverlay;
        private Texture2D _UIBackground;
        private Texture2D _commanderHighlight;
        private Texture2D _commanderImage;
        private List<Texture2D> _mapZones = null;

        private int _zonesCount = 0;
        private long _firstTagValue = 0xD0;

        private List<PopupStr> _displayedPopups;
        private float _popupLifeDuration = 2f;

        private List<Arrow> _directionArrows;

        private List<SmallPlayerUI> _miniUIs;

        private List<RadialUIContainer> _radialUIs;

        private Vector2 _userPosition;

        private TestXNA.Map _map;

        private DialogBox _currentDialog;
        private TargetedDialogBox _pickZoneDialog;
        private ButtonDialogBox _commanderInstructionsDialog;

        private List<Commander> _commanders;

        private bool isC5 = false;

        private System.Action<float> _updateAction;


        //Pick zone for players data

        private PickZoneData _pickZoneData;

        public class PickZoneData
        {
            public PickZoneData()
            {
                zonePicked = new Dictionary<int, List<int>> ();
            }

            public int currentIndex;
            public Dictionary<int, List<int> > zonePicked;
        }

        #endregion

        public WarRoom()
        {
            initialize();
        }


        #region IRoom
        
        public void update(float dt)
        {
            if (_updateAction != null)
            {
                _updateAction(dt);
            }
        }

        public void draw()
        {
            drawMap();
            if (_currentDialog != null)
            {
                _currentDialog.draw();
            }
        }

        #endregion

        #region phase update methods

        #region Pick Zone Phase

        /// <summary>
        /// Handle the picking of new zones by the players
        /// </summary>
        /// <param name="players"> An ordonated list of the players </param>
        /// <param name="nbOfZones"> An ordonated list (same order as the players') of the bumber of zones each player can pick</param>
        /// <param name="dt"></param>
        public void pickZonesForPLayers(List<int> players, List<int> nbOfZones, float dt)
        {
            if (_pickZoneData == null)
            {
                _pickZoneData = new PickZoneData();
                _pickZoneData.currentIndex = 0;

                //init for first player

                //display dialog
                initPickZoneDialog(players[_pickZoneData.currentIndex],
                    nbOfZones[_pickZoneData.currentIndex]);
           }

            if (pickZonesForPlayer(players[_pickZoneData.currentIndex]
                , nbOfZones[_pickZoneData.currentIndex], dt))
            {
                Console.WriteLine("\nzones captured for player : " + players[_pickZoneData.currentIndex] + "\n");
                //send added territories for this player
                ServerCom.Instance.Socket.Emit("capturedTerritories", new
                {
                    gameID = players[_pickZoneData.currentIndex]
                    , territories = _pickZoneData.zonePicked[players[_pickZoneData.currentIndex]]
                });

                //move to next player
                ++_pickZoneData.currentIndex;

                
                //re display dialog for new player
                if(_pickZoneData.currentIndex < players.Count)
                {
                    if (nbOfZones[_pickZoneData.currentIndex] == 0)
                    {
						//TODO send empty capturedTerritories message
						ServerCom.Instance.Socket.Emit("capturedTerritories", new
						                               {
							gameID = players[_pickZoneData.currentIndex]
							, territories = new List<int>()
						});

                        ++_pickZoneData.currentIndex;
                    }
                    else
                    {
                        initPickZoneDialog(players[_pickZoneData.currentIndex],
                            nbOfZones[_pickZoneData.currentIndex]);
                    }
                }

                //all players have picked zones
                if (_pickZoneData.currentIndex >= players.Count)
                {
                    _pickZoneData = null;
                    //send message to server
                    _currentDialog = null;
                    //_updateAction = oldUpdate;
                    //_updateAction = placeCommanderUpdate;
                    Console.WriteLine("\nsetting update method to empty\n");
                    _updateAction = emptyUpdate;
                }
            }

        }

        /// <summary>
        /// Process zone pick for player
        /// Return true when all zone have bee picked for this player
        /// </summary>
        /// <param name="player">the player currently picking</param>
        /// <param name="nbOfZones">the number of zones this player is allowed to pick</param>
        /// <param name="dt"></param>
        /// <returns></returns>
        private bool pickZonesForPlayer(int player, int nbOfZones, float dt)
        {
            updateDialogBox(dt);

            if (_currentDialog.IsShown)
            {
                _pickZoneData.zonePicked[player] = new List<int>();
                return false;
            }

            ReadOnlyTouchPointCollection touches = MyGame.TouchTarget.GetState();

            foreach (TouchPoint touch in touches)
            {
                Vector2 touchVec = Utils.touchPointToV2(touch);

                int touchedZone = _map.getZoneAt(touchVec);

                if (canPickZone(touchedZone) && _pickZoneData.zonePicked[player].Count < nbOfZones)
                {
                    //The server wants zone from 0 to 15
                    _pickZoneData.zonePicked[player].Add(touchedZone - 1);
                    _map.setZoneOwner(touchedZone, player);

                    if (_pickZoneData.zonePicked[player].Count == nbOfZones)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void initPickZoneDialog(int player, int nbOfZone)
        {
            UIElements.SimpleButton button = new UIElements.SimpleButton(
            _UIBackground,
            new Rectangle(0, 0, 100, 50),
            "OK");

            Rectangle boxArea = new Rectangle(0, 0, 300, 300);

            _pickZoneDialog = new DialogBoxes.TargetedDialogBox(
                _miniUIs[player], button, 0.8f,
                PlayerData.Instance[player].Name + "\n"
                + "Please, pick " + nbOfZone + " free territories",
                boxArea);

            _pickZoneDialog.Position = MyGame.ScreenCenter;
            _pickZoneDialog.Show();

            _currentDialog = _pickZoneDialog;
        }

        private bool canPickZone(int zone)
        {
            return _map.getZoneOwner(zone) == -1;
        }
        
        #endregion

        #region Place Commanders

        //Wait for all commanders (2 for each player) to be placed on the table, 5 seconds skippable countdown after that
        private void placeCommanderUpdate(float dt)
        {
            if(_currentDialog == null)
            {
                //display popup
                initCommandersInstructionsDialog();
            }

            if (_currentDialog.IsShown)
            {
                updateDialogBox(dt);
                return;
            }

            ReadOnlyTouchPointCollection touches = MyGame.TouchTarget.GetState();
            foreach (TouchPoint touch in touches)
            {
                if (touch.IsTagRecognized && touch.Tag.Value >= _firstTagValue && touch.Tag.Value < _firstTagValue + MyGame.NUMBER_OF_PLAYER * 2)
                {
                    //if tag is a commander tag, add or update the commander
                    int owner = getOwnerForTag(touch.Tag.Value);
                    //check if the tag is in the right zone
                    int zone = _map.getZoneAt(Utils.touchPointToV2(touch));
                    if (zone >= 0 && _map.getZoneOwner(zone) == owner)
                    {
                        addCommanderTag(touch, owner);
                    }
                }
            }

            if (areCommandersReady())
            {
                //tell the server this phase is over
                ServerCom.Instance.sendSimpleMessage("commandersPlaced");
                //switch update method
                _updateAction = oldUpdate;
            }

            updateCommanders(dt);
        }

        private void initCommandersInstructionsDialog()
        {
            UIElements.SimpleButton button = new UIElements.SimpleButton(
            _UIBackground,
            new Rectangle((int)MyGame.ScreenCenter.X - 300, (int)MyGame.ScreenCenter.Y + MyGame.ScreenArea.Height / 4
                , 600, 100),
            "OK, and stop bothering  with messages !");

            Rectangle boxArea = new Rectangle(0, 0, 300, 300);

            _commanderInstructionsDialog = new DialogBoxes.ButtonDialogBox(
                button,
                "Place commanders & stuff",
                boxArea);

            _commanderInstructionsDialog.Position = MyGame.ScreenCenter;
            _commanderInstructionsDialog.Show();

            _currentDialog = _commanderInstructionsDialog;
        }

        /// <summary>
        /// Check if all of each player commanders have been placed
        /// </summary>
        /// <returns></returns>
        private bool areCommandersReady()
        {
            for (int owner = 0; owner < MyGame.NUMBER_OF_PLAYER; ++owner)
            {
                int commandersNb = 0;
                foreach (Commander command in _commanders)
                {
                    if (command.Owner == owner)
                    {
                        ++commandersNb;
                    }
                }

                if (commandersNb != 2)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Return the woner for this tag, between 0 and 3
        /// </summary>
        /// <param name="tagValue"></param>
        /// <returns></returns>
        private int getOwnerForTag(long tagValue)
        {
            return (int)(tagValue - _firstTagValue) / 2;
        }

        private void addCommanderTag(TouchPoint touch, int owner)
        {
            foreach (Commander commander in _commanders)
            {
                if (commander.TagValue == touch.Tag.Value)
                {
                    commander.Position = Utils.touchPointToV2(touch);
                    return; //Our work here is done
                }
            }

            //if the tag does not already exist
            Commander command = new Commander(_commanderHighlight);
            command.Position = Utils.touchPointToV2(touch);
            command.Owner = owner;
            command.TagValue = touch.Tag.Value;

            _commanders.Add(command);
        }

        #endregion

        #region Play Turn

        //Wait for the player X to make a move
        //Scan for other players moves
        //Scan for illegual player moves

        #endregion

        private void onCaptureTerritories(SocketIOClient.Messages.IMessage data)
        {
            Console.WriteLine("\n oncaptureteritories \n" + data.Json.ToJsonString());
            //Switch update to pick zone
            List<int> players;
            List<int> nbZones = new List<int>() { 2, 1, 1, 0 };
            CaptureInfo capInfo;
            CaptureInfoRoot obj = Newtonsoft.Json.JsonConvert.DeserializeObject<CaptureInfoRoot>(data.Json.ToJsonString());
            capInfo = obj.args[0];
            players = capInfo.orderedPlayers;
            _updateAction = delegate(float delta) { pickZonesForPLayers(players, nbZones, delta); };

        }

        private void onPlaceCommander(SocketIOClient.Messages.IMessage data)
        {
            _updateAction = placeCommanderUpdate;
        }

        private void MajPlayerInfo(SocketIOClient.Messages.IMessage data)
        {
            Console.WriteLine("\n \n " + data.Json.ToJsonString() + "\n \n");
            Console.WriteLine("in maj player info");
            PlayerInfo player = null;
            //player = PlayerInfo.Deserialize(message.Json.ToJsonString());//(PlayerInfo)message.Json.GetFirstArgAs<PlayerInfo>();
            PlayerInfoRoot obj = Newtonsoft.Json.JsonConvert.DeserializeObject<PlayerInfoRoot>(data.Json.ToJsonString());
            player = obj.args[0];
            for (int i = 0; i < player.territories.Count; ++i)
            {
                Console.WriteLine(" setting zone : " + (player.territories[i] + 1) + " owner to " + player.gameID);
                _map.setZoneOwner(player.territories[i] + 1, player.gameID);
            }
        }

        private void emptyUpdate(float dt)
        {
        }

        public void oldUpdate(float dt)
        {
            // TODO: Process touches, 
            // use the following code to get the state of all current touch points.
            ReadOnlyTouchPointCollection touches = MyGame.TouchTarget.GetState();

            List<Vector2> tagPositions = new List<Vector2>();
            List<TouchPoint> unknownTags = new List<TouchPoint>();

            Vector2 arrowTarget = Vector2.Zero;

            //first pass to record touches && known tags
            foreach (TouchPoint touch in touches)
            {
                bool isUIDrag = checkForUIDrag(touch, dt);


                if (touch.IsTagRecognized)
                {
                    //C0 tag = user position test
                    if (touch.Tag.Value == 0xC0)
                    {
                        _userPosition = new Vector2(touch.X, touch.Y);
                    }
                    //C1 tag = arrow Target test
                    else if (touch.Tag.Value == 0xC1)
                    {
                        arrowTarget = new Vector2(touch.X, touch.Y);
                    }
                    //C2 tag = arrow start test
                    else if (touch.Tag.Value == 0xC2)
                    {
                        unknownTags.Add(touch);
                    }
                    //Commander tag
                    /* else if (touch.Tag.Value >= 0xD0 && touch.Tag.Value <= 0xD7)
                    {
                        addCommanderTag(touch);
                    }
                    */
                    //TODO remove this
                    else if (touch.Tag.Value >= 0x0)
                    {
                        List<int> players = new List<int>() { 2, 1, 3, 0 };
                        List<int> nbZones = new List<int>() { 4, 2, 2, 1 };
                        _updateAction = delegate(float delta) { pickZonesForPLayers(players, nbZones, delta); };

                    }
                }
                else
                {
                    bool isPopup = true;
                    //Check if an UI is beeing moved
                    isPopup = !isUIDrag;

                    foreach (RadialUIContainer cont in _radialUIs.Reverse<RadialUIContainer>())
                    {
                        if (cont.processTouch(touch, dt))
                        {
                            isPopup = false;
                            break;
                        }
                    }

                    if (isPopup)
                    {

                        //Add popup
                        PopupStr newPopup = new PopupStr();
                        newPopup.position = new Vector2(touch.X, touch.Y);
                        newPopup.scale = 0f;
                        newPopup.alpha = 1f;
                        newPopup.livedTime = 0f;
                        newPopup.angleInRadians = Utils.lookAt(newPopup.position, _userPosition);

                        _displayedPopups.Add(newPopup);
                    }
                }
            }

            // TODO: Add your update logic here

            updateArrows(unknownTags, arrowTarget, dt);
            updatePopups(dt);
            updatePlayersUI(dt);
            updateRadialUIs(dt);
            updateCommanders(dt);
            updateDialogBox(dt);

        }

        #endregion

        private void initialize()
        {

            NodeJSClient.ServerCom.Instance.majPlayerInfoCB = MajPlayerInfo;
            NodeJSClient.ServerCom.Instance.captureZonesCB = onCaptureTerritories;
            NodeJSClient.ServerCom.Instance.placeCommandersCB= onPlaceCommander;
 
            //My initialization logic
            _displayedPopups = new List<WarRoom.PopupStr>();
            _directionArrows = new List<Arrow>();
            _commanders = new List<Commander>();

            _userPosition = new Vector2(MyGame.ScreenCenter.X, MyGame.ScreenArea.Height - 100);

            _map = new TestXNA.Map(MyGame.SURFACE_WIDTH, MyGame.SURFACE_HEIGHT);
            _map.loadMap();

            GraphicsDevice GraphicsDevice = MyGame.SpriteBatch.GraphicsDevice;

            _iconImage = MyGame.ContentManager.Load<Texture2D>("Images/GameThumbnail");
            _popupImage = MyGame.ContentManager.Load<Texture2D>("Images/trollFace");
            _arrowImage = MyGame.ContentManager.Load<Texture2D>("Images/Arrow");
            _mapBackground = MyGame.ContentManager.Load<Texture2D>("Images/Map");
            _mapOverlay = MyGame.ContentManager.Load<Texture2D>("Images/MapOverlay");
            _UIBackground = MyGame.ContentManager.Load<Texture2D>("Images/UIBack");


            _commanderHighlight = MyGame.ContentManager.Load<Texture2D>("Images/TagHighlight");

            _updateAction = oldUpdate;

            initializePlayerUIs();
            initializeRadialUI();

        }


        private void initializePlayerUIs()
        {
            //hack
            GameData.PlayerData.Instance[0].Name = "Gwenn";
            GameData.PlayerData.Instance[1].Name = "Aurel";
            GameData.PlayerData.Instance[2].Name = "Jerom";
            GameData.PlayerData.Instance[3].Name = "Bastien";

            _miniUIs = new List<SmallPlayerUI>();

            SmallPlayerUI playerUI = new SmallPlayerUI(_UIBackground, new Vector2(50f, 50f), 0);
            _miniUIs.Add(playerUI);

            playerUI = new SmallPlayerUI(_UIBackground, new Vector2(200f, 50f), 1);
            _miniUIs.Add(playerUI);

            playerUI = new SmallPlayerUI(_UIBackground, new Vector2(400f, 50f), 2);
            _miniUIs.Add(playerUI);

            playerUI = new SmallPlayerUI(_UIBackground, new Vector2(600f, 50f), 3);
            _miniUIs.Add(playerUI);

            //now that the font i s;oaded we can compute the size of each UI
            foreach (SmallPlayerUI ui in _miniUIs)
            {
                ui.initializeArea(MyGame.BasicFont);
            }
        }

        private void initializeRadialUI()
        {
            int nbUI = 3;

            _radialUIs = new List<RadialUIContainer>();

            RadialUIContainer baseRadial = new RadialUIContainer(300f, 200f);
            baseRadial.Position = MyGame.ScreenCenter;
            _radialUIs.Add(baseRadial);

            int NbContainer = 0;// 3;

            for (int h = 0; h < NbContainer; ++h)
            {
                RadialUIContainer container = new RadialUIContainer(150f, 100f);
                baseRadial.ContainedUIs.Add(container);
                _radialUIs.Add(container);

                for (int i = 0; i < nbUI; ++i)
                {
                    SimpleRotatableUI ui = new SimpleRotatableUI(_popupImage, "Nb : " + i, MyGame.BasicFont);
                    container.ContainedUIs.Add(ui);
                }
            }
        }

        private bool checkTagC5()
        {
            return isC5;
        }


        private void drawMap()
        {
            MyGame.SpriteBatch.Draw(_mapBackground, MyGame.MapArea, Color.White);

            _map.draw();

            MyGame.SpriteBatch.Draw(_mapOverlay, MyGame.MapArea, Color.White);

            //if (ApplicationServices.WindowAvailability != WindowAvailability.Unavailable)
            {
                //if (ApplicationServices.WindowAvailability == WindowAvailability.Interactive)
                {

                    drawPopups();
                    drawArrows();
                }
            }

            MyGame.SpriteBatch.Draw(_iconImage, _userPosition, Color.White);

            drawPlayersUI();
            drawRadialUIs();

            foreach (Commander commander in _commanders)
            {
                commander.draw();
            }
        }

        private void drawPopups()
        {
            Vector2 popupOrigin = new Vector2(_popupImage.Width / 2, _popupImage.Height / 2);
            //Draw popups
            foreach (PopupStr popup in _displayedPopups)
            {
                Color popupColor = new Color(1f, 1f, 1f, popup.alpha);
                MyGame.SpriteBatch.Draw(_popupImage, popup.position, null, popupColor, popup.angleInRadians, popupOrigin,
                    popup.scale, SpriteEffects.None, 0f);
            }
        }

        private void drawRadialUIs()
        {
            _radialUIs[0].draw();
        }

        private void drawArrows()
        {
            //draw arrows
            foreach (Arrow arrow in _directionArrows)
            {
                arrow.draw();
            }
        }

        private void drawPlayersUI()
        {
            //updatePlayersUI(dt);
            foreach (SmallPlayerUI ui in _miniUIs)
            {
                ui.draw();
            }
        }

        private void updateDialogBox(float dt)
        {
            if (_currentDialog != null)
            {
                _currentDialog.update(dt);

                ReadOnlyTouchPointCollection touches = MyGame.TouchTarget.GetState();

                foreach (TouchPoint touch in touches)
                {
                    _currentDialog.processTouch(touch, dt);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt">time since last frame in seconds</param>
        private void updatePlayersUI(float dt)
        {
            for (int i = 0; i < _miniUIs.Count; ++i)
            {
                SmallPlayerUI ui = _miniUIs[i];

                ui.update(dt);
            }
        }

        private void updateCommanders(float dt)
        {
            /*foreach (Commander commander in _commanders)
            {
                commander.update(dt);
                commander.Angle = Utils.lookAt(commander.Position, _userPosition);
                _map.setZoneOwner(_map.getZoneAt(commander.Position), commander.Owner);
            }*/
        }

        private void updatePopups(float dt)
        {
            List<PopupStr> toRemove = new List<PopupStr>();

            for (int i = 0; i < _displayedPopups.Count; ++i)
            {
                PopupStr popup = _displayedPopups[i];
                if (popup.livedTime >= _popupLifeDuration)
                {
                    toRemove.Add(popup);
                }
                else
                {
                    popup.livedTime = popup.livedTime + dt;
                    popup.alpha = Utils.bumpy(MathHelper.Lerp(1f, 0f, popup.livedTime / _popupLifeDuration));
                    popup.scale = Utils.bumpy(MathHelper.Lerp(0f, 1f, popup.livedTime / _popupLifeDuration));
                }
            }

            foreach (PopupStr removed in toRemove)
            {
                _displayedPopups.Remove(removed);
            }
        }


        private bool checkForUIDrag(TouchPoint touch, float dt)
        {
            foreach (SmallPlayerUI ui in _miniUIs)
            {
                if (ui.processDrag(touch, dt))
                {
                    return true;
                }
            }
            return false;
        }


        private void updateRadialUIs(float dt)
        {
            _radialUIs[0].update(dt);
        }

        /// <summary>
        /// Clean arrows with no tag and update animation timers
        /// </summary>
        private void updateArrows(List<TouchPoint> unknownTags, Vector2 arrowTarget, float dt)
        {
            //reset id checker
            foreach (Arrow arrow in _directionArrows)
            {
                arrow.UpToDate = false;
            }

            foreach (TouchPoint touch in unknownTags)
            {
                if (!lookForArrowWithTag(touch, arrowTarget, dt))
                {
                    Arrow arrow = new Arrow(_arrowImage, new Vector2(touch.X, touch.Y),
                        arrowTarget, touch.Id);

                    _directionArrows.Add(arrow);
                }
            }

            //remove arrow without tag
            foreach (Arrow arrow in _directionArrows.ToArray())
            {
                if (!arrow.UpToDate)
                {
                    _directionArrows.Remove(arrow);
                }
            }
        }

        private bool lookForArrowWithTag(TouchPoint touch, Vector2 newTarget, float dt)
        {
            foreach (Arrow arrow in _directionArrows)
            {
                if (arrow.updateArrowWithTag(touch, newTarget, dt))
                {
                    return true;
                }
            }
            return false;
        }

        public System.Action<float> UpdateAction
        {
            get { return _updateAction; }
            set { _updateAction = value; }
        }
    }
}
