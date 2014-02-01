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
    class Arrow
    {

        public const float ANIM_DURATION = 1f;
        private const float _SCALE = 1f;

        private Vector2     _position;
        private Vector2     _target;
        private float       _angle;
        private float       _upTime = 0f;
        private Color       _color;

        private Texture2D _image;

        private static Vector2 arrowOrigin;

        private Rectangle _partialImageRect;

        public Arrow(Texture2D img, Vector2 position, Vector2 target, Color color)
        {
            //Console.WriteLine("new arrow : position : " + position + " target : " + target);
            _image = img;
            _color = color;

            arrowOrigin = new Vector2(_image.Width / 2, 0);

            _position       = position;
            _angle          = Utils.lookAt(_position, target);
            _target         = target;

            float ratio = (target - _position).Length() / (float)_image.Height;

            int startY = (int)((1f - ratio) * (float)_image.Height);
            int height = (int)(ratio * (float)_image.Height);
            _partialImageRect = new Rectangle(0, startY, _image.Width, height);
        }


        public void update(float dt)
        {
            //Console.WriteLine("arrow update time : " + _upTime + " duration : " + ANIM_DURATION + " target scale " + _targetScale + " scale " + _scale);
            _angle          = Utils.lookAt(_position, _target);
            _upTime         += dt;
            _upTime         = Math.Min(_upTime, ANIM_DURATION);
         }

        public void draw()
        {
            float part = Utils.hill(_upTime / ANIM_DURATION);

            int startY = _partialImageRect.Y + (int)((1f - part) * (float)_partialImageRect.Height);
            int height = (int)(part * (float)_partialImageRect.Height);
            Rectangle partial = new Rectangle(0, startY, _image.Width, height);
            
            //Console.WriteLine("arrow dranw : " + _scale);
            MyGame.SpriteBatch.Draw(_image, Position, partial, _color, Angle, arrowOrigin,
                    _SCALE, SpriteEffects.None, 0f);
        }

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }
        
        public float Angle
        {
            get { return _angle; }
            set { _angle = value; }
        }
    }
}
