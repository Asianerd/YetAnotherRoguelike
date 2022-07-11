using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace YetAnotherRoguelike
{
    class Scene
    {
        public delegate void SceneEvents();
        public static event SceneEvents OnSceneChange;

        public static Scenes activeScene { get; private set; }
        public static Dictionary<Scenes, Scene> sceneLibrary = new Dictionary<Scenes, Scene>();

        public static ContentManager Content
        {
            get { return Game.Instance.Content; }
        }
        public static SpriteBatch spriteBatch
        {
            get { return Game.spriteBatch; }
        }

        #region SharedContent
        public static SpriteFont defaultFont;
        public static Texture2D blank;
        #endregion

        #region Statics
        public static void Initialize() {
            defaultFont = UI.defaultFont;
            blank = UI.blank;

            var _ = new MainGame();
            var __ = new MainMenu();
            var ___ = new DebugScene();

            ChangeScene(Scenes.MainGame);
        }

        public static void ChangeScene(Scenes scene)
        {
            activeScene = scene;
            if (OnSceneChange != null)
            {
                OnSceneChange();
            }
            sceneLibrary[activeScene].OnSceneLoad();
        }
        #endregion

        #region Object-tied
        Scenes sceneType;
        public Color backgroundColor = Color.CornflowerBlue;

        public Scene(Scenes type)
        {
            sceneType = type;
            sceneLibrary.Add(sceneType, this);
        }

        public virtual void Update(GameTime gameTime)
        {

        }

        public virtual void Draw(GameTime gameTime)
        {

        }

        public virtual void DrawUI(GameTime gameTime)
        {

        }

        public virtual void OnSceneLoad()
        {

        }
        #endregion

        public enum Scenes
        {
            MainMenu,
            MainGame,
            Debug,
            Credits
        }
    }
}
