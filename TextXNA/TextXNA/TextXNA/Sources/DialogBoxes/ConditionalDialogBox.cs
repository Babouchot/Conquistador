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
    class ConditionalDialogBox : MessageDialogBox
    {
        private Utils.conditionChecker _conditionTest;

        public ConditionalDialogBox(Utils.conditionChecker checker, string content, Rectangle area)
            : base(content, area)
        {
            _conditionTest = checker;
        }


        public override void update(float dt)
        {
            //check condition
            if (_conditionTest())
            {
                //close the dialog
                Hide();
            }
            base.update(dt);
        }
    }
}
