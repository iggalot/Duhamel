using Godot;
using Godot.Collections;
using ProjectDuhamel.scripts;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection.Metadata;
using System.Text;
using System.Xml.Linq;

namespace ProjectDuhamel.models.monsters
{
    public enum MonsterIdentifiers
    {
        MONSTER_RACE_UNDEFINED = -1,
        MONSTER_RACE_SKELETON = 0
    }

    public enum MonsterRanks
    {
        MONSTER_RANK_NONE = 0,
        MONSTER_RANK_COMMON = 1,
        MONSTER_RANK_UNCOMMON = 2,
        MONSTER_RANK_RARE = 3,
        MONSTER_RANK_EPIC = 4,
        MONSTER_RANK_LEGENDARY = 5,
    }


    // This must inherit from GodotObject for some reason to make the dictionary work.
    public partial class MonsterData : GodotObject
    {
        // the weight factors to be used wth MonsterRanks
        private static float[] RankWeightFactors = new float[] { 0.0f, 0.5f, 0.3f, 0.15f, 0.04f, 0.01f };
        public MonsterRanks MonsterRank { get; set; } = MonsterRanks.MONSTER_RANK_COMMON;
        public string[] MonsterRankNames { get; set; } = new string[] { "Unknown","Common", "Uncommon", "Rare", "Epic", "Legendary" };

        public float MonsterScaleFactor { get; set; } = 1.0f;

        public MonsterIdentifiers MonsterID { get; set; }
        public int MonsterHitPoints { get; set; }
        public int MonsterExp { get; set; }
        public string MonsterName { get; set; }
        public int MonsterMinDamage { get; set; }
        public int MonsterMaxDamage { get; set; }
        public int MonsterRange { get; set; }
        public int MonsterSpeed { get; set; }
        public int MonsterAttackSpeed { get; set; }
        public Size MonsterSize { get; set; } = new Size();
        public int MonsterCost { get; set; }
        public RectangleShape2D MonsterShape { get; set; }

        public MonsterData()
        {
        }

        public MonsterData(string name, MonsterIdentifiers id, int hit_points, int exp, int minDamage, int maxDamage, int range, int speed, int attack_speed,
            Size size, RectangleShape2D shape = null)
        {
            MonsterID = id;
            MonsterName = name;
            MonsterHitPoints = hit_points;
            MonsterExp = exp;
            MonsterMinDamage = minDamage;
            MonsterMaxDamage = maxDamage;
            MonsterRange = range;
            MonsterSpeed = speed;
            MonsterAttackSpeed = attack_speed;
            MonsterSize = size;
            MonsterShape = shape;
            MonsterRank = MonsterRanks.MONSTER_RANK_COMMON;
            MonsterScaleFactor = 1.0f;
        }
        /// <summary>
        /// A copy constructor for the MonsterData object
        /// </summary>
        /// <returns></returns>
        public MonsterData Copy()
        {
            return new MonsterData
            {
                MonsterID = this.MonsterID,
                MonsterName = this.MonsterName,
                MonsterHitPoints = this.MonsterHitPoints,
                MonsterExp = this.MonsterExp,
                MonsterMinDamage = this.MonsterMinDamage,
                MonsterMaxDamage = this.MonsterMaxDamage,
                MonsterRange = this.MonsterRange,
                MonsterSpeed = this.MonsterSpeed,
                MonsterAttackSpeed = this.MonsterAttackSpeed,
                MonsterSize = this.MonsterSize,
                MonsterShape = this.MonsterShape,
                MonsterRank = this.MonsterRank,
                MonsterScaleFactor = this.MonsterScaleFactor,
            };
        }


        /// <summary>
        /// used to scale a monsters stats by a scale factor
        /// </summary>
        /// <param name="scale_factor">scale_factor</param>
        public void ScaleMonsterData (float scale_factor)
        {
            MonsterHitPoints = (int)(MonsterHitPoints * scale_factor);
            MonsterExp = (int)(MonsterExp * scale_factor);
            MonsterMinDamage = (int)(MonsterMinDamage * scale_factor);
            MonsterMaxDamage = (int)(MonsterMaxDamage * scale_factor);
            MonsterRange = (int)(MonsterRange * scale_factor);
            MonsterSpeed = (int)(MonsterSpeed * scale_factor);
            MonsterAttackSpeed = (int)(MonsterAttackSpeed * (1.0f /scale_factor ));  // tweak the attack speed -- lower number is fast
            MonsterSize = new Size((int)(MonsterSize.Width * scale_factor), (int)(MonsterSize.Height * scale_factor));
            MonsterScaleFactor = scale_factor;
        }

