using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using YetAnotherRoguelike.Gameplay;
using YetAnotherRoguelike.Graphics;

namespace YetAnotherRoguelike
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        public static Game Instance;

        public static GraphicsDeviceManager graphics;
        public static SpriteBatch spriteBatch;
        public static Vector2 screenSize = new Vector2(1920, 1080);
        //public static Vector2 screenSize = new Vector2(500, 500);
        //public static Vector2 screenSize = new Vector2(1000, 1000);
        public static Rectangle playArea = new Rectangle(0, 0, 0, 0);

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

            IsFixedTimeStep = false;

            graphics.ApplyChanges();

            Content.RootDirectory = "Content";
            IsMouseVisible = false;
        }

        protected override void Initialize()
        {
            var _ = new GaussianBlur();
            Lightmap.Initialize();
            playArea = new Rectangle(Vector2.Zero.ToPoint(), screenSize.ToPoint());

            Perlin_Noise.Initialize();

            GeneralDependencies.Initialize();
            var x = new Camera();

            Item.Initialize(); // item has to be initialized before cursor lmao
            UI.Initialize();
            Scene.Initialize();

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

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            compensation = (float)gameTime.ElapsedGameTime.TotalSeconds * updateFrequency;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            keyboardState = Keyboard.GetState();
            mouseState = Mouse.GetState();
            Cursor.Update();
            

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

            UI_Container.Update();

            Scene.sceneLibrary[Scene.activeScene].Update(gameTime);

            playArea = new Rectangle(((-1f * Camera.Instance.renderOffset) - (Vector2.One * 100)).ToPoint(), (screenSize + (Vector2.One * 200)).ToPoint());

            base.Update(gameTime);

            Lightmap.Update();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Scene.sceneLibrary[Scene.activeScene].backgroundColor);

            fpsOffset++;
            if (fpsOffset >= 60)
            {
                fpsOffset = 0;
                fps = (float)Math.Round(1f / gameTime.ElapsedGameTime.TotalSeconds);
            }

            Matrix renderPosition = Matrix.CreateTranslation(new Vector3(Camera.Instance.renderOffset, 0));
            //spriteBatch.Begin(SpriteSortMode.Immediate, blendState:BlendState.AlphaBlend, samplerState: SamplerState.PointClamp, transformMatrix: renderPosition);
            //shader.CurrentTechnique.Passes[0].Apply();
            // TODO : Add Gaussian blur
            //spriteBatch.Draw(Lightmap.final, new Rectangle((-Camera.Instance.renderOffset).ToPoint(), screenSize.ToPoint()), Color.White);
            //spriteBatch.End();

            Lightmap.Draw(spriteBatch);
            spriteBatch.Begin(SpriteSortMode.Immediate, blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp, transformMatrix: renderPosition);
            Scene.sceneLibrary[Scene.activeScene].Draw(gameTime);
            //spriteBatch.Draw(Lightmap.lightmap, screenSize, Color.White);
            //spriteBatch.Draw(Lightmap.lightmap, Vector2.Zero, Color.White);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Immediate, blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
            Scene.sceneLibrary[Scene.activeScene].DrawUI(gameTime);
            Cursor.Draw();
            if (showDebug)
            {
                spriteBatch.Draw(UI.blank, new Rectangle(0, 0, 370, 160), Color.Black * 0.4f);
                spriteBatch.DrawString(UI.defaultFont,
                    $"FPS : {fps}\n" +
                    $"Position : {(Player.Instance != null ? $"X : {(int)Player.Instance.position.X}, Y : {(int)Player.Instance.position.Y}" : "?")}\n" +
                    $"Lights : {LightSource.sources.Count}\n" +
                    $"Compensation : {MathF.Round(compensation, 3)}",
                    Vector2.Zero, Color.White);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
