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
            public List<int> accecibleZones;


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


        private static List<int>[] _nearFieldData = new List<int>[] 
            {
                new List<int>() {2, 3, 6, 7}, //zone 1
                new List<int>() {1, 3, 4, 5, 7, 10}, //zone 2
                new List<int>() {1, 2, 4}, //zone 3
                new List<int>() {2, 3, 5}, //zone 4
                new List<int>() {2, 4, 10, 9}, //zone 5
                new List<int>() {1, 7, 16}, //zone 6
                new List<int>() {6, 1, 2, 10, 16}, //zone 7
                new List<int>() {9, 10, 11, 12, 13}, //zone 8
                new List<int>() {8, 5, 10, 13}, //zone 9
                new List<int>() {2, 7, 5, 9, 8}, //zone 10
                new List<int>() {8, 12, 15, 16}, //zone 11
                new List<int>() {8, 11, 13, 15, 14}, //zone 12
                new List<int>() {14, 9, 8, 12}, //zone 13
                new List<int>() {12, 13, 15}, //zone 14
                new List<int>() {11, 12, 14}, //zone 15
                new List<int>() {11, 7, 6} //zone 16
            };

        private static Dictionary<int, Vector2> _zoneCenters = new Dictionary<int, Vector2>();


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
                _zoneData[i - 1].accecibleZones = _nearFieldData[i - 1];
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

            _zoneCenters[1] = new Vector2(292, 812);
            _zoneCenters[2] = new Vector2(550, 562);
            _zoneCenters[3] = new Vector2(152, 244);
            _zoneCenters[4] = new Vector2(482, 164);
            _zoneCenters[5] = new Vector2(714, 306);
            _zoneCenters[6] = new Vector2(658, 928);
            _zoneCenters[7] = new Vector2(712, 750);
            _zoneCenters[8] = new Vector2(1366, 566);
            _zoneCenters[9] = new Vector2(1304, 344);
            _zoneCenters[10] = new Vector2(992, 400);
            _zoneCenters[11] = new Vector2(1384, 826);
            _zoneCenters[12] = new Vector2(1532, 642);
            _zoneCenters[13] = new Vector2(1552, 310);
            _zoneCenters[14] = new Vector2(1822, 394);
            _zoneCenters[15] = new Vector2(1616, 874);
            _zoneCenters[16] = new Vector2(1164, 824);

            for (int zoneInd = 0; zoneInd < _zoneTextures.Count; ++zoneInd)
            {
                Texture2D zone = _zoneTextures[zoneInd];
                Color[,] zoneColors = Utils.TextureTo2DArray(zone);
                
                for (int y = 0; y < _mapHeight / _step; ++y)
                {
                    for (int x = 0; x < _mapWidth / _step; ++x)
                    {
                        Color zoneColor = zoneColors[x * _step + _step / 2, y * _step + _step / 2];
                        //if this coordinate is in the current zone
                        if (zoneColor.A != 0)
                        {
                            /*if (zoneColor.R == 0)
                            {
                                _zoneCenters[zoneInd + 1] = new Vector2(x, y);
                            }*/
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

            //drawDebug();
        }

        public void drawDebug()
        {

            for (int i = 0; i < _zoneTextures.Count; ++i)
            {
                //MyGame.SpriteBatch.Draw(_zoneTextures[i], MyGame.MapArea, Color.White);
                MyGame.SpriteBatch.DrawString(MyGame.BasicFont, (i+1).ToString(), getCenterOfZone(i+1), Color.Black, 0f,
                    MyGame.BasicFont.MeasureString((i+1).ToString()) / 2f, 2f, SpriteEffects.None, 0f);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="zone">between 1 to 16</param>
        /// <returns></returns>
        public Vector2 getCenterOfZone(int zone)
        {
            if (zone == -1)
            {
                return Vector2.Zero;
            }

            Vector2 center = Vector2.Zero;
            
            Vector2 sum = Vector2.Zero;

            sum = _zoneCenters[zone];

            float xRatio = (float)_mapWidth / (float)MyGame.MapArea.Width;
            float yRatio = (float)_mapHeight / (float)MyGame.MapArea.Height;

            center = new Vector2(sum.X / xRatio, sum.Y / yRatio);
            center.X += MyGame.MapArea.X;
            center.Y += MyGame.MapArea.Y;

            return center;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="zone">1 .. 16</param>
        /// <returns></returns>
        public List<int> getZonesReachableFrom(int zone)
        {
            return _zoneData[zone - 1].accecibleZones;
        }

        public int getClosestZoneOfOwner(int owner, int currentZone)
        {
            Vector2 currentCenter = getCenterOfZone(currentZone);

            int closestZone = -1;
            Vector2 closestCenter = new Vector2(-100000, -100000);

            for (int zone = 1; zone <= 16; ++zone)
            {
                if (getZoneOwner(zone) == owner)
                {
                    Vector2 zoneCenter = getCenterOfZone(zone);

                    if (Vector2.Distance(zoneCenter, currentCenter) < Vector2.Distance(closestCenter, currentCenter))
                    {
                        closestCenter = zoneCenter;
                        closestZone = zone;
                    }
                }
            }

            return closestZone;
        }

        public int getNbOfZonesForOwner(int owner)
        {
            int count = 0;

            foreach (ZoneData zone in _zoneData)
            {
                if (zone.Owner == owner)
                {
                    ++count;
                }
            }

            return count;
        }

    }
}
