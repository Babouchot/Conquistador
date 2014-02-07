using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Surface.Core;
using System.Net;
using System.Net.Sockets;

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

        /// <summary>
        /// Return local ip address or an empty string if the network is not reachable
        /// </summary>
        /// <returns></returns>
        public static string LocalIPAddress()
        {
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                return "";
            }
            IPHostEntry host;
            string localIP = "";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    break;
                }
            }
            return "http://"+localIP+":8080";
            //return "http://192.168.1.7:8080";
        }

        #region Create Part Image
        /// <summary>
        /// Creates a new image from an existing image.
        /// </summary>
        /// <param name="bounds">Area to use as the new image.</param>
        /// <param name="source">Source image used for getting a part image.</param>
        /// <returns>Texture2D.</returns>
        public static Texture2D CreatePartImage(Rectangle bounds, Texture2D source, GraphicsDevice graphicsDevice)
        {
            //Declare variables
            Texture2D result;
            Color[]
                sourceColors,
                resultColors;

            //Setup the result texture
            result = new Texture2D(graphicsDevice, bounds.Width, bounds.Height);

            //Setup the color arrays
            sourceColors = new Color[source.Height * source.Width];
            resultColors = new Color[bounds.Height * bounds.Width];

            //Get the source colors
            source.GetData<Color>(sourceColors);

            //Loop through colors on the y axis
            for (int y = bounds.Y; y < bounds.Height + bounds.Y; y++)
            {
                //Loop through colors on the x axis
                for (int x = bounds.X; x < bounds.Width + bounds.X; x++)
                {
                    //Get the current color
                    resultColors[x - bounds.X + (y - bounds.Y) * bounds.Width] =
                        sourceColors[x + y * source.Width];
                }
            }

            //Set the color data of the result image
            result.SetData<Color>(resultColors);
            
            //return the result
            return result;
        }
        #endregion


        public static string fitText(string text, SpriteFont font, Rectangle area)
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

    }
}
