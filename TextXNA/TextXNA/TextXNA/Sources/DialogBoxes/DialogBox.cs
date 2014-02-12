using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestXNA.Sources
{
    class DialogBox : TouchRotatable
    {
        private bool _isShown = false;

        public bool IsShown
        {
            get { return _isShown; }
            set { _isShown = value; }
        }

        public DialogBox()
        { }

        public void Show()
        {
            _isShown = true;
        }

        public void Hide()
        {
            _isShown = false;
        }

    }
}
