using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using YetAnotherRoguelike.Data;
using YetAnotherRoguelike.Graphics;
using YetAnotherRoguelike.PhysicsObject;
using YetAnotherRoguelike.Tile_Classes;

namespace YetAnotherRoguelike.UI
{
    class UI_Gameplay_Container : UI_Container
    {
        public static UI_Gameplay_Container Instance = null;
        public static Texture2D tileSelectionSprite;
        public static Vector2 tileSelectionSpriteOrigin;

        public Vector2 tileSelectionPosition;
        public Vector2 tileSelectionTarget;
        public GameValue tileSelectionProgress;
        public GameValue tileSelectionBob;

        public List<Texture2D> tooltipSprite;
        public Texture2D interactionTooltipSprite;
        public Vector2 interactionTooltipSpriteOrigin;
        public Vector2 interactionTooltipLocation;
        public GameValue interactionTooltipProgress;

        // for all the gameplay stuff like health, etc etc etc
        public UI_Gameplay_Container():base(new List<UI_Element>(), true)
        {
            if (Instance == null)
            {
                Instance = this;
            }

            tileSelectionSprite = Game.Instance.Content.Load<Texture2D>("Player/tileSelectionSprite");
            tileSelectionSpriteOrigin = tileSelectionSprite.Bounds.Size.ToVector2() / 2f;

            tileSelectionPosition = Vector2.Zero;
            tileSelectionTarget = Vector2.Zero;

            tileSelectionProgress = new GameValue(0, 20, 1);
            tileSelectionBob = new GameValue(0, 120, 1, _repeat:true);

            tooltipSprite = GeneralDependencies.Split(Game.Instance.Content.Load<Texture2D>("UI/Gameplay/tooltipSprite"), 4, 4);
            interactionTooltipSprite = Game.Instance.Content.Load<Texture2D>("UI/Gameplay/interactionTooltip");
            interactionTooltipSpriteOrigin = interactionTooltipSprite.Bounds.Size.ToVector2() / 2f;
            interactionTooltipLocation = new Vector2(0, 0);
            interactionTooltipProgress = new GameValue(0, 20, 1);
        }

        public override void Update()
        {
            base.Update();

            if (Player.Instance.cursorTile != null)
            {
                tileSelectionTarget = Player.Instance.cursorTile.tileCoordinatesV;

                Tile _targeted = Chunk.FetchTileAt(Cursor.tPosition.X, Cursor.tPosition.Y);

                JSON_ItemData result = JSON_ItemData.FetchData(Player.Instance.selectedItem.type);
                switch (Player.Instance.selectedItem.selectionType)
                {
                    case Item.Species.Tool:
                        if ((result != null) && (Player.Instance.cursorTile.type != Tile.BlockType.Air))
                        {
                            tileSelectionProgress.Regenerate(Game.compensation * 3f);
                        }
                        else
                        {
                            tileSelectionProgress.Regenerate(-Game.compensation);
                        }
                        break;
                    case Item.Species.Placeable:
                        if ((result != null) && (Player.Instance.cursorTile.type == Tile.BlockType.Air))
                        {
                            tileSelectionProgress.Regenerate(Game.compensation * 3f);
                        }
                        else
                        {
                            if ((Player.Instance.cursorTile != null) && (Player.Instance.cursorTile.type != Tile.BlockType.Air) && (Cursor.tSubPosition.Y >= 0.5f) && (_targeted.neighbours[1, 2] == Tile.BlockType.Air))
                            {
                                tileSelectionProgress.Regenerate(Game.compensation * 3f);
                                tileSelectionTarget.Y++;
                            }
                            else
                            {
                                tileSelectionProgress.Regenerate(-Game.compensation);
                            }
                        }
                        break;
                    default:
                        tileSelectionProgress.Regenerate(-Game.compensation);
                        break;
                }
            }
            else
            {
                tileSelectionProgress.Regenerate(-Game.compensation);
            }

            tileSelectionPosition.X = GeneralDependencies.Lerp(tileSelectionPosition.X, tileSelectionTarget.X, 0.2f * Game.compensation, 0.01f);
            tileSelectionPosition.Y = GeneralDependencies.Lerp(tileSelectionPosition.Y, tileSelectionTarget.Y, 0.2f * Game.compensation, 0.01f);

            if (Tile.targetedTile != null)
            {
                interactionTooltipLocation = Vector2.Lerp(interactionTooltipLocation, Tile.targetedTile.renderedPosition + Camera.renderOffset, 0.1f);
                interactionTooltipProgress.Regenerate(Game.compensation);
            }
            else
            {
                interactionTooltipLocation = Vector2.Lerp(interactionTooltipLocation, (Player.Instance.position * Tile.tileSize) + Camera.renderOffset, 0.1f);
                interactionTooltipProgress.Regenerate(-Game.compensation * 3);
            }
        }

