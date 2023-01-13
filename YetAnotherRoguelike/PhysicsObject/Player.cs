using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using YetAnotherRoguelike.Graphics;
using YetAnotherRoguelike.Tile_Classes;

namespace YetAnotherRoguelike.PhysicsObject
{
    class Player:Entity
    {
        public static Player Instance = null;
        public static Texture2D sprite = null;
        public static Vector2 spriteOrigin;

        public static int inventorySize = 32; // 32 slots

        public Point currentChunkPos;
        public Chunk[,] surroundingChunks = new Chunk[3, 3];

        public Tile cursorTile;

        public List<Item> inventory;

        public Player(Vector2 pos, Texture2D _sprite):base(pos)
        {
            if (Instance == null)
            {
                Instance = this;
            }
            if (sprite == null)
            {
                sprite = _sprite;
                spriteOrigin = sprite.Bounds.Size.ToVector2() / 2f;
            }

            inventory = new List<Item>();
        }

        public override void Update()
        {
            bool hasMoved = false;

            Vector2 final = Vector2.Zero;
            foreach (Keys x in new Keys[] { Keys.W, Keys.A, Keys.S, Keys.D })
            {
                if (Input.collection[x].isPressed)
                {
                    final += GeneralDependencies.axialVectors[x];
                }
            }
            if ((final.X != 0) || (final.Y != 0))
            {
                final.Normalize();
                hasMoved = true;
            }
            final *= Input.collection[Keys.LeftShift].isPressed ? 0.25f : 0.12f;
            velocity = final;

            base.Update();

            currentChunkPos = Chunk.TileToChunkCoordinates((int)position.X, (int)position.Y);
            for (int x = -Chunk.chunkGenerationRange; x <= Chunk.chunkGenerationRange; x++)
            {
                for (int y = -Chunk.chunkGenerationRange; y <= Chunk.chunkGenerationRange; y++)
                {
                    Chunk.GenerateChunk(currentChunkPos.X + x, currentChunkPos.Y + y);
                }
            }

            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    surroundingChunks[x + 1, y + 1] = (Chunk.FetchChunkAt(currentChunkPos.X + x, currentChunkPos.Y + y));
                }
            }

            cursorTile = Chunk.FetchTileAt(Cursor.tPosition.X, Cursor.tPosition.Y);
            if ((cursorTile != null) && (cursorTile.type == Tile.BlockType.Air) && (cursorTile.neighbours[1, 0] != Tile.BlockType.Air))
            {
                if (Cursor.tSubPosition.Y < 0.4f) // <-- not reliable, not too sure why
                {
                    cursorTile = Chunk.FetchTileAt(Cursor.tPosition.X, Cursor.tPosition.Y - 1);
                }
            }

            if (MouseInput.left.isPressed)
            {
                if ((cursorTile != null) && (cursorTile.type != Tile.BlockType.Air))
                {
                    cursorTile.DecreaseDurability(-5f * Game.compensation);
                }
            }

            if (MouseInput.right.active)
            {
                if (cursorTile != null)
                {
                    Chunk.FetchChunkAt(cursorTile.tileCoordinates.X, cursorTile.tileCoordinates.Y).ReplaceTile(new Point(cursorTile.chunkTileCoordinates.X, cursorTile.chunkTileCoordinates.Y), Tile.CreateTile(new Point(cursorTile.tileCoordinates.X, cursorTile.tileCoordinates.Y), new Point(cursorTile.chunkTileCoordinates.X, cursorTile.chunkTileCoordinates.Y), Tile.BlockType.Neon_White));
                }
            }

            // lights
            if (Input.collection[Keys.D1].active)
            {
                if (cursorTile != null)
                {
                    Chunk.FetchChunkAt(cursorTile.tileCoordinates.X, cursorTile.tileCoordinates.Y).ReplaceTile(new Point(cursorTile.chunkTileCoordinates.X, cursorTile.chunkTileCoordinates.Y), Tile.CreateTile(new Point(cursorTile.tileCoordinates.X, cursorTile.tileCoordinates.Y), new Point(cursorTile.chunkTileCoordinates.X, cursorTile.chunkTileCoordinates.Y), Tile.BlockType.Neon_R));
                }
            }

            if (Input.collection[Keys.D2].active)
            {
                if (cursorTile != null)
                {
                    Chunk.FetchChunkAt(cursorTile.tileCoordinates.X, cursorTile.tileCoordinates.Y).ReplaceTile(new Point(cursorTile.chunkTileCoordinates.X, cursorTile.chunkTileCoordinates.Y), Tile.CreateTile(new Point(cursorTile.tileCoordinates.X, cursorTile.tileCoordinates.Y), new Point(cursorTile.chunkTileCoordinates.X, cursorTile.chunkTileCoordinates.Y), Tile.BlockType.Neon_G));
                }
            }

            if (Input.collection[Keys.D3].active)
            {
                if (cursorTile != null)
                {
                    Chunk.FetchChunkAt(cursorTile.tileCoordinates.X, cursorTile.tileCoordinates.Y).ReplaceTile(new Point(cursorTile.chunkTileCoordinates.X, cursorTile.chunkTileCoordinates.Y), Tile.CreateTile(new Point(cursorTile.tileCoordinates.X, cursorTile.tileCoordinates.Y), new Point(cursorTile.chunkTileCoordinates.X, cursorTile.chunkTileCoordinates.Y), Tile.BlockType.Neon_B));
                }
            }
            //

            if (Input.collection[Keys.F].active)
            {
                foreach (GroundItem x in GroundItem.collection)
                {
                    x.follow = true;
                }
            }

            if (Input.collection[Keys.OemPlus].active)
            {
                Lightmap.brightness += 0.05f;
            }

            if (Input.collection[Keys.OemMinus].active)
            {
                Lightmap.brightness -= 0.05f;
            }

            if (hasMoved)
            {
                UI.UI_Inventory_Container.Instance.active = false;
            }
            else
            {
                if (Input.collection[Keys.E].active)
                {
                    UI.UI_Inventory_Container.Instance.Toggle();
                }
            }
        }

        #region Item handling
        public bool InventoryCanFit(Item item)
        {
            if (inventory.Count <= inventorySize)
            {
                return true;
            }
            foreach (Item x in inventory)
            {
                if (x.type != item.type)
                {
                    continue;
                }
                if (x.Full())
                {
                    continue;
                }
                return true;
            }
            return false;
        }

        public void InventoryAppend(Item item)
        {
            foreach (Item x in inventory)
            {
                if (x.type != item.type)
                {
                    continue;
                }
                if (x.Full())
                {
                    continue;
                }
                int leftover;
                x.amount += item.amount;
                if (x.amount > x.stackSize)
                {
                    leftover = x.stackSize - x.amount;
                    x.amount -= leftover;
                    GroundItem.collection.Add(new GroundItem(new Item(item.type, leftover), position, position, false));
                }
            }
        }
        #endregion

        public override void Draw(SpriteBatch spritebatch)
        {
            spritebatch.Draw(sprite, position * Tile.tileSize, null, Color.White, 0f, spriteOrigin, Tile.spriteRenderScale, SpriteEffects.None, drawnLayer);

            base.Draw(spritebatch);
        }
    }
}
