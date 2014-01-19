using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Surface.Core;
using Microsoft.Xna.Framework.Graphics;
using TestXNA;

namespace TestXNA.Sources.DialogBoxes
{
    class ButtonDialogBox : MessageDialogBox
    {
        private UIElements.SimpleButton _button;
        private float _distFromMes;

        public ButtonDialogBox(UIElements.SimpleButton button, string message, Rectangle messageArea)
            : base(message, messageArea)
        {
            _button = button;
        }

        public override bool processTouch(TouchPoint touch, float dt)
        {
            _button.update(dt);
            if (_button.isTouchOn(Utils.touchPointToV2(touch)))
            {
                Hide();
            }
            return base.processTouch(touch, dt);
        }

        public override void update(float dt)
        {
            base.update(dt);
            _button.update(dt);
        }

        public override void draw()
        {
            base.draw();
            if (IsShown)
            {
                _button.draw();
            }
        }
    }
}
