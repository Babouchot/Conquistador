using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Surface.Core;
using Microsoft.Xna.Framework.Graphics;
using TestXNA;

namespace TestXNA.Sources.DialogBoxes
{
    class TargetedDialogBox : ButtonDialogBox
    {
        private SmallPlayerUI _target;
        protected float _distFromMes;

        public TargetedDialogBox(SmallPlayerUI target, UIElements.SimpleButton button, float distanceFromMessage,
            string message, Rectangle messageArea, UIElements.StretchableImage back)

            : base(button, message, messageArea, back)
        {
            _target = target;
            _distFromMes = distanceFromMessage;
            followTarget();
        }

        protected override bool processTouch(TouchPoint touch, float dt)
        {
            _button.update(dt);
            if (_button.isTouchOn(Utils.touchPointToV2(touch)))
            {
                Hide();
            }
            return false;
        }

        private void followTarget()
        {
            _angle = _target.Angle;
            _button.Angle = _target.Angle;

            Vector2 dirToUI = _target.Position - MyGame.ScreenCenter;
            //dirToUI.Normalize();

            _button.Position = MyGame.ScreenCenter + dirToUI * _distFromMes;
            _button.updateArea();
        }

        public override void update(float dt)
        {
            _angle = 0f;
            base.update(dt);
            _button.update(dt);
            followTarget();
        }
    }
}
