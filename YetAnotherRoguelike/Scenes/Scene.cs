using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using YetAnotherRoguelike.Scenes;

namespace YetAnotherRoguelike
{
    class Scene
    {
        public static Dictionary<SceneTypes, Scene> sceneLibrary;
        static SceneTypes _activeScene; // not to be used anywhere, just use public activeScene instead, same thing
        public static SceneTypes activeScene {
            get { return _activeScene; }
            set
            {
                sceneLibrary[_activeScene].OnUnload();
                _activeScene = value;
                sceneLibrary[_activeScene].OnLoad();
            }
        }
        public static Scene currentScene
        {
            get { return sceneLibrary[activeScene]; }
            private set { }
        }

        public static void Initialize()
        {
            sceneLibrary = new Dictionary<SceneTypes, Scene>()
            {
                { SceneTypes.MainMenu, new MainMenuScene() },
                { SceneTypes.MainGame, new MainGameScene() }
            };

            activeScene = SceneTypes.MainGame;
        }


        public SceneTypes type;
        public Color backgroundColor;

        public Scene(SceneTypes t, Color bgColor)
        {
            type = t;
            backgroundColor = bgColor;
        }

        public virtual void Update() { }

        public virtual void Draw() { }

        public virtual void OnLoad() { }

        public virtual void OnUnload() { }

        public enum SceneTypes
        {
            MainGame,
            MainMenu
        }
    }
}
