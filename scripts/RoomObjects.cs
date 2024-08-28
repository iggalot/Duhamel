using Godot;

namespace ProjectDuhamel.scripts
{
    public partial class RoomObjects : CharacterBody2D
    {
        // The speed of the object
        public float Speed { get; set; } = 50;

        private Vector2 unit_vector;
        private float rotation = 0;

        public Vector2 CurrentLocation { get; set; }
        public Vector2 PreviousLocation { get; set; }
        public TileMapLayer Layer { get; set; } = new TileMapLayer();
        public int TileSetSourceId { get; set; } = 0;
        public Vector2I[] AtlasCoordArray { get; set; } = null;

        public int[] CollisionLayer { get; set; }
        public int[] CollisionMaskValues { get; set; }

        public Utilities.Directions Direction { get; set; } = Utilities.Directions.DIR_NONE;
        public Vector2 DirectionUnitVector { get; set; } = new Vector2();
        public Vector2 DirectionVector { get; set; }

        // the static body object (with collision and image) tied into it
        //public CharacterBody2D CharacterBodyObj { get; set; }

        public RectangleShape2D BodyShape { get; set; } = new RectangleShape2D();

        // the location of the tileset image
        public string AssetPath { get; set; } = string.Empty;

        /// <summary>
        /// Parameterless constructor -- needed for Instatiation.
        /// </summary>
        public RoomObjects()
        {
        }

        //public RoomObjects(Vector2 tile_pos, 
        //    TileMapLayer map_layer, 
        //    int tileset_source_id,
        //    Vector2I[] atlas_coord_array,
        //    CharacterBody2D obj) {


        //    TilePos = tile_pos;
        //    Layer = map_layer;
        //    TileSetSourceId = tileset_source_id;
        //    AtlasCoordArray = atlas_coord_array;
        //    CharacterBodyObj = obj;
        //}

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
        public void Initialize(Vector2 position, TileMapLayer layer, int tile_set_source_id,
            Vector2I[] atlas_coord_array, Vector2 dir_vector, Vector2 initial_vel_vec,
            RectangleShape2D shape,
            string resource_path, int[] collision_layer_values, int[] collision_mask_values)
        {

            CurrentLocation = position;
            PreviousLocation = position;
            Layer = layer;
            TileSetSourceId = tile_set_source_id;
            AtlasCoordArray = atlas_coord_array;
            DirectionVector = dir_vector;
            BodyShape = shape;
            AssetPath = resource_path;

            DirectionUnitVector = dir_vector.Normalized();
            Direction = Utilities.GetDirection_9WAY(DirectionUnitVector);

            CollisionLayer = collision_layer_values;
            CollisionMaskValues = collision_mask_values;

            //GD.Print("unit vec: }" + DirectionUnitVector );

            // set the initial location of the object in global coordinates
            GlobalPosition = position;
            GD.Print("loc: " + GlobalPosition);

            Velocity = initial_vel_vec;  // set an initial velocity
            GD.Print("vel: " + Velocity);

            // set the velocity of the object
            //Velocity = Speed * DirectionUnitVector;
            //GD.Print("velocity: " + Velocity);

            // Set the collision layers in GODOT
            SetCollisionLayerValue(1, false); // turn off the default layer
            foreach(int value in collision_layer_values)
            {
                SetCollisionLayerValue(value, true); // and set our own
            }

            // Set the collision mask in GODOT
            SetCollisionMaskValue(1, false);  // turn of the defaults mask
            foreach (int value in collision_mask_values)
            {
                SetCollisionMaskValue(value, true); // and set our own
            }

            GD.Print("coll. layer: " + CollisionLayer + " bits:" + CollisionMaskValues);

            ////assign the character body object to the item that was just created.
            var collision_body = GetNode<CollisionShape2D>("CollisionShape2D");
            collision_body.Shape = BodyShape;
        }


        //public Vector2I GetAtlasCoord_Random()
        //{
        //    var rng = new RandomNumberGenerator();
        //    var rand_number = rng.RandiRange(0, AtlasCoordArray.Length - 1);
        //    return AtlasCoordArray[rand_number];
        //}

        //public Vector2I GetAtlasCoord_AtIndex(int index)
        //{
        //    if (index >= 0 && index < AtlasCoordArray.Length)
        //    {
        //        return AtlasCoordArray[index];
        //    }
        //    else
        //    {
        //        GD.Print("Error: index(" + index + ") out of bounds in GetAtlasCoord_AtIndex()");
        //        return new Vector2I(0, 0);
        //    }
        //}

        public override void _Ready()
        {
            string path = AssetPath;
            Sprite2D sprite = GetNode<Sprite2D>("RoomObjectSprite");

            // TODO:  This should be abstracted out as a utility function for loading the subregion of an image from an atlas
            // 1. load the image from file and create a texture from it
            // --- can we use a current resource instead?

            //// for loading an image from a non-resource image file
            //var image = Image.LoadFromFile(path);
            //image.GetRegion(new Rect2I(16, 16, 16, 16));
            //var texture = ImageTexture.CreateFromImage(image);

            // an alternate way to load an image
            // TODO:  need to make resources out of images
            Image loaded_image = new Image();
            var error = loaded_image.Load(path);
            if (error != Error.Ok)
            {
                GD.Print("Error loading image: " + error);
            }
            loaded_image.GetRegion(new Rect2I(16, 16, 16, 16));

            var texture = new ImageTexture();
            texture.SetImage(loaded_image);

            // 2. create a new atlas texture
            var atlas_texture = new AtlasTexture();

            // 3. set the new atlas_texture's image atlas to the texture we loaded
            atlas_texture.Atlas = texture;

            // 4. get the sub region within the atlast texture based on the coordinate in the atlast image --
            //    if there is more than one sub region, this will be a random one
            var rng = new RandomNumberGenerator();
            var rand_number = rng.RandiRange(0, AtlasCoordArray.Length - 1);
            atlas_texture.Region = new Rect2(AtlasCoordArray[rand_number].X * 16, AtlasCoordArray[rand_number].Y * 16, 16, 16);

            // 5. set our sprite's texture to the new sub region image
            sprite.Texture = atlas_texture;
        }

        // called every frame/ 'delta' is the elapsed time since the previous frame
        public override void _PhysicsProcess(double delta)
        {
            GlobalPosition = PreviousLocation;

            var collision = MoveAndCollide(Velocity * (float)delta);

            if (collision != null)
            {
                //GD.Print("RoomObject collided at " + GlobalPosition + " with object " +  collision);

                // move back to our previous location and stop its movement
                CurrentLocation = PreviousLocation;
                Velocity = new Vector2(0, 0);
                //// if the room object is moving, then destroy it
                //if(Velocity > Vector2.Zero)
                //{
                //    QueueFree();
                //}
            }

            // store our location
            PreviousLocation = CurrentLocation;
        }
    }
}
