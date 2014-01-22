using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Surface.Core;
using Microsoft.Xna.Framework.Graphics;

namespace TestXNA.Sources
{
    class SmallPlayerUI : RotatableUI
    {
        public const float SPEED_COEF = 1.2f;
        public const float NB_MOVE_RECORDED = 20f;

        private int _player;

        private Vector2 _velocity = Vector2.Zero;
        private float _width = -1f;
        private Rectangle _area;
        private float _weigth = 0.7f;

        private int _touchId = -1;
        private bool _touchReleased = true;

        private Texture2D _image;
        private SpriteFont _playerUIFont;


        public SmallPlayerUI(Texture2D img, Vector2 position, int player) 
        {
            _image = img;
            _position = position;
            _player = player;
            _playerUIFont = null;
        }

        public bool processDrag(TouchPoint touch, float dt)
        {
            //If the window is touch or a previous touch is still going on
            if (_area.Contains((int)touch.X, (int)touch.Y) || _touchId == touch.Id)
            {

                Vector2 newPos = new Vector2(touch.X, touch.Y);

                Vector2 bckUpPos = _position;
                _position = newPos;
                Rectangle newArea = computeUIArea(this);
                _position = bckUpPos;

                if (!MyGame.ScreenArea.Contains(newArea) || MyGame.MapArea.Intersects(newArea))
                {
                    newPos = _position;
                }

                _velocity = (_velocity * NB_MOVE_RECORDED + (newPos - _position) / dt * SPEED_COEF) / (NB_MOVE_RECORDED + 1f);

                _position = newPos;
                _area = computeUIArea(this);
                _touchId = touch.Id;
                _touchReleased = false;

                //A touch should only move one UI window
                return true;
            }
            else
            {
                _touchReleased = true;
                return false;
            }

        }

        public override void update(float dt)
        {

            _angle = getUpdatedAngle();

            if (_touchReleased)
            {
                _touchId = -1;

                Vector2 newPos = _position + _velocity * dt;
                _position = newPos;
                _area = computeUIArea(this);

                _velocity -= _velocity * _weigth * dt;
            }
            else
            {
                _touchReleased = true;
            }

            keepIn(MyGame.ScreenArea, dt);
            keepOut(MyGame.MapArea, dt);
        }

        private float getUpdatedAngle()
        {
            float newAngle = 0f;

            //Side boundaries
            if (_position.Y > MyGame.MapArea.Top && _position.Y < MyGame.MapArea.Bottom)
            {
                //Left side
                if (_position.X < MyGame.MapArea.Left)
                {
                    newAngle = (float)Math.PI / 2f;
                }//Right side
                else
                {
                    newAngle = (float)Math.PI * 3f / 2f;
                }
            } // Up & down boundaries
            else
            {
                if (_position.Y < MyGame.MapArea.Top)
                {
                    newAngle = (float)Math.PI;
                }
                else
                {
                    newAngle = 0f;
                }
            }

            return newAngle;
        }

        private void keepIn(Rectangle outRect, float dt)
        {
            if (!outRect.Contains(_area))
            {
                Vector2 correctionVector = Vector2.Zero;
                float correctionAmountPerSec = 50f;
                float correction = correctionAmountPerSec * dt;
                if (_area.Left < outRect.Left)
                {
                    correctionVector.X += correction;
                }

                if (_area.Top < outRect.Top)
                {
                    correctionVector.Y += correction;
                }

                if (_area.Right > outRect.Right)
                {
                    correctionVector.X -= correction;
                }

                if (_area.Bottom > outRect.Bottom)
                {
                    correctionVector.Y -= correction;
                }
                _position += correctionVector;
                _velocity += correctionVector / dt;
                _area = computeUIArea(this);
            }

        }

        private void keepOut(Rectangle inRect, float dt)
        {

            if (inRect.Intersects(_area) || inRect.Contains(_area))
            {
                float correctionAmountPerSec = 100f;
                float correction = correctionAmountPerSec * dt;

                Vector2 correctionVector = Vector2.Zero;

                Vector2 upLeft = new Vector2(inRect.Left, inRect.Top);
                Vector2 upRight = new Vector2(inRect.Right, inRect.Top);
                Vector2 bottomLeft = new Vector2(inRect.Left, inRect.Bottom);
                Vector2 bottomRight = new Vector2(inRect.Right, inRect.Bottom);
                Vector2 middleLeft = new Vector2(inRect.Left, inRect.Y + inRect.Height / 2);
                Vector2 middleRight = new Vector2(inRect.Right, inRect.Y + inRect.Height / 2);
                Vector2 center = new Vector2(inRect.X + inRect.Width / 2, inRect.Y + inRect.Height / 2);
                Vector2 middleTop = new Vector2(inRect.X + inRect.Width / 2, inRect.Top);
                Vector2 middleBottom = new Vector2(inRect.X + inRect.Width / 2, inRect.Bottom);

                Rectangle leftR = new Rectangle((int)upLeft.X, (int)upLeft.Y, inRect.Width / 2, inRect.Height);
                Rectangle rightR = new Rectangle((int)middleTop.X, (int)middleTop.Y, inRect.Width / 2, inRect.Height);
                Rectangle topR = new Rectangle((int)upLeft.X, (int)upLeft.Y, inRect.Width, inRect.Height / 2);
                Rectangle bottomR = new Rectangle((int)middleLeft.X, (int)middleLeft.Y, inRect.Width, inRect.Height / 2);


                //Side boundaries
                if (_area.Top > inRect.Top && _area.Bottom < inRect.Bottom)
                {
                    if (leftR.Intersects(_area))
                    {
                        correctionVector += new Vector2(-1, 0) * correction;
                    }

                    if (rightR.Intersects(_area))
                    {
                        correctionVector += new Vector2(1, 0) * correction;
                    }

                } // Up & down boundaries
                else
                {
                    if (topR.Intersects(_area))
                    {
                        correctionVector += new Vector2(0, -1) * correction;
                    }

                    if (bottomR.Intersects(_area))
                    {
                        correctionVector += new Vector2(0, 1) * correction;
                    }
                }

                _position += correctionVector;
                _velocity += correctionVector / dt;
                _area = computeUIArea(this);
            }
        }

        public override void draw()
        {
            string _name = GameData.PlayerData.Instance[_player].Name;
            Vector2 size = _playerUIFont.MeasureString(_name + "\n" + _name + "\n" + _name);
            Vector2 position = Utils.pointToVector2(_area.Center) -size / 2f;

            MyGame.SpriteBatch.Draw(_image, _area, GameData.PlayerData.Instance[_player].HighlitColor);

            MyGame.SpriteBatch.DrawString(_playerUIFont, _name + "\n" + _name + "\n" + _name, Utils.pointToVector2(_area.Center), Color.Black, _angle, size / 2f, 1f,
                SpriteEffects.None, 0f);
        }

        public void initializeArea(SpriteFont font)
        {
            _playerUIFont = font;
            _width = computeUISize(this, font).X;
            _area = computeUIArea(this);
        }

        private static Rectangle computeUIArea(SmallPlayerUI ui)
        {
            return new Rectangle((int)(ui._position.X - ui._width / 2f),
                (int)(ui._position.Y - ui._width / 2f), (int)ui._width, (int)ui._width);
        }

        private static Vector2 computeUISize(SmallPlayerUI ui, SpriteFont font)
        {
            return font.MeasureString(GameData.PlayerData.Instance[ui._player].Name);
        }
    }
}
