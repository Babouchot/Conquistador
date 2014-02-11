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

namespace MapWorker
{
    /// <summary>
    /// This is the main type for your application.
    /// </summary>
    public class MapWorker : Microsoft.Xna.Framework.Game
    {
        private readonly GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private TouchTarget touchTarget;
        private Color backgroundColor = new Color(81, 81, 81);
        private bool applicationLoadCompleteSignalled;

        private UserOrientation currentOrientation = UserOrientation.Bottom;
        private Matrix screenTransform = Matrix.Identity;



        //My Data

        private SpriteFont playerUIFont;

        private int nbOfZones = 16;
        private string zoneImagesPath = "Resources/Zones/";
        private string resultFilePath = "D:/Cours/Surface/Map.txt";
        private int[,] map;//0 = water // every other number = zone number
        private int step = 20; //size of an area in pixel

        private List<Texture2D> zoneTextures;

        private int currentlyProccessedZone = -1;
        private int currentlyProccessedLine = 0;

        private int mapWidth = 1920;
        private int mapHeight = 1080;

        /// <summary>
        /// The target receiving all surface input for the application.
        /// </summary>
        protected TouchTarget TouchTarget
        {
            get { return touchTarget; }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MapWorker()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        #region Initialization

        /// <summary>
        /// Moves and sizes the window to cover the input surface.
        /// </summary>
        private void SetWindowOnSurface()
        {
            System.Diagnostics.Debug.Assert(Window != null && Window.Handle != IntPtr.Zero,
                "Window initialization must be complete before SetWindowOnSurface is called");
            if (Window == null || Window.Handle == IntPtr.Zero)
                return;

            // Get the window sized right.
            Program.InitializeWindow(Window);
            // Set the graphics device buffers.
            graphics.PreferredBackBufferWidth = Program.WindowSize.Width;
            graphics.PreferredBackBufferHeight = Program.WindowSize.Height;
            graphics.ApplyChanges();
            // Make sure the window is in the right location.
            Program.PositionWindow();
        }

        /// <summary>
        /// Initializes the surface input system. This should be called after any window
        /// initialization is done, and should only be called once.
        /// </summary>
        private void InitializeSurfaceInput()
        {
            System.Diagnostics.Debug.Assert(Window != null && Window.Handle != IntPtr.Zero,
                "Window initialization must be complete before InitializeSurfaceInput is called");
            if (Window == null || Window.Handle == IntPtr.Zero)
                return;
            System.Diagnostics.Debug.Assert(touchTarget == null,
                "Surface input already initialized");
            if (touchTarget != null)
                return;

            // Create a target for surface input.
            touchTarget = new TouchTarget(Window.Handle, EventThreadChoice.OnBackgroundThread);
            touchTarget.EnableInput();
        }

        #endregion

        #region Overridden Game Methods

        /// <summary>
        /// Allows the app to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            IsMouseVisible = true; // easier for debugging not to "lose" mouse
            SetWindowOnSurface();
            InitializeSurfaceInput();

            // Set the application's orientation based on the orientation at launch
            currentOrientation = ApplicationServices.InitialOrientation;

            // Subscribe to surface window availability events
            ApplicationServices.WindowInteractive += OnWindowInteractive;
            ApplicationServices.WindowNoninteractive += OnWindowNoninteractive;
            ApplicationServices.WindowUnavailable += OnWindowUnavailable;

            // Setup the UI to transform if the UI is rotated.
            // Create a rotation matrix to orient the screen so it is viewed correctly
            // when the user orientation is 180 degress different.
            Matrix inverted = Matrix.CreateRotationZ(MathHelper.ToRadians(180)) *
                       Matrix.CreateTranslation(graphics.GraphicsDevice.Viewport.Width,
                                                 graphics.GraphicsDevice.Viewport.Height,
                                                 0);

            if (currentOrientation == UserOrientation.Top)
            {
                screenTransform = inverted;
            }

            map = new int[mapWidth/step, mapHeight/step];

            for (int x = 0; x < mapWidth/step; ++x)
            {
                for (int y = 0; y < mapHeight/step; ++y)
                {
                    map[x, y] = 0;
                }
            }

            try
            {
                fileToMap(resultFilePath);
            }
            catch (Exception e)
            {

            }

            currentlyProccessedZone = 1;
            base.Initialize();

        }


        /// <summary>
        /// LoadContent will be called once per app and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your application content here
            zoneTextures = new List<Texture2D>();
            Stream imageStream;

