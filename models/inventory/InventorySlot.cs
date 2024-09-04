using Godot;
using System;

namespace ProjectDuhamel.models.inventory
{
    public partial class InventorySlot : Control
    {
        private const int DEFAULT_MAX_QUANTITY = 99;

        // store our global scripts as a singleton....
        private GlobalScripts global_scripts { get; set; } = null;

        // The item contained in this slot
        public InventoryItem Item { get; set; } = null;

        // The position in the inventory array for this slot item
        public int SlotId { get; set; } = -1;

        // The quantity of this item contained in this slot
        public int Quantity { get; set; } = 0;


        // GEtters and SEtters

        public Sprite2D ItemIcon { get; set; } = new Sprite2D();
        public Label QuantityLabel { get; set; } = new Label();
        public ColorRect DetailsPanel { get; set; } = new ColorRect();
        public Label ItemName { get; set; } = new Label();
        public Label ItemType { get; set; } = new Label();
        public Label ItemEffect { get; set; } = new Label();
        public ColorRect UsagePanel { get; set; } = new ColorRect();


        public override void _Process(double delta)
        {
            // Called every frame. Delta is time since last frame.
            // Update game logic here.
        }

        public InventorySlot()
        {
            
        }

        // Called every time the node is added to the scene.
        // Initialization here
        public override void _Ready()
        {
            // store our global scripts reference
            global_scripts = GetNode<GlobalScripts>("//root/GlobalScripts") as GlobalScripts;

            // set up the getters and setters
            ItemIcon = GetNode<Sprite2D>("InnerBorder/ItemIcon");
            QuantityLabel = GetNode<Label>("InnerBorder/ItemQuantity");
            DetailsPanel = GetNode<ColorRect>("DetailsPanel");
            ItemName = GetNode<Label>("DetailsPanel/ItemName");
            ItemType = GetNode<Label>("DetailsPanel/ItemType");
            ItemEffect = GetNode<Label>("DetailsPanel/ItemEffect");
            UsagePanel = GetNode<ColorRect>("UsagePanel");
        }


        public InventorySlot(InventoryItem item, int slotId, int quantity)
        {
            Item = item;
            SlotId = slotId;
            Quantity = quantity;
        }

        public void AddItemAtIndex(InventoryItem item, int index)
        {
            Item = item;
            SlotId = index;
            Quantity = item.Quantity;
        }


        public void _on_item_button_mouse_exited()
        {
            DetailsPanel.Visible = false;

           // GD.Print("item button exited");
        }

        // our hover state
        public void _on_item_button_mouse_entered()
        {
            //GD.Print("item button entered");
            UsagePanel.Visible = false;
            DetailsPanel.Visible = true;

        }

        // hide item details on hover exit
        public void _on_item_button_pressed()
        {
            //GD.Print("item button pressed");
            DetailsPanel.Visible = false;
            UsagePanel.Visible = true;
        }

        public void SetEmpty()
        {
            ItemIcon.Texture = null;
            QuantityLabel.Text = "";
        }

        public void SetItem(InventoryItem new_item)
        {
            Item = new_item;
            ItemIcon.Texture = new_item.ItemTexture; ;
            QuantityLabel.Text = new_item.Quantity.ToString();
            ItemName.Text = new_item.ItemName;
            ItemType.Text = new_item.ItemType.ToString();
            if (Item.ItemEffect != String.Empty)
            {
                ItemEffect.Text = "+ " + Item.ItemEffect.ToString();
            }
            else
            {
                ItemEffect.Text = "";
            }
        }

        /// <summary>
        /// updates the invenotry slot quantities.  If all items are removed, the slot item is set to null (empty)
        /// otherwise, the quantity in the slot is updated, and a new item is returned with the removed quanitity
        /// </summary>
        /// <param name="qty"></param>
        /// <returns></returns>
        public InventoryItem RemoveItemQty(int qty)
        {
            int returnedQty;
            InventoryItem returnedItem = Item.Copy();

            if (qty > Quantity)
            {
                Item.Quantity -= qty;
                returnedQty = qty;
            }
            else
            {
                Item = null;
                returnedQty = Quantity;
            }

            returnedItem.Quantity = returnedQty;

            return returnedItem;
        }
    }
}
