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

        private UIElements.SimpleButton _button;
        private float _distFromMes;
        private SmallPlayerUI _target;

        public TargetedDialogBox(SmallPlayerUI target, UIElements.SimpleButton button, float distanceFromMessage, string message, Rectangle messageArea)
            : base(button, message, messageArea)
        {
            _target = target;
            _distFromMes = distanceFromMessage;
            _button = button;
            followTarget();
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
            base.update(dt);
            _button.update(dt);
            followTarget();
        }
    }
}
