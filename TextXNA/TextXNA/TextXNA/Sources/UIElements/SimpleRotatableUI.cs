using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Surface.Core;
using Microsoft.Xna.Framework.Graphics;
using TestXNA;

namespace TestXNA.Sources
{
    class SimpleRotatableUI : RotatableUI
    {

        private Texture2D _image;
        private Vector2 _origin;
        private string _text = "";
        private SpriteFont _font;

        public SimpleRotatableUI(Texture2D image, string text, SpriteFont font)
        {
            _image = image;
            _origin = new Vector2(_image.Width / 2f, _image.Height / 2f);
            _font = font;
            _text = text;
        }

        public override void draw()
        {
            Vector2 drawPos = _position;
            Vector2 textPos = _position;

            MyGame.SpriteBatch.Draw(_image, drawPos, null, Color.White, Angle, _origin,
            Scale, SpriteEffects.None, 0f);
            MyGame.SpriteBatch.DrawString(_font, _text, textPos, Color.Red, Angle,
                _font.MeasureString(_text) / 2f, Scale, SpriteEffects.None, 0f);
            base.draw();
        }
    }
}
