using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TestXNA.Sources.UIElements
{
    class RadialAnswerContainer : RadialUIContainer
    {

        private Texture2D _centralImg;
        private float _bestAnswerPosition = 0f;
        private string _questionText = "How many ping pong balls does Gwenn has ?";

        public RadialAnswerContainer(Texture2D center, float outterRad, float innerRad, float touchedScale = 1.3f)
            : base(outterRad, innerRad, touchedScale)
        {
            _centralImg = center;
            float width = (float)innerRad * 1f;
            Rectangle textArea = new Rectangle(0, 0, (int)width , (int)width);
            _questionText = Utils.fitText(_questionText, MyGame.BasicFont, textArea);
        }

        public override void update(float dt)
        {
            base.update(dt);

            for (int i = 0; i < _containedUIs.Count; ++i)
            {
                AnswerUI ui = (AnswerUI) _containedUIs[i];

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
            }
        }

        public override void draw()
        {
            Vector2 imgSize = new Vector2(_centralImg.Width, _centralImg.Height);
            float imgAngle = _angle + (float)Math.PI * 2f / (float)_containedUIs.Count * _bestAnswerPosition;
            MyGame.SpriteBatch.Draw(_centralImg, _position, null, Color.White, imgAngle, imgSize / 2f, _innerRadius / imgSize.X, SpriteEffects.None, 0f);

            MyGame.SpriteBatch.Draw(MyGame.Black, MyGame.MapArea, MyGame.ColorPanel.dialogBackColor);

            base.draw();

            Vector2 textSize = MyGame.BasicFont.MeasureString(_questionText);
            MyGame.SpriteBatch.DrawString(MyGame.BasicFont, _questionText, _position, Color.White, _angle, textSize / 2f, 1f, SpriteEffects.None, 0f);
            //MyGame.SpriteBatch.DrawString(MyGame.BasicFont, _questionText, _position, Color.Black, _angle, textSize / 2f, 1f, SpriteEffects.None, 0f);
        }
    }
}
