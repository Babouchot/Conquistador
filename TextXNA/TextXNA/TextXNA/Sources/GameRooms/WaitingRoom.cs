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
using TestXNA.Sources.NodeJSClient;
using TestXNA.Sources.NodeJSClient.MessageData;
using TestXNA.Sources.GameData;



namespace TestXNA.Sources.GameRooms
{
    class WaitingRoom : GameRooms.IRoom
    {

        public System.Action startGameCallback = null;

        private int _buttonWidth = 400;
        private int _buttonHeight = 150;
        private UIElements.SimpleButton _startButton;

        private List<UIElements.LargePLayerUI> _playerUIs;
        private Vector2 _posStart;
        private Vector2 _offset;

        public WaitingRoom()
        {
            Rectangle area = new Rectangle((int)MyGame.ScreenCenter.X - _buttonWidth / 2, (int)(MyGame.ScreenCenter.Y * 1.8f) - _buttonHeight / 2
                , _buttonWidth, _buttonHeight);
            
            Texture2D uiBack = MyGame.ContentManager.Load<Texture2D>("Images/UIBack");

            _startButton = new UIElements.SimpleButton(uiBack, area, "Start Game");

            _playerUIs = new List<UIElements.LargePLayerUI>();

            _posStart = new Vector2(250f, 400f);
            _offset = new Vector2(400f, 0f);

            /*
            for (int i = 0; i < 4; ++i)
            {
                UIElements.LargePLayerUI ui = new UIElements.LargePLayerUI(i, avat, uiBack);
                ui.Position = posStart;
                _playerUIs.Add(ui);
                posStart += offset;
            }
            */
            NodeJSClient.ServerCom.Instance.playerConnectCB = OnPlayerConnect;
            NodeJSClient.ServerCom.Instance.Execute();
        }


        //TODO
        /*
         * Launch Game button (4 players mandatory)
         * QR code with node server adress (computer ip)
         * Handle player connection
         * 
         */
        public void update(float dt)
        {
            _startButton.update(dt);
            ReadOnlyTouchPointCollection touches = MyGame.TouchTarget.GetState();

            foreach (TouchPoint touch in touches)
            {
                if (_startButton.isTouchOn(Utils.touchPointToV2(touch)) && canStartGame())
                {
                    //Change Rooms
                    startGameCallback();
                }
            }
        }

        public void draw()
        {
            foreach (UIElements.LargePLayerUI ui in _playerUIs)
            {
                ui.draw();
            }
            _startButton.draw();    
        }


        private void OnPlayerConnect(SocketIOClient.Messages.IMessage message)
        {

            Console.WriteLine("OnPlayerConnect");
            //Get info from message
            Console.WriteLine("\n message : " + message.Json.ToJsonString() + "\n");
            PlayerInfo player = null;

            PlayerInfoRoot obj = Newtonsoft.Json.JsonConvert.DeserializeObject<PlayerInfoRoot>(message.Json.ToJsonString());
            player = obj.args[0];
            Console.WriteLine("player name " + player.pseudo);
            if (PlayerData.Instance[player.gameID].Name == null || PlayerData.Instance[player.gameID].Name.Length > 0)
            {
                PlayerData.Instance[player.gameID].Name = player.pseudo;
                InitPlayerUI(player.gameID);
            }
        }

        private void InitPlayerUI(int gamerID)
        {
            Texture2D avat = MyGame.ContentManager.Load<Texture2D>("Images/trollFace");
            Texture2D uiBack = MyGame.ContentManager.Load<Texture2D>("Images/UIBack");

            UIElements.LargePLayerUI ui = new UIElements.LargePLayerUI(gamerID, avat, uiBack);
            ui.Position = _posStart;
            _playerUIs.Add(ui);
            _posStart += _offset;

        }


        private bool canStartGame()
        {
            int nbPlayer = 0;
            foreach (PlayerData play in PlayerData.Instance)
            {
                if (play.Name == null || play.Name.Length == 0)
                {
                    return false;
                }
                /*else
                {
                    ++nbPlayer;
                }*/
            }

            return true;// nbPlayer >= 2;
        }
        


    }
}
