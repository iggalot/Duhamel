using Godot;
using ProjectDuhamel.models.inventory;
using System;

public partial class GlobalScripts : Node
{
    public int MAX_INVENTORY_SIZE { get; set; } = 30;
    public static InventoryItem[] inventory { get; set; } = null;

    [Signal]
    public delegate void InventoryUpdatedEventHandler();

    public static Node player_node { get; set; } = null;

    public override void _Ready()
    {
        // initializes the inventory with 30 slots (spread over 9 blocks per row
        inventory = new InventoryItem[MAX_INVENTORY_SIZE];
    }

    //public static bool AddItem(InventoryItem item)
    //{
        
    //    for (int i = 0; i < inventory.Length; i++)
    //    {
    //        if (inventory[i] != null && inventory[i].ItemType == item.ItemType && inventory[i].ItemEffect == item.ItemEffect)
    //        {
    //            inventory[i].Quantity += item.Quantity;
    //            var my_emit_node = new Node();
    //            my_emit_node.EmitSignal(SignalName.InventoryUpdated);
    //            GD.Print("Item added to inventory", inventory);
    //            for (int j = 0; j < inventory.Length; j++)
    //            {
    //                GD.Print("j: " + j + inventory[j]);
    //            }
    //            return true;
    //        } else if (inventory[i] == null)
    //        {
    //            inventory[i] = item;
    //            var my_emit_node = new Node();
    //            my_emit_node.EmitSignal(SignalName.InventoryUpdated);
    //            GD.Print("Item added to null inventory slot", inventory);
    //            for (int j = 0; j < inventory.Length; j++)
    //            {
    //                GD.Print("j: " + j + inventory[j]);
    //            }

    //            return true;
    //        }
    //    }

    //    return false;
    //}

    //public static bool RemoveItem(string ItemType, string ItemEffect)
    //{
    //    for (int i = 0; i < inventory.Length; i++)
    //    {
    //        if (inventory[i] != null && inventory[i].ItemType == ItemType && inventory[i].ItemEffect == ItemEffect)
    //        {
    //            inventory[i].Quantity -= 1;

    //            if(inventory[i].Quantity <=0)
    //            {
    //                inventory[i] = null;
    //            }
    //            var my_emit_node = new Node();
    //            my_emit_node.EmitSignal(SignalName.InventoryUpdated);
    //            return true;
    //        }
    //    }

    //    return false;
    //}

    //public static void IncreaseInventorySize()
    //{
    //    var my_emit_node = new Node();
    //    my_emit_node.EmitSignal(SignalName.InventoryUpdated);

    //    return;
    //}

    public static void SetPlayerReference(Player player)
    {
        player_node = player;
    }

    //public static Vector2 AdjustDropPosition(Vector2 pos)
    //{
    //    var radius = 100;
    //    var my_node = new Node();
    //    var nearby_items = player_node.GetTree().GetNodesInGroup("Items");

    //    foreach (var nearby_item in nearby_items)
    //    {
    //        var item = nearby_item as Node2D;
    //        if (item.GlobalPosition.DistanceTo(pos) < radius)
    //        {
    //            var rng = new RandomNumberGenerator();
    //            var random_offset = new Vector2(rng.RandfRange(-radius, radius), rng.RandfRange(-radius, radius));
    //            pos += random_offset;
    //            break;
    //        }
    //    }
    //    return pos;
    //}

 //   public static void DropItem(InventoryItem item_data, Vector2 drop_pos)
    //{
    //    // load the scene and instantiate it
    //    PackedScene item_scene = GD.Load<PackedScene>(item_data.ScenePath);
    //    var item_instance = item_scene.Instantiate() as InventoryItem;
    //    item_instance.SetItemData(item_data);
    //    drop_pos = AdjustDropPosition(drop_pos);
    //    item_instance.GlobalPosition = drop_pos;
    //    player_node.GetTree().CurrentScene.AddChild(item_instance); 
    //}


}
