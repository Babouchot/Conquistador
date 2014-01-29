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
        private int imgOffset = 80;
        protected string _message;
        protected Rectangle _area;
        protected UIElements.StretchableImage _backTexture;

        public MessageDialogBox(string content, Rectangle area, UIElements.StretchableImage back)
        {
            _backTexture = back;
            _message = Utils.fitText(content, MyGame.BasicFont, area);
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

        public override void draw()
        {
            if (IsShown)
            {
                Color backColor = MyGame.ColorPanel.dialogBackColor;
                Vector2 areaCenter = new Vector2(_area.Center.X, _area.Center.Y);
                MyGame.SpriteBatch.Draw(MyGame.White, MyGame.MapArea, backColor);

                Rectangle imgRect = new Rectangle((int)_position.X - _area.Width / 2, (int)_position.Y - _area.Height / 2, _area.Width, _area.Height);
                imgRect.X -= imgOffset;
                imgRect.Y -= imgOffset;
                imgRect.Width += imgOffset * 2;
                imgRect.Height += imgOffset * 2;

                //MyGame.SpriteBatch.Draw(MyGame.Black, imgRect, Color.White);

                _backTexture.draw(imgRect, Color.White);

                MyGame.SpriteBatch.DrawString(MyGame.BasicFont, _message, _position
                    , MyGame.ColorPanel.textColor, _angle, areaCenter, 1f, SpriteEffects.None, 0f);

            }
            base.draw();
        }

    }
}
