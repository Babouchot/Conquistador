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
        private Texture2D _buttonTexture;
        private Texture2D _commanderHighlight;
        private Texture2D _commanderImage;
        private Texture2D _messageTexture;
        private Texture2D _UIBack;
        private Texture2D _fireSprite;
        private Texture2D _ropeSprite;
        private Texture2D _radialUICenter;
        private Texture2D _playerUIBack;
        private List<Texture2D> _mapZones = null;

        private UIElements.StretchableImage _buttonStretchImage;
        private UIElements.StretchableImage _messageStretchImage;


        private int _zonesCount = 0;
        private long _firstTagValue = 0xE0;

        private List<PopupStr> _displayedPopups;
        private float _popupLifeDuration = 2f;

        private List<SmallPlayerUI> _miniUIs;

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
                zonePicked = new Dictionary<int, List<int>>();
            }

            public int currentIndex;
            public Dictionary<int, List<int>> zonePicked;
        }


        //Question Timeout data

        private float _timeSinceLastQuestion = 0f;
        private float _questionMaxAllowedTime = 20f;
        private int _questionId = 0;
        private UIElements.ProgressBar _progressBar = null;
        private UIElements.AnimatedTexture _animatedFire = null;

        // Answer display Data
        private RadialUIContainer _radialUI;
        private float _timeSinceAnswerDisplayStart;
        private float _answerDisplayDuration = 10f;

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

            if (_progressBar != null)
            {
                _progressBar.draw();
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


            commonUpdate(dt);

            if (pickZonesForPlayer(players[_pickZoneData.currentIndex]
                , nbOfZones[_pickZoneData.currentIndex], dt))
            {
                Console.WriteLine("\nzones captured for player : " + players[_pickZoneData.currentIndex] + "\n"
                    + " captured : " + _pickZoneData.zonePicked[players[_pickZoneData.currentIndex]].Count);
                //send added territories for this player
                ServerCom.Instance.Socket.Emit("capturedTerritories", new
                {
                    gameID = players[_pickZoneData.currentIndex]
                    ,
                    territories = _pickZoneData.zonePicked[players[_pickZoneData.currentIndex]]
                });

                //move to next player
                ++_pickZoneData.currentIndex;


                //re display dialog for new player
                if (_pickZoneData.currentIndex < players.Count)
                {
                    if (nbOfZones[_pickZoneData.currentIndex] == 0)
                    {
                        //TODO send empty capturedTerritories message
                        ServerCom.Instance.Socket.Emit("capturedTerritories", new
                                                       {
                                                           gameID = players[_pickZoneData.currentIndex]
                            ,
                                                           territories = new List<int>()
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

            if (_pickZoneDialog.IsShown)
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
            _buttonStretchImage,
            new Rectangle(0, 0, 150, 90),
            "OK");

            Rectangle boxArea = new Rectangle(0, 0, 400, 600);

            _pickZoneDialog = new DialogBoxes.TargetedDialogBox(
                _miniUIs[player], button, 0.7f,
                PlayerData.Instance[player].Name + "\n"
                + "Please, pick " + nbOfZone + " free territories",
                boxArea
                , _messageStretchImage);

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
            if (_currentDialog == null)
            {
                //display popup
                initCommandersInstructionsDialog();
            }

            commonUpdate(dt);

            if (_currentDialog.IsShown)
            {
                updateDialogBox(dt);
                return;
            }

            KeyboardState keyState = Keyboard.GetState();
            if (keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Enter)
                && keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Space))
            {
                cheatAddCommanders();   
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
                _updateAction = emptyUpdate;
                Console.WriteLine("commanders Placed message sent");
            }

            updateCommanders(dt, true, -1, null);
        }

        private void cheatAddCommanders()
        {
            long lastAddedTag = _firstTagValue;
            Random rand = new Random();

            while (_commanders.Count < MyGame.NUMBER_OF_PLAYER * 2)

            {
                int owner = getOwnerForTag(lastAddedTag);
                int zone = rand.Next(1, 17); //1..16
                if (zone >= 0 && _map.getZoneOwner(zone) == owner)
                {
                    Commander command = new Commander(_commanderHighlight);
                    command.Position = _map.getCenterOfZone(zone);
                    command.CurrentZone = zone;
                    command.Owner = owner;
                    command.TagValue = lastAddedTag;

                    _commanders.Add(command);
                    ++lastAddedTag;
                }
            }
        }

        private void initCommandersInstructionsDialog()
        {

            UIElements.SimpleButton button = new UIElements.SimpleButton(
            _buttonStretchImage,
            new Rectangle((int)MyGame.ScreenCenter.X - 400, (int)MyGame.ScreenCenter.Y + MyGame.ScreenArea.Height / 4
                , 800, 200),
            "OK, and stop bothering  with messages !");

            Rectangle boxArea = new Rectangle(0, 0, 400, 600);

            _commanderInstructionsDialog = new DialogBoxes.ButtonDialogBox(
                button,
                "Place commanders & stuff",
                boxArea,
                _messageStretchImage);

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
        /// Return the owner for this tag, between 0 and 3
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
                    commander.CurrentZone = _map.getZoneAt(commander.Position);
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


        #region Question Timeout

        private void onQuestion(SocketIOClient.Messages.IMessage data)
        {
            Console.WriteLine("\nonQuestion\n");
            Console.WriteLine("\n\n");
            Console.WriteLine(data.Json.ToJsonString());
            Console.WriteLine("\n\n");


            /*QuestionData questionInfo;
            QuestionDataRoot obj = Newtonsoft.Json.JsonConvert.DeserializeObject<QuestionDataRoot>(data.Json.ToJsonString());
            questionInfo = obj.args[0];
            */

            _timeSinceLastQuestion = 0f;
            //_questionId = questionInfo.id;

            Rectangle progressArea = new Rectangle((int)MyGame.ScreenCenter.X - 300, (int)MyGame.ScreenCenter.Y - 300
                , 600, 50);
            //_progressBar = new UIElements.ProgressBar(_UIBack, _UIBack, progressArea);
            _animatedFire = new UIElements.AnimatedTexture(_fireSprite, _fireSprite.Width / 4, _fireSprite.Height, 0.1f, true);
            _animatedFire.Scale = 0.3f;
            _progressBar = new UIElements.AnimatedProgressBar(_animatedFire, _ropeSprite, progressArea);

            MessageDialogBox msgBox = new MessageDialogBox("You should be answering questions on your phone right now",
                new Rectangle(0, 0, 400, 600), _messageStretchImage);
            msgBox.Position = MyGame.ScreenCenter;
            msgBox.Show();

            _currentDialog = msgBox;

            UpdateAction = questionWaitUpdate;
        }

        private void questionWaitUpdate(float dt)
        {
            lock (_progressBar)
            {
                if (_animatedFire != null)
                {
                    _animatedFire.update(dt);
                    updateDialogBox(dt);
                }

                commonUpdate(dt);

                _timeSinceLastQuestion += dt;
                //display progress bar
                _progressBar.Progress = _timeSinceLastQuestion / _questionMaxAllowedTime;

                bool cheat = false;

                KeyboardState keyState = Keyboard.GetState();
                if (keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Enter)
                    && keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Space))
                {
                    cheat = true;
                }

                //send timeout onEnd
                if (_timeSinceLastQuestion >= _questionMaxAllowedTime || cheat)
                {
                    Console.WriteLine("send timeout");
                    ServerCom.Instance.Socket.Emit("timeout", new
                    {
                        id = 0//_questionId
                    });

                    _progressBar = null;
                    _currentDialog = null;
                    UpdateAction = emptyUpdate;
                }
            }
        }

        #endregion

        #region Question Answers

        private void onQuestionAnswered(SocketIOClient.Messages.IMessage data)
        {
            Console.WriteLine(" on question answer message : \n" + data.Json.ToJsonString());

            AnswerList answers;
            AnswersRoot obj = Newtonsoft.Json.JsonConvert.DeserializeObject<AnswersRoot>(data.Json.ToJsonString());

            answers = obj.args[0];

            _radialUI = new UIElements.RadialAnswerContainer(_radialUICenter, 500f, 150f);

            bool best = true;

            foreach (OrderedAnswer answer in answers.orderedAnswers)
            {
                UIElements.AnswerUI ansUI = new UIElements.AnswerUI(
                    answer.id
                    , best
                    , "answer : " + answer.value.ToString()
                    , _playerUIBack);

                _radialUI.ContainedUIs.Add(ansUI);
                best = false;
            }

            _radialUI.Position = MyGame.ScreenCenter;

            _timeSinceAnswerDisplayStart = 0f;
            UpdateAction = answersDisplayUpdate;
        }

        private void answersDisplayUpdate(float dt)
        {
            updateRadialUI(dt);
            commonUpdate(dt);
            _timeSinceAnswerDisplayStart += dt;

            if (_timeSinceAnswerDisplayStart > _answerDisplayDuration)
            {
                _radialUI = null;

                ServerCom.Instance.sendSimpleMessage("nextPhase1");
                UpdateAction = emptyUpdate;
            }

        }


        #endregion

        #region Play Turn

        //Wait for the player X to make a move
        //Scan for other players moves
        //Scan for illegual player moves

        private void onMoveRequested(SocketIOClient.Messages.IMessage data)
        {
            clearProgressAndDialog();

            Console.WriteLine("\nmessage : \n" + data.Json.ToJsonString() + "\n");

            //do stuff
            MoveData move;
            MoveRoot root = Newtonsoft.Json.JsonConvert.DeserializeObject<MoveRoot>(data.Json.ToJsonString());
            move = root.args[0];

            Console.WriteLine(" \n player to move is : " + move.id + "\n");

            initMove(move.id);
        }

        private void initMove(int player)
        {
            //init arrows for each commanders
            clearPositionLock();
            foreach (Commander command in _commanders)
            {
                command.Arrows.Clear();
            }

            foreach (Commander command in _commanders)
            {
                if (command.Owner == player)
                {
                    int zone = _map.getZoneAt(command.Position);

                    command.AttackStartZone = zone;

                    List<int> reachables = _map.getZonesReachableFrom(zone);

                    //for(int reach = 1; reach <= 16; ++reach)
                    foreach (int reach in reachables)
                    {
                        Vector2 center = _map.getCenterOfZone(reach);
                        //Vector2 startCenter = _map.getCenterOfZone(zone);
                        Vector2 startCenter = command.Position;
                        Console.WriteLine("target : " + center);

                        Color col = PlayerData.Instance[_map.getZoneOwner(reach)].BaseColor;

                        Arrow arrow = new Arrow(_arrowImage, startCenter, center, col);
                        command.Arrows.Add(arrow);
                    }
                }
            }


            //set update action
            UpdateAction = delegate(float dt) { moveUpdate(dt, player); };

        }

        private void moveUpdate(float dt, int player)
        {
            //display arrows
            commonUpdate(dt);

            Dictionary<long, List<int>> reachableZones = new Dictionary<long, List<int>>();

            foreach(Commander cmd in _commanders)
            {
                if(cmd.Owner == player)
                {
                    int zone = _map.getZoneAt(cmd.Position);
                    reachableZones[cmd.TagValue] = (_map.getZonesReachableFrom(zone));
                }
            }

            updateCommanders(dt, false, player, reachableZones);

            //TODO
            //check to stuff
            //Wait for the player X to make a move
            //Scan for other players moves
            //Scan for illegual player moves
            //Process Move (battle or move)

            lookForMove(dt, player);
            
        }

        private bool isPositionValid(Vector2 newPos, Commander command)
        {

            return false;
        }

        private void lookForMove(float dt, int player)
        {
            foreach (Commander command in _commanders)
            {
                if (command.Owner == player)
                {
                    int zone = _map.getZoneAt(command.Position);

                    if (zone != command.AttackStartZone)
                    {
                        //commander is in a new zone
                        Console.WriteLine("commander move");
                        //if he moves in one of is own zone
                        int zoneOwner = _map.getZoneOwner(zone); 
                        if (zoneOwner == command.Owner)
                        {
                            ServerCom.Instance.sendSimpleMessage("nextPhase3");
                        }
                        //if he attacs another player's zone
                        else
                        {
                            Console.WriteLine("\n battle : attack : " + command.Owner + " defend : " + zoneOwner);

                            ServerCom.Instance.battleResultCB = delegate(SocketIOClient.Messages.IMessage data)
                            {
                                onBattleResult(data, command, zone);
                            };

                            ServerCom.Instance.Socket.Emit("startBattle", new
                            {
                                player1 = command.Owner,
                                player2 = zoneOwner
                            });
                        }

                        clearArrows();
                        UpdateAction = emptyUpdate;
                        return;
                    }
                }
            }
        }

        private void clearArrows()
        {
            foreach (Commander command in _commanders)
            {
                command.Arrows.Clear();
            }
        }

        private void clearPositionLock()
        {
            lock(_commanders)
            {
                foreach (Commander command in _commanders)
                {
                    command.PositionLocked = false;
                }
            }
        }

        private void onBattleResult(SocketIOClient.Messages.IMessage data, Commander command, int fightZone)
        {
            lock (_commanders)
            {
                Console.WriteLine("\nonBattleResult\n");

                Console.WriteLine("\n" + data.Json.ToJsonString() + "\n");

                BattleResult br;
                BattleResultRoot root = Newtonsoft.Json.JsonConvert.DeserializeObject<BattleResultRoot>(data.Json.ToJsonString());

                br = root.args[0];

                Console.WriteLine("winner : " + br.winner + " looser : " + br.loser + " attacker : " + command.Owner + " fightzone : " + fightZone);

                if (command.Owner == br.winner)
                {
                    _map.setZoneOwner(fightZone, command.Owner);
                    ServerCom.Instance.Socket.Emit("territoryWon", new
                    {
                        zone = fightZone - 1,
                        owner = command.Owner
                    });

                    foreach (Commander looserCommand in _commanders)
                    {
                        if (looserCommand.Owner == br.loser
                            && _map.getZoneAt(looserCommand.Position) == fightZone)
                        {
                            int newZone = _map.getClosestZoneOfOwner(br.loser, fightZone);
                            looserCommand.Position = _map.getCenterOfZone(newZone);
                            looserCommand.PositionLocked = true;
                        }
                    }

                }
                else
                {
                    Console.WriteLine("\nbattle lost, fall back to " + command.AttackStartZone + "\n");
                    //put attacker back to his start zone
                    command.Position = _map.getCenterOfZone(command.AttackStartZone);
                }

                command.PositionLocked = true;

                UpdateAction = emptyUpdate;
                ServerCom.Instance.sendSimpleMessage("nextPhase3");
            }
        }

        #endregion

        #region Capture Territories

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
            clearProgressAndDialog();

        }

        private void onPlaceCommanders(SocketIOClient.Messages.IMessage data)
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

        #endregion

        private void emptyUpdate(float dt)
        {
            commonUpdate(dt);

            clearProgressAndDialog();

            /*ReadOnlyTouchPointCollection touches = MyGame.TouchTarget.GetState();

            foreach (TouchPoint touch in touches)
            {
                if (touch.Tag != null && touch.Tag.Value == 0xD0)
                {
                    addCommanderTag(touch, getOwnerForTag(touch.Tag.Value));
                    initMove(getOwnerForTag(touch.Tag.Value));
                }
            }*/
        }

        #endregion

        private void initialize()
        {

            NodeJSClient.ServerCom.Instance.majPlayerInfoCB = MajPlayerInfo;
            NodeJSClient.ServerCom.Instance.captureZonesCB = onCaptureTerritories;
            NodeJSClient.ServerCom.Instance.placeCommandersCB = onPlaceCommanders;
            NodeJSClient.ServerCom.Instance.questionCB = onQuestion;
            //NodeJSClient.ServerCom.Instance.answersReceivedCB = onQuestionAnswered;
            NodeJSClient.ServerCom.Instance.playerMoveCB = onMoveRequested;
            //NodeJSClient.ServerCom.Instance.battleResultCB = onBattleResult; -> done in lookForMove

            //My initialization logic
            _displayedPopups = new List<WarRoom.PopupStr>();

            _commanders = new List<Commander>();

            _userPosition = new Vector2(MyGame.ScreenCenter.X, MyGame.ScreenArea.Height - 100);

            _map = new TestXNA.Map(MyGame.SURFACE_WIDTH, MyGame.SURFACE_HEIGHT);
            _map.loadMap();

            GraphicsDevice GraphicsDevice = MyGame.SpriteBatch.GraphicsDevice;

            _iconImage = MyGame.ContentManager.Load<Texture2D>("Images/GameThumbnail");
            _popupImage = MyGame.ContentManager.Load<Texture2D>("Images/trollFace");
            _arrowImage = MyGame.ContentManager.Load<Texture2D>("Images/Arrow2");
            _mapBackground = MyGame.ContentManager.Load<Texture2D>("Images/Map");
            _mapOverlay = MyGame.ContentManager.Load<Texture2D>("Images/MapOverlay");
            _buttonTexture = MyGame.ContentManager.Load<Texture2D>("Images/buttonScroll");
            _messageTexture = MyGame.ContentManager.Load<Texture2D>("Images/messageScroll");
            _UIBack = MyGame.ContentManager.Load<Texture2D>("Images/RoundedRect");
            _commanderHighlight = MyGame.ContentManager.Load<Texture2D>("Images/TagHighlight");
            _fireSprite = MyGame.ContentManager.Load<Texture2D>("Images/fire");
            _ropeSprite = MyGame.ContentManager.Load<Texture2D>("Images/rope");
            _radialUICenter = MyGame.ContentManager.Load<Texture2D>("Images/centralRose");
            _playerUIBack = MyGame.ContentManager.Load<Texture2D>("Images/playerScroll");

            Rectangle stretchAreaButton = new Rectangle(15, 20, 70, 60);
            _buttonStretchImage = new UIElements.StretchableImage(_buttonTexture, stretchAreaButton);

            Rectangle stretchAreaMessage = new Rectangle(40, 30, 20, 40);
            _messageStretchImage = new UIElements.StretchableImage(_messageTexture, stretchAreaMessage);

            _updateAction = emptyUpdate;

            initializePlayerUIs();
            //initializeRadialUI();


        }


        private void initializePlayerUIs()
        {
            //hack
            /*GameData.PlayerData.Instance[0].Name = "Gwenn";
            GameData.PlayerData.Instance[1].Name = "Aurel";
            GameData.PlayerData.Instance[2].Name = "Jerom";
            GameData.PlayerData.Instance[3].Name = "Bastien";*/

            _miniUIs = new List<SmallPlayerUI>();

            SmallPlayerUI playerUI = new SmallPlayerUI(_buttonTexture, new Vector2(50f, 50f), 0);
            _miniUIs.Add(playerUI);

            playerUI = new SmallPlayerUI(_buttonTexture, new Vector2(200f, 50f), 1);
            _miniUIs.Add(playerUI);

            playerUI = new SmallPlayerUI(_buttonTexture, new Vector2(400f, 50f), 2);
            _miniUIs.Add(playerUI);

            playerUI = new SmallPlayerUI(_buttonTexture, new Vector2(600f, 50f), 3);
            _miniUIs.Add(playerUI);

            //now that the font i s;oaded we can compute the size of each UI
            foreach (SmallPlayerUI ui in _miniUIs)
            {
                ui.initializeArea(MyGame.BasicFontSmall);
            }
        }

        private void clearProgressAndDialog()
        {
            if (_progressBar != null)
            {
                lock (_progressBar)
                {
                    _progressBar = null;
                }
            }
            _currentDialog = null;
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
                }
            }

            MyGame.SpriteBatch.Draw(_iconImage, _userPosition, Color.White);

            drawPlayersUI();
            drawRadialUI();

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

        private void drawRadialUI()
        {
            if (_radialUI != null)
            {
                _radialUI.draw();
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

        private void commonUpdate(float dt)
        {
            updatePlayersUI(dt);
        }

        private void updateDialogBox(float dt)
        {
            if (_currentDialog != null)
            {
                _currentDialog.update(dt);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt">time since last frame in seconds</param>
        private void updatePlayersUI(float dt)
        {
            ReadOnlyTouchPointCollection touches = MyGame.TouchTarget.GetState();

            foreach (TouchPoint touch in touches)
            {
                checkForUIDrag(touch, dt);
            }


            for (int i = 0; i < _miniUIs.Count; ++i)
            {
                SmallPlayerUI ui = _miniUIs[i];

                ui.update(dt);
            }
        }

        private void updateCommanders(float dt, bool phase2, int playingPlayer, Dictionary<long, List<int>> allowedZones)
        {
            lock (_commanders)
            {

                ReadOnlyTouchPointCollection touches = MyGame.TouchTarget.GetState();

                foreach (TouchPoint touch in touches)
                {
                    int zone = _map.getZoneAt(Utils.touchPointToV2(touch));
                    int zoneOwner = _map.getZoneOwner(zone);

                    foreach (Commander command in _commanders)
                    {
                        if (touch.Tag != null && command.TagValue == touch.Tag.Value && !command.PositionLocked)
                        {
                            //phase 2, we can put commanders in every zone we own
                            if (phase2)
                            {
                                if (zoneOwner == command.Owner)
                                {
                                    command.Position = Utils.touchPointToV2(touch);
                                    command.CurrentZone = zone;
                                }
                            }
                            else//phase 3
                            {

                                //the currently playing player can move in every reachable zone
                                if (playingPlayer == command.Owner
                                    && (allowedZones[command.TagValue].Contains(zone) || zone == command.CurrentZone))
                                {
                                    command.Position = Utils.touchPointToV2(touch);
                                    command.CurrentZone = zone;
                                }
                                //other players can only stay in their current zone
                                else
                                {
                                    if (zone == command.CurrentZone)
                                    {
                                        command.Position = Utils.touchPointToV2(touch);
                                        command.CurrentZone = zone;
                                    }
                                }
                            }
                        }
                    }
                }

                foreach (Commander command in _commanders)
                {
                    command.update(dt);
                }

            }
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


        private void updateRadialUI(float dt)
        {
            if (_radialUI != null)
            {
                _radialUI.update(dt);
            }
        }

        public System.Action<float> UpdateAction
        {
            get { return _updateAction; }
            set { _updateAction = value; }
        }
    }
}
