using Godot;
using System;
using static Utilities;

public partial class Player : CharacterBody2D
{


	public const float _speed = 200.0f;

    private Vector2 TargetDestinationPosition = new Vector2();

    [Export]
    private bool HasCollided = false;
    [Export]
    private bool IsAttacking = false;
    [Export]
    private bool IsMoving = true; // default state is true

    public Utilities.Directions Direction { get; set; } = Utilities.Directions.DIR_NONE;
    public Vector2 DirectionUnitVector { get; set; } = new Vector2();
    public Vector2 DirectionVector { get; set; }

    // the direction our character is facing .... equal to the last moving direction.
    public Vector2 FacingUnitVector { get; set; } = new Vector2();

	public void GetInput()
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

        if (Input.IsActionJustPressed("right_click"))
        {
            IsMoving = false;
            GD.Print("right click");
            //IsAttacking = true;
        }

        if (Input.IsActionJustPressed("middle_click"))
        {
            IsMoving = false;
            //IsAttacking = false;
            GD.Print("middle click");
        }
    }

    public override void _Ready()
    {
        GlobalPosition = new Vector2(50, 20);

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

    }

    public override void _PhysicsProcess(double delta)
	{
        // check for collisions with enemies or objects nearby, or if we are attacking something
        // or if we should move
        GetInput();
        // update the direction of our char.
        Direction = Utilities.GetDirection_9WAY(DirectionVector);

        // compute the current distance of the player from the target destination point
        var distance = GlobalPosition.DistanceSquaredTo(TargetDestinationPosition);

        // var collision = MoveAndCollide(Velocity * (float)delta);

        KinematicCollision2D collision_info = null;

        if (Math.Sqrt(distance) > 3)
        {
            // continue moving the character  for this frame
            collision_info = MoveAndCollide(Velocity * (float)delta);
            IsMoving = true;
        } else
        {
            GD.Print("Player is at target destination at " + GlobalPosition);
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

        }

        //// MOUSE CLICK MOVEMENT
        ////
        //// otherwise move the character
        //if (Math.Sqrt(distance) > 3 && (HasCollided == false) && (IsAttacking == false))
        //{
        //    // move the character  for this frame
        //    collision_info = MoveAndCollide(Velocity * (float)delta);
        //} else
        //{
        //    // we've hit something or reached our target destination, so stop the moving.
        //    TargetDestinationPosition = GlobalPosition;
        //    collision_info = MoveAndCollide(new Vector2(0, 0));

        //    // Zero out all the movement variables
        //    TargetDestinationPosition = GlobalPosition;
        //    Velocity = new Vector2(0, 0);
        //    DirectionUnitVector = new Vector2(0, 0);
        //    DirectionVector = new Vector2(0, 0);
        //    Direction = Directions.DIR_NONE;
        //}

        //// COLLISION CHECKS
        ////
        //// check if we collided with something...if so...signal that we've collided so the next iteration zeroes the values.
        //if(collision_info != null)
        //{
        //    GD.Print("collided");
        //    HasCollided = true;

        //    // Zero out all the movement variables except the facing unit vector
        //    TargetDestinationPosition = GlobalPosition;
        //    Velocity = new Vector2(0, 0);
        //    DirectionUnitVector = new Vector2(0, 0);
        //    DirectionVector = new Vector2(0, 0);
        //    Direction = Directions.DIR_NONE;

        //} else
        //{
        //    //GD.Print("no collision");

        //    HasCollided = false;
        //}

        Direction = Utilities.GetDirection_9WAY(DirectionVector);

        // load the arrow graphic
        var direction_arrow_sprites = GetNode<Sprite2D>("DirectionArrowSprite");
        direction_arrow_sprites.RegionRect = new Rect2((int)Direction * 16, 0, 16, 16);
    }


}
