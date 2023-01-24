using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using YetAnotherRoguelike.PhysicsObject;

namespace YetAnotherRoguelike.UI
{
    class UI_Inventory_Container : UI_Container
    {
        public static UI_Inventory_Container Instance = null;
        public static Texture2D slotSelectionSprite;
        public static int slotSelectionOffset = 4;
        public static Texture2D hotbarSelectionSprite;
        public static int hotbarSelectionOffset = 4;
        public static int rowLength = 5;

        public InventoryPage page = InventoryPage.Storage;
        public bool opened = false;
        public GameValue openedValue;

        public UI_Background background;
        public List<UI_Inventory_TabButton> tabButtons;
        public Dictionary<InventoryPage, List<UI_Element>> inventoryElements;
        public int containerOffset;

        #region Storage
        public List<UI_ItemSlot> itemSlots;
        public List<UI_ItemSlot> hotbarSlots;

        public Rectangle slotSelectionRect;
        public Vector2 slotSelectionTarget;
        public Vector2 slotSelectionLocation;
        public GameValue slotSelectionVisibility;

        public Rectangle hotbarSelectRect;
        public float hotbarSelectProgress = 0;
        #endregion

        #region Crafting
        public UI_Inventory_CraftingParent craftingParent;
        #endregion

        public UI_Inventory_Container() : base(new List<UI_Element>() { })
        {
            if (Instance == null)
            {
                Instance = this;
                UI_Inventory_TabButton.Initialize();
                UI_Inventory_CraftingParent.Initialize();
            }

            openedValue = new GameValue(0, 10, 1);
            active = true;

            background = new UI_Background(this, new Rectangle(0, 0, (int)((rowLength + 1f) * UI_ItemSlot.size), (int)((MathF.Ceiling((float)Player.inventorySize / (float)rowLength) + 2.5f) * UI_ItemSlot.size)));

            inventoryElements = new Dictionary<InventoryPage, List<UI_Element>>();
            tabButtons = new List<UI_Inventory_TabButton>();
            foreach (var x in Enum.GetValues(typeof(InventoryPage)).Cast<InventoryPage>().Select((value, index) => new { value, index }))
            {
                inventoryElements.Add(x.value, new List<UI_Element>());
                if (x.value == InventoryPage.None)
                {
                    continue;
                }
                tabButtons.Add(new UI_Inventory_TabButton(this, new Rectangle(
                    x.index * (background.rect.Width / 3),
                    background.rect.Top - UI_ItemSlot.size,
                    background.rect.Width / 3,
                    UI_ItemSlot.size + (int)(UI_Element.pixel * 0.75f)
                    ), x.value));
            }

            #region Storage section
            itemSlots = new List<UI_ItemSlot>();
            foreach (var i in (Player.Instance.inventory.Select((value, index) => new { value, index })))
            {
                itemSlots.Add(new UI_ItemSlot(this, new Rectangle(
                    new Point(
                        (int)(((i.index * UI_ItemSlot.size) + (UI_ItemSlot.size / 2)) - ((MathF.Floor(i.index / (float)rowLength) * (UI_ItemSlot.size * rowLength)))),
                        (int)((UI_ItemSlot.size * (MathF.Floor(i.index / (float)rowLength))) - (((Player.inventorySize / rowLength) * UI_ItemSlot.size) / 2f))
                        ),
                    new Point(UI_ItemSlot.size)
                    ), i.value));
            }

            hotbarSlots = new List<UI_ItemSlot>();
            foreach (var i in (Player.Instance.hotbar.Select((value, index) => new { value, index })))
            {
                hotbarSlots.Add(new UI_ItemSlot(this,
                    new Rectangle(
                        new Point(
                            (int)((rowLength + 1f) * UI_ItemSlot.size),
                            (int)((((float)i.index / (float)Player.hotbarSize) * (int)((MathF.Ceiling((float)Player.inventorySize / (float)rowLength)) * UI_ItemSlot.size))
                            + (Game.screenSize.Y * 0.5f)
                            - ((MathF.Ceiling((float)Player.inventorySize / (float)rowLength) - 1f) * UI_ItemSlot.size * 0.5f))
                            ),
                        new Point(UI_ItemSlot.size)
                        ),
                    i.value
                    ));
            }

            hotbarSelectRect = new Rectangle(0, 0, (int)(UI_ItemSlot.size + (hotbarSelectionOffset * 2f)), (int)(UI_ItemSlot.size + (hotbarSelectionOffset * 2f)));
            hotbarSelectionSprite = Game.Instance.Content.Load<Texture2D>("UI/Storage_Items/hotbar_selection_sprite");

            slotSelectionRect = new Rectangle(0, 0, (int)(UI_ItemSlot.size + (slotSelectionOffset * 2f)), (int)(UI_ItemSlot.size + (slotSelectionOffset * 2f)));
            slotSelectionSprite = Game.Instance.Content.Load<Texture2D>("UI/Storage_Items/selection_sprite");
            slotSelectionTarget = new Vector2(0);
            slotSelectionVisibility = new GameValue(0, 20, 1, 0);

            inventoryElements[InventoryPage.Storage].Add(background);
            foreach (UI_ItemSlot x in itemSlots)
            {
                inventoryElements[InventoryPage.Storage].Add(x);
            }

            foreach (UI_ItemSlot x in hotbarSlots)
            {
                inventoryElements[InventoryPage.Storage].Add(x);
            }

            foreach (UI_Inventory_TabButton x in tabButtons)
            {
                inventoryElements[InventoryPage.Storage].Add(x);
            }
            #endregion

            #region Crafting section
            craftingParent = new UI_Inventory_CraftingParent(this, new Rectangle(0, 0, 0, 0));
            inventoryElements[InventoryPage.Crafting].Add(background);

            inventoryElements[InventoryPage.Crafting].Add(craftingParent);

            foreach (UI_Inventory_TabButton x in tabButtons)
            {
                inventoryElements[InventoryPage.Crafting].Add(x);
            }
            #endregion

            #region Equipment section


            inventoryElements[InventoryPage.Equipment].Add(background);

            foreach (UI_Inventory_TabButton x in tabButtons)
            {
                inventoryElements[InventoryPage.Equipment].Add(x);
            }
            #endregion

            OnPageChange();
            OnScreenResize();
        }

