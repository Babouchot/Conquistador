using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Surface.Core;

namespace TestXNA.Sources
{
    class Utils
    {
        public delegate bool conditionChecker();
        public delegate void actionCallback();


        public class Line
        {
            public Line(Vector2 startV, Vector2 endV)
            {
                start = startV;
                end = endV;
            }

            public Vector2 start;
            public Vector2 end;
        }

        public static Vector2 touchPointToV2(TouchPoint touch)
        {
            return new Vector2(touch.X, touch.Y);
        }

        public static Vector2 pointToVector2(Point point)
        {
            return new Vector2(point.X, point.Y);
        }

        public static float bumpy(float x)
        {
            return Math.Max(0f, (float)Math.Cos(x * 4f - 2f));
        }

        public static float hill(float x)
        {
            return Math.Max(0f, (float)x * 1.6f - (float)Math.Abs(0.4 - x) * x);
        }

        /// <summary>
        /// Return the rotation needed for a sprite at the position to face the target
        /// </summary>
        /// <param name="position"></param>
        /// <param name="targetPosition"></param>
        /// <returns></returns>
        public static float lookAt(Vector2 position, Vector2 targetPosition)
        {
            Vector2 direction = position - targetPosition;
            direction.Normalize();

            return (float)(Math.Atan2(direction.Y, direction.X) + Math.PI / 2d);
        }


        public static float directionToAngle(Vector2 direction)
        {
            direction.Normalize();
            return (float)Math.Atan2(direction.Y, direction.X);
        }

        public static Color[,] TextureTo2DArray(Texture2D texture)
        {
            Color[] colors1D = new Color[texture.Width * texture.Height];
            texture.GetData(colors1D);

            Color[,] colors2D = new Color[texture.Width, texture.Height];
            for (int x = 0; x < texture.Width; x++)
            {
                for (int y = 0; y < texture.Height; y++)
                {
                    colors2D[x, y] = colors1D[x + y * texture.Width];
                }
            }

            return colors2D;
        }
        
    }
}
