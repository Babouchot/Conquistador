using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Surface;
using Microsoft.Surface.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using TestXNA.Sources;
using TestXNA.Sources.GameData;
using TestXNA.Sources.GameRooms;

namespace TestXNA
{
    class Map
    {

        public class ZoneData
        {
            private Color _currentColor = Color.Transparent;

            private int _owner = -1;
            private int _alphaBlend = 10;


            public ZoneData(int owner)
            {
                _owner = owner;
            }

            public void normalNode()
            {
                if(_owner >= 0 && _owner < PlayerData.Instance.Length)
                {
                    Color col = PlayerData.Instance[_owner].BaseColor;
                    _currentColor = new Color(col.R, col.G, col.B, col.A);
                }
            }

            public void grayedMode()
            {
                if (_owner >= 0 && _owner < PlayerData.Instance.Length)
                {
                    Color col = PlayerData.Instance[_owner].GrayedColor;
                    _currentColor = new Color(col.R, col.G, col.B, (float)col.A / _alphaBlend);
                }
            }

            public void highlightedMode()
            {
                if (_owner >= 0 && _owner < PlayerData.Instance.Length)
                {
                    Color col = PlayerData.Instance[_owner].HighlitColor;
                    _currentColor = new Color(col.R, col.G, col.B, (float)col.A / _alphaBlend);
                }
            }

            public Color CurrentColor
            {
                get { return _currentColor; }
            }

            public int Owner
            {
                get { return _owner; }
                set 
                {
                    _owner = value;
                    normalNode();
                }
            }
        }

        private const int NB_OF_ZONES = 16;

        private string _zoneImagesPath = "Images/Zones/";
        private string _resultFilePath = "Map.txt";
        private int[,] _map;//0 = water // every other number = zone number
        private int _step = 20; //size of an area in pixel

        private List<ZoneData> _zoneData;
        private List<Texture2D> _zoneTextures;

        private int _mapWidth = 1920;
        private int _mapHeight = 1080;


        public Map(int mapWidth, int mapHeight)
        {
            _mapWidth = mapWidth;
            _mapHeight = mapHeight;

            _map = new int[mapWidth / _step, mapHeight / _step];

            clearMap();
        }
        
        private void clearMap()
        {
            for (int x = 0; x < _mapWidth / _step; ++x)
            {
                for (int y = 0; y < _mapHeight / _step; ++y)
                {
                    _map[x, y] = 0;
                }
            }
        }

        public void loadMap()
        {
            // TODO: use this.Content to load your application content here
            _zoneTextures = new List<Texture2D>();
            _zoneData = new List<ZoneData>();


            for (int i = 1; i <= NB_OF_ZONES; ++i)
            {
                Texture2D zoneTex = MyGame.ContentManager.Load<Texture2D>(_zoneImagesPath + (i < 10 ? "0" : "") + i);
                _zoneTextures.Add(zoneTex);
                _zoneData.Add(new ZoneData(-1));
            }


            /*try
            {
                fileToMap(_resultFilePath);
            }
            catch (Exception e)
            {
                Console.WriteLine("A problem occurred while reading the map file : " + e.Message 
                    + "\n re-generating map");

                clearMap();
                generateMapFromTex();

                try
                {
                    Console.WriteLine("Attempt to save map to file : " + _resultFilePath);
                    mapToFile(_resultFilePath);
                    Console.WriteLine("\n Successful");
                }
                catch (Exception e2)
                {
                    Console.WriteLine("\n Failed : " + e2.Message);
                }
            }*/

            clearMap();
            generateMapFromTex();

        }
    
        private void mapToFile(string filename)
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter(filename);
            for (int y = 0; y < _mapHeight / _step; ++y)
            {
                string line = "";
                for (int x = 0; x < _mapWidth / _step; ++x)
                {
                    line += (_map[x, y] < 10 ? "0" : "") + _map[x, y] + ",";
                }
                file.WriteLine(line);
            }
        }

        private void fileToMap(string filename)
        {
            System.IO.StreamReader file = new System.IO.StreamReader(filename);
            for (int y = 0; y < _mapHeight / _step; ++y)
            {
                string line = file.ReadLine();
                if (line == null)
                {
                    Console.WriteLine("no more line to read");
                    break;
                }
                string[] words = line.Split(',');
                //Console.WriteLine("words : " + words.Length + "_map : " + mapWidth/_step);
                for (int x = 0; x < words.Length - 1; ++x)
                {
                    int zone = 0;
                    int.TryParse(words[x], out zone);
                    _map[x, y] = zone;
                }
            }
        }

        private void generateMapFromTex()
        {
            for (int zoneInd = 0; zoneInd < _zoneTextures.Count; ++zoneInd)
            {
                Texture2D zone = _zoneTextures[zoneInd];
                Color[,] zoneColors = Utils.TextureTo2DArray(zone);
                
                for (int y = 0; y < _mapHeight / _step; ++y)
                {
                    for (int x = 0; x < _mapWidth / _step; ++x)
                    {
                        //if this coordinate is in the current zone
                        if (zoneColors[x * _step + _step / 2, y * _step + _step / 2].A != 0)
                        {
                            _map[x, y] = zoneInd + 1;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Draw the zones of the map
        /// </summary>
        public void draw()
        {
            for (int i = 0; i < _zoneTextures.Count; ++i)
            {
                MyGame.SpriteBatch.Draw(_zoneTextures[i], MyGame.MapArea, _zoneData[i].CurrentColor);
            }
        }

        /// <summary>
        /// Set the owner of the designated zone
        /// </summary>
        /// <param name="zone"> between 1 and 16</param>
        /// <param name="owner">between 0 and 3</param>
        public void setZoneOwner(int zone, int owner)
        {
            if (zone > 0 && zone <= _zoneData.Count)
            {
                _zoneData[zone - 1].Owner = owner;
            }
        }

        /// <summary>
        /// Return the zone owner, between 0 and 3
        /// </summary>
        /// <param name="zone"></param>
        /// <returns></returns>
        public int getZoneOwner(int zone)
        {
            if (zone > 0 && zone <= _zoneData.Count)
            {
                return _zoneData[zone - 1].Owner;
            }

            return -100;
        }


        public int getZoneAt(Vector2 pos)
        {
            float xRatio = (float)_mapWidth / (float)MyGame.MapArea.Width;
            float yRatio = (float)_mapHeight / (float)MyGame.MapArea.Height;
            
            pos -= new Vector2(MyGame.MapArea.X, MyGame.MapArea.Y);

            pos.X *= xRatio;
            pos.Y *= yRatio;
            
            int posX = (int)pos.X;
            int posY = (int)pos.Y;

            posX = posX / _step;
            posY = posY / _step;

            if (posX > 0 && posY > 0 && posX < _mapWidth / _step && posY < _mapHeight / _step)
            {
                return _map[posX, posY];
            }
            else
            {
                return -1;
            }
        }

    }
}
