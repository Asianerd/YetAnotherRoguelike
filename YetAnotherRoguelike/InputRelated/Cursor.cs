using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using YetAnotherRoguelike.Graphics;
using YetAnotherRoguelike.Tile_Classes;
using YetAnotherRoguelike.PhysicsObject;
using YetAnotherRoguelike.Data;

namespace YetAnotherRoguelike
{
    class Cursor
    {
        static Dictionary<CursorStates, Texture2D> cursorSprites = new Dictionary<CursorStates, Texture2D>();
        static Dictionary<CursorStates, Vector2> cursorOrigins = new Dictionary<CursorStates, Vector2>();

        public static CursorStates state = CursorStates.Default;

        public static Point position;
        public static Point tPosition;
        public static Vector2 tPositionV;
        static Vector2 _tSubPosition;
        public static Vector2 tSubPosition {
            get
            {
                _tSubPosition.X = ((tPositionV.X + 0.5f) % 1f);
                _tSubPosition.Y = ((tPositionV.Y + 0.5f) % 1f);
                if (MathF.Sign(_tSubPosition.X) == -1)
                {
                    _tSubPosition.X += 1;
                }
                if (MathF.Sign(_tSubPosition.Y) == -1)
                {
                    _tSubPosition.Y += 1;
                }
                _tSubPosition.X %= 1f;
                _tSubPosition.Y %= 1f;
                return _tSubPosition;
            }
            set
            {
                _tSubPosition = value;
            }
        }
        // the position in a tile
        // if cursor is in upper half of a tile, then y > 0.5
        // if cursor is in right half of a tile, then x > 0.5
        /*      #(0.5,0)#
         *      #   #   #
         * (0, 0.5) # (1, 0.5)
         *      #   #   #
         *      #(0.5,1)#
         */
        public static float angleToPlayer; // radians?

        public static LightSource light;

        public static Item item;

        public static void Initialize()
        {
            foreach (CursorStates x in Enum.GetValues(typeof(CursorStates)).Cast<CursorStates>())
            {
                cursorSprites.Add(x, Game.Instance.Content.Load<Texture2D>($"UI/Cursors/{x.ToString().ToLower()}"));
                cursorOrigins.Add(x, cursorSprites[x].Bounds.Size.ToVector2() * 0.5f);
            }

            light = new LightSource(Vector2.Zero, Color.White, 5000, 500);
            item = Item.Empty();
            //LightSource.Append(light);
        }

        public static void Update()
        {
            if (UI.UI_Element.hoveredElement == null)
            {
                state = Player.Instance.selectedItem.selectionType switch
                {
                    Item.Species.Placeable => CursorStates.Placing,
                    Item.Species.Tool => CursorStates.Mining,
                    _ => CursorStates.Default
                };
            } else
            {
                state = CursorStates.Default;
            }

            position = Game.mouseState.Position;
            Vector2 tPos = ((position.ToVector2() - Camera.renderOffset) / Tile.tileSize) - new Vector2(0, 0.5f);
            tPositionV = tPos;
            tPos = new Vector2(MathF.Round(tPos.X), MathF.Round(tPos.Y));
            tPosition = tPos.ToPoint();

            angleToPlayer = MathF.Atan2(
                ((Game.screenSize.Y * 0.5f) - Game.mouseState.Position.Y),
                ((Game.screenSize.X * 0.5f) - Game.mouseState.Position.X)
                ) + MathF.PI;

            light.position = tPositionV;

            if (item.type == Item.Type.None)
            {
                return;
            }
            if (MouseInput.left.active || MouseInput.right.active)
            {
                if (UI.UI_Element.hoveredElement == null)
                {
                    float angle = MathF.Atan2(
                        tPositionV.Y - Player.Instance.position.Y,
                        tPositionV.X - Player.Instance.position.X
                        );
                    Vector2 offset = new Vector2(
                        MathF.Cos(angle),
                        MathF.Sin(angle)
                        ) * Player.dropDistance;
                    GroundItem.collection.Add(new GroundItem(new Item(item.type, item.amount, item.data), Player.Instance.position + offset, Player.Instance.position, true));
                    item.type = Item.Type.None;
                    item.amount = 0;
                    item.data = null;
                }
            }
        }

        public static void Draw()
        {
            if (state == CursorStates.Select)
            {
                if (MouseInput.left.isPressed || MouseInput.right.isPressed)
                {
                    state = CursorStates.Select_pressed;
                }
            }
            if (item.type != Item.Type.None)
            {
                state = CursorStates.Grabbing;
                Game.spriteBatch.Draw(item.FetchSprite(), position.ToVector2(), null, Color.White, 0f, Item.spriteOrigin, 3f, SpriteEffects.None, 0f);
            }
            Game.spriteBatch.Draw(cursorSprites[state], Game.mouseState.Position.ToVector2(), null, Color.White, 0f, cursorOrigins[state], 3f, SpriteEffects.None, 0f);
        }

        public enum CursorStates
        {
            Default,
            Select,
            Select_pressed,
            Grab,
            Grabbing,
            Orb,

            Placing,
            Mining,

            Hidden
        }
    }
}
