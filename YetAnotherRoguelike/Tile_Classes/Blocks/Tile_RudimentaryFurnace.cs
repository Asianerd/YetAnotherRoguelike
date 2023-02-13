using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using YetAnotherRoguelike.Data;
using YetAnotherRoguelike.Graphics;

namespace YetAnotherRoguelike.Tile_Classes
{
    class Tile_RudimentaryFurnace:Tile
    {
        public List<Item> inputs;
        public Item crucible, fuel;
        public GameValue progress;
        Item currentOutput; // the item produced based on the inputs and etc
        JSON_FurnaceData.Recipe currentRecipe;

        int temperature = 0;

        List<Item> previousInputs; // compared with the current items to check whether there are any changes
        Item previousCrucible, previousFuel; // currentOutput is fetched if there are any changes (OnItemsChanged)

        GameValue animationProgress;
        int animationIndex;
        GameValue fireAnimation;

        float targetRange = 0; // range of light emitted

        public Tile_RudimentaryFurnace(Point tCoords, Point cCoords, GameValue d = null) :base(tCoords, cCoords, BlockType.Rudimentary_Furnace, d,
            l:new LightSource(tCoords.ToVector2(), new Color(255, 189, 0), 1, 0)
            )
        {
            interactable = true;

            inputs = new List<Item>();
            previousInputs = new List<Item>();
            for (int i = 0; i < 3; i++)
            {
                inputs.Add(Item.Empty());
                previousInputs.Add(Item.Empty());
            }
            crucible = Item.Empty();
            fuel = Item.Empty();
            previousCrucible = Item.Empty();
            previousFuel = Item.Empty();

            progress = new GameValue(0, 100, 1);
            currentOutput = Item.Empty();

            animationProgress = new GameValue(0, 5, 1);
            fireAnimation = new GameValue(0, 1, 1);
            lightsource.position.Y += 0.75f;
        }

        bool ItemChanged()
        {
            for (int i = 0; i < inputs.Count; i++)
            {
                if (!inputs[i].IsSame(previousInputs[i]))
                {
                    return true;
                }
            }
            if (!fuel.IsSame(previousFuel))
            {
                return true;
            }
            if (!crucible.IsSame(previousCrucible))
            {
                return true;
            }

            return false;
        }

        public void OnItemsChange()
        {
            currentOutput = Item.Empty();

            SetFuelData();

            Dictionary<Item.Type, int> itemCount = new Dictionary<Item.Type, int>();
            foreach (Item x in inputs)
            {
                if (x.type == Item.Type.None)
                {
                    continue;
                }
                if (!itemCount.ContainsKey(x.type))
                {
                    itemCount.Add(x.type, 0);
                }
                itemCount[x.type] += x.amount;
            }

            foreach (JSON_FurnaceData.Recipe r in JSON_FurnaceData.recipes)
            {
                if (itemCount.Keys.Count != r.inputs.Count)
                {
                    continue;
                }

                if (temperature < r.temperature)
                {
                    continue;
                }

                bool result = true;
                foreach (Item x in r.inputs)
                {
                    if (x.type == Item.Type.None)
                    {
                        continue;
                    }
                    if (!itemCount.ContainsKey(x.type))
                    {
                        result = false;
                        break;
                    }
                    if (itemCount[x.type] < x.amount)
                    {
                        result = false;
                        break;
                    }
                }
                if (!result)
                {
                    continue;
                }

                if (currentOutput.type != r.output.type)
                {
                    progress.AffectValue(0f);
                    // reset the progress if the two items arent the same
                }
                currentRecipe = r;
                currentOutput = r.output;
                return;
            }
        }

        void SetOutput()
        {
            if (currentOutput.type == Item.Type.None)
            {
                return;
            }

            if (crucible.type == Item.Type.None)
            {
                crucible.type = currentOutput.type;
            }
            crucible.amount += currentOutput.amount;

            // create deepcopy of recipe
            JSON_FurnaceData.Recipe _recipe = new JSON_FurnaceData.Recipe(new List<Item>(), Item.Empty(), currentRecipe.temperature);
            foreach (Item x in currentRecipe.inputs)
            {
                _recipe.inputs.Add(Item.DeepCopy(x));
            }
            _recipe.output = Item.DeepCopy(currentRecipe.output);

            foreach (Item x in inputs)
            {
                if (x.type == Item.Type.None)
                {
                    continue;
                }
                foreach (Item r in _recipe.inputs)
                {
                    // continuously subtract items from input based on recipe
                    if (r.amount == 0)
                    {
                        continue;
                    }
                    if (x.type == r.type)
                    {
                        if (x.amount < r.amount)
                        {
                            r.amount -= x.amount;
                            x.amount = 0;
                        }
                        else
                        {
                            x.amount -= r.amount;
                            r.amount = 0;
                        }
                    }
                }
            }
        }

