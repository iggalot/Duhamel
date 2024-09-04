using Godot;
using ProjectDuhamel.models.items;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDuhamel.models.inventory
{

    // the basic inventory class -- ideally will be used by monsters, rooms, chest, and the player
    // created as a character body 2D so we have access to GetNode functions
    public partial class Inventory : CharacterBody2D
    {
        // number of items we can carry
        private const int DEFAULT_INVENTORY_MAX = 30;

        public InventoryItem[] InventoryItems { get; set; } = null;

        /// <summary>
        /// Constructor to make an empty inventory
        /// </summary>
        /// <param name="size"></param>
        public Inventory(int size=DEFAULT_INVENTORY_MAX)
        {
            InventoryItems = new InventoryItem[size];
        }

        public bool AddItem(InventoryItem item, int qty = 1)
        {
            for (int i = 0; i < InventoryItems.Length; i++)
            {
                if (InventoryItems[i] == null)
                {
                    item.Quantity = qty;
                    InventoryItems[i] = item;
                    
                    return true; // success
                }
            }
            return false; // unable to add item
        }

        /// <summary>
        /// The Character2D node that owns this inventory
        /// </summary>
        /// <param name="node"></param>
        public void Display(CharacterBody2D node)
        {
            Player player = node as Player;
            
            CanvasLayer ui = player.GetNode<CanvasLayer>("InventoryUI");
            //GD.Print(ui);

            Control ui_control = player.GetNode<Control>("InventoryUI/ColorRect/Inventory_UI");
            ui_control.Scale = new Vector2(0.5f, 0.5f); // sets the scale of the inventory window display
            //GD.Print(ui_control);

            Label ui_label = ui.GetNode<Label>("ColorRect/Label");
            ui.Visible = !ui.Visible;  // toggle the visibility of the inventory

            // now display the contents of our inventory
            PackedScene slot_scene = GD.Load<PackedScene>("res://scenes/inventory_slot.tscn");
            GridContainer container = ui_control.GetNode<GridContainer>("GridContainer");  // gets the container node in the visual tree

            // clear the container of any children
            container.GetChildren().Clear();

            // loop through our inventory, setting values and textures for our inventiry object
            for (int i = 0; i < InventoryItems.Length; i++)
            {
                if (InventoryItems[i] != null)
                {
                    InventoryItem inv_item = InventoryItems[i];
                    ItemData item_data = inv_item.itemData;
                    BaseItemObjectGraphics item_graphics = inv_item.itemGraphics;

                    InventorySlot slot = new InventorySlot(inv_item, i, inv_item.Quantity);

                    Node slot_instance = slot_scene.Instantiate();
                    slot_instance.Name = "Inventory_Slot_" + i.ToString();

                    ui_label.Text = item_data.ItemName;

                    // Set the quantity and the graphic within the InnerBorder
                    Label item_qty_label = slot_instance.GetNode<Label>("InnerBorder/ItemQuantity");
                    item_qty_label.Text = inv_item.Quantity.ToString();
                    Sprite2D item_sprite_texture = slot_instance.GetNode<Sprite2D>("InnerBorder/ItemIcon");
                    item_sprite_texture.Texture = item_graphics.GetTextureRandom();

                    // Set the item data for the slot
                    Label item_name_label = slot_instance.GetNode<Label>("DetailsPanel/ItemName");
                    item_name_label.Text = item_data.ItemName;
                    Label item_type_label = slot_instance.GetNode<Label>("DetailsPanel/ItemType");
                    item_type_label.Text = item_data.ItemType.ToString();
                    Label item_effect_label = slot_instance.GetNode<Label>("DetailsPanel/ItemEffect");
                    item_effect_label.Text = item_data.ItemEffect.ToString();

                    // Add the slot image to the scene
                    container.AddChild(slot_instance);
                }
            }
        }
    }
}
