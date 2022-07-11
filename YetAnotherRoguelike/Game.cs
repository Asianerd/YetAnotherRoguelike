using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace YetAnotherRoguelike
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        /* Todo :
            - Perlin noise
            - Block breaking animation
                - Refer to block health
            - Lighting
         */

        public static Game Instance;

        public static GraphicsDeviceManager graphics;
        public static SpriteBatch spriteBatch;
        public static Vector2 screenSize = new Vector2(1920, 1080);
        //public static Vector2 screenSize = new Vector2(1000, 1000);
        public static Rectangle playArea = new Rectangle(0, 0, 0, 0);

        public static KeyboardState keyboardState;
        public static MouseState mouseState;

        public static Random random = new Random(8192);

        public Game()
        {
            Instance = this;

            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = (int)screenSize.X;
            graphics.PreferredBackBufferHeight = (int)screenSize.Y;
            graphics.IsFullScreen = screenSize == new Vector2(1920, 1080);

            Content.RootDirectory = "Content";
            IsMouseVisible = false;
        }

        protected override void Initialize()
        {
            playArea = new Rectangle(Vector2.Zero.ToPoint(), screenSize.ToPoint());

            Perlin_Noise.Initialize();

            GeneralDependencies.Initialize();
            var x = new Camera();

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

                Keys.F11,

                Keys.Up,
                Keys.Down,
                Keys.Left,
                Keys.Right
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

            Scene.sceneLibrary[Scene.activeScene].Update(gameTime);

            playArea = new Rectangle((-1f * Camera.Instance.renderOffset).ToPoint(), screenSize.ToPoint());

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Scene.sceneLibrary[Scene.activeScene].backgroundColor);

            Matrix renderPosition = Matrix.CreateTranslation(new Vector3(Camera.Instance.renderOffset, 0));
            spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: renderPosition);
            Scene.sceneLibrary[Scene.activeScene].Draw(gameTime);
            spriteBatch.End();

            spriteBatch.Begin(samplerState:SamplerState.PointClamp);
            Scene.sceneLibrary[Scene.activeScene].DrawUI(gameTime);
            Cursor.Draw();
            spriteBatch.Draw(UI.blank, new Rectangle(0, 0, 370, 120), Color.Black * 0.4f);
            spriteBatch.DrawString(UI.defaultFont,
                $"FPS : {1f / gameTime.ElapsedGameTime.TotalSeconds}\n" +
                $"Position : {(Player.Instance != null ? Player.Instance.position.ToString() : "?")}\n" +
                $"Lights : {LightSource.sources.Count}",
                Vector2.Zero, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
