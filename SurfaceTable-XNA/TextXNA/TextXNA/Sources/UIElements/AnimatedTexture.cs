using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace TestXNA.Sources.UIElements
{
    class AnimatedTexture : RotatableUI
    {
        private Texture2D _image;
        private int _frameWidth;
        private int _frameHeight;
        private float _frameDuration;
        private float _currentTime;
        private int _currentFrame;
        private int _nbFrame;
        private bool _loop;


        /// <summary>
        /// The sprite image must be a line of images
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="frameW"></param>
        /// <param name="frameH"></param>
        /// <param name="frameDuration"></param>
        /// <param name="loop"></param>
        public AnimatedTexture(Texture2D texture, int frameW, int frameH, float frameDuration, bool loop)
        {
            _loop = loop;
            _image = texture;
            _frameWidth = frameW;
            _frameHeight = frameH;
            _frameDuration = frameDuration;
            _nbFrame = _image.Width / _frameWidth;
        }

        public override void update(float dt)
        {
            _currentTime += dt;
            if (_currentTime >= _frameDuration)
            {
                ++_currentFrame;
                _currentTime = 0f;

                if (_currentFrame >= _nbFrame && _loop)
                {
                    _currentFrame = 0;
                }
            }
        }

        public override void draw()
        {
            Rectangle sourceRect = new Rectangle(
                _currentFrame * _frameWidth,
                0,
                _frameWidth,
                _frameHeight
                );
            Rectangle area = new Rectangle(0, 0, _frameWidth, _frameHeight);
            MyGame.SpriteBatch.Draw(_image, _position, sourceRect, Color.White, _angle
                , Utils.pointToVector2(area.Center), _scale, SpriteEffects.None, 0f);
        }
    }
}
