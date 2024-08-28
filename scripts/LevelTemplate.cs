using Godot;
using ProjectDuhamel.models.spells;
using ProjectDuhamel.scripts;
using System;
using System.Collections.Generic;


/// <summary>
/// Default values -- for multiples, add the bit values together and then call
/// SetCollisionMaskValue to assign the bits
/// SetCollisionLayerValue to assign the specific layer as indicated below
/// LAYER ASSIGNMENTS      MASK BITS (used for SetCollisionMask function)
/// 1. FLOORS               1
/// 2. WALLS                2
/// 3. ENVIRONMENT          3
/// 4. SPELLS FRIENDLY      4
/// 5. SPELLS HOSTILE       5
/// 5. ITEMS                6
/// 6. MONSTERS             7
/// 7. PLAYER               8
/// 
/// 
/// </summary>
public enum CollisionLayerAssignments
{
    FLOORS = 1,
    WALLS = 2,
    ENVIRONMENT = 3,
    SPELLS_FRIENDLY = 4,
    SPELLS_HOSTILE = 5,
    ITEMS = 6,
    MONSTERS = 7,
    PLAYER = 8
}
public enum CollisionMaskAssignments
{
    FLOORS = 1,
    WALLS = 2,
    ENVIRONMENT = 3,
    SPELLS_FRIENDLY = 4,
    SPELLS_HOSTILE = 5,
    ITEMS = 6,
    MONSTERS = 7,
    PLAYER = 8
}

public partial class LevelTemplate : Node2D
{
    public List<RoomObjects> activeRoomObjects { get; set; } = new List<RoomObjects>();
    public Dictionary<SpellIdentifiers, BaseSpellObjectGraphics> spellObjectGraphicsDictionary { get; set; } = new Dictionary<SpellIdentifiers, BaseSpellObjectGraphics>();


    PackedScene RoomObjectScene = GD.Load<PackedScene>("res://scenes/room_object.tscn");

    public enum WallDirections
    {
        Top = 0,
        Right = 1,
        Bottom = 2,
        Left = 3
    }

    const int tileSize = 16;  // pixel tile size

    // Tilemap Layers in GoDOT project
    const string floor = "Floor";
    const string walls = "Walls";
    const string spell_effects = "SpellEffects";
    const string items = "Items";

    // Our tilemap layers for our template
    public TileMapLayer spell_effects_map_layer { get; set; }
    public TileMapLayer floor_effects_map_layer { get; set; }
    public TileMapLayer wall_effects_map_layer { get; set; }
    public TileMapLayer item_effects_map_layer { get; set; }


    // tileset source IDs for the layers -- should be zero if only one tileset on the tilemaplayer
    int floor_tileset_source_id = 0; 
    int walls_tileset_source_id = 0;
    int spell_effects_tileset_source_id = 0;
    int monster_images_tileset_source_id = 0;

    /// <summary>
    /// Room dimensions
    /// </summary>
    public int roomWidth { get; set; } = 150;
    public int roomHeight { get; set; } = 150;

    /// <summary>
    /// Room corners in pixel dimensions
    /// </summary>
    Vector2 roomUpperLeft;
    Vector2 roomUpperRight;
    Vector2 roomLowerRight;
    Vector2 roomLowerLeft;

    /// <summary>
    /// Number of tiles used to layout the room
    /// </summary>
    public int num_floor_tiles_hor { get; set; } = 10;
    public int num_floor_tiles_ver { get; set; } = 10;

    /// <summary>
    /// Number of wall tiles used to layout the room
    /// </summary>
    public int num_wall_tiles_hor { get; set; } = 12;
    public int num_wall_tiles_ver { get; set; } = 12;

    /// <summary>
    /// Room floor corners in tiled index dimensions -- measured to inside of wal...for walls must add +1 in appropriate direction
    /// </summary>
    Vector2 tiled_floor_UpperLeft;
    Vector2 tiled_floor_UpperRight;
    Vector2 tiled_floor_LowerRight;
    Vector2 tiled_floor_LowerLeft;

    /// <summary>
    /// Room floor corners in tiled index dimensions -- measured to inside of wal...for walls must add +1 in appropriate direction
    /// </summary>
    Vector2 tiled_wallcorner_UpperLeft;
    Vector2 tiled_wallcorner_UpperRight;
    Vector2 tiled_wallcorner_LowerRight;
    Vector2 tiled_wallcorner_LowerLeft;

