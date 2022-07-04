using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YetAnotherRoguelike
{
    class Tile
    {
        public static Dictionary<Type, List<Texture2D>> blockSprites = new Dictionary<Type, List<Texture2D>>();
        public static int tileSize = 64;

        public static void Initialize()
        {
            foreach (Type x in Enum.GetValues(typeof(Type)).Cast<Type>())
            {
                List<Texture2D> final = new List<Texture2D>();
                for (int i = 1; i <= 16; i++)
                {
                    final.Add(Game.Instance.Content.Load<Texture2D>($"Tiles/{x}/{i}"));
                }
                blockSprites.Add(x, final);
            }
        }

        public Type type;
        public Vector2 position;
        public Rectangle rect;
        int spriteIndex = 0;

        public Tile(Type _type, Vector2 pos)
        {
            type = _type;
            position = pos;
            rect = new Rectangle((pos * tileSize).ToPoint(), new Point(tileSize, tileSize));
        }

        public void UpdateSprite()
        {
            if (type == Type.Air)
            {
                return;
            }
            if (!Game.playArea.Intersects(rect))
            {
                return;
            }

            int right, left, up, down;
            up = Map.TypeAt(position - Vector2.UnitY) == type ? 0 : 1;
            right = Map.TypeAt(position + Vector2.UnitX) == type ? 0 : 2;
            down = Map.TypeAt(position + Vector2.UnitY) == type ? 0 : 4;
            left = Map.TypeAt(position - Vector2.UnitX) == type ? 0 : 8;
            spriteIndex = right + left + up + down;
        }

        public void Update()
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (type == Type.Air)
            {
                return;
            }
            if (Game.playArea.Intersects(rect))
            {
                spriteBatch.Draw(blockSprites[type][spriteIndex], rect, Color.White);
            }
        }

        public enum Type
        {
            Air,
            Stone
        }
    }
}