        public override void Update()
        {
            #region Input
            foreach (var i in (new Keys[] { Keys.D1, Keys.D2, Keys.D3, Keys.D4 }).Select((value, index) => new { value, index }))
            {
                if (Input.collection[i.value].active)
                {
                    Player.Instance.selectionIndex = i.index;
                }
            }
            #endregion

            openedValue.Regenerate(Game.compensation * (opened ? -1f : 1f));
            if (openedValue.Percent() == 1f)
            {
                if (page != InventoryPage.None)
                {
                    page = InventoryPage.None;
                    OnPageChange();
                }
            }

            containerOffset = (int)(MathF.Sin((1f - openedValue.Percent()) * MathF.PI * 0.5f) * background.rect.Width) - background.rect.Width;

            switch (page)
            {
                case InventoryPage.Storage: StorageUpdate(); break;
                case InventoryPage.Equipment: EquipmentUpdate(); break;
                case InventoryPage.Crafting: CraftingUpdate(); break;
                default: break;
            }

            foreach (UI_Element x in inventoryElements[page])
            {
                x.Update();
            }

            slotSelectionVisibility.Regenerate(Game.compensation * ((Cursor.item.type != Item.Type.None) && (UI_Element.hoveredElement.type == UI_Element.ElementType.ItemSlot) ? 1 : -1));
            slotSelectionLocation = Vector2.Lerp(slotSelectionLocation, slotSelectionTarget, Game.compensation * 0.5f);
        }

        public void ChangePage(InventoryPage p)
        {
            page = p;
            OnPageChange();
        }

        public void OnPageChange()
        {
            elements = inventoryElements[page];

            if (page == InventoryPage.Crafting)
            {
                craftingParent.UpdateList();
            }
        }

        public override void Draw()
        {
            foreach (UI_Element x in inventoryElements[page])
            {
                x.Draw(Game.spriteBatch, offsetX:containerOffset);
            }
            switch (page)
            {
                case InventoryPage.Storage:
                    // draw the selection box for the hotbar
                    hotbarSelectRect.X += containerOffset;
                    Game.spriteBatch.Draw(hotbarSelectionSprite, hotbarSelectRect, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);
                    break;
                default:
                    break;
            }
            Rectangle _slotRect = new Rectangle(
                new Point(
                    (int)(slotSelectionLocation.X
                        + (MathF.Sin((1f - openedValue.Percent()) * MathF.PI * 0.5f) * background.rect.Width)
                        - background.rect.Width
                        - slotSelectionOffset),
                    (int)(slotSelectionLocation.Y
                        - slotSelectionOffset)
                    ),
                slotSelectionRect.Size);
            Game.spriteBatch.Draw(slotSelectionSprite, _slotRect, null, Color.White * MathF.Sin(slotSelectionVisibility.Percent()) * MathF.PI, 0f, Vector2.Zero, SpriteEffects.None, 1f);
        }

