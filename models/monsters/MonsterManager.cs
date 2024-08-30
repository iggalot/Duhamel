using Godot;
using Godot.Collections;
using ProjectDuhamel.scripts;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace ProjectDuhamel.models.monsters
{
    public enum MonsterIdentifiers
    {
        MONSTER_RACE_UNDEFINED = -1,
        MONSTER_RACE_SKELETON = 0
    }

    // This must inherit from GodotObject for some reason to make the dictionary work.
    public partial class MonsterData : GodotObject
    {
        public MonsterIdentifiers MonsterID { get; set; }
        public string MonsterName { get; set; }
        public int MonsterMinDamage { get; set; }
        public int MonsterMaxDamage { get; set; }
        public int MonsterRange { get; set; }
        public int MonsterSpeed { get; set; }
        public Size MonsterSize { get; set; } = new Size();
        public int MonsterCost { get; set; }
        public RectangleShape2D MonsterShape { get; set; }

        public MonsterData()
        {
        }

        public MonsterData(string name, MonsterIdentifiers id, int minDamage, int maxDamage, int range, int speed, Size size, RectangleShape2D shape = null)
        {
            MonsterID = id;
            MonsterName = name;
            MonsterMinDamage = minDamage;
            MonsterMaxDamage = maxDamage;
            MonsterRange = range;
            MonsterSpeed = speed;
            MonsterSize = size;
            MonsterShape = shape;
        }

        public override string ToString()
        {
            string str = String.Empty;
            str += ("\nMonsterID: "         + MonsterID);
            str += ("\nMonsterName: "       + MonsterName);
            str += ("\nMonsterMinDamage: "  + MonsterMinDamage);
            str += ("\nMonsterMaxDamage: "  + MonsterMaxDamage);
            str += ("\nMonsterRange: "      + MonsterRange);
            str += ("\nMonsterSpeed: "      + MonsterSpeed);
            str += ("\nMonsterSize: "       + MonsterSize);
            str += ("\nMonsterCost: "       + MonsterCost);
            str += ("\nMonsterShape: "      + MonsterShape);

            return str; ;
        }
    }

    public partial class MonsterManager : RoomObject
    {
        // resource file names
        const string monster_image_resource1 = "res://assets/character_and_tileset/Dungeon_Character.png";

        // tileset source IDs for the layers -- should be zero if only one tileset on the tilemaplayer
        int monster_images_tileset_source_id = 0;

        // Our tilemap layers for our template
        public TileMapLayer monster_map_layer { get; set; }

        /// <summary>
        /// The dictionaries that contain our spell data
        /// </summary>
        public System.Collections.Generic.Dictionary<MonsterIdentifiers, MonsterData> baseMonsterData { get; set; } = new System.Collections.Generic.Dictionary<MonsterIdentifiers, MonsterData>();
        public System.Collections.Generic.Dictionary<MonsterIdentifiers, BaseMonsterObjectGraphics> monsterObjectGraphicsDictionary { get; set; } = new System.Collections.Generic.Dictionary<MonsterIdentifiers, BaseMonsterObjectGraphics>();
        public System.Collections.Generic.Dictionary<MonsterIdentifiers, Vector2I[]> monsterAtlasArray { get; set; } = new System.Collections.Generic.Dictionary<MonsterIdentifiers, Vector2I[]>();

        #region TilesetImage Vectors -- Used for atast array coords

        Vector2I[] monster_images_tiles =
        {
        //new Vector2I(0, 0),
        //new Vector2I(0, 1),
        //new Vector2I(0, 2),
        //new Vector2I(0, 3),
       
        //new Vector2I(1, 0),
        //new Vector2I(1, 1),
        //new Vector2I(1, 2),
        //new Vector2I(1, 3),
       
        //new Vector2I(2, 0),
        //new Vector2I(2, 1),
        //new Vector2I(2, 2),
        //new Vector2I(2, 3),
        
        //new Vector2I(3, 0),
        //new Vector2I(3, 1),
        //new Vector2I(3, 2),
        //new Vector2I(3, 3),

        //new Vector2I(4, 0),
        new Vector2I(4, 1),
        //new Vector2I(4, 2),
        new Vector2I(4, 3),

        //new Vector2I(5, 0),
        new Vector2I(5, 1),
        //new Vector2I(5, 2),
        new Vector2I(5, 3),

        //new Vector2I(6, 0),
        new Vector2I(6, 1),
        //new Vector2I(6, 2),
        new Vector2I(6, 3),
    };

        #endregion

        public MonsterManager(TileMapLayer layer)
        {
            // setup our layer constants
            monster_map_layer = layer;

            LoadData();
            CreateMonsterGraphics();
            CreateMonsterShapeObjects();
            CreateMonsterAtlasArrays();
        }

        /// <summary>
        /// TODO:  this needs to be done differently
        /// The atlast array values for a given spell
        /// </summary>
        private void CreateMonsterAtlasArrays()
        {
            monsterAtlasArray.Clear();
            monsterAtlasArray.Add(MonsterIdentifiers.MONSTER_RACE_SKELETON, monster_images_tiles);
        }

        private void LoadData()
        {
            baseMonsterData.Clear();
            baseMonsterData.Add(MonsterIdentifiers.MONSTER_RACE_SKELETON, new MonsterData("Skeleton", MonsterIdentifiers.MONSTER_RACE_SKELETON, 1, 10, 20, 50, new Size(5, 5)));
                    }

        public void CreateMonsterGraphics()
        {
            monsterObjectGraphicsDictionary.Clear();
            List<BaseMonsterObjectGraphics> monster_list = new List<BaseMonsterObjectGraphics>
            {
                //create our monster
                new BaseMonsterObjectGraphics(MonsterIdentifiers.MONSTER_RACE_SKELETON, monster_image_resource1, monster_map_layer, monster_images_tileset_source_id, monster_images_tiles)
            };

            //GD.Print("monster_image_resource1: " + monster_image_resource1);
            //GD.Print("monster_map_layer: " + monster_map_layer);
            //GD.Print("monster_images_tileset_id: " + monster_images_tileset_source_id);
            //GD.Print("monster_images_tiles" + monster_images_tiles.Length);

            // create our spell dictionary
            foreach (BaseMonsterObjectGraphics new_monster in monster_list)
            {
                GD.Print("added monster: " + new_monster.ID + " to graphics dictionary");
                this.monsterObjectGraphicsDictionary.Add(new_monster.ID, new_monster);
            }
        }

        /// <summary>
        /// Creates a physical GODOT object for the spell
        /// with a rectangular collision shape
        /// </summary>
        /// <returns></returns>
        private void CreateMonsterShapeObjects()
        {
            foreach (var data in baseMonsterData)
            {
                // create the node for the object
                var spell_shape = new RectangleShape2D();
                spell_shape.Size = new Vector2(data.Value.MonsterSize.Width, data.Value.MonsterSize.Height);
                data.Value.MonsterShape = spell_shape;
            }
        }

    }
}
