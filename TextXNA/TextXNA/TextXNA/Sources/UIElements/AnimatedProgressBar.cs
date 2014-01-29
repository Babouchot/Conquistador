using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


namespace TestXNA.Sources.UIElements
{
    class AnimatedProgressBar : ProgressBar
    {

        private AnimatedTexture _barAnim;

        public AnimatedProgressBar(AnimatedTexture barText, Texture2D backgroundText, Rectangle area)
             : base(null, backgroundText, area)
        {
            _barAnim = barText;
        }

        protected override void drawBack()
        {
            Rectangle subRect = new Rectangle(
                _area.Left + (int)((float)_area.Width * _progress)
                , _area.Top
                , _area.Width - (int)((float)_area.Width * _progress)
                , _area.Height
                );

            Rectangle source = new Rectangle(
                (int)(0 + _progress * (float)_backgroundTexture.Width)
                , 0
                , _backgroundTexture.Width - (int)(_progress * (float)_backgroundTexture.Width)
                , _backgroundTexture.Height
                );

            MyGame.SpriteBatch.Draw(_backgroundTexture, _area, new Color(0f, 0f, 0f, 175f));
            MyGame.SpriteBatch.Draw(_backgroundTexture, subRect, source, Color.White);
        }

        protected override void drawBar()
        {
            _barAnim.Position = new Vector2((float)_area.Left + (float)_area.Width * _progress
                , (float)_area.Center.Y - _area.Height/4f);
            _barAnim.draw();
        }
    }
}
