using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestXNA.Sources.GameRooms
{
    interface IRoom
    {
        void update(float dt);
        void draw();
    }
}
