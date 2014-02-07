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
using TestXNA.Sources.UIElements;


namespace TestXNA.Sources.GameRooms
{
    class EndGameRoom : GameRooms.IRoom
    {
        private int _buttonWidth = 300;
        private int _buttonHeight = 175;
        private UIElements.SimpleButton _restartButton;

        private Texture2D _buttonBack;
        private Texture2D _playerUIBack;
        private Texture2D _roomBackground;
        private Texture2D _centerRose;
        private Texture2D _progressTexture;

        private SoundEffect _music;
        private SoundEffectInstance _musicInstance;

        private RadialAnswerContainer _radialUI;

        public EndGameRoom()
        {
            Rectangle area = new Microsoft.Xna.Framework.Rectangle(
                (int)MyGame.ScreenArea.Width - _buttonWidth
                , (int)_buttonHeight/2
                , _buttonWidth
                , _buttonHeight);

            /*_music = MyGame.ContentManager.Load<SoundEffect>("Sounds/endMusic");
            _musicInstance = _music.CreateInstance();
            _musicInstance.IsLooped = true;
            _musicInstance.Play();*/

            _buttonBack = MyGame.ContentManager.Load<Texture2D>("Images/buttonScroll");
            _roomBackground = MyGame.ContentManager.Load<Texture2D>("Images/WaitingBack");
            _playerUIBack = MyGame.ContentManager.Load<Texture2D>("Images/playerScroll");
            _centerRose = MyGame.ContentManager.Load<Texture2D>("Images/centralRose");
            _progressTexture = MyGame.ContentManager.Load<Texture2D>("Images/centralProgress");

            Texture2D messageBack = MyGame.ContentManager.Load<Texture2D>("Images/messageScroll");

            Rectangle stretchAreaButton = new Rectangle(15, 20, 70, 60);

            UIElements.StretchableImage stretchButtonTexture = new UIElements.StretchableImage(_buttonBack, stretchAreaButton);

            _restartButton = new UIElements.SimpleButton(stretchButtonTexture, area, "Restart");

            initScoreUI();
        }

        private void initScoreUI()
        {
            _radialUI = new RadialAnswerContainer(_centerRose, _progressTexture, 500f, 150f
                , "Sometimes you win...\nSomtimes you loose...");

            for (int i = 0; i < 4; ++i)
            {
                PlayerResultUI ui = new PlayerResultUI(i, i == 0, "Score : "+PlayerData.Instance[i].Score
                    , _playerUIBack);
                _radialUI.ContainedUIs.Add(ui);
            }

            _radialUI.Position = MyGame.ScreenCenter;
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
            _radialUI.update(dt);
            //_restartButton.update(dt);
            ReadOnlyTouchPointCollection touches = MyGame.TouchTarget.GetState();

            /*foreach (TouchPoint touch in touches)
            {
                if (_restartButton.isTouchOn(Utils.touchPointToV2(touch)))
                {
                    //Change Rooms

                }
            }*/
        }

        public void draw()
        {

            MyGame.SpriteBatch.Draw(_roomBackground, MyGame.MapArea, Color.White);

            _radialUI.draw();
            //_restartButton.draw();

        }



    }
}
