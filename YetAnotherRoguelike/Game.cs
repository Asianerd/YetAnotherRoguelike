using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using YetAnotherRoguelike.PhysicsObject;
using YetAnotherRoguelike.Graphics;
using YetAnotherRoguelike.Tile_Classes;

namespace YetAnotherRoguelike
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        public static Game Instance;

        public delegate void ScreenEvents();
        public static ScreenEvents OnScreenResize;

        public static GraphicsDeviceManager graphics;
        public static SpriteBatch spriteBatch;
        public static Vector2 screenSize = new Vector2(1920, 1080);
        //public static Vector2 screenSize = new Vector2(500, 500);
        //public static Vector2 screenSize = new Vector2(1000, 1000);
        public static Rectangle playArea = new Rectangle(0, 0, 0, 0); // measured in tile-coordinates
        public static Texture2D emptySprite;
        public static SpriteFont mainFont;

        public static float compensation = 1f;
        public static float updateFrequency = 60; // treat this as a multiplier to time delta
        // time delta * frequency = speed multiplier
        // distance travelled per second is constant, no matter the fps

        public static KeyboardState keyboardState;
        public static MouseState mouseState;

        public static Random random = new Random(8192);
        public static int seed = random.Next(0, 10000);

        public static float fps = 0f;
        public static int fpsOffset = 0; // a little timer just for ease of reading fps
        public static bool showDebug = false;

        public Game()
        {
            Instance = this;

            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = (int)screenSize.X;
            graphics.PreferredBackBufferHeight = (int)screenSize.Y;
            graphics.IsFullScreen = screenSize == new Vector2(1920, 1080);

            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += (s, e) =>
            {
                screenSize = new Vector2(graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height);
                OnScreenResize(); // might be null
            };

            IsFixedTimeStep = false;

            graphics.ApplyChanges();

            Content.RootDirectory = "Content";
            IsMouseVisible = false;
        }

        protected override void Initialize()
        {
            Data.JSON_BlockData.Initialize();

            PerlinNoise.Initialize();
            var _ = new GaussianBlur();
            Lightmap.Initialize();
            Particle.Initialize();

            GeneralDependencies.Initialize();

            UI.UI_Container.Initialize();
            Input.Initialize(new List<Keys>() {
                Keys.Q,
                Keys.W,
                Keys.E,
                Keys.R,
                Keys.T,
                Keys.Y,
                Keys.U,
                Keys.I,
                Keys.O,
                Keys.P,
                Keys.A,
                Keys.S,
                Keys.D,
                Keys.F,
                Keys.G,
                Keys.H,
                Keys.J,
                Keys.K,
                Keys.L,
                Keys.Z,
                Keys.X,
                Keys.C,
                Keys.V,
                Keys.B,
                Keys.N,
                Keys.M,

                Keys.Space,
                Keys.LeftShift,

                Keys.F1,
                Keys.F11,
                Keys.Tab,

                Keys.Up,
                Keys.Down,
                Keys.Left,
                Keys.Right,

                Keys.D0,
                Keys.D1,
                Keys.D2,
                Keys.D3,
                Keys.D4,
                Keys.D5,
                Keys.D6,
                Keys.D7,
                Keys.D8,
                Keys.D9,
            });
            MouseInput.Initialize();
            Cursor.Initialize();

            Scene.Initialize();

            // move to main game scene init?
            Item.Initialize();
            GroundItem.Initialize();
            Tile.Initialize();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            mainFont = Instance.Content.Load<SpriteFont>("Fonts/defaultFont");
            emptySprite = Instance.Content.Load<Texture2D>("blank");
        }

        protected override void Update(GameTime gameTime)
        {
            compensation = (float)gameTime.ElapsedGameTime.TotalSeconds * updateFrequency;

            #region Input related
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            keyboardState = Keyboard.GetState();
            mouseState = Mouse.GetState();

            MouseInput.UpdateAll();
            Input.UpdateAll(keyboardState);

            if (Input.collection[Keys.F11].active)
            {
                graphics.ToggleFullScreen();
            }

            if (Input.collection[Keys.F1].active)
            {
                showDebug = !showDebug;
            }
            #endregion

            Cursor.Update();

            Camera.Update();
            Scene.currentScene.Update();

            base.Update(gameTime);

            playArea = new Rectangle(
                ((int)((Camera.position.X - (screenSize.X / 2f)) / Tile.tileSize)) - 3,
                ((int)((Camera.position.Y - (screenSize.Y / 2f)) / Tile.tileSize)) - 3,
                ((int)(screenSize.X / Tile.tileSize)) + 6,
                ((int)(screenSize.Y / Tile.tileSize)) + 6
                );
        }

        protected override void Draw(GameTime gameTime)
        {
            fpsOffset++;
            if (fpsOffset >= 60)
            {
                fpsOffset = 0;
                fps = (float)Math.Round(1f / gameTime.ElapsedGameTime.TotalSeconds);
            }

            GraphicsDevice.Clear(Scene.currentScene.backgroundColor);

            Scene.currentScene.Draw();

            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            string debugText = $"FPS : {fps}\n" +
                $"{screenSize}\n" +
                $"Pos : {(int)Player.Instance.position.X}:{(int)Player.Instance.position.Y}\n" +
                $"Lights : {LightSource.sources.Count}\n" +
                $"Ground items : {GroundItem.collection.Count}";
            spriteBatch.Draw(emptySprite, new Rectangle(new Point(0, 0), mainFont.MeasureString(debugText).ToPoint()), Color.Black * 0.2f);
            spriteBatch.DrawString(mainFont, debugText, Vector2.Zero, Color.White);

            Cursor.Draw();
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
