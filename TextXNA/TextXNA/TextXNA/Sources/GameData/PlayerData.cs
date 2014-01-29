using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Surface.Core;
using Microsoft.Xna.Framework.Content;

namespace TestXNA.Sources.GameData
{
    /// <summary>
    /// Hold the color palette (and perhaps other things) for each player 
    /// </summary>
    class PlayerData
    {
        private static PlayerData[] _instance;

        private Color _baseColor;
        private string _name = "";
        private Texture2D _fullImage;
        private Texture2D _faceImage;

        public Texture2D FaceImage
        {
            get { return _faceImage; }
            set { _faceImage = value; }
        }


        public Texture2D FullImage
        {
            get { return _fullImage; }
            set { _fullImage = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
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
                Color col = new Color(_baseColor.R, _baseColor.G, _baseColor.B, _baseColor.A / 2f);
                return col;
            }
        }

        public Color HighlitColor
        {
            get
            {
                Color col = new Color(_baseColor.R, _baseColor.G, _baseColor.B, _baseColor.A * 2f);
                return col;
            }
        }

        public Color OppositeColor
        {
            get
            {
                Color col = new Color(255 - _baseColor.R, 255 - _baseColor.G, 255 - _baseColor.B, _baseColor.A * 2f);
                return col;
            }
        }


        public static PlayerData[] Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PlayerData[4];
                    _instance[0] = new PlayerData(new Color(255, 0, 0, 122));
                    _instance[1] = new PlayerData(new Color(255, 255, 0, 122));
                    _instance[2] = new PlayerData(new Color(0, 255, 0, 122));
                    _instance[3] = new PlayerData(new Color(0, 0, 255, 122));
                }
                return PlayerData._instance;
            }
        }

        public static void loadImages(ContentManager contentLoader)
        {
            for(int i = 0; i < 4; ++i)
            {
                Instance[i].FullImage = contentLoader.Load<Texture2D>("Images/Commanders/Conqui" + (i + 1));
                Instance[i].FaceImage = contentLoader.Load<Texture2D>("Images/Commanders/ConquiFace" + (i + 1));
            }
             
        }
    }
}
