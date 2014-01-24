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
using TestXNA.Sources.GameRooms;

namespace TestXNA
{
    /// <summary>
    /// This is the main type for your application.
    /// </summary>
    public class MyGame : Microsoft.Xna.Framework.Game
    {
        private readonly GraphicsDeviceManager graphics;
        private static SpriteBatch spriteBatch;
        private static ContentManager _content;

        private static TouchTarget touchTarget;
        private Color backgroundColor;
        private bool applicationLoadCompleteSignalled;

        private UserOrientation currentOrientation = UserOrientation.Bottom;
        //private Matrix screenTransform = Matrix.Identity;


        //My data

        private const int _SURFACE_WIDTH = 1920;
        private const int _SURFACE_HEIGHT = 1020;

        private const int _NUMBER_OF_PLAYER = 4;

        private static SpriteFont basicFont;
        private static Rectangle mapArea;
        private static Rectangle screenArea;
        private static Vector2 screenCenter;
        private static Texture2D black;
        private static Texture2D white;
        private static TestXNA.Sources.GameData.GraphicColors _colorPanel;

        private int borderWidth = 100;

        private IRoom _currentRoom;
        private WarRoom _warRoom;
        private WaitingRoom _waitingRoom;

        /// <summary>
        /// The target receiving all surface input for the application.
        /// </summary>
        public static TouchTarget TouchTarget
        {
            get { return touchTarget; }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MyGame()
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
                //screenTransform = inverted;
            }

            myInitialize();

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

            ///TitleContainer.OpenStream
            // TODO: use this.Content to load your application content here
            //popupImage = this.Content.Load<Texture2D>("trollFace.png");
            

            Color[] whiteArray = Enumerable.Range(0, 100).Select(i => Color.White).ToArray();
            white = new Texture2D(GraphicsDevice, 10, 10, false, SurfaceFormat.Color);
            white.SetData(whiteArray);

            Color[] blackArray = Enumerable.Range(0, 100).Select(i => Color.Black).ToArray();
            black = new Texture2D(GraphicsDevice, 10, 10, false, SurfaceFormat.Color);
            black.SetData(blackArray);
            
            /*popupImage = Content.Load<Texture2D>("Images/trollFace.png");
            iconImage = Content.Load<Texture2D>("Images/GameThumbnail.png");
            arrowImage = Content.Load<Texture2D>("Images/Arrow.png");
            mapBackground = Content.Load<Texture2D>("Images/icon.png");
             */

            _content = Content;
            basicFont = Content.Load<SpriteFont>("Fonts/MaturaScript");

            secondInitialize();
        }


        /// <summary>
        /// UnloadContent will be called once per app and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            
            //done automagically ?
            //this.Content.Unload();
            GC.Collect();
        }

        /// <summary>
        /// Allows the app to run logic such as updating the world,
        /// checking for collisions, gathering input and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.Milliseconds / 1000f;

            //if (ApplicationServices.WindowAvailability != WindowAvailability.Unavailable)

            //if (ApplicationServices.WindowAvailability == WindowAvailability.Interactive)

            _currentRoom.update(dt);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the app should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.Milliseconds / 1000f;

            if (!applicationLoadCompleteSignalled)
            {
                // Dismiss the loading screen now that we are starting to draw
                ApplicationServices.SignalApplicationLoadComplete();
                applicationLoadCompleteSignalled = true;
            }

            //TODO: Rotate the UI based on the value of screenTransform here if desired

            GraphicsDevice.Clear(backgroundColor);

            //TODO: Add your drawing code here
            //TODO: Avoid any expensive logic if application is neither active nor previewed
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
            //spriteBatch.Begin();
            drawRoom();

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


        #region myFunctions


        private void myInitialize()
        {
            _colorPanel = new TestXNA.Sources.GameData.GraphicColors();
            backgroundColor = _colorPanel.backgroundColor;
            screenCenter = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);

            mapArea = new Rectangle(borderWidth, borderWidth, GraphicsDevice.Viewport.Width - borderWidth * 2,
                GraphicsDevice.Viewport.Height - borderWidth * 2);

            screenArea = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

        }

        /// <summary>
        /// This is called after all contents have been loaded
        /// </summary>
        private void secondInitialize()
        {
            //construct rooms
            _warRoom = new WarRoom();
            _waitingRoom = new WaitingRoom();
            //_currentRoom = _warRoom;
            _currentRoom = _waitingRoom;
            _waitingRoom.startGameCallback = goToWarRoom;
        }

        private void goToWarRoom()
        {
            Sources.NodeJSClient.ServerCom.Instance.sendSimpleMessage("startGame");
            _currentRoom = _warRoom;
        }


        private void drawRoom()
        {
            _currentRoom.draw();
        }

        #endregion


        #region Static properties

        public static SpriteBatch SpriteBatch
        {
            get { return MyGame.spriteBatch; }
            set { MyGame.spriteBatch = value; }
        }

        public static Texture2D White
        {
            get { return MyGame.white; }
            set { MyGame.white = value; }
        }

        public static Texture2D Black
        {
            get { return MyGame.black; }
            set { MyGame.black = value; }
        }


        public static Rectangle ScreenArea
        {
            get { return MyGame.screenArea; }
            set { MyGame.screenArea = value; }
        }

        public static Vector2 ScreenCenter
        {
            get { return MyGame.screenCenter; }
            set { MyGame.screenCenter = value; }
        }

        public static SpriteFont BasicFont
        {
            get { return MyGame.basicFont; }
        }

        public static Rectangle MapArea
        {
            get { return MyGame.mapArea; }
            set { MyGame.mapArea = value; }
        }

        public static int SURFACE_WIDTH
        {
            get { return _SURFACE_WIDTH; }
        }

        public static int SURFACE_HEIGHT
        {
            get { return _SURFACE_HEIGHT; }
        }


        public static ContentManager ContentManager
        {
            get { return MyGame._content; }
            set { MyGame._content = value; }
        }

        public static int NUMBER_OF_PLAYER
        {
            get { return _NUMBER_OF_PLAYER; }
        }

        public static TestXNA.Sources.GameData.GraphicColors ColorPanel
        {
            get { return MyGame._colorPanel; }
        }

        #endregion
    }
}
