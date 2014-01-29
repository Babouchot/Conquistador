using System.Windows.Forms;
using System;
using System.Collections.Generic;
using Microsoft.Surface;
using Microsoft.Surface.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using TestXNA.Sources;


namespace TestXNA.Sources.GameData
{
    class Commander : RotatableUI
    {
        private const float Draw_Scale = 0.5f;
        private long tagValue = 0x0f;
        private Texture2D _highlight;
        private int _owner = -1;

        private List<Arrow> _arrows;

        public Commander(Texture2D highlight)
        {
            _highlight = highlight;
            _arrows = new List<Arrow>();
        }

        public override void update(float dt)
        {
            foreach (Arrow arrow in _arrows)
            {
                arrow.update(dt);
                arrow.Position = _position;
            }

            base.update(dt);
        }

        public override void draw()
        {
            foreach (Arrow arrow in _arrows)
            {
                arrow.draw();
            }

            float scale = Draw_Scale;
            Color col = PlayerData.Instance[_owner].BaseColor;

            Vector2 highlightCenter = new Vector2(_highlight.Width / 2f, _highlight.Height / 2f);

            MyGame.SpriteBatch.Draw(_highlight, _position, null, col, 0f,
                highlightCenter, scale, SpriteEffects.None, 0f);

            base.draw();
        }

        public long TagValue
        {
            get { return tagValue; }
            set 
            {
                tagValue = value;
            }
        }

        public int Owner
        {
            get { return _owner; }
            set { _owner = value; }
        }


        public List<Arrow> Arrows
        {
            get { return _arrows; }
            set { _arrows = value; }
        }

    }
}
