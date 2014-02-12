using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestXNA.Sources.UIElements
{
    interface IDrawable
    {
        void update(float dt);
        void draw();
    }
}
