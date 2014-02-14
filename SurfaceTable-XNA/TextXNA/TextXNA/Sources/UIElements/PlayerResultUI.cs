using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace TestXNA.Sources.UIElements
{
    class PlayerResultUI : RotatableUI
    {

        private bool _isBest = false;
        private int _player;
        private string _text;
        private Texture2D _background;

        private Vector2 _target = Vector2.Zero;


        public PlayerResultUI(int player, bool isBest, string text, Texture2D back)
        {

            Console.WriteLine("\n init result with : "+text+"\n");

            _background = back;
            _player = player;
            _isBest = isBest;
            _text = text;

            if (_text == "void")
            {
                _text = "None";
            }
        }

        public override void draw()
        {
            Texture2D avat = GameData.PlayerData.Instance[_player].FaceImage;
            Color col = _isBest ? Color.White : Color.Gray;
            Color oppositeCol = Color.Black;//GameData.PlayerData.Instance[_player].OppositeColor;
            Vector2 avatSize = new Vector2(avat.Width, avat.Height);
            Vector2 backSize = new Vector2(_background.Width, _background.Height);

            MyGame.SpriteBatch.Draw(_background, _position, null, col, _angle, backSize / 2f, _scale, SpriteEffects.None, 0f);
            MyGame.SpriteBatch.Draw(avat, _position, null, col, _angle, avatSize / 2f, _scale * 0.7f, SpriteEffects.None, 0f);

            Vector2 targetDir = _target - _position;
            targetDir.Normalize();

            Vector2 stringPos = _position - targetDir * backSize.Y * 0.3f * _scale ;

            MyGame.SpriteBatch.DrawString(MyGame.BasicFont, _text,
                stringPos, oppositeCol, _angle, MyGame.BasicFont.MeasureString(_text) / 2f, _scale, SpriteEffects.None, 0f);

            string name = GameData.PlayerData.Instance[_player].Name;
            Vector2 namePos = _position - targetDir * backSize.Y * -0.3f * _scale;

            MyGame.SpriteBatch.DrawString(MyGame.BasicFont, name,
                namePos, oppositeCol, _angle, MyGame.BasicFont.MeasureString(name) / 2f, _scale, SpriteEffects.None, 0f);

        }

        public Vector2 Target
        {
            get { return _target; }
            set { _target = value; }
        }

        public float Height
        {
            get { return _background.Height; }
        }

        public bool IsBest
        {
            get { return _isBest; }
            set { _isBest = value; }
        }
    }
}