        public override void Draw()
        {
            base.Draw();
            if (UI_Element.hoveredElement == null)
            {
                Game.spriteBatch.Draw(tileSelectionSprite, (tileSelectionPosition * Tile.tileSize) + Camera.renderOffset, null, Color.White * (tileSelectionProgress.Percent() + ((MathF.Sin((float)tileSelectionBob.Percent() * 2f * MathF.PI) * 0.2f) - 0.5f)), 0f, tileSelectionSpriteOrigin, Tile.spriteRenderScale, SpriteEffects.None, 1f);
                if ((Player.Instance.cursorTile != null) && (Player.Instance.cursorTile.neighbours[1, 2] == Tile.BlockType.Air))
                {
                    Game.spriteBatch.Draw(tileSelectionSprite, ((tileSelectionPosition + new Vector2(0, 0.75f)) * Tile.tileSize) + Camera.renderOffset, null, Color.White * (tileSelectionProgress.Percent() + ((MathF.Sin((float)tileSelectionBob.Percent() * 2f * MathF.PI) * 0.2f) - 0.5f)), 0f, tileSelectionSpriteOrigin, new Vector2(1, 0.5f) * Tile.spriteRenderScale, SpriteEffects.None, 1f);
                }
            }
            else
            {
                bool _found = true;
                string final = "";
                switch (UI_Element.hoveredElement.type)
                {
                    case UI_Element.ElementType.ItemSlot:
                        final = ((UI_ItemSlot)UI_Element.hoveredElement).item.FetchName() + "\n" + ((UI_ItemSlot)UI_Element.hoveredElement).item.FetchDescription();
                        if (((UI_ItemSlot)UI_Element.hoveredElement).item.type == Item.Type.None)
                        {
                            _found = false;
                        }
                        break;
                    case UI_Element.ElementType.CraftingChildInput:
                        final = ((UI_Inventory_CraftingChild)UI_Element.hoveredElement).FetchInput();
                        break;
                    case UI_Element.ElementType.CraftingChildOutput:
                        final = ((UI_Inventory_CraftingChild)UI_Element.hoveredElement).FetchOutput();
                        break;
                    default:
                        _found = false;
                        break;
                }
                final = final.TrimEnd('\n');
                final = final.TrimStart('\n');
                if (_found)
                {
                    Vector2 size = Game.mainFont.MeasureString(final);
                    Vector2 renderedPosition = Game.mouseState.Position.ToVector2();
                    //renderedPosition.Y -= size.Y * 0.5f;
                    renderedPosition.X += (UI_ItemSlot.size * 0.5f);

                    UI_Element.multiRect.X = (int)(renderedPosition.X - (UI_ItemSlot.size * 0.25f));
                    UI_Element.multiRect.Y = (int)(renderedPosition.Y - (UI_ItemSlot.size * 0.25f));
                    UI_Element.multiRect.Size = (size + new Vector2(UI_ItemSlot.size * 0.5f)).ToPoint();
                    //UI_Element.multiRect.Size = size.ToPoint();

                    Game.spriteBatch.Draw(tooltipSprite[0], new Rectangle(UI_Element.multiRect.X, UI_Element.multiRect.Y, UI_Element.pixel, UI_Element.pixel), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.98f);
                    Game.spriteBatch.Draw(tooltipSprite[1], new Rectangle(UI_Element.multiRect.X + UI_Element.pixel, UI_Element.multiRect.Y, UI_Element.multiRect.Width - (UI_Element.pixel * 2), UI_Element.pixel), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.98f);
                    Game.spriteBatch.Draw(tooltipSprite[2], new Rectangle(UI_Element.multiRect.Right - UI_Element.pixel, UI_Element.multiRect.Y, UI_Element.pixel, UI_Element.pixel), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.98f);

                    Game.spriteBatch.Draw(tooltipSprite[3], new Rectangle(UI_Element.multiRect.X, UI_Element.multiRect.Y + UI_Element.pixel, UI_Element.pixel, UI_Element.multiRect.Height - (UI_Element.pixel * 2)), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.98f);
                    Game.spriteBatch.Draw(tooltipSprite[4], new Rectangle(UI_Element.multiRect.X + UI_Element.pixel, UI_Element.multiRect.Y + UI_Element.pixel, UI_Element.multiRect.Width - (UI_Element.pixel * 2), UI_Element.multiRect.Height - (UI_Element.pixel * 2)), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.98f);
                    Game.spriteBatch.Draw(tooltipSprite[5], new Rectangle(UI_Element.multiRect.Right - UI_Element.pixel, UI_Element.multiRect.Y + UI_Element.pixel, UI_Element.pixel, UI_Element.multiRect.Height - (UI_Element.pixel * 2)), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.98f);

                    Game.spriteBatch.Draw(tooltipSprite[6], new Rectangle(UI_Element.multiRect.X, UI_Element.multiRect.Bottom - UI_Element.pixel, UI_Element.pixel, UI_Element.pixel), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.98f);
                    Game.spriteBatch.Draw(tooltipSprite[7], new Rectangle(UI_Element.multiRect.X + UI_Element.pixel, UI_Element.multiRect.Bottom - UI_Element.pixel, UI_Element.multiRect.Width - (UI_Element.pixel * 2), UI_Element.pixel), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.98f);
                    Game.spriteBatch.Draw(tooltipSprite[8], new Rectangle(UI_Element.multiRect.Right - UI_Element.pixel, UI_Element.multiRect.Bottom - UI_Element.pixel, UI_Element.pixel, UI_Element.pixel), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.98f);

                    Game.spriteBatch.DrawString(Game.mainFont, final, renderedPosition, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                }
            }

            Game.spriteBatch.Draw(interactionTooltipSprite, interactionTooltipLocation, null, Color.White * interactionTooltipProgress.Percent(), 0f, interactionTooltipSpriteOrigin, 3f, SpriteEffects.None, 0f);
        }
    }
}
