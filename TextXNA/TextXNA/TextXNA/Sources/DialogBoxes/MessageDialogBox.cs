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
    class MessageDialogBox : DialogBox
    {
        protected float backAlpha = 0.7f;
        protected string _message;
        protected Rectangle _area;

        public MessageDialogBox(string content, Rectangle area)
        {
            _message = fitText(content, MyGame.BasicFont, area);
            updateArea();
        }


        protected void updateArea()
        {
            Vector2 stringSize = MyGame.BasicFont.MeasureString(_message);
            Vector2 start = stringSize / 2;
            _area = new Rectangle((int)(start.X - stringSize.X / 2), (int)(start.Y - stringSize.Y / 2)
                , (int)(stringSize.X), (int)(stringSize.Y));
        }

        protected override bool isTouchOnUI(Vector2 touch)
        {
            Vector2 vec = new Vector2(_area.Right, _area.Bottom);

            return Vector2.Distance(touch, _position) < vec.Length();
        }

        protected string fitText(string text, SpriteFont font, Rectangle area)
        {
            string fittedText = "";

            string[] words = text.Split(' ');
            int lineStart = 0;

            while (lineStart < words.Length)
            {
                string line = "";

                while (lineStart < words.Length &&
                    (font.MeasureString(line + words[lineStart]).X < area.Width
                    ||
                    line == "" && lineStart == words.Length - 1))
                {
                    line += words[lineStart] + " ";
                    ++lineStart;
                }

                line += "\n";
                fittedText += line;
            }

            return fittedText;
        }

        public override void draw()
        {
            if (IsShown)
            {
                Color backColor = new Color(1f, 1f, 1f, backAlpha);
                Vector2 areaCenter = new Vector2(_area.Center.X, _area.Center.Y);
                MyGame.SpriteBatch.Draw(MyGame.Black, MyGame.ScreenArea, backColor);
                MyGame.SpriteBatch.DrawString(MyGame.BasicFont, _message, _position
                    , Color.OrangeRed, _angle, areaCenter, 1f, SpriteEffects.None, 0f);

            }
            base.draw();
        }

    }
}