        #region Updates
        void StorageUpdate()
        {
            hotbarSelectProgress = GeneralDependencies.Lerp(hotbarSelectProgress, Player.Instance.selectionIndex, Game.compensation * 0.5f, snapWeight: 0.01f);
            /*hotbarSelectRect.Location = new Point(
                    (int)((rowLength + 1f) * UI_ItemSlot.size)
                        - hotbarSelectionOffset,
                    (int)((((float)hotbarSelectProgress / (float)Player.hotbarSize) * (int)((MathF.Ceiling((float)Player.inventorySize / (float)rowLength)) * UI_ItemSlot.size))
                        + (Game.screenSize.Y * 0.5f)
                        - ((MathF.Ceiling((float)Player.inventorySize / (float)rowLength) - 1f) * UI_ItemSlot.size * 0.5f))
                        - hotbarSelectionOffset
                    );*/
            hotbarSelectRect.Location = new Point(
                    (int)(
                    (((rowLength - 1) * UI_ItemSlot.size) * ((float)hotbarSelectProgress / ((float)Player.hotbarSize - 1f)))
                    + (UI_ItemSlot.size * 0.5f)
                    + background.rect.Left
                    - hotbarSelectionOffset
                    ),
                    (int)(
                    background.rect.Bottom
                    - (1.5f * UI_ItemSlot.size)
                    - hotbarSelectionOffset
                    ));

            if ((Cursor.item.type != Item.Type.None) && (hoveredContainer != null) && (hoveredContainer == this) && (UI_Element.hoveredElement.type == UI_Element.ElementType.ItemSlot))
            {
                slotSelectionTarget = UI_Element.hoveredElement.rect.Location.ToVector2();
            }
        }

        void CraftingUpdate()
        {

        }

        void EquipmentUpdate()
        {

        }
        #endregion

        public override void Toggle()
        {
            opened = !opened;
            if (opened)
            {
                page = InventoryPage.Storage;
            }
            OnPageChange();
        }

        public override void OnScreenResize()
        {
            base.OnScreenResize();

            background.rect.Location = new Point(0, (int)((Game.screenSize.Y / 2f) - (background.rect.Height / 2f)));

            foreach (var i in (itemSlots.Select((value, index) => new { value, index })))
            {
                i.value.rect.Location = new Point(
                    (int)(
                        (((i.index % rowLength) + 0.5f) * UI_ItemSlot.size)
                        + background.rect.Left
                        ),
                    (int)(
                        (MathF.Floor(i.index / rowLength) * UI_ItemSlot.size)
                        + background.rect.Top
                        + (UI_ItemSlot.size * 0.5f)
                        )
                    /*+ (Game.screenSize.Y * 0.5f)
                    - (MathF.Ceiling((float)Player.inventorySize / (float)rowLength) * UI_ItemSlot.size * 0.5f))*/
                    );
            }

            foreach (var i in (hotbarSlots.Select((value, index) => new { value, index })))
            {
                /*i.value.rect.Location = new Point(
                    (int)((rowLength + 1f) * UI_ItemSlot.size),
                    (int)((((float)i.index / (float)Player.hotbarSize) * (int)((MathF.Ceiling((float)Player.inventorySize / (float)rowLength)) * UI_ItemSlot.size))
                        + (Game.screenSize.Y * 0.5f)
                        - ((MathF.Ceiling((float)Player.inventorySize / (float)rowLength) - 1f) * UI_ItemSlot.size * 0.5f))
                    );*/
                i.value.rect.Location = new Point(
                    (int)(
                    (((rowLength - 1) * UI_ItemSlot.size) * ((float)i.index / ((float)Player.hotbarSize - 1f)))
                    + (UI_ItemSlot.size * 0.5f)
                    + background.rect.Left
                    ),
                    (int)(
                    background.rect.Bottom
                    - (1.5f * UI_ItemSlot.size)
                    ));
            }

            foreach (var x in tabButtons.Select((value, index) => new { value, index }))
            {
                x.value.rect.Location = new Point(
                    x.index * (background.rect.Width / 3),
                    background.rect.Top - UI_ItemSlot.size
                    );
            }

            craftingParent.rect = new Rectangle(
                (int)(background.rect.Left + (UI_ItemSlot.size * 0.5f)),
                (int)(background.rect.Top + UI_ItemSlot.size),
                (int)(background.rect.Width - UI_ItemSlot.size),
                (int)(background.rect.Height - (UI_ItemSlot.size * 2f))
                );

            craftingParent.OnScreenResize(background.rect);
        }

        public enum InventoryPage
        {
            None,
            Storage,
            Crafting,
            Equipment
        }
    }
}
