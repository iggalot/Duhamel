using Godot;

public partial class InventoryUi : Control
{
    // store our global scripts as a singleton....
    private GlobalScripts global_scripts { get; set; } = null;


    // the reference to our grid container in the scene
    public GridContainer UiGridContainer { get; set; } = null;

    public override void _Ready()
    {
        //// set up our nodes getters and setters
        //UiGridContainer = GetNode<GridContainer>("GridContainer");
        //global_scripts = GetNode<GlobalScripts>("//root/GlobalScripts") as GlobalScripts;

        //// connect the signal
        //global_scripts.InventoryUpdated += _on_inventory_updated;
        //_on_inventory_updated(); // run it for the first time automatically.
    }

    public override void _Process(double delta)
    {

    }

    // update the inventory UI
    public void _on_inventory_updated()
    {
        GD.Print("inventory was updated");

        // clear existing slots
        clear_grid_container();

        
        //// add slots for each inventory position
        //for (int i = 0; i < GlobalScripts.inventory.Length; i++)
        //{
        //    InventoryItem item = GlobalScripts.inventory[i];
        //    var slot = GlobalScripts.inventory_slot_scene.Instantiate() as InventorySlot ;

        //    if(item != null)
        //    {
        //        slot.SetItem(item);
        //    } else
        //    {
        //        slot.SetEmpty();
        //    }

        //    UiGridContainer.AddChild(slot);
        //}

    }

    public void clear_grid_container()
    {
        while (UiGridContainer.GetChildCount() > 0)
        {
            var child = UiGridContainer.GetChild(0);
            UiGridContainer.RemoveChild(child);
            child.QueueFree();
        }
    }
}
