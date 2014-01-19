using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Surface.Core;

namespace TestXNA.Sources.GameData
{
    /// <summary>
    /// Hold the color palette (and perhaps other things) for each player 
    /// </summary>
    class PlayerData
    {
        private static PlayerData[] _instance;

        private Color _baseColor;
        private string name = "";


        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public PlayerData(Color baseCol)
        {
            _baseColor = baseCol;
        }

        public Color BaseColor
        {
            get { return _baseColor; }
            set { _baseColor = value; }
        }

        public Color GrayedColor
        {
            get 
            { 
                Color grayCol = new Color(_baseColor.R, _baseColor.G, _baseColor.B, _baseColor.A / 2f);
                return grayCol;
            }
        }

        public Color HighlitColor
        {
            get
            {
                Color grayCol = new Color(_baseColor.R, _baseColor.G, _baseColor.B, _baseColor.A * 2f);
                return grayCol;
            }
        }


        internal static PlayerData[] Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PlayerData[4];
                    _instance[0] = new PlayerData(new Color(0, 255, 0, 122));
                    _instance[1] = new PlayerData(new Color(255, 0, 0, 122));
                    _instance[2] = new PlayerData(new Color(0, 0, 255, 122));
                    _instance[3] = new PlayerData(new Color(255, 255, 0, 122));
                }
                return PlayerData._instance;
            }
        }
    }
}
