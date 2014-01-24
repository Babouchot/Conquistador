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
using MessagingToolkit.QRCode.Codec;
using MessagingToolkit.QRCode.Codec.Data;



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

        private DialogBox _dialog = null;
        private Texture2D _QRCode = null;
        private Rectangle _QrCodeArea;

        Texture2D _UIBackRect;
        Texture2D _roomBackground;

        public WaitingRoom()
        {
            Microsoft.Xna.Framework.Rectangle area = new Microsoft.Xna.Framework.Rectangle(
                (int)MyGame.ScreenCenter.X - _buttonWidth / 2
                , (int)(MyGame.ScreenCenter.Y * 1.8f) - _buttonHeight / 2
                , _buttonWidth
                , _buttonHeight);
            
            _UIBackRect = MyGame.ContentManager.Load<Texture2D>("Images/RoundedRect");
            _roomBackground = MyGame.ContentManager.Load<Texture2D>("Images/WaitingBack");

            _startButton = new UIElements.SimpleButton(_UIBackRect, area, "Start Game");

            _playerUIs = new List<UIElements.LargePLayerUI>();

            _posStart = new Vector2(MyGame.ScreenArea.Width / 5, 500f);
            _offset = new Vector2(MyGame.ScreenArea.Width / 5, 0f);


            //Create QR Code

            QRCodeEncoder encoder = new QRCodeEncoder();
            string ip = Utils.LocalIPAddress();

            Console.WriteLine("\n\n\n\nServer ip : " + ip + "\n\n\n");

            if (ip == "")
            {
                Console.WriteLine("Creating dialog");
                _dialog = new ConditionalDialogBox(
                    delegate() { return Utils.LocalIPAddress() != ""; }
                    , "Network could not be reached\n"
                    + "Please find de way to connect to a reliable, working network\n"
                    + "(Unice hotsport is NOT one of those networks...)"
                    , new Microsoft.Xna.Framework.Rectangle((int)MyGame.ScreenCenter.X , (int)MyGame.ScreenCenter.Y , 400, 400));
                _dialog.Position = MyGame.ScreenCenter;
                _dialog.Show();
            }
            else
            {
                
                System.Drawing.Bitmap qrCodeImage = encoder.Encode(ip);

                Color[] pixels = new Color[qrCodeImage.Width * qrCodeImage.Height];
                for (int y = 0; y < qrCodeImage.Height; y++)
                {
                    for (int x = 0; x < qrCodeImage.Width; x++)
                    {
                        System.Drawing.Color c = qrCodeImage.GetPixel(x, y);
                        pixels[(y * qrCodeImage.Width) + x] = new Color(c.R, c.G, c.B, c.A);
                    }
                }

                _QRCode = new Texture2D(
                  MyGame.SpriteBatch.GraphicsDevice,
                  qrCodeImage.Width,
                  qrCodeImage.Height);

                _QRCode.SetData<Color>(pixels);

                int recWidth = MyGame.ScreenArea.Width / 8;
                _QrCodeArea = new Rectangle((int)(MyGame.ScreenCenter.X - recWidth / 2), 10, recWidth, recWidth);
            }

            //

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
            if (_dialog != null && _dialog.IsShown)
            {
                _dialog.update(dt);
                return;
            }

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

            MyGame.SpriteBatch.Draw(_roomBackground, MyGame.ScreenArea, Color.White);

            if (_QRCode != null)
            {
                MyGame.SpriteBatch.Draw(_QRCode, _QrCodeArea, Color.White);
            }

            foreach (UIElements.LargePLayerUI ui in _playerUIs)
            {
                ui.draw();
            }
            _startButton.draw();

            if (_dialog != null)
            {
                _dialog.draw();
            }

            //MyGame.SpriteBatch.Draw(uiBack, MyGame.ScreenArea, Color.Red);
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

            UIElements.LargePLayerUI ui = new UIElements.LargePLayerUI(gamerID, avat, _UIBackRect);
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
