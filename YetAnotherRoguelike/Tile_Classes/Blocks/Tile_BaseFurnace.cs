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
    class Tile_BaseFurnace:Tile
    {
        public enum FurnaceType
        {
            Rudimentary,
            Blast
        }

        public FurnaceType furnaceType;
        public List<Item> inputs;
        public Item crucible, fuel;
        public GameValue progress;
        Chemical currentOutput; // the item produced based on the inputs and etc
        JSON_FurnaceData.Recipe currentRecipe;

        int temperature = 0;
        bool canSmelt; // if the crucible is full, this is false

        List<Item> previousInputs; // compared with the current items to check whether there are any changes
        Item previousCrucible, previousFuel; // currentOutput is fetched if there are any changes (OnItemsChanged)

        GameValue animationProgress;
        int animationIndex;
        GameValue fireAnimation;

        float targetRange = 0; // range of light emitted

        public Tile_BaseFurnace(Point tCoords, Point cCoords, BlockType t, LightSource l, GameValue d = null) :base(tCoords, cCoords, t, d, l)
        {
            furnaceType = t switch
            {
                BlockType.Rudimentary_Furnace => FurnaceType.Rudimentary,
                BlockType.Blast_Furnace => FurnaceType.Blast,
                _ => FurnaceType.Rudimentary
            };

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
            currentOutput = null;

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
            currentOutput = null;

            SetFuelData();

            canSmelt = true;
            if (crucible.type != Item.Type.Crucible)
            {
                canSmelt = false;
            }
            else
            {
                //if (Chemical.collection[crucible.data[Item.DataType.Chemical]].IsFull())
                if (((Chemical)crucible.data[Item.DataType.Chemical]).IsFull())
                {
                    canSmelt = false;
                }
            }


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

                /*if (currentOutput.type != r.output.type)
                {
                    progress.AffectValue(0f);
                    // reset the progress if the two items arent the same
                }*/
                currentRecipe = r;

                currentOutput = r.output;
                break;
            }
            if (currentOutput == null)
            {
                canSmelt = false;
            }
        }

        void SetOutput()
        {
            if (currentOutput == null)
            {
                return;
            }

            if (crucible.type == Item.Type.Crucible)
            {
                // add both chemical compositions together
                //TODO:finish this
                //((Item_Crucible)crucible).chemical.Add(currentOutput);
                ((Chemical)crucible.data[Item.DataType.Chemical]).Add(currentOutput);
            }

            // create deepcopy of recipe
            JSON_FurnaceData.Recipe _recipe = new JSON_FurnaceData.Recipe(new List<Item>(), currentRecipe.output, currentRecipe.temperature);
            foreach (Item x in currentRecipe.inputs)
            {
                _recipe.inputs.Add(Item.DeepCopy(x));
            }
            //_recipe.output = Item.DeepCopy(currentRecipe.output);

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


            if (canSmelt)
            {
                progress.Regenerate(Game.compensation);
            }
            else
            {
                progress.AffectValue(0f);
            }
            if (progress.Percent() >= 1f)
            {
                SetOutput();
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
                targetRange = ((currentOutput != null) && canSmelt) ? MathF.Sin((Game.random.Next(-1000, 1000) / 1000f) * 2f * MathF.PI) + 14f : 0;
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

            if ((currentOutput != null) && (canSmelt))
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

            if (currentOutput == null)
            {
                return;
            }
            if (!canSmelt)
            {
                return;
            }
            // 20-24
            spritebatch.Draw(tileSprites[type][20 + animationIndex], renderedPosition + lowerSpriteOffset, null, lowerSpriteTint, 0f, spriteOrigin, lowerSpriteRenderScale, SpriteEffects.None, Camera.GetDrawnLayer(renderedPosition.Y + lowerSpriteOffset.Y, -0.09f));
        }

        public override void OnDestroy()
        {
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

            base.OnDestroy();
        }
    }
}
