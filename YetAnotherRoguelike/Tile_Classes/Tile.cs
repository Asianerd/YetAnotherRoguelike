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
        public Vector2 tPosition;   // Position in own chunk
                                    // Top left = (0, 0)
        public Rectangle rect;
        int spriteIndex = 0;

        public Tile(Type _type, Vector2 pos)
        {
            type = _type;
            position = pos;
            tPosition = Chunk.FixTilePos(position);
            rect = new Rectangle((pos * tileSize).ToPoint(), new Point(tileSize, tileSize));
        }

        public void UpdateSprite(Chunk parent)
        {
            if (type == Type.Air)
            {
                return;
            }
            if (!Game.playArea.Intersects(rect))
            {
                return;
            }


            // PROBLEM : chunk position not added properly
            /*Debug.WriteLine(parent.position);
            Debug.WriteLine(Chunk.size);
            Debug.WriteLine(parent.position * Chunk.size);
            Debug.WriteLine(position - Vector2.UnitY);
            Debug.WriteLine((Chunk.FixTilePos(position) - Vector2.UnitY) + (parent.position * Chunk.size));*/
            int right, left, up, down;
            up = Map.TypeAt(tPosition - Vector2.UnitY + (parent.position * Chunk.size)) == type ? 0 : 1;
            right = Map.TypeAt(tPosition + Vector2.UnitX + (parent.position * Chunk.size)) == type ? 0 : 2;
            down = Map.TypeAt(tPosition + Vector2.UnitY + (parent.position * Chunk.size)) == type ? 0 : 4;
            left = Map.TypeAt(tPosition - Vector2.UnitX + (parent.position * Chunk.size)) == type ? 0 : 8;
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
                //spriteBatch.DrawString(UI.defaultFont, , rect.Location.ToVector2(), Map.Fetch(position).type == Type.Air ? Color.Red : Color.LightBlue);
                spriteBatch.DrawString(UI.defaultFont, $"{Chunk.FixTilePos(position)}", rect.Location.ToVector2(), Color.White);
            }
        }

        public enum Type
        {
            Air,
            Stone
        }
    }
}