    // indices (atlas coords) for the floor tiles in the tileset
    // this is graphic specific
    Vector2I[] floor_tiles =
    {
        new Vector2I(6, 0),
        new Vector2I(6, 1),
        new Vector2I(6, 2),
        new Vector2I(7, 0),
        new Vector2I(7, 1),
        new Vector2I(7, 2),
        new Vector2I(8, 0),
        new Vector2I(8, 1),
        new Vector2I(8, 2),
        new Vector2I(9, 0),
        new Vector2I(9, 1),
        new Vector2I(9, 2)
    };

    // indices (atlas coords) for the upper wall tiles in the tileset
    // this is graphic specific
    Vector2I[] wall_tiles_upper =
    {
        new Vector2I(1, 0),
        new Vector2I(2, 0),
        new Vector2I(3, 0),
        new Vector2I(4, 0),
    };

    // indices (atlas coords) for the lower wall tiles in the tileset
    // this is graphic specific
    Vector2I[] wall_tiles_lower =
    {
        new Vector2I(1, 4),
        new Vector2I(2, 4),
        new Vector2I(3, 4),
        new Vector2I(4, 4),
    };

    // indices (atlas coords) for the left wall tiles in the tileset
    // this is graphic specific
    Vector2I[] wall_tiles_left =
    {
        new Vector2I(0, 1),
        new Vector2I(0, 2),
        new Vector2I(0, 3),
    };

    // indices (atlas coords) for the right wall tiles tiles in the tileset
    // this is graphic specific
    Vector2I[] wall_tiles_right =
    {
        new Vector2I(5, 1),
        new Vector2I(5, 2),
        new Vector2I(5, 3),
    };

    // indices for the wall corner graphics in the tile set
    // this is graphic specific
    Vector2I[] wall_tiles_upper_left_corner = { new Vector2I(0, 0) };
    Vector2I[] wall_tiles_upper_right_corner = { new Vector2I(5, 0) };
    Vector2I[] wall_tiles_lower_left_corner = { new Vector2I(0, 4) };
    Vector2I[] wall_tiles_lower_right_corner = { new Vector2I(5, 4) };


    Vector2I[] spell_firebolt_effects_tiles =
    {
        new Vector2I(0, 0),
        new Vector2I(1, 0),
        new Vector2I(2, 0)
    };


    Vector2I[] spell_frostbolt_effects_tiles =
    {
        new Vector2I(0, 1)
    };

    Vector2I[] spell_lightningbolt_effects_tiles =
    {
        new Vector2I(0, 2)
    };

    Vector2I[] spell_poisonbolt_effects_tiles =
{
        new Vector2I(0, 3)
    };

    Vector2I[] spell_earthbolt_effects_tiles =
    {
        new Vector2I(0, 4)
    };

    Vector2I[] monster_images_tiles =
    {
        new Vector2I(0, 0),
        new Vector2I(0, 1),
        new Vector2I(0, 2),
        new Vector2I(0, 3),
       
        new Vector2I(1, 0),
        new Vector2I(1, 1),
        new Vector2I(1, 2),
        new Vector2I(1, 3),
       
        new Vector2I(2, 0),
        new Vector2I(2, 1),
        new Vector2I(2, 2),
        new Vector2I(2, 3),
        
        new Vector2I(3, 0),
        new Vector2I(3, 1),
        new Vector2I(3, 2),
        new Vector2I(3, 3),

        new Vector2I(4, 0),
        new Vector2I(4, 1),
        new Vector2I(4, 2),
        new Vector2I(4, 3),

        new Vector2I(5, 0),
        new Vector2I(5, 1),
        new Vector2I(5, 2),
        new Vector2I(5, 3),

        new Vector2I(6, 0),
        new Vector2I(6, 1),
        new Vector2I(6, 2),
        new Vector2I(6, 3),
    };

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // setup our layer constants
        spell_effects_map_layer = GetNode<TileMapLayer>(spell_effects);
        floor_effects_map_layer = GetNode<TileMapLayer>(floor);
        wall_effects_map_layer = GetNode<TileMapLayer>(walls);
        item_effects_map_layer = GetNode<TileMapLayer>(items);

        // set up our spell graphics dictionary
        CreateSpellGraphics();

        // create a shape by determining the number of tiles for the room and the walls in each direction
        num_floor_tiles_hor = (int)Math.Ceiling((double)(roomWidth / tileSize));
        num_floor_tiles_ver = (int)Math.Ceiling((double)(roomHeight / tileSize));
        num_wall_tiles_hor = num_floor_tiles_hor + 2;
        num_wall_tiles_ver = num_floor_tiles_ver + 2;

