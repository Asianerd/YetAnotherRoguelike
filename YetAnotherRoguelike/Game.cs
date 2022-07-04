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
        public static Game Instance;

        public static GraphicsDeviceManager graphics;
        public static SpriteBatch spriteBatch;
        public static Vector2 screenSize = new Vector2(700, 1080);
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

                Keys.F11
            });

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
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Matrix renderPosition = Matrix.CreateTranslation(new Vector3(Camera.Instance.renderOffset, 0));
            spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: renderPosition);
            Scene.sceneLibrary[Scene.activeScene].Draw(gameTime);
            spriteBatch.End();

            spriteBatch.Begin(samplerState:SamplerState.PointClamp);
            Scene.sceneLibrary[Scene.activeScene].DrawUI(gameTime);
            Cursor.Draw();
            spriteBatch.DrawString(UI.defaultFont, $"FPS : {1f / gameTime.ElapsedGameTime.TotalSeconds}", Vector2.Zero, Color.Purple);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
