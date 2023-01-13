using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using YetAnotherRoguelike.Tile_Classes;
using YetAnotherRoguelike.PhysicsObject;
using YetAnotherRoguelike.Graphics;

namespace YetAnotherRoguelike.Scenes
{
    class MainGameScene:Scene
    {
        public static List<Entity> entities;

        public MainGameScene():base(SceneTypes.MainGame, Color.Black)
        {
            entities = new List<Entity>();
            entities.Add(new Player(Vector2.Zero, Game.Instance.Content.Load<Texture2D>("Entities/Player/Body")));
        }

        public override void Update()
        {
            base.Update();

            foreach (Chunk x in Chunk.chunks)
            {
                x.Update();
            }
            Chunk.chunks = Chunk.chunks.Where(n => !n.dead).ToList();

            foreach (Entity e in entities)
            {
                e.Update();
            }

            UI.UI_Inventory_Container.Instance.Update();

            GroundItem.UpdateAll();
            Particle.UpdateAll();
        }

        public override void Draw()
        {
            /*Game.spriteBatch.Begin(SpriteSortMode.Immediate, blendState: BlendState.AlphaBlend);
            Lightmap.Draw(Game.spriteBatch);
            Game.spriteBatch.End();*/

            Lightmap.GenerateMap();

            Matrix renderMatrix = Matrix.CreateTranslation(new Vector3(Camera.renderOffset, 0f));
            Game.spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, samplerState: SamplerState.PointClamp, transformMatrix: renderMatrix);
            Game.graphics.GraphicsDevice.Clear(Color.White * 0.2f);
            base.Draw();

            foreach (Chunk x in Chunk.chunks)
            {
                x.Draw(Game.spriteBatch);
            }

            foreach (Entity e in entities)
            {
                e.Draw(Game.spriteBatch);
            }

            GroundItem.DrawAll(Game.spriteBatch);

            Particle.DrawAll();
            Game.spriteBatch.End();

            Game.spriteBatch.Begin(sortMode:SpriteSortMode.Immediate, blendState:Lightmap.MultiplyBlendState);
            Lightmap.Draw(Game.spriteBatch);
            Game.spriteBatch.End();

            // draw UI here
            Game.spriteBatch.Begin(samplerState:SamplerState.PointClamp);
            UI.UI_Inventory_Container.Instance.Draw();
            Game.spriteBatch.End();
        }
    }
}
