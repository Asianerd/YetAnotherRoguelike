using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using YetAnotherRoguelike.Data;
using YetAnotherRoguelike.Graphics;
using YetAnotherRoguelike.Tile_Classes;

namespace YetAnotherRoguelike.PhysicsObject
{
    class Player:Entity
    {
        public static Player Instance = null;
        public static Texture2D sprite = null;
        public static Vector2 spriteOrigin;

        public static int inventorySize = 25;
        public static int hotbarSize = 4;
        public static float dropDistance = 0.5f; // how far to drop items

        public Point currentChunkPos;
        public Chunk[,] surroundingChunks = new Chunk[3, 3];
        public Tile[,] neighbourTiles = new Tile[5, 5];

        public Tile cursorTile;

        public List<Item> inventory;
        public Dictionary<Item.Type, int> inventoryItemCount; // for performance reasons
        public List<Item> hotbar;
        public int selectionIndex = 0;
        public Item selectedItem
        {
            get { return hotbar[selectionIndex];  }
        }

        public Tile.BlockType tilePlaced; // null if selected item doesnt have a tilePlaced

        public float speed; // current speed of player, used to close UI containers and etc
        Vector2 _previousPosition;

        public Player(Vector2 pos, Texture2D _sprite):base(pos, new Vector2(0.5f, 0.5f))
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
            for (int i = 0; i < inventorySize; i++)
            {
                inventory.Add(Item.Empty());
            }

            hotbar = new List<Item>();
            for (int i = 0; i < hotbarSize; i++)
            {
                hotbar.Add(Item.Empty());
            }

            for (int i = 0; i <= 5; i++)
            {
                inventory[i] = new Item(Item.Type.Crucible, 1);
                Chemical c = (Chemical)inventory[i].data[Item.DataType.Chemical];
                List<Chemical.Element> e = Enum.GetValues(typeof(Chemical.Element)).Cast<Chemical.Element>().ToList();
                while (e.Count >= 1)
                {
                    int _i = Game.random.Next(0, e.Count - 1);
                    c.AddElement(e[_i], (double)(Game.random.Next(50, 500) / 1000f));
                    e.RemoveAt(_i);
                }
            }
            inventory[6] = new Item(Item.Type.Hematite, 256);
            inventory[7] = new Item(Item.Type.Hematite, 256);
            inventory[8] = new Item(Item.Type.Stone, 256);
            inventory[9] = new Item(Item.Type.Stone, 256);
            hotbar[0] = new Item(Item.Type.Rudimentary_Furnace, 1);
        }

        public override void Update()
        {
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
            }
            final *= Input.collection[Keys.LeftShift].isPressed ? 0.25f : 0.12f;
            velocity = final;

            base.Update();

