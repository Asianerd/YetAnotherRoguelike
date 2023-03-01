using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public Texture2D tileSelectionSprite;
        public Vector2 tileSelectionSpriteOrigin;

        public Texture2D slotSelectionSprite;
        public int slotSelectionOffset = 4;
        public Rectangle slotSelectionRect;
        public Vector2 slotSelectionTarget;
        public Vector2 slotSelectionLocation;
        public GameValue slotSelectionVisibility;

        public Vector2 tileSelectionPosition;
        public Vector2 tileSelectionTarget;
        public GameValue tileSelectionProgress;
        public GameValue tileSelectionBob;

        public List<Texture2D> tooltipSprite;
        public Texture2D interactionTooltipSprite;
        public Vector2 interactionTooltipSpriteOrigin;
        public Vector2 interactionTooltipLocation;
        public GameValue interactionTooltipProgress;

        public List<Texture2D> chemicalBarSprite;
        public List<Texture2D> chemicalBarBorderSprite;
        public List<Texture2D> chemicalBarFillSprite;

        // for all the gameplay stuff like health, etc etc etc
        public UI_Gameplay_Container():base(new List<UI_Element>(), true)
        {
            if (Instance == null)
            {
                Instance = this;
            }

            tileSelectionSprite = Game.Instance.Content.Load<Texture2D>("Player/tileSelectionSprite");
            tileSelectionSpriteOrigin = tileSelectionSprite.Bounds.Size.ToVector2() / 2f;

            slotSelectionRect = new Rectangle(0, 0, (int)(UI_ItemSlot.size + (slotSelectionOffset * 2f)), (int)(UI_ItemSlot.size + (slotSelectionOffset * 2f)));
            slotSelectionSprite = Game.Instance.Content.Load<Texture2D>("UI/Storage_Items/selection_sprite");
            slotSelectionTarget = new Vector2(0);
            slotSelectionVisibility = new GameValue(0, 20, 1, 0);

            tileSelectionPosition = Vector2.Zero;
            tileSelectionTarget = Vector2.Zero;

            tileSelectionProgress = new GameValue(0, 20, 1);
            tileSelectionBob = new GameValue(0, 120, 1, _repeat:true);

            tooltipSprite = GeneralDependencies.Split(Game.Instance.Content.Load<Texture2D>("UI/Gameplay/tooltipSprite"), 4, 4);
            interactionTooltipSprite = Game.Instance.Content.Load<Texture2D>("UI/Gameplay/interactionTooltip");
            interactionTooltipSpriteOrigin = interactionTooltipSprite.Bounds.Size.ToVector2() / 2f;
            interactionTooltipLocation = new Vector2(0, 0);
            interactionTooltipProgress = new GameValue(0, 20, 1);

            chemicalBarSprite = GeneralDependencies.Split(Game.Instance.Content.Load<Texture2D>("UI/Gameplay/Chemical/chemicalBar"), 2, 2);
            chemicalBarBorderSprite = GeneralDependencies.Split(Game.Instance.Content.Load<Texture2D>("UI/Gameplay/Chemical/chemicalBarBorder"), 2, 2);
            chemicalBarFillSprite = GeneralDependencies.Split(Game.Instance.Content.Load<Texture2D>("UI/Gameplay/Chemical/chemicalBarFill"), 2, 2);
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

            // Slot selection outline
            if ((Cursor.item.type != Item.Type.None) && (UI_Element.hoveredElement != null) && (UI_Element.hoveredElement.type == UI_Element.ElementType.ItemSlot))
            {
                slotSelectionTarget = UI_Element.hoveredElement.rect.Location.ToVector2();
                slotSelectionVisibility.Regenerate(Game.compensation);
            }
            else
            {
                slotSelectionVisibility.Regenerate(-Game.compensation);
            }
            slotSelectionLocation = Vector2.Lerp(slotSelectionLocation, slotSelectionTarget, Game.compensation * 0.5f);
            //

            // Tile selection
            tileSelectionPosition.X = GeneralDependencies.Lerp(tileSelectionPosition.X, tileSelectionTarget.X, 0.2f * Game.compensation, 0.01f);
            tileSelectionPosition.Y = GeneralDependencies.Lerp(tileSelectionPosition.Y, tileSelectionTarget.Y, 0.2f * Game.compensation, 0.01f);
            //

            // Mining outline
            if (Tile.targetedTile != null)
            {
                // goes to tile
                interactionTooltipLocation = Vector2.Lerp(interactionTooltipLocation, Tile.targetedTile.renderedPosition + Camera.renderOffset, 0.1f);
                interactionTooltipProgress.Regenerate(Game.compensation);
            }
            else
            {
                // goes to player
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
                // draws the outline of selected item slot
                /*Rectangle _slotRect = new Rectangle(
                new Point(
                    (int)(slotSelectionLocation.X
                        + (MathF.Sin((1f - openedValue.Percent()) * MathF.PI * 0.5f) * background.rect.Width)
                        - background.rect.Width
                        - slotSelectionOffset),
                    (int)(slotSelectionLocation.Y
                        - slotSelectionOffset)
                    ),
                slotSelectionRect.Size);*/

                slotSelectionRect.X = (int)(slotSelectionLocation.X - slotSelectionOffset);
                slotSelectionRect.Y = (int)(slotSelectionLocation.Y - slotSelectionOffset);
                Game.spriteBatch.Draw(slotSelectionSprite, slotSelectionRect, null, Color.White * MathF.Sin(slotSelectionVisibility.Percent()) * MathF.PI, 0f, Vector2.Zero, SpriteEffects.None, 0.8f);

                // draws the item tooltip
                int fontWidth = 13, fontHeight = 26; // the y of mainFont.MeasureString("?");
                bool _found = true;
                Item.DataType _special = Item.DataType.None;
                string final = "";
                switch (UI_Element.hoveredElement.type)
                {
                    case UI_Element.ElementType.ItemSlot:
                        Item item = ((UI_ItemSlot)UI_Element.hoveredElement).item;
                        final = item.FetchName() + "\n" + item.FetchDescription();
                        if ((item.data != null) && (item.data.Count >= 1))
                        {
                            _special = item.data.Keys.ToList()[0];
                        }
                        if (item.type == Item.Type.None)
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
                    //UI_Element.multiRect.Size.Y = _special == Item.DataType.Chemical ? 0 : UI_Element.multiRect.Size.Y;
                    //UI_Element.multiRect.Size = size.ToPoint();

                    switch (_special)
                    {
                        case Item.DataType.Chemical:
                            Chemical chem = ((Chemical)(((UI_ItemSlot)UI_Element.hoveredElement).item.data[Item.DataType.Chemical]));
                            if (chem.Total() <= 0)
                            {
                                break;
                            }
                            Rectangle chemicalBarRect = new Rectangle(
                                UI_Element.multiRect.X + (int)(UI_ItemSlot.size * 0.25f),
                                UI_Element.multiRect.Y + (int)(UI_ItemSlot.size * 0.75),
                                (int)(fontWidth * ChemicalContainer.crucibleBarSizes[chem.container.type].X),
                                (int)(fontHeight * ChemicalContainer.crucibleBarSizes[chem.container.type].Y)
                                );

                            int _wantedHeight = chemicalBarRect.Height + UI_ItemSlot.size;
                            if (_wantedHeight > UI_Element.multiRect.Height)
                            {
                                UI_Element.multiRect.Height = _wantedHeight;
                            }

                            GeneralDependencies.NineSliceDraw(Game.spriteBatch, chemicalBarSprite, chemicalBarRect, 6, Color.White, 0.915f);
                            // bar fill is drawn at layer 0.92 -> 0.98
                            GeneralDependencies.NineSliceDraw(Game.spriteBatch, chemicalBarBorderSprite, chemicalBarRect, 6, Color.White, 0.99f);

                            double _accumulated = 0;
                            foreach (var x in chem.composition.Reverse().Select((value, index) => new { value, index }))
                            {
                                int index = chem.composition.Count - x.index;
                                Rectangle r = new Rectangle(
                                    chemicalBarRect.X,
                                    chemicalBarRect.Bottom - (int)((((x.value.Value + _accumulated) / chem.container.Size()) * chemicalBarRect.Height)),
                                    chemicalBarRect.Width,
                                    (int)(((x.value.Value / chem.container.Size()) * chemicalBarRect.Height))
                                    );
                                int w = (int)(chemicalBarRect.Width * 0.5f) + (int)(fontWidth * 0.5f);
                                GeneralDependencies.DrawLine(Game.spriteBatch, r.Center, r.Center + new Point(w, 0), 1, Chemical.elementColors[x.value.Key], 1f);
                                // -
                                Point d = new Point(r.Center.X + w + (int)(fontWidth), chemicalBarRect.Y + (((index - 1) * fontHeight) + (int)(fontHeight * 0.25)));
                                GeneralDependencies.DrawLine(Game.spriteBatch, r.Center + new Point(w, 0), d, 1, Chemical.elementColors[x.value.Key], 1f);
                                // /
                                GeneralDependencies.DrawLine(Game.spriteBatch, d, d + new Point((int)(fontWidth * 0.5), 0), 1, Chemical.elementColors[x.value.Key], 1f);
                                // -
                                _accumulated += x.value.Value;
                                r.Height = (int)(((_accumulated / chem.container.Size()) * chemicalBarRect.Height));
                                Game.spriteBatch.Draw(UI_Element.blank, r, null, Chemical.elementColors[x.value.Key], 0f, Vector2.Zero, SpriteEffects.None, 0.93f + (0.001f * (float)((float)index  / (float)chem.composition.Count)));
                            }
                            break;
                        default:
                            break;
                    }

                    GeneralDependencies.NineSliceDraw(Game.spriteBatch, tooltipSprite, UI_Element.multiRect, UI_Element.pixel, Color.White, 0.91f);

                    Game.spriteBatch.DrawString(Game.mainFont, final, renderedPosition, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                }
            }

            Game.spriteBatch.Draw(interactionTooltipSprite, interactionTooltipLocation, null, Color.White * interactionTooltipProgress.Percent(), 0f, interactionTooltipSpriteOrigin, 3f, SpriteEffects.None, 0f);
        }
    }
}
