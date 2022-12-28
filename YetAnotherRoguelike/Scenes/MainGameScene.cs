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

            GroundItem.UpdateAll();
            Particle.UpdateAll();

            Lightmap.GenerateMap();
        }

        public override void Draw()
        {
            Game.spriteBatch.Begin(SpriteSortMode.Immediate, blendState: BlendState.AlphaBlend);
            Lightmap.Draw(Game.spriteBatch);
            Game.spriteBatch.End();

            Matrix renderMatrix = Matrix.CreateTranslation(new Vector3(Camera.renderOffset, 0f));
            Game.spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: renderMatrix);
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

            /*Game.spriteBatch.Begin();
            // draw UI here
            Game.spriteBatch.End();*/
        }
    }
}
