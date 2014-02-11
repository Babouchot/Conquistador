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
    /// <summary>
    /// Base class for all kind of rotatable UIs
    /// </summary>
    class RotatableUI 
    {

        protected float _angle;
        protected Vector2 _position;
        protected float _scale;

        public RotatableUI()
        {
            _angle = 0f;
            _scale = 1f;
        }


        public virtual void draw() { }

        public virtual void update(float dt) { }


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

        public float Scale
        {
            get { return _scale; }
            set { _scale = value; }
        }


    }
}
