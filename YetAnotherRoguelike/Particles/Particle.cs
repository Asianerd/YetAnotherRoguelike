using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YetAnotherRoguelike
{
    class Particle
    {
        public static List<Particle> particles = new List<Particle>();
        public static Texture2D blank;

        public static void Initialize()
        {
            blank = Game.Instance.Content.Load<Texture2D>("Particles/blank");
        }

        public static void UpdateAll()
        {
            foreach(Particle x in particles)
            {
                x.Update();
            }

            particles = particles.Where(n => !n.dead).ToList();
        }

        public static void DrawAll(SpriteBatch spriteBatch)
        {
            foreach(Particle x in particles)
            {
                // maybe dont pass spriteBatch to every particle?
                // might help performance (if performance is bad)
                x.Draw(spriteBatch);
            }
        }


        public Type type;
        public Vector2 position;
        public GameValue age;
        bool dead = false;

        public Particle(Type _type, Vector2 _position, GameValue _age)
        {
            position = _position;
            age = _age;
            type = _type;
        }

        public virtual void Update()
        {
            age.Regenerate(Game.compensation, clamp: false);
            dead = age.Percent() >= 1;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (!Game.playArea.Contains(position))
            {
                return;
            }
        }

        public enum Type
        {
            Smoke,
            Fire,
            Block_break
        }
    }
}
