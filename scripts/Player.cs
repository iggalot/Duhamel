using Godot;
using System;
using static Utilities;

public partial class Player : CharacterBody2D
{


	public const float _speed = 50.0f;

    private Vector2 TargetDestinationPosition = new Vector2();

    [Export]
    private bool HasCollided = false;
    [Export]
    public bool IsAttacking = false;
    [Export]
    public bool IsMoving = true; // default state is true
    // for handling funky collision checking on first start
    public bool IsFirstLoad = true;

    public Utilities.Directions Direction { get; set; } = Utilities.Directions.DIR_NONE;
    public Vector2 DirectionUnitVector { get; set; } = new Vector2();
    public Vector2 DirectionVector { get; set; }

    // the direction our character is facing .... equal to the last moving direction.
    public Vector2 FacingUnitVector { get; set; } = new Vector2();

	public void GetInput()
	{
        if (Input.IsActionJustPressed("left_click"))
        {
            //// assign our target destination click
            //TargetDestinationPosition = GetGlobalMousePosition();
            //if(TargetDestinationPosition == GlobalPosition)
            //{
            //    IsMoving = false;
            //} else
            //{
            //    IsMoving = true;
            //}

            ////Velocity = DirectionVector * _speed;
            //GD.Print("left click DV: " + TargetDestinationPosition);

        }

        if (Input.IsActionJustPressed("right_click"))
        {
            IsMoving = false;
            GD.Print("right click");
            IsAttacking = true;
        }

        if (Input.IsActionJustPressed("middle_click"))
        {
            IsMoving = false;
            IsAttacking = false;
            GD.Print("middle click");
        }

        return;
    }

    public override void _Ready()
    {
////        GlobalPosition = new Vector2(20, 20);  // set the player position inside the room slightly
//        DirectionVector = new Vector2(0, 0);
//        DirectionUnitVector = new Vector2(0, 0);
//        Direction = Utilities.Directions.DIR_NONE;
//        IsMoving = false;
//        IsAttacking = false;

//        // set out target to our current location
//        TargetDestinationPosition = GlobalPosition;

    //    SetCollisionLayerValue(1, false);  // turn off the default
    //    SetCollisionLayerValue((int)CollisionLayerAssignments.PLAYER, true);  // assign to proper layer

    //    SetCollisionMaskValue(1, false);  // turn off the default
    //    SetCollisionMaskValue((int)CollisionLayerAssignments.WALLS, true);  // assign to proper layer
    //    SetCollisionMaskValue((int)CollisionLayerAssignments.SPELLS_HOSTILE, true);  // assign to proper layer
    //    SetCollisionMaskValue((int)CollisionLayerAssignments.ENVIRONMENT, true);  // assign to proper layer
    //    SetCollisionMaskValue((int)CollisionLayerAssignments.ITEMS, true);  // assign to proper layer
    //    SetCollisionMaskValue((int)CollisionLayerAssignments.MONSTERS, true);  // assign to proper layer
    }

    public override void _PhysicsProcess(double delta)
	{
        // If its the first cycle through, character may have some funky updates
        // so shortcut out.
        if (IsFirstLoad is true)
        {
            IsFirstLoad = false;
            return;
        }

        // check for collisions with enemies or objects nearby, or if we are attacking something
        // or if we should move
        GetInput();

        //        UpdateDirectionVectors();

        Velocity = new Vector2(50, 50);

        MoveAndSlide();

        ////Direction = Utilities.GetDirection_9WAY(DirectionVector);
        ////Velocity = _speed * DirectionUnitVector;

        //GD.Print("Before MoveAndCollide");
        //GD.Print("target dest: " + TargetDestinationPosition);
        //GD.Print("directon vector: " + DirectionVector);
        //GD.Print("direction unit vector: " + DirectionUnitVector);
        //GD.Print("velocity: " + Velocity);

        ////// compute the current distance of the player from the target destination point
        ////var distance = TargetDestinationPosition.DistanceSquaredTo(GlobalPosition);

        //var increment = Velocity * (float)delta;
        //GD.Print("increment: " + increment);
        //var collision = MoveAndCollide((Velocity * (float)delta));






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



        //Direction = Utilities.GetDirection_9WAY(DirectionVector);

        //// load the arrow graphic
        //var direction_arrow_sprites = GetNode<Sprite2D>("DirectionArrowSprite");
        //direction_arrow_sprites.RegionRect = new Rect2((int)Direction * 16, 0, 16, 16);
    }

    //private void UpdateDirectionVectors()
    //{
    //    // don't update on the initial run
    //    if (IsFirstLoad is true)
    //        return;

    //    if(IsMoving is false)
    //    {
    //        TargetDestinationPosition = GlobalPosition;
    //    }
    //    DirectionVector = TargetDestinationPosition - GlobalPosition;
    //    DirectionUnitVector = DirectionVector.Normalized();
    //    // update the direction enum of our char.
    //    Direction = Utilities.GetDirection_9WAY(DirectionVector);

    //}
}
