using Godot;
using ProjectDuhamel.models.inventory;
using ProjectDuhamel.models.items;
using ProjectDuhamel.models.monsters;
using ProjectDuhamel.scripts;
using System;
using static Utilities;

public partial class Player : CharacterBody2D
{
    private const int DEFAULT_INV_SIZE = 10;
	public const float _speed = 200.0f;
    public const int _bare_damage = 4;

    public AnimatedSprite2D AnimatedSprite { get; set; } = null;
    public CanvasLayer UIInteract { get; set; } = null;
    public CanvasLayer UIInventory { get; set; } = null;

    /// <summary>
    /// The inventory of objects for this player
    /// </summary>
    public Inventory PlayerInventory { get; set; } = new Inventory(DEFAULT_INV_SIZE);

    private Vector2 TargetDestinationPosition = new Vector2();

    private bool HasCollided = false;
    private bool IsAttacking = false;
    private bool IsMoving = true; // default state is true

    // Player specific
    public int HitPoints { get; set; } = 250;
    public int Experience { get; set; } = 0;
    public int[] KillHistory { get; set; } = new int[6] { 0, 0, 0, 0, 0, 0 };

    /// <summary>
    /// Directional information for the player
    /// </summary>
    public Utilities.Directions Direction { get; set; } = Utilities.Directions.DIR_NONE;
    public Vector2 DirectionUnitVector { get; set; } = new Vector2();
    public Vector2 DirectionVector { get; set; }
    public Vector2 FacingUnitVector { get; set; } = new Vector2();   // the direction our character is facing .... equal to the last moving direction.


    public override void _Input(InputEvent input_event)
    {
        if (input_event.IsActionPressed("ui_inventory"))
        {
            GD.Print("opening inventory");
            PlayerInventory.Display(this);

        }
    }
    private void GetInput()
	{
        // based WASD movement
        DirectionUnitVector = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
        FacingUnitVector = DirectionUnitVector;
        Velocity = DirectionUnitVector * _speed;

        if (Input.IsActionJustPressed("left_click"))
        {
            // assign our target destination click
            TargetDestinationPosition = GetGlobalMousePosition();

            // determine the direction and velocity from current player position to the target destination
            DirectionVector = GlobalPosition.DirectionTo(TargetDestinationPosition);
            DirectionUnitVector = DirectionVector.Normalized();
            FacingUnitVector = DirectionVector.Normalized();
            Direction = Utilities.GetDirection_9WAY(DirectionVector);

            // ser our velocity
            Velocity = new Vector2(0, 0);

            GD.Print("left click DV: " + DirectionVector);
        }
        if (Input.IsActionJustPressed("right_click"))
        {
            IsMoving = true;

            // assign our target destination click
            TargetDestinationPosition = GetGlobalMousePosition();

            // determine the direction and velocity from current player position to the target destination
            DirectionVector = GlobalPosition.DirectionTo(TargetDestinationPosition);
            DirectionUnitVector = DirectionVector.Normalized();
            FacingUnitVector = DirectionVector.Normalized();
            Direction = Utilities.GetDirection_9WAY(DirectionVector);

            // ser our velocity
            Velocity = DirectionVector * _speed;
        }
    }

    public override void _Ready()
    {
        // set the global player reference noe as this player
        GlobalScripts.SetPlayerReference(this);

        // setup getters and setters for nodes on our Player scene
        AnimatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        UIInteract = GetNode<CanvasLayer>("InteractUI");
        UIInventory = GetNode<CanvasLayer>("InventoryUI");

        // create our inventory
        PlayerInventory = new Inventory(DEFAULT_INV_SIZE);

        GlobalPosition = new Vector2(150, 100);

        // set our initial player vectors
        DirectionVector = new Vector2(0, 0);
        DirectionUnitVector = DirectionVector.Normalized();
        TargetDestinationPosition = GlobalPosition;
        FacingUnitVector =DirectionVector.Normalized();
        Direction = Utilities.Directions.DIR_NONE;

        // Set the collision layers and masks for this player in Godot
        SetCollisionLayerValue(1, false);  // turn off the default
        SetCollisionLayerValue((int)CollisionLayerAssignments.PLAYER, true);  // assign to proper layer

        SetCollisionMaskValue(1, false);  // turn off the default
        SetCollisionMaskValue((int)CollisionLayerAssignments.WALLS, true);  // assign to proper layer
        SetCollisionMaskValue((int)CollisionLayerAssignments.SPELLS_HOSTILE, true);  // assign to proper layer
        SetCollisionMaskValue((int)CollisionLayerAssignments.ENVIRONMENT, true);  // assign to proper layer
        SetCollisionMaskValue((int)CollisionLayerAssignments.ITEMS, true);  // assign to proper layer
        SetCollisionMaskValue((int)CollisionLayerAssignments.MONSTERS, true);  // assign to proper layer

        // Initialize the health bars
        var health_bar = GetNode<ProgressBar>("HealthBar/ProgressBar");
        health_bar.MaxValue = HitPoints;  // sets the upper limit of the progress bar
        health_bar.Value = HitPoints; // sets the current limit of the progress bar
        health_bar.MinValue = 0; // set the lower bound of the progress bar
        UpdateHealthBar();
    }

