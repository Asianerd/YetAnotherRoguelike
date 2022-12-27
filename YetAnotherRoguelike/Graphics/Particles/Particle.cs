using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YetAnotherRoguelike
{
    class Particle
    {
        public static Texture2D blank;
        public static Vector2 blankOrigin;
        public static List<Particle> collection = new List<Particle>();

        #region Statics
        public static void Initialize()
        {
            blank = Game.Instance.Content.Load<Texture2D>("Particles/blank");
            blankOrigin = blank.Bounds.Size.ToVector2() / 2f;
        }

        public static void UpdateAll()
        {
            foreach (Particle x in collection)
            {
                x.Update();
                if (x.dead)
                {
                    x.OnDeath();
                }
            }

            collection = collection.Where(n => !n.dead).ToList();
        }

        public static void DrawAll()
        {
            foreach (Particle x in collection)
            {
                x.Draw(Game.spriteBatch);
            }
        }

        public static void OnSceneChange()
        {
            collection = new List<Particle>();
        }
        #endregion

        public Vector2 position; // in tile coordinates
        public GameValue age;
        public bool dead = false;

        public Particle(Vector2 p, GameValue g)
        {
            position = p;
            age = g;
        }

        public virtual void Update()
        {
            if (dead)
            {
                return;
            }

            age.Regenerate(Game.compensation, clamp: false);
            dead = age.Percent() >= 1f;
        }

        public virtual void Draw(SpriteBatch spritebatch)
        {

        }

        public virtual void OnDeath()
        {

        }
    }
}
