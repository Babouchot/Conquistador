using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TestXNA.Sources.UIElements
{
    class RadialAnswerContainer : RadialUIContainer
    {

        private Texture2D _centralImg;
        private float _bestAnswerPosition = 0f;
        private string _questionText = "blabla questio blabla";
        private Action<float> _touchCenterCallback;
        private ProgressBar _progressBar;

        public RadialAnswerContainer(Texture2D center, Texture2D progressText, float outterRad, float innerRad, string text, float touchedScale = 1.3f)
            : base(outterRad, innerRad, touchedScale)
        {
            _centralImg = center;
            float width = (float)innerRad * 1.8f;
            Rectangle textArea = new Rectangle(0, 0, (int)width, (int)width);
            _questionText = Utils.fitText(text, MyGame.BasicFont, textArea);

            _progressBar = new ProgressBar(progressText, progressText, textArea);
        }

        public Action<float> TouchCenterCallback
        {
            get { return _touchCenterCallback; }
            set { _touchCenterCallback = value; }
        }

        public float Progress
        {
            get { return _progressBar.Progress; }
            set { _progressBar.Progress = value; }
        }

        protected override bool processTouch(Microsoft.Surface.Core.TouchPoint touch, float dt)
        {
            Vector2 touchPos = Utils.touchPointToV2((touch));


            if (TouchCenterCallback != null && Vector2.Distance(_position, touchPos) < _innerRadius)
            {
                TouchCenterCallback(dt);
            }

            return base.processTouch(touch, dt);
        }

        public override void update(float dt)
        {
            base.update(dt);

            for (int i = 0; i < _containedUIs.Count; ++i)
            {
                PlayerResultUI ui = (PlayerResultUI)_containedUIs[i];

                if (ui.IsBest)
                {
                    _bestAnswerPosition = i;
                }

                ui.Angle = Utils.lookAt(ui.Position, _position) + (float)Math.PI;
                ui.Target = _position;
                ui.Scale = (_outterRadius - _innerRadius) / ui.Height;

                if (!_touchReleased)
                {
                    ui.Scale *= _touchedScale;
                }

                ui.update(dt);

                Rectangle progressArea = _progressBar.Area;
                progressArea.X = (int)(_position.X - _innerRadius);
                progressArea.Y = (int)(_position.Y - _innerRadius);
                progressArea.Width = (int)(_innerRadius * 2f);
                progressArea.Height = (int)(_innerRadius * 2f);

                _progressBar.Area = progressArea;


            }
        }

        public override void draw()
        {
            Vector2 imgSize = new Vector2(_centralImg.Width, _centralImg.Height);
            float imgAngle = _angle + (float)Math.PI * 2f / (float)_containedUIs.Count * _bestAnswerPosition;

            _progressBar.draw();

            MyGame.SpriteBatch.Draw(_centralImg, _position, null, Color.White
               , imgAngle, imgSize / 2f, _innerRadius / imgSize.X * 2f, SpriteEffects.None, 0f);

            MyGame.SpriteBatch.Draw(MyGame.Black, MyGame.MapArea, MyGame.ColorPanel.dialogBackColor);

            base.draw();

            Vector2 textSize = MyGame.BasicFont.MeasureString(_questionText);
            MyGame.SpriteBatch.DrawString(MyGame.BasicFont, _questionText, _position, Color.White
                , _angle, textSize / 2f, 1f, SpriteEffects.None, 0f);

        }
    }
}