    public override void _PhysicsProcess(double delta)
	{
        // check for collisions with enemies or objects nearby, or if we are attacking something
        // or if we should move
        GetInput();

        MoveAndCollide(Velocity * (float)delta);
        DirectionUnitVector = (1.0f / _speed) * Velocity;
        DirectionVector = DirectionUnitVector;

        // compute the current distance of the player from the target destination point
        var distance = GlobalPosition.DistanceSquaredTo(TargetDestinationPosition);

        //// update the direction of our char.
        Direction = Utilities.GetDirection_9WAY(DirectionUnitVector);

        KinematicCollision2D collision_info = null;

        //if (Math.Sqrt(distance) > 3)
        //{
        //    // continue moving the character  for this frame
        //    collision_info = MoveAndCollide(Velocity * (float)delta);
        //    IsMoving = true;
        //}
        //else
        //{
        //    //GD.Print("Player is at target destination at " + GlobalPosition);
        //    IsMoving = false;
        //    Velocity = new Vector2(0, 0);
        //    DirectionVector = new Vector2(0, 0);
        //    DirectionUnitVector = DirectionVector.Normalized();
        //    TargetDestinationPosition = GlobalPosition;
        //}

        if (collision_info != null)
        {
            IsMoving = false;
            // GD.Print("Player collided at " + GlobalPosition);
            Velocity = new Vector2(0, 0);
            DirectionVector = new Vector2(0, 0);
            DirectionUnitVector = DirectionVector.Normalized();
            TargetDestinationPosition = GlobalPosition;

            var collider_obj = collision_info.GetCollider();

            if (collision_info.GetCollider() is MonsterObject)
            {
                GD.Print("Player object hit a monster: " + ((Node)collision_info.GetCollider()).Name);
                //GD.Print("Player object hit a monster: " + ((Node)collision_info.GetCollider()));

                // Damage the monster
                var monster_obj = (MonsterObject)collider_obj;
                var monster_data = monster_obj.monsterData.Copy();  // copy the data in case the monster dies in the next line

                // this line can kill the mosnter and remove it from gam
                monster_obj.TakeDamage(_bare_damage);


                // check if we killed the monster
                if (monster_obj == null || monster_obj.monsterData.MonsterHitPoints <= 0)
                {
                    GD.Print("player killed the monster");
                    GD.Print("exp earned: " + monster_data.MonsterExp);

                    this.Experience += monster_data.MonsterExp;
                    int index = (int)monster_data.MonsterRank;
                    GD.Print("index: " + index + "    Rank killed: " + monster_data.MonsterRank);

                    GD.Print("Experience: " + this.Experience);
                    GD.Print("Kill history[index]: " + this.KillHistory[index]);
                    GD.Print("FullKill history: " + this.KillHistory[0] + "," + this.KillHistory[1] + "," +
                        this.KillHistory[2] + "," + this.KillHistory[3] + "," + this.KillHistory[4]);

                    UpdateExperienceAndHistory_FromMonsterKill(monster_data);

                }

            }
            else if (collision_info.GetCollider() is RoomObject)
            {
                GD.Print("Player object hit a room object");
            }
            else if (collision_info.GetCollider() is Player)
            {
                GD.Print("Player object hit a player");
            }
            else
            {
                GD.Print("Player object hit something else");
            }
        }

        // load the direction arrow graphic
        var direction_arrow_sprites = GetNode<Sprite2D>("DirectionArrowSprite");
        direction_arrow_sprites.RegionRect = new Rect2((int)Direction * 16, 0, 16, 16);

        // update the health bar
        UpdateHealthBar();

        // animate our sprite
        UpdateAnimations(true);

        // update UI
        var ui = GetNode<Ui>("../HUD/UI");
        ui.Update(this);
    }

