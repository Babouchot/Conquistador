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

        public const float ANIM_DURATION = 0.5f;

        private Vector2     _position;
        private Vector2     _target;
        private float       _angle;
        private float       _scale;
        private float       _targetScale;
        private float       _upTime = 0f;

        private Texture2D _image;

        private static Vector2 arrowOrigin;


        public Arrow(Texture2D img, Vector2 position, Vector2 target)
        {
            //Console.WriteLine("new arrow : position : " + position + " target : " + target);
            _image = img;

            arrowOrigin = new Vector2(_image.Width / 2, 0);

            _position       = position;
            _angle          = Utils.lookAt(_position, target);
            _targetScale    = (target - _position).Length() / (float)_image.Height;
            _target         = target;
        }


        public void update(float dt)
        {
            //Console.WriteLine("arrow update time : " + _upTime + " duration : " + ANIM_DURATION + " target scale " + _targetScale + " scale " + _scale);
            _angle          = Utils.lookAt(_position, _target);
            _upTime         += dt;
            _upTime         = Math.Min(_upTime, ANIM_DURATION);
            _targetScale    = (_target - _position).Length() / _image.Height;
            _scale          = MathHelper.Lerp(0f, _targetScale, Utils.hill(_upTime / ANIM_DURATION));

         }

        public void draw()
        {
            //Console.WriteLine("arrow dranw : " + _scale);
            MyGame.SpriteBatch.Draw(_image, Position, null, Color.White, Angle, arrowOrigin,
                    Scale, SpriteEffects.None, 0f);
        }

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public Vector2 Target
        {
            get { return _target; }
            set {
                _target = value;
                _angle = Utils.lookAt(_position, _target);
                _targetScale = (_target - _position).Length() / _image.Height;
            }
        }
        
        public float Scale
        {
            get { return _scale; }
            set { _scale = value; }
        }

        public float Angle
        {
            get { return _angle; }
            set { _angle = value; }
        }
    }
}
