using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Surface.Core;
using Microsoft.Xna.Framework.Graphics;
using TestXNA;

namespace TestXNA.Sources.DialogBoxes
{
    class ButtonDialogBox : MessageDialogBox
    {
        protected UIElements.SimpleButton _button;

        public ButtonDialogBox(UIElements.SimpleButton button, string message, Rectangle messageArea, UIElements.StretchableImage back)
            : base(message, messageArea, back)
        {
            _button = button;
        }

        protected override bool processTouch(TouchPoint touch, float dt)
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
