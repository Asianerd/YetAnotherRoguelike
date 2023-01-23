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

        // for all the gameplay stuff like hp, etc etc etc
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
        }

        public override void Update()
        {
            base.Update();

            if (Player.Instance.cursorTile != null)
            {
                tileSelectionTarget = Player.Instance.cursorTile.tileCoordinatesV;
            }

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

            tileSelectionPosition.X = GeneralDependencies.Lerp(tileSelectionPosition.X, tileSelectionTarget.X, 0.2f * Game.compensation, 0.01f);
            tileSelectionPosition.Y = GeneralDependencies.Lerp(tileSelectionPosition.Y, tileSelectionTarget.Y, 0.2f * Game.compensation, 0.01f);
        }

        public override void Draw()
        {
            base.Draw();
            Game.spriteBatch.Draw(tileSelectionSprite, (tileSelectionPosition * Tile.tileSize) + Camera.renderOffset, null, Color.White * (tileSelectionProgress.Percent() + ((MathF.Sin((float)tileSelectionBob.Percent() * 2f * MathF.PI) * 0.2f) - 0.5f)), 0f, tileSelectionSpriteOrigin, Tile.spriteRenderScale, SpriteEffects.None, 1f);
            if (Player.Instance.cursorTile.neighbours[1, 2] == Tile.BlockType.Air)
            {
                Game.spriteBatch.Draw(tileSelectionSprite, ((tileSelectionPosition + new Vector2(0, 0.75f)) * Tile.tileSize) + Camera.renderOffset, null, Color.White * (tileSelectionProgress.Percent() + ((MathF.Sin((float)tileSelectionBob.Percent() * 2f * MathF.PI) * 0.2f) - 0.5f)), 0f, tileSelectionSpriteOrigin, new Vector2(1, 0.5f) * Tile.spriteRenderScale, SpriteEffects.None, 1f);
            }
        }
    }
}
