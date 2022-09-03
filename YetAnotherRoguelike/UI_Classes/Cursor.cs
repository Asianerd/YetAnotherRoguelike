using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using YetAnotherRoguelike.Gameplay;
using Microsoft.Xna.Framework.Input;

namespace YetAnotherRoguelike
{
    class Cursor
    {
        public static CursorStates state = CursorStates.Default;
        public static Dictionary<CursorStates, Texture2D> cursorSprites = new Dictionary<CursorStates, Texture2D>();

        public static LightSource cursorLight;
        public static float colorAge = 0;
        public static Vector2 worldPosition;

        public static Item item;

        public static void Initialize()
        {
            foreach (CursorStates x in Enum.GetValues(typeof(CursorStates)).Cast<CursorStates>())
            {
                cursorSprites.Add(x, Game.Instance.Content.Load<Texture2D>($"UI/Cursors/{x.ToString().ToLower()}"));
            }
            cursorLight = new LightSource(Vector2.Zero, 10, 5, Color.White);

            item = new Item(Item.Type.None, 0);
            //LightSource.Append(cursorLight);
        }

        public static void Update()
        {
            state = CursorStates.Default;
            cursorLight.position = Chunk.CorrectedWorldToTile(WorldPosition());
            colorAge += 0.1f;
            if (colorAge >= Math.PI * 2f)
            {
                colorAge = 0;
            }
            cursorLight.color = new Color((MathF.Sin(colorAge - MathF.PI) + 1) / 2f, (MathF.Sin(colorAge) + 1) / 2f, (MathF.Sin(colorAge + MathF.PI) + 1) / 2f);

            worldPosition = WorldPosition();

            if (item.type != Item.Type.None)
            {
                if (MouseInput.right.active || MouseInput.left.active)
                {
                    if (!UI_Container.hoverContainer)
                    {
                        float d = MathF.Atan2(
                            worldPosition.Y - Player.Instance.position.Y,
                            worldPosition.X - Player.Instance.position.X
                            );
                        GroundItem.Spawn(item.type, Player.Instance.position + new Vector2(
                            MathF.Cos(d) * 20f,
                            MathF.Sin(d) * 20f
                            ), item.amount, Player.Instance.position, true);
                        item.type = Item.Type.None;
                        item.amount = 0;
                    }
                }
            }
        }

        public static void Draw()
        {
            if (item.type != Item.Type.None)
            {
                Game.spriteBatch.Draw(Item.itemSprites[item.type], Game.mouseState.Position.ToVector2(), Color.White);
                Game.spriteBatch.DrawString(UI.defaultFont, item.amount.ToString(), Game.mouseState.Position.ToVector2(), Color.White);
            }
            Game.spriteBatch.Draw(
                cursorSprites[
                    (item.type == Item.Type.None) ? (
                        (state == CursorStates.Select) && 
                        (Game.mouseState.LeftButton == ButtonState.Pressed) ? 
                        CursorStates.Select_pressed : state) : 
                        CursorStates.Select_pressed
                    ],
                Game.mouseState.Position.ToVector2(), null, Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, 0f);
        }

        public static Vector2 WorldPosition()
        {
            return Game.mouseState.Position.ToVector2() - Camera.Instance.renderOffset;
        }

        public enum CursorStates
        {
            Default,
            Select,
            Select_pressed
        }
    }
}
