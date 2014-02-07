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

        private int _attackStartZone = 0;
        private int _currentZone = 0;

        private float _time = 0f;
        private float _maxTime = 0.5f;
        private float _coef = 1f;

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

            if (_time >= _maxTime)
            {
                _coef = -1f;
            }
            else if(_time <= 0f)
            {
                _coef = 1f;
            }

            _time += _coef*dt;
            _time = Math.Min(_time, _maxTime);
            _time = Math.Max(_time, 0f);

            base.update(dt);
        }

        public override void draw()
        {
            foreach (Arrow arrow in _arrows)
            {
                arrow.draw();
            }

            float scale = Draw_Scale;

            float dx = _time/_maxTime;
            dx = 0.5f + dx/2f;

            Color col = IsPlaying ? PlayerData.Instance[_owner].HighlitColor : PlayerData.Instance[_owner].BaseColor;
            if (IsPlaying)
            {
                col.A = (byte) ((float)col.A * dx);
            }

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

        public int CurrentZone
        {
            get { return _currentZone; }
            set { _currentZone = value; }
        }


        public int AttackStartZone
        {
            get { return _attackStartZone; }
            set { _attackStartZone = value; }
        }

        public bool PositionLocked
        { get; set; }

        public bool IsPlaying 
        { get; set; }
    }
}
