using Godot;
using ProjectDuhamel.scripts;
using System;
using static Utilities;

public partial class Player : CharacterBody2D
{
	public const float _speed = 200.0f;
    public const int _bare_damage = 4;

    private Vector2 TargetDestinationPosition = new Vector2();

    private bool HasCollided = false;
    private bool IsAttacking = false;
    private bool IsMoving = true; // default state is true

    // Player specific
    public int HitPoints { get; set; } = 50;

    /// <summary>
    /// Directional information for the player
    /// </summary>
    public Utilities.Directions Direction { get; set; } = Utilities.Directions.DIR_NONE;
    public Vector2 DirectionUnitVector { get; set; } = new Vector2();
    public Vector2 DirectionVector { get; set; }
    public Vector2 FacingUnitVector { get; set; } = new Vector2();   // the direction our character is facing .... equal to the last moving direction.


    private void GetInput()
	{
        if (Input.IsActionJustPressed("left_click"))
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

            GD.Print("left click DV: " + DirectionVector);
        }
    }

    public override void _Ready()
    {
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

        // compute the current distance of the player from the target destination point
        var distance = GlobalPosition.DistanceSquaredTo(TargetDestinationPosition);

        // update the direction of our char.
        Direction = Utilities.GetDirection_9WAY(DirectionVector);

        KinematicCollision2D collision_info = null;

        if (Math.Sqrt(distance) > 3)
        {
            // continue moving the character  for this frame
            collision_info = MoveAndCollide(Velocity * (float)delta);
            IsMoving = true;
        } else
        {
            //GD.Print("Player is at target destination at " + GlobalPosition);
            IsMoving = false;
            Velocity = new Vector2(0, 0);
            DirectionVector = new Vector2(0, 0);
            DirectionUnitVector = DirectionVector.Normalized();
            TargetDestinationPosition = GlobalPosition;
        }

        if(collision_info != null)
        {
            IsMoving = false;
            GD.Print("Player collided at " + GlobalPosition);
            Velocity = new Vector2(0, 0);
            DirectionVector = new Vector2(0,0);
            DirectionUnitVector = DirectionVector.Normalized();
            TargetDestinationPosition = GlobalPosition;

            var collider_obj = collision_info.GetCollider();

            if (collision_info.GetCollider() is MonsterObject)
            {
                GD.Print("Player object hit a monster: " + ((Node)collision_info.GetCollider()).Name);
                GD.Print("Player object hit a monster: " + ((Node)collision_info.GetCollider()));

                // Damage the monster
                var monster_obj = (MonsterObject)collider_obj;
                monster_obj.TakeDamage(_bare_damage);
            }
            else if (collision_info.GetCollider() is RoomObject)
            {
                GD.Print("Player object hit a room object");
            }
            else if (collision_info.GetCollider() is Player)
            {
                GD.Print("Player object hit a monster");
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
    }

    public void TakeDamage(int v)
    {
        GD.Print("Monster took damage of " + v + " points");
        this.HitPoints -= v;
        if (HitPoints <= 0)
        {
            GD.Print("Player died");
            GetTree().Paused = true;

        }

        UpdateHealthBar();
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
}
