using Godot;
using ProjectDuhamel.models.monsters;
using ProjectDuhamel.models.spells;
using ProjectDuhamel.scripts;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Drawing;


/// <summary>
/// Default values -- for multiples, add the bit values together and then call
/// SetCollisionMaskValue to assign the bits
/// SetCollisionLayerValue to assign the specific layer as indicated below
/// LAYER ASSIGNMENTS      MASK BITS (used for SetCollisionMask function)
/// 1. FLOORS               1
/// 2. WALLS                2
/// 3. ENVIRONMENT          4
/// 4. SPELLS FRIENDLY      8
/// 5. SPELLS HOSTILE       16
/// 5. ITEMS                32
/// 6. MONSTERS             64
/// 7. PLAYER               128
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
    SpellManager spellManager { get; set; }
    MonsterManager monsterManager { get; set; }

    PackedScene RoomObjectScene = GD.Load<PackedScene>("res://scenes/room_object.tscn");
    PackedScene MonsterObjectScene = GD.Load<PackedScene>("res://scenes/monster_object.tscn");
    PackedScene SpellObjectScene = GD.Load<PackedScene>("res://scenes/spell_object.tscn");

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
    const string items = "Items";
    const string spell_effects = "SpellEffects";
    const string monsters = spell_effects;

    // Our tilemap layers for our template
    public TileMapLayer floor_effects_map_layer { get; set; }
    public TileMapLayer wall_effects_map_layer { get; set; }
    public TileMapLayer item_effects_map_layer { get; set; }
    public TileMapLayer spell_effects_map_layer { get; set; }
    public TileMapLayer monster_map_layer { get; set; }


    // tileset source IDs for the layers -- should be zero if only one tileset on the tilemaplayer
    int floor_tileset_source_id = 0; 
    int walls_tileset_source_id = 0;

    /// <summary>
    /// Room dimensions
    /// </summary>
    public int roomWidth { get; set; } = 300;
    public int roomHeight { get; set; } = 300;

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

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // setup our layer constants
        floor_effects_map_layer = GetNode<TileMapLayer>(floor);
        wall_effects_map_layer = GetNode<TileMapLayer>(walls);
        item_effects_map_layer = GetNode<TileMapLayer>(items);
        spell_effects_map_layer = GetNode<TileMapLayer>(spell_effects);
        monster_map_layer = GetNode<TileMapLayer>(monsters);

        // create our spellManager and monsterManager for handling creation of spell and monster data and graphics
        spellManager = new SpellManager(spell_effects_map_layer);
        monsterManager = new MonsterManager(spell_effects_map_layer);

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






        // top wall
        CreateBoundaryObject_Tiled(this, tiled_wallcorner_UpperLeft,
            tiled_wallcorner_UpperRight, WallDirections.Top);
        // right wall
        CreateBoundaryObject_Tiled(this, tiled_wallcorner_UpperRight,
            tiled_wallcorner_LowerRight, WallDirections.Right);
        // bottom wall
        CreateBoundaryObject_Tiled(this, tiled_wallcorner_LowerLeft,
            tiled_wallcorner_LowerRight, WallDirections.Bottom);
        // left wall
        CreateBoundaryObject_Tiled(this, tiled_wallcorner_UpperLeft,
            tiled_wallcorner_LowerLeft, WallDirections.Left);
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

        // Set the collision layers
        char_body.SetCollisionLayerValue(1, false);  // turn off the default
        char_body.SetCollisionLayerValue((int)CollisionLayerAssignments.WALLS, true);  // assign to proper layer

        char_body.SetCollisionMaskValue(1, false);  // turn off the default
        char_body.SetCollisionMaskValue((int)CollisionLayerAssignments.WALLS, true);  // assign to proper layer
        char_body.SetCollisionMaskValue((int)CollisionLayerAssignments.SPELLS_HOSTILE, true);  // assign to proper layer
        char_body.SetCollisionMaskValue((int)CollisionLayerAssignments.SPELLS_FRIENDLY, true);  // assign to proper layer
        char_body.SetCollisionMaskValue((int)CollisionLayerAssignments.ENVIRONMENT, true);  // assign to proper layer
        char_body.SetCollisionMaskValue((int)CollisionLayerAssignments.ITEMS, true);  // assign to proper layer
        char_body.SetCollisionMaskValue((int)CollisionLayerAssignments.MONSTERS, true);  // assign to proper layer
        char_body.SetCollisionMaskValue((int)CollisionLayerAssignments.PLAYER, true);  // assign to proper layer

        // add the boundary object to the intended scene
        scene.AddChild(char_body);
    }




    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
        Player player = GetNode<CharacterBody2D>("Player") as Player;

        // Testing for player input for shooting and conjuring monsters
        if (Input.IsActionJustPressed("shoot"))
        {
            ConjureSpell(player);
        }

        // Testing for player input for shooting and conjuring monsters
        if (Input.IsActionJustPressed("right_click"))
        {
            ConjureMonster(player, new Vector2(25, 25));
            ConjureMonster(player, new Vector2(25, 75));
            ConjureMonster(player, new Vector2(25, 125));
        }
    }

    private void ConjureMonster(Player player, Vector2 position)
    {
        MonsterIdentifiers monster_id = MonsterIdentifiers.MONSTER_RACE_SKELETON;

        BaseMonsterObjectGraphics monster_graphics = monsterManager.monsterObjectGraphicsDictionary[monster_id];
        MonsterData monster_data = monsterManager.baseMonsterData[monster_id];

        // Create a unique name for the spell object
        monster_graphics.Name = monster_id.ToString() + Guid.NewGuid().ToString().Substring(0, 5);


        // which item type is this?
        int[] layer_bits = { (int)CollisionLayerAssignments.MONSTERS };
        // which items can it hit?
        int[] mask_bits = {
            (int)(CollisionMaskAssignments.WALLS),
            (int)(CollisionMaskAssignments.ITEMS),
            (int)(CollisionMaskAssignments.MONSTERS),
            (int)(CollisionMaskAssignments.PLAYER),
            (int)(CollisionMaskAssignments.SPELLS_FRIENDLY)

        };

        // check if the player is facing a direction / if not, set the spell facing vector to a default direction
        // Need better logic here.  
        if (player.FacingUnitVector == Vector2.Zero)
        {
            monster_graphics.DirectionUnitVector = new Vector2(0, -1);
        }
        else
        {
            monster_graphics.DirectionUnitVector = player.FacingUnitVector;
        }

        // set the position of the spell object
        monster_graphics.Position = position;

        // instantiate the room and initialize its values.
        // Must call Initialize() with the initial values since the scene constructor is parameterless.
        MonsterObject new_monster_obj = MonsterObjectScene.Instantiate() as MonsterObject;
        new_monster_obj.Initialize(
            monster_graphics.Name,
            monster_graphics.Position,
            monster_data,
            monster_graphics,
            monster_graphics.DirectionUnitVector,
            layer_bits,
            mask_bits
        );

        new_monster_obj.Velocity = monster_data.MonsterSpeed * monster_graphics.DirectionUnitVector;

        /// LAYER ASSIGNMENTS      MASK BITS (used for SetCollisionMask function)
        /// 1. FLOORS               1
        /// 2. WALLS                2
        /// 3. ENVIRONMENT          3
        /// 4. SPELLS FRIENDLY      4
        /// 5. SPELLS HOSTILE       5
        /// 5. ITEMS                6
        /// 6. MONSTERS             7
        /// 7. PLAYER               8

        //GD.Print("spell: " + spell_id.ToString());
        //GD.Print("global position of spell: " + new_room.GlobalPosition.ToString());
        //GD.Print("direction vector of spell: " + new_room.DirectionVector.ToString());


        Node2D monsters = GetNode("MonsterObjects") as Node2D;
        monsters.AddChild(new_monster_obj);
    }


    private void ConjureSpell(Player player)
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

        BaseSpellObjectGraphics spell_graphics = spellManager.spellObjectGraphicsDictionary[spell_id];
        SpellData spell_data = spellManager.baseSpellData[spell_id];

        // Create a unique name for the spell object
        spell_graphics.Name = spell_id.ToString() + Guid.NewGuid().ToString().Substring(0, 5);

        // which item type is this?
        int[] layer_bits = { (int)CollisionLayerAssignments.SPELLS_FRIENDLY };
        // which items can it hit?
        int[] mask_bits = {
            (int)(CollisionMaskAssignments.WALLS),
            (int)(CollisionMaskAssignments.ITEMS),
            (int)(CollisionMaskAssignments.MONSTERS)
        };

        // check if the player is facing a direction / if not, set the spell facing vector to a default direction
        // Need better logic here.  
        if(player.FacingUnitVector == Vector2.Zero)
        {
            spell_graphics.DirectionUnitVector = new Vector2(0, -1);
        } else
        {
            spell_graphics.DirectionUnitVector = player.FacingUnitVector;
        }

        // set the position of the spell object
        spell_graphics.Position = player.Position;

        // instantiate the room and initialize its values.
        // Must call Initialize() with the initial values since the scene constructor is parameterless.
        SpellObject new_spell_object = SpellObjectScene.Instantiate() as SpellObject;

        new_spell_object.Initialize(
            spell_graphics.Name,
            spell_graphics.Position,
            spell_data,
            spell_graphics,
            spell_graphics.DirectionUnitVector,
            layer_bits,
            mask_bits
        );

        //GD.Print(spell_data.ToString());

        new_spell_object.Velocity = spell_data.SpellSpeed * spell_graphics.DirectionUnitVector;



        /// LAYER ASSIGNMENTS      MASK BITS (used for SetCollisionMask function)
        /// 1. FLOORS               1
        /// 2. WALLS                2
        /// 3. ENVIRONMENT          3
        /// 4. SPELLS FRIENDLY      4
        /// 5. SPELLS HOSTILE       5
        /// 5. ITEMS                6
        /// 6. MONSTERS             7
        /// 7. PLAYER               8

        //GD.Print("spell: " + spell_id.ToString());
        //GD.Print("global position of spell: " + new_room.GlobalPosition.ToString());
        //GD.Print("direction vector of spell: " + new_room.DirectionVector.ToString());


        Node2D Rooms = GetNode("SpellObjects") as Node2D;
        Rooms.AddChild(new_spell_object);
    }

    public void _on_monster_spawn_timer_timeout()
    {
        var rng = new RandomNumberGenerator();
        var rand_number = rng.RandiRange(0, 3);

        Player player = GetNode<Player>("Player") as Player;

        for (int i = 0; i < rand_number; i++)
        {
            var rand_pos_x = rng.RandiRange(25, 200);
            var rand_pos_y = rng.RandiRange(0, 200);
            ConjureMonster(player, new Vector2(rand_pos_x, rand_pos_y));
        }
        GD.Print("spawning a monster");
    }
}