            speed = Vector2.Distance(position, _previousPosition);
            _previousPosition.X = (float)position.X;
            _previousPosition.Y = (float)position.Y;

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
                    surroundingChunks[x + 1, y + 1] = (Chunk.FetchChunkWithCoords(currentChunkPos.X + x, currentChunkPos.Y + y));
                }
            }

            List<Tile> targetedTiles = new List<Tile>();
            for (int x = -2; x < 3; x++)
            {
                for (int y = -2; y < 3; y++)
                {
                    neighbourTiles[x + 2, y + 2] = Chunk.FetchTileAt((int)(MathF.Round(position.X) + x), (int)(MathF.Round(position.Y - 0.5f) + y), surroundingChunks);
                    if (neighbourTiles[x + 2, y + 2].interactable)
                    {
                        targetedTiles.Add(neighbourTiles[x + 2, y + 2]);
                    }
                }
            }

            if (targetedTiles.Count <= 0)
            {
                Tile.targetedTile = null;
            }

            float lowestDiff = 1000;
            foreach (Tile x in targetedTiles)
            {
                float d = MathF.Atan2(
                        ((Game.screenSize.Y * 0.5f) - Camera.renderOffset.Y) - x.renderedPosition.Y,
                        ((Game.screenSize.X * 0.5f) - Camera.renderOffset.X) - x.renderedPosition.X
                        ) + MathF.PI;

                float diff = MathF.Abs(Cursor.angleToPlayer - d);
                if (diff < lowestDiff)
                {
                    lowestDiff = diff;
                    Tile.targetedTile = x;
                }
            }

            JSON_ItemData _result = JSON_ItemData.FetchData(selectedItem.type);
            tilePlaced = _result == null ? Tile.BlockType.Air : _result.tilePlaced;

            #region Input section
            cursorTile = Chunk.FetchTileAt(Cursor.tPosition.X, Cursor.tPosition.Y);
            Tile result = Chunk.FetchTileAt(Cursor.tPosition.X, Cursor.tPosition.Y + 1);
            if ((result != null) && (result.type != Tile.BlockType.Air) && ((Cursor.tSubPosition.Y >= 0.5f)))
            {
                cursorTile = result;
            }

            if (UI.UI_Container.hoveredContainer == null)
            {
                // checks of the respective inputs are handled in their functions
                LeftMouseEvent();
                RightMouseEvent();
            }

            #region Light blocks
            /*if (Input.collection[Keys.D1].active)
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
                    Chunk.FetchChunkAt(cursorTile.tileCoordinates.X, cursorTile.tileCoordinates.Y).ReplaceTile(new Point(cursorTile.chunkTileCoordinates.X, cursorTile.chunkTileCoordinates.Y), Tile.CreateTile(new Point(cursorTile.tileCoordinates.X, cursorTile.tileCoordinates.Y), new Point(cursorTile.chunkTileCoordinates.X, cursorTile.chunkTileCoordinates.Y), Tile.BlockType.Neon_Blue));
                }
            }*/

            if (Input.collection[Keys.R].active)
            {
                if (cursorTile != null)
                {
                    Chunk.FetchChunkAt(cursorTile.tileCoordinates.X, cursorTile.tileCoordinates.Y).ReplaceTile(new Point(cursorTile.chunkTileCoordinates.X, cursorTile.chunkTileCoordinates.Y), Tile.CreateTile(new Point(cursorTile.tileCoordinates.X, cursorTile.tileCoordinates.Y), new Point(cursorTile.chunkTileCoordinates.X, cursorTile.chunkTileCoordinates.Y), Tile.BlockType.Neon_White));
                }
            }
            #endregion

            #region Debug controls
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
            #endregion

            if (Input.collection[Keys.Tab].active)
            {
                UI.UI_Inventory_Container.Instance.Toggle();
            }
            #endregion
        }

        #region Mouse inputs
        public void LeftMouseEvent()
        {
            if (MouseInput.left.isPressed)
            {
                if ((cursorTile != null) && (cursorTile.type != Tile.BlockType.Air))
                {
                    cursorTile.DecreaseDurability(-5f * Game.compensation);
                }
            }
        }

        public void RightMouseEvent()
        {
            if (MouseInput.right.active)
            {
                if (cursorTile != null)
                {
                    JSON_ItemData result = JSON_ItemData.FetchData(selectedItem.type);
                    if (result == null)
                    {
                        return;
                    }
                    if (result.tilePlaced == Tile.BlockType.Air)
                    {
                        return;
                    }
                    Tile _targeted = Chunk.FetchTileAt(Cursor.tPosition.X, Cursor.tPosition.Y);
                    if ((cursorTile.type != Tile.BlockType.Air) && (Cursor.tSubPosition.Y >= 0.5f) && (_targeted.neighbours[1, 2] == Tile.BlockType.Air))
                    {
                        Tile temp = Chunk.FetchTileAt(cursorTile.tileCoordinates.X, cursorTile.tileCoordinates.Y + 1);
                        Chunk.FetchChunkAt(temp.tileCoordinates.X, temp.tileCoordinates.Y)
                            .ReplaceTile(
                            new Point(temp.chunkTileCoordinates.X, temp.chunkTileCoordinates.Y),
                            Tile.CreateTile(
                                new Point(temp.tileCoordinates.X, temp.tileCoordinates.Y),
                                new Point(temp.chunkTileCoordinates.X, temp.chunkTileCoordinates.Y),
                                result.tilePlaced
                                ));
                    }
                    else
                    {
                        if (cursorTile.type != Tile.BlockType.Air)
                        {
                            return; // place the block only if its an empty space
                        }
                        Chunk.FetchChunkAt(cursorTile.tileCoordinates.X, cursorTile.tileCoordinates.Y)
                            .ReplaceTile(
                            new Point(cursorTile.chunkTileCoordinates.X, cursorTile.chunkTileCoordinates.Y),
                            Tile.CreateTile(
                                new Point(cursorTile.tileCoordinates.X, cursorTile.tileCoordinates.Y),
                                new Point(cursorTile.chunkTileCoordinates.X, cursorTile.chunkTileCoordinates.Y),
                                result.tilePlaced
                                ));
                    }
                }
            }
        }
        #endregion

        #region Item handling
        public bool InventoryCanFit(Item item)
        {
            foreach (Item x in inventory)
            {
                if (x.type == Item.Type.None)
                {
                    return true; // theres an empty slot
                }
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
                if (x.type == Item.Type.None)
                {
                    // if its an empty slot
                    x.type = item.type;
                    x.amount = item.amount;
                    x.data = item.data;
                    if (x.Full(over:true))
                    {
                        int balance = x.amount - x.stackSize;
                        x.amount = x.stackSize;
                        GroundItem.collection.Add(new GroundItem(new Item(item.type, balance, item.data), position, position, false));
                    }
                    OnInventoryModify();
                    return;
                }
                else
                {
                    if (x.type != item.type)
                    {
                        continue;
                    }
                    // if the item type is the same
                    if (x.Full())
                    {
                        continue;
                    }
                    // if the item isnt fully stacked
                    int leftover;
                    x.amount += item.amount;
                    if (x.amount > x.stackSize)
                    {
                        leftover = x.amount - x.stackSize;
                        x.amount -= leftover;
                        GroundItem.collection.Add(new GroundItem(new Item(item.type, leftover, item.data), position, position, false));
                    }
                    OnInventoryModify();
                    return;
                }
            }
        }

        public void OnInventoryModify()
        {
            inventoryItemCount = new Dictionary<Item.Type, int>();
            foreach (Item x in inventory)
            {
                if (!inventoryItemCount.ContainsKey(x.type))
                {
                    inventoryItemCount.Add(x.type, 0);
                }
                inventoryItemCount[x.type] += x.amount;
            }

            UI.UI_Inventory_CraftingParent.Instance.UpdateList();
        }

        public void InventoryRemove(Item.Type type, int amount)
        {
            int toBeRemoved = amount;
            while (toBeRemoved > 0) // while loops are the scariest things
            {
                foreach (Item x in inventory)
                {
                    if (x.type == type)
                    {
                        if (x.amount >= toBeRemoved)
                        {
                            x.amount -= toBeRemoved;
                            toBeRemoved = 0; // <- just to be safe
                            return;
                        }
                        else 
                        {
                            toBeRemoved -= x.amount;
                            x.amount = 0;
                            x.data = null;
                        }
                    }
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