        //// Compute the coordinate of the corners of the room
        //roomUpperLeft = new Vector2(0, 0);
        //roomUpperRight = roomUpperLeft + new Vector2(roomWidth, 0);
        //roomLowerRight = roomUpperRight + new Vector2(0, roomHeight);
        //roomLowerLeft = roomLowerRight + new Vector2(-roomWidth, 0);

        // determine  tiled indices for floors...measurements are made on the upper left corner of the tile
        tiled_floor_UpperLeft = new Vector2(0, 0);
        tiled_floor_UpperRight = tiled_floor_UpperLeft + new Vector2(num_floor_tiles_hor, 0);
        tiled_floor_LowerRight = tiled_floor_UpperRight + new Vector2(0, num_floor_tiles_ver);
        tiled_floor_LowerLeft = tiled_floor_LowerRight + new Vector2(-num_floor_tiles_hor, 0);

        // determine  tiled indices for floors...measurements are made on the upper left corner of the tile
        tiled_wallcorner_UpperLeft = tiled_floor_UpperLeft + new Vector2(-1, -1);
        tiled_wallcorner_UpperLeft = tiled_floor_UpperLeft + new Vector2(-1, -1);
        tiled_wallcorner_UpperRight = tiled_wallcorner_UpperLeft + new Vector2(num_floor_tiles_hor + 1, 0);
        tiled_wallcorner_LowerRight = tiled_wallcorner_UpperRight + new Vector2(0, num_floor_tiles_ver + 1);
        tiled_wallcorner_LowerLeft = tiled_wallcorner_LowerRight + new Vector2(-(num_floor_tiles_hor + 1), 0);

        TileMapLayer floor_map_layer = GetNode<TileMapLayer>(floor);
        DrawFloor(floor_map_layer);

        TileMapLayer walls_map_layer = GetNode<TileMapLayer>(walls);
        DrawWalls(walls_map_layer);

