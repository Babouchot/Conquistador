﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


namespace TestXNA.Sources.UIElements
{
    class ProgressBar
    {
        protected float _progress = 0f; //0 to 1
        protected Texture2D _barTexture;
        protected Texture2D _backgroundTexture;
        private Rectangle _area;

        public ProgressBar(Texture2D barText, Texture2D backgroundText, Rectangle area)
        {
            _barTexture = barText;
            _backgroundTexture = backgroundText;
            _area = area;
        }
        
        public float Progress
        {
          get { return _progress; }
          set { _progress = value; }
        }

        public Rectangle Area
        {
            get { return _area; }
            set { _area = value; }
        }

        private Rectangle getCurrentArea()
        {
            Rectangle currentArea = new Rectangle(_area.X, _area.Y
                , (int)((float)_area.Width * _progress), (int)((float)_area.Height));

            return currentArea;
        }

        public void draw()
        {
            drawBack();
            drawBar();
        }

        protected virtual void drawBack()
        {
            MyGame.SpriteBatch.Draw(_backgroundTexture, _area, MyGame.ColorPanel.backgroundColor);
        }

        protected virtual void drawBar()
        {
            int startY = _area.Y + (int)( _progress * (float)_area.Height);
            int height = (int)((1f - _progress) * (float)_area.Height);

            Rectangle partialArea = new Rectangle(_area.X, startY, _area.Width, height);

            Rectangle source = new Rectangle(
                0
                ,(int) (_progress * (float)_barTexture.Height)
                , _barTexture.Width 
                , (int) ((1f - _progress) * (float)_barTexture.Height));


            MyGame.SpriteBatch.Draw(_barTexture, partialArea, source, MyGame.ColorPanel.foregroundColor);
        }
    }
}