        public MonsterRanks CreateRank()
        {
            float sum = 0;

            for (int i = 0; i < RankWeightFactors.Length; i++)
            {
                sum += RankWeightFactors[i];
            }

            var rng = new RandomNumberGenerator();
            var rand = rng.RandfRange(0, sum);

            if(rand < RankWeightFactors[0])  return MonsterRanks.MONSTER_RANK_COMMON;
            else if (rand < RankWeightFactors[0]+RankWeightFactors[1]) return MonsterRanks.MONSTER_RANK_UNCOMMON;
            else if (rand < RankWeightFactors[0] + RankWeightFactors[1] + RankWeightFactors[2]) return MonsterRanks.MONSTER_RANK_RARE;
            else if (rand < RankWeightFactors[0] + RankWeightFactors[1] + RankWeightFactors[2] + RankWeightFactors[3]) return MonsterRanks.MONSTER_RANK_EPIC;
            else if (rand < RankWeightFactors[0] + RankWeightFactors[1] + RankWeightFactors[2] + RankWeightFactors[3] + RankWeightFactors[4]) return MonsterRanks.MONSTER_RANK_LEGENDARY;
            else
            {
               // if all else fails, return common
               return MonsterRanks.MONSTER_RANK_NONE;
            }
        }

        public float GetMonsterScaleFactor()
        {
            MonsterRanks rank = this.MonsterRank;
            float tweak_factor = 0.75f;

            switch (rank)
            {
                case MonsterRanks.MONSTER_RANK_COMMON:
                    return 1.5f * tweak_factor;
                case MonsterRanks.MONSTER_RANK_UNCOMMON:
                    return 2.0f * tweak_factor;
                case MonsterRanks.MONSTER_RANK_RARE:
                    return 2.5f * tweak_factor;
                case MonsterRanks.MONSTER_RANK_EPIC:
                    return 3.0f * tweak_factor;
                case MonsterRanks.MONSTER_RANK_LEGENDARY:
                    return 4.0f * tweak_factor;
                default:
                    return 1.0f * tweak_factor;
            }


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

            CreateAllMonsters();
        }

        private void CreateAllMonsters()
        {
            CreateAllMonsterData();
            CreateAllMonsterGraphics();
            CreateAllMonsterShapeObjects();
            CreateAllMonsterAtlasArrays();
        }

        #region CREATE ALL MONSTERS
        /// <summary>
        /// TODO:  this needs to be done differently
        /// The atlast array values for a given spell
        /// </summary>
        private void CreateAllMonsterAtlasArrays()
        {
            monsterAtlasArray.Clear();
            monsterAtlasArray.Add(MonsterIdentifiers.MONSTER_RACE_SKELETON, monster_images_tiles);
        }

        private void CreateAllMonsterData()
        {
            baseMonsterData.Clear();
            baseMonsterData.Add(MonsterIdentifiers.MONSTER_RACE_SKELETON, new MonsterData("Skeleton", MonsterIdentifiers.MONSTER_RACE_SKELETON, 20, 10, 1, 10, 20, 50, 10, new Size(5, 5)));
                    }

        public void CreateAllMonsterGraphics()
        {
            monsterObjectGraphicsDictionary.Clear();
            List<BaseMonsterObjectGraphics> monster_list = new List<BaseMonsterObjectGraphics>
            {
                //create our monster
                new BaseMonsterObjectGraphics(MonsterIdentifiers.MONSTER_RACE_SKELETON, monster_image_resource1, monster_map_layer, monster_images_tileset_source_id, monster_images_tiles)
            };

            // create our spell dictionary
            foreach (BaseMonsterObjectGraphics new_monster in monster_list)
            {
                //GD.Print("added monster: " + new_monster.ID + " to graphics dictionary");
                this.monsterObjectGraphicsDictionary.Add(new_monster.ID, new_monster);
            }
        }

        /// <summary>
        /// Creates a physical GODOT object for the spell
        /// with a rectangular collision shape
        /// </summary>
        /// <returns></returns>
        private void CreateAllMonsterShapeObjects(float scale_factor = 1.0f)
        {
            foreach (var data in baseMonsterData)
            {
                // create the node for the object
                var spell_shape = new RectangleShape2D();
                spell_shape.Size = new Vector2(data.Value.MonsterSize.Width * scale_factor, data.Value.MonsterSize.Height * scale_factor);
                data.Value.MonsterShape = spell_shape;
            }
        }

        #endregion





        public MonsterData CreateSingleMonsterData(MonsterIdentifiers id)
        {
            
            var data = baseMonsterData[id].Copy();  // make a copy of the data

            // create the ranks for the object
            data.MonsterRank = data.CreateRank();
//            GD.Print("Monster rank is " + data.MonsterRank);

            // determine the scale factor and then scale the base monster
            data.MonsterScaleFactor = data.GetMonsterScaleFactor();
            data.ScaleMonsterData(data.MonsterScaleFactor);

//            GD.Print("Monster scale factor is " + data.MonsterScaleFactor);
//            GD.Print("Monster size is " + data.MonsterSize);

            // create the shape obj for this monster
            var monster_shape = new RectangleShape2D();
            monster_shape.Size = new Vector2(data.MonsterSize.Width * data.MonsterScaleFactor, data.MonsterSize.Height * data.MonsterScaleFactor);
            data.MonsterShape = monster_shape;

            return data;
        }
    }
}
