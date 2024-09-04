using Godot;

namespace ProjectDuhamel.models.inventory
{
    [Tool]
    public partial class InventoryItem : Node2D
    {
        // store our global scripts as a singleton....
        private GlobalScripts global_scripts { get; set; } = null;

        // item details for editor window
        [Export] public string ItemType { get; set; } = "";

        [Export] public string ItemName { get; set; } = "";

        [Export] public string ItemEffect { get; set; } = "";
        [Export] public Texture2D ItemTexture { get; set; }
        [Export] public string ItemTextureStringPath { get; set; } = "res://assets/items/icon3.png";

        // scene-tree node reference
        public string ScenePath { get; set; } = "res://scenes/InventoryItem.tscn";

        public Sprite2D IconSprite { get; set; }

        public bool PlayerIsInRange { get; set; } = false;


        public int Quantity { get; set; }

        public InventoryItem() { }

        public InventoryItem(int quantity, string itemType, string itemName, Texture2D itemTexture, string itemEffect)
        {
            Quantity = quantity;
            ItemType = itemType;
            ItemName = itemName;
            ItemEffect = itemEffect;
        }

        // called when the node enters the scene tree for the first time
        public override void _Ready()
        {
            // store our global scripts reference
            global_scripts = GetNode<GlobalScripts>("//root/GlobalScripts") as GlobalScripts;


            // establish our setters and getters
            IconSprite = GetNode<Sprite2D>("Sprite2D");

            // get the collision oject
            CollisionObject2D collision = GetNode<CollisionObject2D>("Area2D");

            // Set the collision layers and masks for this player in Godot
            collision.SetCollisionLayerValue(1, false);  // turn off the default
            collision.SetCollisionLayerValue((int)CollisionLayerAssignments.ITEMS, true);  // assign to proper layer

            collision.SetCollisionMaskValue(1, false);  // turn off the default
            collision.SetCollisionMaskValue((int)CollisionLayerAssignments.WALLS, true);  // assign to proper layer
            collision.SetCollisionMaskValue((int)CollisionLayerAssignments.SPELLS_HOSTILE, true);  // assign to proper layer
            collision.SetCollisionMaskValue((int)CollisionLayerAssignments.ENVIRONMENT, true);  // assign to proper layer
            collision.SetCollisionMaskValue((int)CollisionLayerAssignments.ITEMS, true);  // assign to proper layer
            collision.SetCollisionMaskValue((int)CollisionLayerAssignments.MONSTERS, true);  // assign to proper layer
            collision.SetCollisionMaskValue((int)CollisionLayerAssignments.PLAYER, true);  // assign to proper layer

            ItemTexture = GD.Load(ItemTextureStringPath) as Texture2D;


            if (!Engine.IsEditorHint())
            {
                IconSprite.Texture = ItemTexture; ;
            }
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(double delta)
        {
            ItemTexture = GD.Load(ItemTextureStringPath) as Texture2D;

            if (Engine.IsEditorHint())
            {
                IconSprite.Texture = ItemTexture as Texture2D;
            }

            if (PlayerIsInRange && Input.IsActionJustPressed("ui_add"))
            {
                PickupItem();
            }
        }

        public void PickupItem()
        {
            var item = new InventoryItem()
            {
                Quantity = 1,
                ItemType = this.ItemType,
                ItemName = this.ItemName,
                ItemTexture = this.ItemTexture,
                ItemEffect = this.ItemEffect,
                ScenePath = this.ScenePath
            };

            if (GlobalScripts.player_node != null)
            {
                GlobalScripts.AddItem(item);
                this.QueueFree();
            }
        }

        public void _on_area_2d_body_entered(Node2D body)
        {
            GD.Print("Player is in range");

            if (body.IsInGroup("Player"))
            {
                var player = body as Player;
                PlayerIsInRange = true;
                player.UIInteract.Visible = true;
            }
        }

        public void _on_area_2d_body_exited(Node2D body)
        {

            if (body.IsInGroup("Player"))
            {
                var player = body as Player;
                PlayerIsInRange = false;
                player.UIInteract.Visible = false;
            }
        }

        public void SetItemData(InventoryItem data)
        {
            this.ItemType = data.ItemType;
            this.ItemName = data.ItemName;
            this.ItemTexture = data.ItemTexture;
            this.ItemEffect = data.ItemEffect;
        }




        // make a copy of the item for changeable values
        public InventoryItem Copy()
        {
            return new InventoryItem(Quantity, ItemType, ItemName, null, ItemEffect);
        }
    }
}
