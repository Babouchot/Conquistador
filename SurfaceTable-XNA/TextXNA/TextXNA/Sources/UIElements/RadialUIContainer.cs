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
    class RadialUIContainer : TouchRotatable
    {
        protected List<RotatableUI> _containedUIs;
        protected float _outterRadius = 0f;
        protected float _innerRadius = 0f;


        public RadialUIContainer(float outterRad, float innerRad, float touchedScale = 1.3f)
            : base(touchedScale)
        {
            _containedUIs = new List<RotatableUI>();
            _outterRadius = outterRad;
            _innerRadius = innerRad;
        }

        public override void update(float dt)
        {            
            float angleOffset = (float)Math.PI * 2 / (float)_containedUIs.Count;
            float distance = (_outterRadius + _innerRadius) / 2f;
            //angle invert needed for proper drawing....don't know why yet
            float uiAngle = -_angle;
            
            for(int i = 0; i < _containedUIs.Count; ++i)
            {
                RotatableUI ui = _containedUIs[i];

                //change position
                ui.Position = _position + new Vector2((float)Math.Sin(uiAngle), (float)Math.Cos(uiAngle)) * distance;
                //ui.Angle = Utils.lookAt(ui.Position, _position) + (float)Math.PI;

                if (!_touchReleased)
                {
                    ui.Scale = _touchedScale;
                }
                else
                {
                    ui.Scale = 1f;
                }
                ui.update(dt);

                uiAngle += angleOffset;
            }

            base.update(dt);
        }

        public override void draw()
        {
            //draw center UI


            //draw leaves UIs
            foreach (RotatableUI ui in _containedUIs)
            {
                ui.draw();
            }
        }

        protected override bool isTouchOnUI(Vector2 touch)
        {
            float dist = Vector2.Distance(touch, _position);
            return dist < _outterRadius && dist > _innerRadius;
        }

        public List<RotatableUI> ContainedUIs
        {
            get { return _containedUIs; }
            set { _containedUIs = value; }
        }

    }
}