    private void UpdateAnimations(bool is_moving = true)
    {
        string animation_string = "";

        if(is_moving is true)
        //if (is_moving == true)
        {

            switch (Direction)
            {
                case Directions.DIR_NONE:
                    {
                        animation_string = "idle_front";
                        break;
                    }
                case Directions.DIR_NORTH:
                    {
                        animation_string = "walk_back";
                        break;
                    }
                case Directions.DIR_SOUTH:
                    {
                        animation_string = "walk_front";
                        break;
                    }
                case Directions.DIR_WEST:
                    {
                        animation_string = "walk_left";
                        break;
                    }
                case Directions.DIR_EAST:
                    {
                        animation_string = "walk_right";
                        break;
                    }
                default:
                    {
                        animation_string = "idle_front";
                        break;
                    }
            }
        }
        else
        {
            switch (Direction)
            {
                case Directions.DIR_NONE:
                    {
                        animation_string = "idle_front";
                        break;
                    }
                case Directions.DIR_NORTH:
                    {
                        animation_string = "idle_back";
                        break;
                    }
                case Directions.DIR_SOUTH:
                    {
                        animation_string = "idle_front";
                        break;
                    }
                case Directions.DIR_WEST:
                    {
                        animation_string = "idle_left";
                        break;
                    }
                case Directions.DIR_EAST:
                    {
                        animation_string = "idle_right";
                        break;
                    }
                default:
                    {
                        animation_string = "idle_front";
                        break;
                    }
            }
        }

        var animated_sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        animated_sprite.Play(animation_string);
    }

    public void TakeDamage(int v, MonsterData monsterData)
    {
        GD.Print("Monster took damage of " + v + " points");
        this.HitPoints -= v;
        if (HitPoints <= 0)
        {
            Die(monsterData);
        }

        UpdateHealthBar();
    }

    /// <summary>
    /// Function called when a monster kills the player
    /// </summary>
    /// <param name="monsterData"></param>
    public void Die(MonsterData monsterData)
    {
        GD.Print("Player has died");

        GD.Print("GAME OVER");
        GD.Print("Experience: " + Experience.ToString());
        GD.Print("Kill history: ");
        GD.Print("----------------------");

        for (int i = 0; i < KillHistory.Length; i++)
        {
            GD.Print(monsterData.MonsterRankNames[i] + ": " + KillHistory[i].ToString());
        }

        GetTree().Paused = true;
    }

    public void UpdateHealthBar()
    {
        var health_bar = GetNode<ProgressBar>("HealthBar/ProgressBar");

        // hide the health bars if full health
        if (health_bar.MaxValue == HitPoints)
        {
            health_bar.Visible = false;
        }
        else
        {
            health_bar.Visible = true;
        }

        // set the current value
        health_bar.Value = HitPoints;
    }

    public void UpdateExperienceAndHistory_FromMonsterKill(MonsterData monster_data)
    {
        GD.Print("player killed the monster");
        GD.Print("exp earned: " + monster_data.MonsterExp);

        this.Experience += monster_data.MonsterExp;
        int index = (int)monster_data.MonsterRank;
        GD.Print("index: " + index + "    Rank killed: " + monster_data.MonsterRank);

        this.KillHistory[(int)monster_data.MonsterRank] = this.KillHistory[(int)monster_data.MonsterRank] + 1;
        GD.Print("Experience: " + this.Experience);
        GD.Print("Kill history[index]: " + this.KillHistory[index]);
        GD.Print("FullKill history: " + this.KillHistory[0] + "," + this.KillHistory[1] + "," +
            this.KillHistory[2] + "," + this.KillHistory[3] + "," + this.KillHistory[4]);
    }
}
