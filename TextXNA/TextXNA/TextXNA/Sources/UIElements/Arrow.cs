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
        private float       _angle;
        private float       _scale;
        private float       _targetScale;
        private float       _upTime = 0f;
        private int         _tagID = -1;
        private bool        _upToDate = true;

        private Texture2D _image;

        private static Vector2 arrowOrigin;


        public Arrow(Texture2D img, Vector2 position, Vector2 target, int tagID)
        {
            _image = img;

            arrowOrigin = new Vector2(_image.Width / 2, 0);

            _position       = position;
            _angle          = Utils.lookAt(_position, target);
            _targetScale    = (target - _position).Length() / _image.Height;
            _upToDate       = true;
            _tagID          = tagID;
        }


        public bool updateArrowWithTag(TouchPoint touch, Vector2 newTarget, float dt)
        {
                if (_tagID == touch.Id)
                {
                    _position       = new Vector2(touch.X, touch.Y);
                    _angle          = Utils.lookAt(_position, newTarget);
                    _upTime         += dt;
                    _upTime         = Math.Min(_upTime, ANIM_DURATION);
                    _targetScale    = (newTarget - _position).Length() / _image.Height;
                    _scale          = MathHelper.Lerp(0f, _targetScale, Utils.hill(_upTime / ANIM_DURATION));

                    _upToDate       = true;

                    return true;
                }
                return false;
        }

        public void draw()
        {
            MyGame.SpriteBatch.Draw(_image, Position, null, Color.White, Angle, arrowOrigin,
                    Scale, SpriteEffects.None, 0f);
        }


        public bool UpToDate
        {
            get { return _upToDate; }
            set { _upToDate = value; }
        }

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
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
