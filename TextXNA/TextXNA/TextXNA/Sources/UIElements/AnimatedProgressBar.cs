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
                Area.Left + (int)((float)Area.Width * _progress)
                , Area.Top
                , Area.Width - (int)((float)Area.Width * _progress)
                , Area.Height
                );

            Rectangle source = new Rectangle(
                (int)(0 + _progress * (float)_backgroundTexture.Width)
                , 0
                , _backgroundTexture.Width - (int)(_progress * (float)_backgroundTexture.Width)
                , _backgroundTexture.Height
                );

            MyGame.SpriteBatch.Draw(_backgroundTexture, Area, new Color(0f, 0f, 0f, 175f));
            MyGame.SpriteBatch.Draw(_backgroundTexture, subRect, source, Color.White);
        }

        protected override void drawBar()
        {
            _barAnim.Position = new Vector2((float)Area.Left + (float)Area.Width * _progress
                , (float)Area.Center.Y - Area.Height/4f);
            _barAnim.draw();
        }
    }
}
