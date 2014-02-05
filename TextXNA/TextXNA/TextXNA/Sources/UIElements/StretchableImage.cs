using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TestXNA.Sources.UIElements
{
    class StretchableImage
    {
        private Rectangle _stretchableArea;
        private SplitedImage _image;
        private SplitedRect _rect;


        private class SplitedImage
        {
            public Texture2D _topLeft;
            public Texture2D _topRight;
            public Texture2D _topCenter;
            public Texture2D _center;
            public Texture2D _left;
            public Texture2D _right;
            public Texture2D _bottomLeft;
            public Texture2D _bottomRight;
            public Texture2D _bottomCenter;

            public Texture2D[] toArray()
            {
                return new Texture2D[] { _topLeft, _topRight, _topCenter, _left, _right, _bottomLeft, _bottomRight
                , _bottomCenter, _center};
            }
        }

        private class SplitedRect
        {
            public Rectangle topRightR;
            public Rectangle topLeftR;
            public Rectangle topCenterR;
            public Rectangle leftR;
            public Rectangle rightR;
            public Rectangle bottomLeftR;
            public Rectangle bottomRightR;
            public Rectangle bottomCenterR;
            public Rectangle centerR;

            public Rectangle[] toArray()
            {
                return new Rectangle[] { topLeftR, topRightR, topCenterR, leftR, rightR, bottomLeftR, bottomRightR, bottomCenterR, centerR };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <param name="stretchArea">rectangle defining the center area in percentage (0-100)</param>
        public StretchableImage(Texture2D image, Rectangle stretchArea)
        {
            _stretchableArea = stretchArea;

            Rectangle imgBounds = new Rectangle(0, 0, image.Width, image.Height);

            _rect = splitRect(imgBounds, stretchArea);
            _image = splitImage(_rect, image);
        }

        private SplitedRect splitRect(Rectangle boundingRect, Rectangle stretchArea)
        {
            SplitedRect splited = new SplitedRect();

            splited.centerR = new Rectangle(
                boundingRect.Left + (stretchArea.X * boundingRect.Width)/100,
                boundingRect.Top + (stretchArea.Y * boundingRect.Height)/100,
                (stretchArea.Width * boundingRect.Width)/100,
                (stretchArea.Height * boundingRect.Height) / 100
                );

            splited.topLeftR = new Rectangle(boundingRect.Left, boundingRect.Top
                , splited.centerR.Left - boundingRect.Left, splited.centerR.Top - boundingRect.Top);

            splited.topRightR = new Rectangle(splited.centerR.Right, boundingRect.Top
                , boundingRect.Right - splited.centerR.Right, splited.centerR.Top - boundingRect.Top);

            splited.topCenterR = new Rectangle(splited.centerR.Left, boundingRect.Top
                , splited.centerR.Width, splited.centerR.Top - boundingRect.Top);

            splited.leftR = new Rectangle(boundingRect.Left, splited.centerR.Top
                , splited.centerR.Left - boundingRect.Left, splited.centerR.Height);

            splited.rightR = new Rectangle(splited.centerR.Right, splited.centerR.Top
            , boundingRect.Right - splited.centerR.Right, splited.centerR.Height);

            splited.bottomLeftR = new Rectangle(boundingRect.Left, splited.centerR.Bottom
                , splited.centerR.Left - boundingRect.Left, boundingRect.Bottom - splited.centerR.Bottom);

            splited.bottomRightR = new Rectangle(splited.centerR.Right, splited.centerR.Bottom
                , boundingRect.Right - splited.centerR.Right, boundingRect.Bottom - splited.centerR.Bottom);

            splited.bottomCenterR = new Rectangle(splited.centerR.Left, splited.centerR.Bottom
                , splited.centerR.Width, boundingRect.Bottom - splited.centerR.Bottom);

            return splited;
        }



        private SplitedRect splitRect2(Rectangle boundingRect)
        {
            SplitedRect splited = new SplitedRect();

            splited.centerR = new Rectangle(
                boundingRect.Left + _rect.topLeftR.Width,
                boundingRect.Top + _rect.topLeftR.Height,
                boundingRect.Width - _rect.topLeftR.Width * 2,
                boundingRect.Height - _rect.topLeftR.Height * 2
                );

            splited.topLeftR = new Rectangle(boundingRect.Left, boundingRect.Top
                , splited.centerR.Left - boundingRect.Left, splited.centerR.Top - boundingRect.Top);

            splited.topRightR = new Rectangle(splited.centerR.Right, boundingRect.Top
                , boundingRect.Right - splited.centerR.Right, splited.centerR.Top - boundingRect.Top);

            splited.topCenterR = new Rectangle(splited.centerR.Left, boundingRect.Top
                , splited.centerR.Width, splited.centerR.Top - boundingRect.Top);

            splited.leftR = new Rectangle(boundingRect.Left, splited.centerR.Top
                , splited.centerR.Left - boundingRect.Left, splited.centerR.Height);

            splited.rightR = new Rectangle(splited.centerR.Right, splited.centerR.Top
            , boundingRect.Right - splited.centerR.Right, splited.centerR.Height);

            splited.bottomLeftR = new Rectangle(boundingRect.Left, splited.centerR.Bottom
                , splited.centerR.Left - boundingRect.Left, boundingRect.Bottom - splited.centerR.Bottom);

            splited.bottomRightR = new Rectangle(splited.centerR.Right, splited.centerR.Bottom
                , boundingRect.Right - splited.centerR.Right, boundingRect.Bottom - splited.centerR.Bottom);

            splited.bottomCenterR = new Rectangle(splited.centerR.Left, splited.centerR.Bottom
                , splited.centerR.Width, boundingRect.Bottom - splited.centerR.Bottom);

            return splited;
        }

        private SplitedImage splitImage(SplitedRect rect, Texture2D image)
        {
            SplitedImage splitedImg = new SplitedImage();

            splitedImg._topLeft = Utils.CreatePartImage(rect.topLeftR, image, MyGame.SpriteBatch.GraphicsDevice);
            splitedImg._topRight = Utils.CreatePartImage(rect.topRightR, image, MyGame.SpriteBatch.GraphicsDevice);
            splitedImg._topCenter = Utils.CreatePartImage(rect.topCenterR, image, MyGame.SpriteBatch.GraphicsDevice);
            splitedImg._left = Utils.CreatePartImage(rect.leftR, image, MyGame.SpriteBatch.GraphicsDevice);
            splitedImg._right = Utils.CreatePartImage(rect.rightR, image, MyGame.SpriteBatch.GraphicsDevice);
            splitedImg._bottomLeft = Utils.CreatePartImage(rect.bottomLeftR, image, MyGame.SpriteBatch.GraphicsDevice);
            splitedImg._bottomRight = Utils.CreatePartImage(rect.bottomRightR, image, MyGame.SpriteBatch.GraphicsDevice);
            splitedImg._bottomCenter = Utils.CreatePartImage(rect.bottomCenterR, image, MyGame.SpriteBatch.GraphicsDevice);
            splitedImg._center = Utils.CreatePartImage(rect.centerR, image, MyGame.SpriteBatch.GraphicsDevice);

            return splitedImg;
        }

        public void draw(Rectangle targetRect, Color color, float angle)
        {
            SplitedRect drawRect = splitRect2(targetRect);
            Rectangle[] arrayRect = drawRect.toArray();
            Texture2D[] arrayImage = _image.toArray();

            Vector2 imageCenter = Utils.pointToVector2(drawRect.centerR.Center);

            for (int i = 0; i < arrayImage.Length; ++i)
            {
                Vector2 rectCenter = Utils.pointToVector2(arrayRect[i].Center);
                float length = Vector2.Distance(rectCenter, imageCenter);
                Vector2 origin = new Vector2((float)arrayImage[i].Width / 2f, (float)arrayImage[i].Height / 2f);
                Vector2 scale = new Vector2((float)arrayRect[i].Width / (float)arrayImage[i].Width, (float)arrayRect[i].Height / (float)arrayImage[i].Height);


                Vector2 newCenter;

                if (rectCenter == imageCenter)
                {
                    newCenter = rectCenter;
                }
                else
                {
                    float positionAngle = Utils.directionToAngle(rectCenter - imageCenter);
                    positionAngle += angle;

                    newCenter = new Vector2((float)Math.Cos(positionAngle), (float)Math.Sin(positionAngle));
                    newCenter *= length;
                    newCenter += imageCenter;
                }
                MyGame.SpriteBatch.Draw(arrayImage[i], newCenter, null, Color.White, angle, origin, scale, SpriteEffects.None, 0f);

                //MyGame.SpriteBatch.Draw(arrayImage[i], arrayRect[i], color);
            }
            //MyGame.SpriteBatch.Draw(_image._center, drawRect.centerR, color);
            /*MyGame.SpriteBatch.Draw(_image._topLeft, drawRect.topLeftR, color);
            MyGame.SpriteBatch.Draw(_image._topRight, drawRect.topRightR, color);
            MyGame.SpriteBatch.Draw(_image._left, drawRect.leftR, color);
            MyGame.SpriteBatch.Draw(_image._right, drawRect.rightR, color);
            MyGame.SpriteBatch.Draw(_image._bottomLeft, drawRect.bottomLeftR, color);
            MyGame.SpriteBatch.Draw(_image._bottomRight, drawRect.bottomRightR, color);
            MyGame.SpriteBatch.Draw(_image._topCenter, drawRect.topCenterR, color);
            MyGame.SpriteBatch.Draw(_image._bottomCenter, drawRect.bottomCenterR, color);*/


            /*MyGame.SpriteBatch.Draw(_image._topLeft, _rect.topLeftR, color);
            MyGame.SpriteBatch.Draw(_image._topRight, _rect.topRightR, color);
            MyGame.SpriteBatch.Draw(_image._left, _rect.leftR, color);
            MyGame.SpriteBatch.Draw(_image._right, _rect.rightR, color);
            MyGame.SpriteBatch.Draw(_image._bottomLeft, _rect.bottomLeftR, color);
            MyGame.SpriteBatch.Draw(_image._bottomRight, _rect.bottomRightR, color);
            MyGame.SpriteBatch.Draw(_image._topCenter, _rect.topCenterR, color);
            MyGame.SpriteBatch.Draw(_image._bottomCenter, _rect.bottomCenterR, color);*/
        }
    }
}
