using Godot;
using System;

namespace ProjectDuhamel.scripts
{
    public partial class RoomObjects : CharacterBody2D
    {
        // The speed of the object
        public float RoomObjectSpeed { get; set; }
        public float RoomObjectRotationAngle { get; set; } = 0.0f;
        public Texture2D RoomObjectTexture { get; set; } = new Texture2D();

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

        // the static body object (with collision and image) tied into it
        public CharacterBody2D CharacterBodyObj { get; set; }
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
        public void Initialize(string name,Vector2 position, float speed, TileMapLayer layer, int tile_set_source_id, 
            Vector2I[] atlas_coord_array, Vector2 dir_vector, RectangleShape2D shape, 
            string resource_path, int[] collision_layer_values, int[] collision_mask_values)
        {
            this.Name = name;
            this.RoomObjectSpeed = speed;
            this.Position = position;
            //CurrentPosition = position;
            //LastPosition = position;

            // TODO:  This needs to be removed and decoupled from this code....as it's spell and graphic specific.  Need ideas for how to do this conveniently.
            // graphics information
            Layer = layer;
            TileSetSourceId = tile_set_source_id;
            AtlasCoordArray = atlas_coord_array;
            AssetPath = resource_path;

            // Set the collision body shape
            BodyShape = shape;
            CollisionShape2D collision_shape = GetNode<CollisionShape2D>("CollisionShape2D");
            collision_shape.Shape = shape;

            // set the collision layers in GODOT
            CollisionLayer = collision_layer_values;
            CollisionMaskValues = collision_mask_values;

            // set the vectors of the object
            DirectionVector = dir_vector;
            DirectionUnitVector = dir_vector.Normalized();
            Direction = Utilities.GetDirection_9WAY(DirectionUnitVector);

            // determine the angle made by the unit vector and apply it to the rotation of this Node object in Godot.
            RoomObjectRotationAngle = (float)Math.Atan2(dir_vector.Y, dir_vector.X);
            this.Rotation = RoomObjectRotationAngle;

            // set the velocity of the object
            Velocity = RoomObjectSpeed * DirectionUnitVector;

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

            //Position = GlobalPosition;

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
            RoomObjectTexture = atlas_texture;
            // 5. set our sprite's texture to the new sub region image
            sprite.Texture = RoomObjectTexture;
        }

        // called every frame/ 'delta' is the elapsed time since the previous frame
        public override void _PhysicsProcess(double delta)
        {
            this.Rotation = RoomObjectRotationAngle;

            // Update the velocity just in case....
            Velocity = RoomObjectSpeed * DirectionUnitVector;

            var collision = MoveAndCollide(Velocity * (float)delta);

            if (collision != null)
            {
                GD.Print("RoomObject collided at " + GlobalPosition);
                this.QueueFree();
            }
        }
    }
}
