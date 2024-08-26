using Godot;

namespace ProjectDuhamel.scripts
{
    public partial class RoomObjects : CharacterBody2D
    {
        private float speed = 500;
        private Vector2 unit_vector;
        private float rotation = 0;

        public Vector2 TilePos { get; set; } 
        public TileMapLayer Layer { get; set; } = new TileMapLayer();
        public int TileSetSourceId { get; set; } = 0;
        public Vector2I[] AtlasCoordArray { get; set; } = null;

        // Vector representing how far to move the item in a given turn 
        public Vector2 MoveOffsetUnitVector { get; set; } = new Vector2();

        // the static body object (with collision and image) tied into it
        public CharacterBody2D CharacterBodyObj { get; set; }

        // the location of the tileset image
        public string AssetPath { get; set; } = string.Empty;

        public Vector2 MoveDirectionUnitVector { get; set; } = new Vector2(0, 0);

        // parameterless constructor
        public RoomObjects()
        {
        }

        public RoomObjects(Vector2 tile_pos, 
            TileMapLayer map_layer, 
            int tileset_source_id,
            Vector2I[] atlas_coord_array,
            Vector2 move_offset,
            CharacterBody2D obj) {

            var move_offset_unit_vector = move_offset.Normalized();

            TilePos = tile_pos;
            Layer = map_layer;
            TileSetSourceId = tileset_source_id;
            AtlasCoordArray = atlas_coord_array;
            MoveOffsetUnitVector = move_offset_unit_vector;
            MoveDirectionUnitVector = move_offset_unit_vector;
            CharacterBodyObj = obj;
        }

        /// <summary>
        /// Used to initialize data for the object after it has been instantiated since you can't call a
        /// constructor with parameters.  Call this immediately after instantiation of the object.
        /// </summary>
        /// <param name="tile_pos"></param>
        /// <param name="layer"></param>
        /// <param name="tile_set_source_id"></param>
        /// <param name="atlas_coord_array"></param>
        /// <param name="move_vector"></param>
        /// <param name="obj"></param>
        /// <param name="resource_path"></param>
        public void AddData(Vector2 tile_pos, TileMapLayer layer, int tile_set_source_id, 
            Vector2I[] atlas_coord_array, Vector2 move_vector, CharacterBody2D obj, string resource_path)
        {
            var move_offset_unit_vector = move_vector.Normalized();

            TilePos = tile_pos;
            Layer = layer;
            TileSetSourceId = tile_set_source_id;
            AtlasCoordArray = atlas_coord_array;
            MoveOffsetUnitVector = move_offset_unit_vector;
            MoveDirectionUnitVector = move_offset_unit_vector;
            CharacterBodyObj = obj;
            AssetPath = resource_path;
        }


        public Vector2I GetAtlasCoord_Random()
        {
            var rng = new RandomNumberGenerator();
            var rand_number = rng.RandiRange(0, AtlasCoordArray.Length - 1);
            return AtlasCoordArray[rand_number];
        }

        public Vector2I GetAtlasCoord_AtIndex(int index)
        {
            if (index >= 0 && index < AtlasCoordArray.Length)
            {
                return AtlasCoordArray[index];
            }
            else
            {
                GD.Print("Error: index(" + index + ") out of bounds in GetAtlasCoord_AtIndex()");
                return new Vector2I(0, 0);
            }
        }

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

            // set the start position
            GlobalPosition = TilePos;
        }

        //// called every frame/ 'delta' is the elapsed time since the previous frame
        //public override void _Process(double delta)
        //{
        //    //// for random movement
        //    //var rng = new RandomNumberGenerator();
        //    //float rand_X = rng.RandfRange(-1.0f, 1.0f);
        //    //float rand_Y = rng.RandfRange(-1.0f, 1.0f);
        //    //GlobalPosition += new Vector2((float)(rand_X * speed * delta), (float)(rand_Y * speed * delta));

        //    GlobalPosition += new Vector2((float)(MoveDirection.X * speed * delta), (float)(MoveDirection.Y * speed * delta));
        //}

        // called every frame/ 'delta' is the elapsed time since the previous frame
        public override void _PhysicsProcess(double delta)
        {
            GlobalPosition += new Vector2((float)(MoveDirectionUnitVector.X * speed * delta), (float)(MoveDirectionUnitVector.Y * speed * delta));
        }
    }
}