        void SetFuelData()
        {
            temperature = 0;
            if (JSON_FurnaceData.fuelData.ContainsKey(fuel.type))
            {
                temperature = JSON_FurnaceData.fuelData[fuel.type][0];
            }
        }

        public override void Update()
        {
            base.Update();

            if (currentOutput.type != Item.Type.None)
            {
                progress.Regenerate(Game.compensation);
                if (progress.Percent() >= 1f)
                {
                    SetOutput();
                    // dont have to set progress to 0% because its done automatically when the items are changed
                }
            }
            else
            {
                progress.AffectValue(0f);
            }

            if (targeted && (Input.collection[Keys.E].active))
            {
                UI.UI_Furnace.Instance.Activate("Rudimentary Furnace", inputs, crucible, fuel, progress);
            }

            if (ItemChanged())
            {
                OnItemsChange();
            }
            for (int i = 0; i < inputs.Count; i++)
            {
                previousInputs[i].AssignData(inputs[i]);
            }
            previousFuel.AssignData(fuel);
            previousCrucible.AssignData(crucible);

            lightsource.range = GeneralDependencies.Lerp(lightsource.range, targetRange, 0.2f, 0.2f);
            if (lightsource.range == targetRange)
            {
                targetRange = currentOutput.type == Item.Type.None ? 0 : MathF.Sin((Game.random.Next(-1000, 1000) / 1000f) * 2f * MathF.PI) + 14f;
            }

            animationProgress.Regenerate(Game.compensation);
            if (animationProgress.Percent() >= 1f)
            {
                animationProgress.AffectValue(0f);
                animationIndex++;

                if (animationIndex > 4)
                {
                    animationIndex = 0;
                }
            }

            if (currentOutput.type != Item.Type.None)
            {
                fireAnimation.Regenerate(Game.compensation);
                if (fireAnimation.Percent() >= 1f)
                {
                    fireAnimation.AffectValue(0f);
                    Particle.collection.Add(new Particles.Fire(
                        new Vector2(
                            tileCoordinatesV.X + (Game.random.Next(-1000, 1000) / 10000f),
                            tileCoordinatesV.Y + (Game.random.Next(-1000, 1000) / 10000f) + 0.7f
                            )
                        ));
                }
            }
        }

        public override void Draw(SpriteBatch spritebatch)
        {
            base.Draw(spritebatch);

            if (currentOutput.type == Item.Type.None)
            {
                return;
            }
            // 20-24
            spritebatch.Draw(tileSprites[type][20 + animationIndex], renderedPosition + lowerSpriteOffset, null, lowerSpriteTint, 0f, spriteOrigin, lowerSpriteRenderScale, SpriteEffects.None, Camera.GetDrawnLayer(renderedPosition.Y + lowerSpriteOffset.Y, -0.09f));
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            // TODO : FIX (not spawning for some reason)

            foreach (Item x in inputs)
            {
                if (x.type == Item.Type.None)
                {
                    continue;
                }
                GroundItem.collection.Add(new GroundItem(
                        x,
                        tileCoordinatesV + new Vector2(
                            Game.random.Next(-100, 100) / 200f,
                            Game.random.Next(-100, 100) / 200f
                            ),
                        tileCoordinatesV
                        ));
            }

            if (fuel.type != Item.Type.None)
            {
                GroundItem.collection.Add(new GroundItem(
                        fuel,
                        tileCoordinatesV + new Vector2(
                            Game.random.Next(-100, 100) / 200f,
                            Game.random.Next(-100, 100) / 200f
                            ),
                        tileCoordinatesV
                        ));
            }

            if (crucible.type != Item.Type.None)
            {
                GroundItem.collection.Add(new GroundItem(
                        crucible,
                        tileCoordinatesV + new Vector2(
                            Game.random.Next(-100, 100) / 200f,
                            Game.random.Next(-100, 100) / 200f
                            ),
                        tileCoordinatesV
                        ));
            }
        }
    }
}