            for(int i = 1; i <= nbOfZones; ++i)
            {
                imageStream = TitleContainer.OpenStream(zoneImagesPath + (i<10 ? "0" : "") + i +".png");
                Texture2D zoneTex = Texture2D.FromStream(GraphicsDevice, imageStream);
                zoneTextures.Add(zoneTex);
            }


            playerUIFont = Content.Load<SpriteFont>("Fonts/MaturaScript");
            
        }

        /// <summary>
        /// UnloadContent will be called once per app and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        private Color[,] TextureTo2DArray(Texture2D texture)
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
        /// Allows the app to run logic such as updating the world,
        /// checking for collisions, gathering input and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (currentlyProccessedZone <= nbOfZones)
            {

                Texture2D zone = zoneTextures[currentlyProccessedZone - 1];
                Color[,] zoneColors = TextureTo2DArray(zone);
                int y = currentlyProccessedLine;
                //for (int y = 0; y < mapHeight / step; ++y)
                 {
                    for (int x = 0; x < mapWidth / step; ++x)
                    {
                        //if this coordinate is in the current zone
                        if (zoneColors[x * step + step/2, y * step + step/2].A != 0)
                        {
                            map[x, y] = currentlyProccessedZone;
                        }
                    }
                }

                Console.WriteLine("zone " + currentlyProccessedZone + " proccessing");

                ++currentlyProccessedLine;

                if (currentlyProccessedLine >= mapHeight / step)
                {
                    currentlyProccessedLine = 0;
                    ++currentlyProccessedZone;
                }

            }
            else
            {
                mapToFile(resultFilePath);
                Exit();
            }

            base.Update(gameTime);
        }


        private void mapToFile(string filename)
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter(filename);
            for (int y = 0; y < mapHeight / step; ++y)
            {
                string line = "";
                for (int x = 0; x < mapWidth / step; ++x)
                {
                    line += ( map[x, y]<10 ? "0" : "") + map[x, y] + ",";
                }
                file.WriteLine(line);
            }
        }

        private void fileToMap(string filename)
        {
            System.IO.StreamReader file = new System.IO.StreamReader(filename);
            for (int y = 0; y < mapHeight / step; ++y   )
            {
                string line = file.ReadLine();
                if (line == null)
                {
                    Console.WriteLine("no more line to read");
                    break;
                }
                string[] words = line.Split(',');
                //Console.WriteLine("words : " + words.Length + "map : " + mapWidth/step);
                for (int x = 0; x < words.Length - 1; ++x)
                {
                    int zone = 0;
                    int.TryParse(words[x], out zone);
                    map[x, y] = zone;
                }
            }
        }

        /// <summary>
        /// This is called when the app should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {

            GraphicsDevice.Clear(backgroundColor);

            spriteBatch.Begin();

            for (int i = 0; i < zoneTextures.Count; ++i)
            {
                Color drawColor;
                if (i < currentlyProccessedZone - 1)
                {
                    drawColor = Color.White;
                }
                else if (i == currentlyProccessedZone - 1)
                {
                    drawColor = Color.Red;
                }
                else
                {
                    drawColor = Color.Gray;
                }
                spriteBatch.Draw(zoneTextures[i], Vector2.Zero, drawColor);
            }

            for (int x = 0; x < mapWidth / step; ++x)
            {
                for (int y = 0; y < mapHeight / step; ++y)
                {
                    Vector2 offset = playerUIFont.MeasureString(map[x, y].ToString()) / 2;
                    spriteBatch.DrawString(playerUIFont, map[x, y].ToString(), new Vector2(x * step + step / 2, y * step + step / 2) - offset, Color.Black);
                }
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        #endregion

        #region Application Event Handlers

        /// <summary>
        /// This is called when the user can interact with the application's window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowInteractive(object sender, EventArgs e)
        {
            //TODO: Enable audio, animations here

            //TODO: Optionally enable raw image here
        }

        /// <summary>
        /// This is called when the user can see but not interact with the application's window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowNoninteractive(object sender, EventArgs e)
        {
            //TODO: Disable audio here if it is enabled

            //TODO: Optionally enable animations here
        }

        /// <summary>
        /// This is called when the application's window is not visible or interactive.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowUnavailable(object sender, EventArgs e)
        {
            //TODO: Disable audio, animations here

            //TODO: Disable raw image if it's enabled
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Release managed resources.
                IDisposable graphicsDispose = graphics as IDisposable;
                if (graphicsDispose != null)
                {
                    graphicsDispose.Dispose();
                }
                if (touchTarget != null)
                {
                    touchTarget.Dispose();
                    touchTarget = null;
                }
            }

            // Release unmanaged Resources.

            // Set large objects to null to facilitate garbage collection.

            base.Dispose(disposing);
        }

        #endregion
    }
}