 //       TileMapLayer spell_effects_map_layer = GetNode<TileMapLayer>(spell_effects);
        DrawSpellEffects(spell_effects_map_layer);
    }

    private void DrawSpellEffects(TileMapLayer tilemap_layer)
    {
        //// setup random number generator
        //var rng = new RandomNumberGenerator();

        //var player = GetNode<CharacterBody2D>("Player");
        //var rand_number = rng.RandiRange(0, spell_fireball_effects_tiles.Length - 1);
        //Vector2I atlas_coord = spell_fireball_effects_tiles[rand_number];
        //Vector2I tile_pos = new Vector2I(3, 0);
        //tilemap_layer.SetCell(tile_pos, spell_effects_tileset_source_id, atlas_coord);
    }

    private void DrawFloor(TileMapLayer tilemap_layer)
    {
        // setup random number generator
        var rng = new RandomNumberGenerator();

        // draw the floor area in terms of number of tiles
        for (int i = (int)tiled_floor_UpperLeft.X; i < (int)tiled_floor_LowerRight.X; i++)
        {
            for (int j = (int)tiled_floor_UpperLeft.Y; j < (int)tiled_floor_LowerRight.Y; j++)
            {
                // set a random floor type
                var rand_number = rng.RandiRange(0, floor_tiles.Length - 1);
                Vector2I atlas_coord = floor_tiles[rand_number];

                Vector2I tile_pos = new Vector2I(i, j);
                tilemap_layer.SetCell(tile_pos, floor_tileset_source_id, atlas_coord);
            }
        }
    }

    private void DrawWalls(TileMapLayer tilemap_layer)
    {
        var rand_number = 0;
        Vector2I atlas_coord = new Vector2I();
        Vector2I tile_pos = new Vector2I();

        // setup random number generator
        var rng = new RandomNumberGenerator();

        // draw the top wall in terms of number of tiles
        for (int i = (int)tiled_wallcorner_UpperLeft.X + 1; i <= (int)tiled_wallcorner_UpperRight.X - 1; i++)
        {
            // set a random wall type
            rand_number = rng.RandiRange(0, wall_tiles_upper.Length - 1);
            atlas_coord = wall_tiles_upper[rand_number];

            tile_pos = new Vector2I(i, (int)tiled_wallcorner_UpperLeft.Y);
            tilemap_layer.SetCell(tile_pos, walls_tileset_source_id, atlas_coord);
        }

        // draw the right wall in terms of number of tiles
        for (int j = (int)tiled_wallcorner_UpperRight.Y + 1; j <= (int)tiled_wallcorner_LowerRight.Y - 1; j++)
        {
            // set a random wall type
            rand_number = rng.RandiRange(0, wall_tiles_right.Length - 1);
            atlas_coord = wall_tiles_right[rand_number];

            tile_pos = new Vector2I((int)(tiled_wallcorner_UpperRight.X), j);
            tilemap_layer.SetCell(tile_pos, walls_tileset_source_id, atlas_coord);
        }

        // draw the bottom wall in terms of number of tiles
        for (int i = (int)tiled_wallcorner_LowerLeft.X + 1; i <= (int)tiled_wallcorner_LowerRight.X - 1; i++)
        {
            // set a random wall type
            rand_number = rng.RandiRange(0, wall_tiles_lower.Length - 1);
            atlas_coord = wall_tiles_lower[rand_number];

            tile_pos = new Vector2I(i, (int)(tiled_wallcorner_LowerRight.Y));
            tilemap_layer.SetCell(tile_pos, walls_tileset_source_id, atlas_coord);
        }

        // draw the left wall in terms of number of tiles
        for (int j = (int)tiled_wallcorner_UpperLeft.Y + 1; j <= (int)tiled_wallcorner_LowerLeft.Y - 1; j++)
        {
            // set a random wall type
            rand_number = rng.RandiRange(0, wall_tiles_left.Length - 1);
            atlas_coord = wall_tiles_left[rand_number];

            tile_pos = new Vector2I((int)tiled_wallcorner_LowerLeft.X, j);
            tilemap_layer.SetCell(tile_pos, walls_tileset_source_id, atlas_coord);
        }

        // Draw upper left corner
        // set a random wall type
        rand_number = rng.RandiRange(0, wall_tiles_upper_left_corner.Length - 1);
        atlas_coord = wall_tiles_upper_left_corner[rand_number];
        tile_pos = (Vector2I)tiled_wallcorner_UpperLeft;
        tilemap_layer.SetCell(tile_pos, walls_tileset_source_id, atlas_coord);

        // Draw upper right corner
        // set a random wall type
        rand_number = rng.RandiRange(0, wall_tiles_upper_right_corner.Length - 1);
        atlas_coord = wall_tiles_upper_right_corner[rand_number];
        tile_pos = (Vector2I)tiled_wallcorner_UpperRight;
        tilemap_layer.SetCell(tile_pos, walls_tileset_source_id, atlas_coord);

        // Draw lower right corner
        // set a random wall type
        rand_number = rng.RandiRange(0, wall_tiles_lower_right_corner.Length - 1);
        atlas_coord = wall_tiles_lower_right_corner[rand_number];
        tile_pos = (Vector2I)tiled_wallcorner_LowerRight;
        tilemap_layer.SetCell(tile_pos, walls_tileset_source_id, atlas_coord);

        // Draw lower left corner
        // set a random wall type
        rand_number = rng.RandiRange(0, wall_tiles_lower_left_corner.Length - 1);
        atlas_coord = wall_tiles_lower_left_corner[rand_number];
        tile_pos = (Vector2I)tiled_wallcorner_LowerLeft;
        tilemap_layer.SetCell(tile_pos, walls_tileset_source_id, atlas_coord);






        //// top wall
        //CreateBoundaryObject_Tiled(this, tiled_wallcorner_UpperLeft,
        //    tiled_wallcorner_UpperRight, WallDirections.Top);
        //// right wall
        //CreateBoundaryObject_Tiled(this, tiled_wallcorner_UpperRight,
        //    tiled_wallcorner_LowerRight, WallDirections.Right);
        //// bottom wall
        //CreateBoundaryObject_Tiled(this, tiled_wallcorner_LowerLeft,
        //    tiled_wallcorner_LowerRight, WallDirections.Bottom);
        //// left wall
        //CreateBoundaryObject_Tiled(this, tiled_wallcorner_UpperLeft,
        //    tiled_wallcorner_LowerLeft, WallDirections.Left);
    }

    /// <summary>
    /// Creates a static boundary object based on specified coordinates
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="start_point"></param>
    /// <param name="end_point"></param>
    /// <param name="angle"></param>
    private void CreateBoundaryObject_Tiled(Node2D scene, Vector2 start, Vector2 end, WallDirections wall_dir)
    {
        Vector2 right;
        Vector2 left;

        float shape_width;
        float shape_height;

        Vector2 start_pt = new Vector2();
        Vector2 end_pt = new Vector2();

        // top wall
        if (wall_dir == WallDirections.Top)
        {
            left = start; // top left corner
            right = (end + new Vector2(1, 1));     // lower right corner of end coordinate
        }
        // right wall
        else if (wall_dir == WallDirections.Right)
        {
            left = start + new Vector2(0, 0); ; // top left corner
            right = end + new Vector2(1, 1);     // lower right corner of end coordinate
        }
        else if (wall_dir == WallDirections.Bottom)
        {
            left = start; // top left corner
            right = end + new Vector2(1, 1);     // lower right corner of end coordinate
        }

        // left wall
        else if (wall_dir == WallDirections.Left)
        {
            left = start; // top left corner
            right = end + new Vector2(1, 1);     // lower right corner of end coordinate
        }
        else
        {
            left = start;
            right = end;
            // do nothing
        }
            
        start_pt = left * tileSize;
        end_pt = right * tileSize;

        shape_width = Math.Abs(end_pt.X - start_pt.X);
        shape_height = Math.Abs(end_pt.Y - start_pt.Y);

        var shape = new RectangleShape2D();
        shape.Size = new Vector2((float)shape_width, shape_height);

        // create a new collision shape
        CollisionShape2D collision_shape = new CollisionShape2D();
        collision_shape.Shape = shape;
        collision_shape.Rotation = 0;

        CharacterBody2D char_body = new CharacterBody2D();
        char_body.AddChild(collision_shape);
        char_body.Position = (start_pt + end_pt) / 2;
        char_body.ZIndex = 0;
        char_body.Name = "WallBoundary" + wall_dir;

        //// Set the collision layers
        //char_body.SetCollisionLayerValue(1, false);  // turn off the default
        //char_body.SetCollisionLayerValue((int)CollisionLayerAssignments.WALLS, true);  // assign to proper layer

        //char_body.SetCollisionMaskValue(1, false);  // turn off the default
        //char_body.SetCollisionMaskValue((int)CollisionLayerAssignments.WALLS, true);  // assign to proper layer
        //char_body.SetCollisionMaskValue((int)CollisionLayerAssignments.SPELLS_HOSTILE, true);  // assign to proper layer
        //char_body.SetCollisionMaskValue((int)CollisionLayerAssignments.SPELLS_FRIENDLY, true);  // assign to proper layer
        //char_body.SetCollisionMaskValue((int)CollisionLayerAssignments.ENVIRONMENT, true);  // assign to proper layer
        //char_body.SetCollisionMaskValue((int)CollisionLayerAssignments.ITEMS, true);  // assign to proper layer
        //char_body.SetCollisionMaskValue((int)CollisionLayerAssignments.MONSTERS, true);  // assign to proper layer
        //char_body.SetCollisionMaskValue((int)CollisionLayerAssignments.PLAYER, true);  // assign to proper layer



        // add the boundary object to the intended scene
        scene.AddChild(char_body);


    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
        Player player = GetNode<CharacterBody2D>("Player") as Player;
        player.GlobalPosition = new Vector2(50, 20);
        
        if (Input.IsActionJustPressed("shoot"))
        {
 //           ShootSpell(player);
        }
    }

    public void CreateSpellGraphics()
    {
        List<BaseSpellObjectGraphics> spell_list = new List<BaseSpellObjectGraphics>();
        // create our spell
        spell_list.Add(new BaseSpellObjectGraphics("Fireball", SpellIdentifiers.SPELL_FIRE_BOLT,
            "res://spell-effects.png", spell_effects_map_layer, spell_effects_tileset_source_id, spell_firebolt_effects_tiles));
        spell_list.Add(new BaseSpellObjectGraphics("Lightning Bolt", SpellIdentifiers.SPELL_LIGHTNING_BOLT,
            "res://spell-effects.png", spell_effects_map_layer, spell_effects_tileset_source_id, spell_lightningbolt_effects_tiles));
        spell_list.Add(new BaseSpellObjectGraphics("Poison Bolt", SpellIdentifiers.SPELL_POISON_BOLT,
            "res://spell-effects.png", spell_effects_map_layer, spell_effects_tileset_source_id, spell_poisonbolt_effects_tiles));
        spell_list.Add(new BaseSpellObjectGraphics("Frost Bolt", SpellIdentifiers.SPELL_FROST_BOLT,
            "res://spell-effects.png", spell_effects_map_layer, spell_effects_tileset_source_id, spell_frostbolt_effects_tiles));
        spell_list.Add(new BaseSpellObjectGraphics("Earth Bolt", SpellIdentifiers.SPELL_EARTH_BOLT,
            "res://spell-effects.png", spell_effects_map_layer, spell_effects_tileset_source_id, spell_earthbolt_effects_tiles));

        // create our spell dictionary
        foreach (BaseSpellObjectGraphics new_spell in spell_list)
        {
            spellObjectGraphicsDictionary.Add(new_spell.ID, new_spell);
            //GD.Print("added " + new_spell.ID.ToString());
        }

        //GD.Print(spellObjectGraphicsDictionary.ToString() + "  Count: " + spellObjectGraphicsDictionary.Count.ToString());


    }

    private void ShootSpell(Player player)
    {
        var rng = new RandomNumberGenerator();
        var rand_number = rng.RandiRange(0, 5);
        SpellIdentifiers spell_id;
        if (rand_number == 0)
        {
            spell_id = SpellIdentifiers.SPELL_FIRE_BOLT;
            GD.Print("fire selected");
        }
        else if (rand_number == 1)
        {
            spell_id = SpellIdentifiers.SPELL_LIGHTNING_BOLT;
            GD.Print("lightning selected");
        }
        else if (rand_number == 2)
        {
            spell_id = SpellIdentifiers.SPELL_POISON_BOLT;
            GD.Print("poison selected");
        }
        else if (rand_number == 3)
        {
            spell_id = SpellIdentifiers.SPELL_FROST_BOLT;
            GD.Print("frost selected");
        }
        else
        {
            spell_id = SpellIdentifiers.SPELL_EARTH_BOLT;
            GD.Print("earth selected");
        }

        Vector2 tile_pos = player.GlobalPosition;
        BaseSpellObjectGraphics new_spell = spellObjectGraphicsDictionary[spell_id];
        //GD.Print("new spell: " + new_spell.ID.ToString() + "  res: " + new_spell.ResourcePath.ToString());

        // Instantiate the scene object and then add the data from the spell dictionary --
        // this is needed because the room scene is instantiated through the parameterless constructor.
        RoomObjects new_room = RoomObjectScene.Instantiate() as RoomObjects;
        // which item type is this?
        int[] layer_bits = { (int)CollisionLayerAssignments.SPELLS_FRIENDLY };
        // which items can it hit?
        int[] mask_bits = {
            (int)(CollisionMaskAssignments.WALLS),
            (int)(CollisionMaskAssignments.ITEMS),
            (int)(CollisionMaskAssignments.MONSTERS)
        };

        var spell_pos = player.GlobalPosition;

        Vector2 initial_vel_vec = new Vector2(0, 0);
        if (player.IsMoving)
        {
            initial_vel_vec = player.DirectionUnitVector * new_spell.Speed;
            GD.Print("player is moving");
        } else
        {
            initial_vel_vec = new Vector2(20, 0);  // default velocity
            GD.Print("player is not moving - using defualt");
        }

        GD.Print("initial velocity: " + initial_vel_vec.ToString());

        new_room.Initialize(
            spell_pos,
            new_spell.GraphicsLayer,
            new_spell.TileSetSourceId,
            new_spell.AtlasCoordArray,
            player.DirectionVector,
            initial_vel_vec,
            new_spell.SpellShape,
            new_spell.AssetPath,
            layer_bits,
            mask_bits

        /// LAYER ASSIGNMENTS      MASK BITS (used for SetCollisionMask function)
        /// 1. FLOORS               1
        /// 2. WALLS                2
        /// 3. ENVIRONMENT          3
        /// 4. SPELLS FRIENDLY      4
        /// 5. SPELLS HOSTILE       5
        /// 5. ITEMS                6
        /// 6. MONSTERS             7
        /// 7. PLAYER               8
        );

        GD.Print("spell: " + spell_id.ToString());
        GD.Print("global position of spell: " + new_room.GlobalPosition.ToString());
        GD.Print("direction vector of spell: " + new_room.DirectionVector.ToString());
        

        Node2D Rooms = GetNode("RoomObjects") as Node2D;
        new_room.Name = "item_" + spell_id.ToString();
        Rooms.AddChild(new_room);
    }
}
