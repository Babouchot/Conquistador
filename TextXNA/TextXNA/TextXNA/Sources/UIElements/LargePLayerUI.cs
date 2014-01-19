using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Surface.Core;
using Microsoft.Xna.Framework.Graphics;


namespace TestXNA.Sources.UIElements
{
    class LargePLayerUI : IDrawable
    {
        private const int Width = 300;
        private const int Height = 600;

        private int _player;
        private Texture2D _avatar;
        private Texture2D _background;
        private Vector2 _position;

        private Rectangle _avatarArea;
        private Rectangle _backgroundArea;

        public LargePLayerUI(int player, Texture2D avatar, Texture2D background)
        {
            _avatar = avatar;
            _background = background;
            _player = player;
        }

        public void update(float dt)
        {

        }

        public void draw()
        {
            MyGame.SpriteBatch.Draw(_background, _backgroundArea, Color.White);
            MyGame.SpriteBatch.Draw(_avatar, _avatarArea, Color.White);
            string name = GameData.PlayerData.Instance[_player].Name;
            Vector2 nameSize = MyGame.BasicFont.MeasureString(name);
            Vector2 namePosition = _position + new Vector2(0f, Height/3f);
            MyGame.SpriteBatch.DrawString(MyGame.BasicFont, name, namePosition, Color.White, 0f, nameSize / 2f
                , 1f, SpriteEffects.None, 0f);
        }

        private void replace()
        {
            _avatarArea = new Rectangle((int)_position.X - Width / 6, (int)_position.Y - Height / 6, Width/3, Height/3);
            _backgroundArea = new Rectangle((int)_position.X - Width / 2, (int)_position.Y - Height / 2, Width, Height);
        }

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; replace(); }
        }

    }
}
