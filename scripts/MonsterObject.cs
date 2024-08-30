using Godot;
using ProjectDuhamel.models.monsters;
using ProjectDuhamel.models.spells;
using System;

namespace ProjectDuhamel.scripts
{
    public partial class MonsterObject : CharacterBody2D
    {
        // Monster specific
        public int HitPoints { get; set; } = 10;

        // monster data for this monster
        public MonsterData monsterData { get; set; }
        public BaseMonsterObjectGraphics monsterGraphics { get; set; } = new BaseMonsterObjectGraphics();


        // General properties for this monster

        public float MonsterObjectRotationAngle { get; set; } = 0.0f;
        public Texture2D MonsterObjectTexture { get; set; } = new Texture2D();

        public Vector2 CurrentPosition { get; set; } = new Vector2();
        public Vector2 LastPosition { get; set; } = new Vector2();

        public TileMapLayer Layer { get; set; } = new TileMapLayer();
        public int TileSetSourceId { get; set; } = 0;
        public Vector2I[] AtlasCoordArray { get; set; } = null;

        public int[] CollisionLayer { get; set; }
        public int[] CollisionMaskValues { get; set; }

        public Utilities.Directions Direction { get; set; } = Utilities.Directions.DIR_NONE;
        public Vector2 DirectionUnitVector { get; set; } = new Vector2();
        public Vector2 DirectionVector { get; set; }

        // the location of the tileset image
        public string AssetPath { get; set; } = string.Empty;


        /// <summary>
        /// Used to initialize data for the object after it has been instantiated since you can't call a
        /// constructor with parameters.  Call this immediately after instantiation of the object.
        /// </summary>
        /// <param name="position">global position of the object</param>
        /// <param name="layer">tilemap layer of the objects graphics</param>
        /// <param name="tile_set_source_id">source id for the tileset (usually 0)</param>
        /// <param name="atlas_coord_array">array of atlast coordinate values for the graphic within the tileset</param>
        /// <param name="dir_vector">direction that the object is moving -- station is new Vector2(0,0)</param>
        /// <param name="obj">Character2D object for this object</param>
        /// <param name="resource_path">location of the graphic image for this item</param>
        public void Initialize(string name, Vector2 position, MonsterData monster_data, BaseMonsterObjectGraphics graphics, 
            Vector2 dir_vector, int[] collision_layer_values, int[] collision_mask_values)
        {
            this.Name = name;
            this.monsterData = monster_data;
            this.monsterGraphics = graphics;
            this.Position = position;
            //CurrentPosition = position;
            //LastPosition = position;

            // Set the sprite texture
            Sprite2D sprite = GetNode<Sprite2D>("MonsterObjectSprite");
            sprite.Texture = monsterGraphics.GetTextureRandom();
            sprite.Centered = true;


            // Set the collision body shape
            CollisionShape2D collision_shape = GetNode<CollisionShape2D>("CollisionShape2D");
            collision_shape.Shape = monsterData.MonsterShape;

            // set the collision layers in GODOT
            CollisionLayer = collision_layer_values;
            CollisionMaskValues = collision_mask_values;

            // set the vectors of the object
            DirectionVector = dir_vector;
            DirectionUnitVector = dir_vector.Normalized();
            Direction = Utilities.GetDirection_9WAY(DirectionUnitVector);

            // determine the angle made by the unit vector and apply it to the rotation of this Node object in Godot.
            this.Rotation = MonsterObjectRotationAngle;

            // set the velocity of the object
            Velocity = monsterData.MonsterSpeed * DirectionUnitVector;

            // Set the collision layers in GODOT
            SetCollisionLayerValue(1, false); // turn off the default layer
            foreach (int value in collision_layer_values)
            {
                SetCollisionLayerValue(value, true); // and set our own
            }

            // Set the collision mask in GODOT
            SetCollisionMaskValue(1, false);  // turn of the defaults mask
            foreach (int value in collision_mask_values)
            {
                SetCollisionMaskValue(value, true); // and set our own
            }

            // Initialize the health bars
            var health_bar = GetNode<ProgressBar>("HealthBar/ProgressBar");
            health_bar.MaxValue = HitPoints;  // sets the upper limit of the progress bar
            health_bar.Value = HitPoints; // sets the current limit of the progress bar
            health_bar.MinValue = 0; // set the lower bound of the progress bar
            UpdateHealthBar();
        }

        public override void _Ready()
        {
           
        }

        public override void _PhysicsProcess(double delta)
        {
            this.Rotation = MonsterObjectRotationAngle;

            // Update the velocity just in case....
            Velocity = monsterData.MonsterSpeed * DirectionUnitVector;

            var collision = MoveAndCollide(Velocity * (float)delta);

            if (collision != null)
            {
                //GD.Print("MonsterObject collided with " + collision.GetCollider() + " at " + GlobalPosition);

                var collider_obj = collision.GetCollider();

                if (collision.GetCollider() is MonsterObject)
                {
                    GD.Print("Monster hit a monster");
                }
                else if (collision.GetCollider() is RoomObject)
                {
                    GD.Print("Monster hit a room object");
                }
                else if (collision.GetCollider() is Player)
                {
                    var player_obj = (Player)collider_obj;
                    int dam = Utilities.GetRandomNumber(monsterData.MonsterMinDamage, monsterData.MonsterMaxDamage);
                    player_obj.TakeDamage(dam);

                    GD.Print("Monster hit a player for " + dam + " damage");
                }
                else
                {
                    GD.Print("Monster hit something else");
                }

                this.QueueFree();
            }

            UpdateHealthBar();
        }

        public void TakeDamage(int v)
        {
            GD.Print("Monster took damage of " + v + " points");
            this.HitPoints -= v;
            if (HitPoints <= 0)
            {
                GD.Print("Monster died");
                this.QueueFree();

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
}


