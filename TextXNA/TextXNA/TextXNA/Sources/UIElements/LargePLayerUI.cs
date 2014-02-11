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
        private const int Height = 400;

        private int _player;
        private Texture2D _avatar;
        private Texture2D _background;
        private Vector2 _position;

        private Rectangle _avatarArea;
        private Rectangle _backgroundArea;

        public LargePLayerUI(int player, Texture2D background)
        {
            _avatar = GameData.PlayerData.Instance[player].FaceImage;
            _background = background;
            _player = player;
            replace();
        }

        public void update(float dt)
        {

        }

        public void draw()
        {
            MyGame.SpriteBatch.Draw(_background, _backgroundArea, GameData.PlayerData.Instance[_player].HighlitColor);
            MyGame.SpriteBatch.Draw(_avatar, _avatarArea, Color.White);
            string name = GameData.PlayerData.Instance[_player].Name;
            Vector2 nameSize = MyGame.BasicFont.MeasureString(name);
            Vector2 namePosition = _position + new Vector2(0f, Height/4f);

            /*MyGame.SpriteBatch.DrawString(MyGame.BasicFontBold, name, namePosition - new Vector2(5,2), Color.White, 0f, nameSize / 2f
               , 1f, SpriteEffects.None, 0f);*/
            MyGame.SpriteBatch.DrawString(MyGame.BasicFont, name, namePosition, GameData.PlayerData.Instance[_player].OppositeColor, 0f, nameSize / 2f
                , 1f, SpriteEffects.None, 0f);
        }

        private void replace()
        {
            _avatarArea = new Rectangle((int)_position.X - Width / 4, (int)_position.Y - Width / 4, Width / 2, Width / 2);
            _backgroundArea = new Rectangle((int)_position.X - Width / 2, (int)_position.Y - Height / 2, Width, Height);
        }

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; replace(); }
        }

    }
}
