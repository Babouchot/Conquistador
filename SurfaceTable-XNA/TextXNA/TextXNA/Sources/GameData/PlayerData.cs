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

        private Color _textColor;
        private Color _baseColor;
        private string _name = "";
        private int _score = 0;
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

        public PlayerData(Color baseCol, Color textCol)
        {
            _baseColor = baseCol;
            _textColor = textCol;
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
            get { return _textColor; }
        }


        public static PlayerData[] Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PlayerData[4];
                    _instance[0] = new PlayerData(new Color(255, 0, 0, 122), new Color(0, 0, 0, 255));
                    _instance[1] = new PlayerData(new Color(255, 255, 0, 122), new Color(0, 0, 0, 255));
                    _instance[2] = new PlayerData(new Color(0, 255, 0, 122), new Color(0, 0, 0, 255));
                    _instance[3] = new PlayerData(new Color(0, 0, 255, 122), new Color(255, 255, 255, 255));
                }
                return PlayerData._instance;
            }
        }

        public int Score
        {
            get { return _score; }
            set { _score = value; }
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
