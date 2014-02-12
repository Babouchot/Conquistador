using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Surface.Core;
using Microsoft.Xna.Framework.Graphics;


namespace TestXNA.Sources.UIElements
{
    class SimpleButton : RotatableUI
    {
        private UIElements.StretchableImage _buttonTexture;
        private Rectangle _area;
        private string _text;
        private bool _isTouched = false;

        private int _width;
        private int _height;

        public SimpleButton(UIElements.StretchableImage texture, Rectangle area, string text)
            : base()
        {
            _text = text;
            _area = area;
            _position = Utils.pointToVector2(_area.Center);
            _width = _area.Width;
            _height = _area.Height;
            _buttonTexture = texture;
        }

        public bool isTouchOn(Vector2 touch)
        {
            _isTouched = _area.Contains((int)touch.X, (int)touch.Y);
            return _isTouched;
        }

        /// <summary>
        /// Must be called before the isTouchOn() Method
        /// </summary>
        /// <param name="dt"></param>
        public override void update(float dt)
        {
            _isTouched = false;
        }

        public void updateArea()
        {
            int width = _width;
            int height = _height;
            
            /*if (_angle % (float)Math.PI == 0)
            {
                width = _width;
                height = _height;
            }
            else
            {
                width = _height;
                height = _width;
            }*/

            _area = new Rectangle((int)_position.X - _width / 2, (int)_position.Y - height / 2, width, height);
            _position = Utils.pointToVector2(_area.Center);
        }

        public override void draw()
        {
            _buttonTexture.draw(_area, _isTouched ? MyGame.ColorPanel.buttonTouchedColor : MyGame.ColorPanel.buttonColor, _angle);

            MyGame.SpriteBatch.DrawString(MyGame.BasicFont, _text, _position, MyGame.ColorPanel.textColor, _angle,
                MyGame.BasicFont.MeasureString(_text) / 2f, _scale, SpriteEffects.None, 0f);
        }

        public UIElements.StretchableImage ButtonTexture
        {
            get { return _buttonTexture; }
            set { _buttonTexture = value; }
        }
        

        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        public Rectangle Area
        {
            get { return _area; }
            set 
            {
                _area = value;
                _position = Utils.pointToVector2(_area.Center);
                _width = _area.Width;
                _height = _area.Height;
            }
        }

    }
}
