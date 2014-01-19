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
    class TouchRotatable : RotatableUI
    {

        public const float SPEED_COEF = 1.5f;
        public const float NB_MOVE_RECORDED = 20f;

        protected int _touchId = -1;
        protected bool _touchReleased = true;
        protected float _touchedScale;
        protected float _angleOffset = 0f;

        protected float _angularVelocity = 0f;
        protected float _weigth = 0.7f;

        public TouchRotatable(float touchScale = 1.3f)
        {
            _touchedScale = touchScale;
        }

        protected virtual bool isTouchOnUI(Vector2 touch)
        {
            return false;
        }

        public virtual bool processTouch(TouchPoint touch, float dt)
        {
            Vector2 touchPos = Utils.touchPointToV2(touch);
            float dist = Vector2.Distance(touchPos, _position);

            if (touch.Id == _touchId || isTouchOnUI(touchPos))
            {
                Vector2 direction = (touchPos - _position);
                float newTouchAngle = Utils.directionToAngle(direction);

                //same touch
                if (touch.Id == _touchId)
                {
                    float newAngle = newTouchAngle + _angleOffset;
                    float angleDiff = newAngle - _angle;

                    float lastTouchAngle = _angle - _angleOffset;

                    if (lastTouchAngle * newTouchAngle >= 0)
                    {
                        _angularVelocity = (_angularVelocity * NB_MOVE_RECORDED + angleDiff / dt * SPEED_COEF) / (NB_MOVE_RECORDED + 1f);
                    }
                    else
                    {
                        //Console.WriteLine("new angle : " + newTouchAngle + " _angle  : " + lastTouchAngle);
                    }
                    //update angle
                    _angle = newAngle;

                }
                //new touch
                else
                {
                    _angleOffset = _angle - newTouchAngle;
                }

                _touchId = touch.Id;
                _touchReleased = false;

                return true;
            }
            else
            {
                _touchReleased = true;
            }
            return false;
        }

        public override void update(float dt)
        {
            if (_touchReleased)
            {
                _touchId = -1;
                _angleOffset = 0f;
                _angularVelocity -= _angularVelocity * _weigth * dt;
                _angle += _angularVelocity * dt;
            }
            else
            {
                _touchReleased = true;
            }
        }

    }
}
