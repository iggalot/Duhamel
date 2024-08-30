using Godot;
using Godot.Collections;
using ProjectDuhamel.scripts;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace ProjectDuhamel.models.spells
{
    public enum SpellShape
    {
        SPELL_SHAPE_SINGLE_MISSILE = 0,
        SPELL_SHAPE_SINGLE_CIRCLE = 1,
    }
    public enum ElementalDamageTypes
    {
        ELEMENTAL_TYPE_NONE = -1,
        ELEMENTAL_TYPE_PHYSICAL = 0,
        ELEMENTAL_TYPE_FIRE = 1,
        ELEMENTAL_TYPE_COLD = 2,
        ELEMENTAL_TYPE_LIGHTNING = 3,
        ELEMENTAL_TYPE_POISON = 4,
        ELEMENTAL_TYPE_EARTH = 5
    }

    public enum SpellIdentifiers
    {
        SPELL_UNDEFINED = -1,
        SPELL_FIRE_BOLT = 0,
        SPELL_LIGHTNING_BOLT = 1,
        SPELL_POISON_BOLT = 2,
        SPELL_FROST_BOLT = 3,
        SPELL_EARTH_BOLT = 4
    }

    // This must inherit from GodotObject for some reason to make the dictionary work.
    public partial class SpellData : GodotObject
    {
        public SpellIdentifiers SpellID { get; set; }
        public string SpellName { get; set; }
        public int SpellMinDamage { get; set; }
        public int SpellMaxDamage { get; set; }
        public int SpellRange { get; set; }
        public int SpellSpeed { get; set; }
        public Size SpellSize { get; set; } = new Size();
        public int SpellCost { get; set; }
        public RectangleShape2D SpellShape { get; set; }

        public SpellData()
        {
        }

        public SpellData(string name, SpellIdentifiers id, int minDamage, int maxDamage, int range, int speed, Size size, int cost, RectangleShape2D shape = null)
        {
            SpellID = id;
            SpellName = name;
            SpellMinDamage = minDamage;
            SpellMaxDamage = maxDamage;
            SpellRange = range;
            SpellSpeed = speed;
            SpellSize = size;
            SpellCost = cost;
            SpellShape = shape;
        }

        public override string ToString()
        {
            string str = String.Empty;
            str += ("\nSpellID: " + SpellID);
            str += ("\nSpellName: " + SpellName);
            str += ("\nSpellMinDamage: " + SpellMinDamage);
            str += ("\nSpellMaxDamage: " + SpellMaxDamage);
            str += ("\nSpellRange: " + SpellRange);
            str += ("\nSpellSpeed: " + SpellSpeed);
            str += ("\nSpellSize: " + SpellSize);
            str += ("\nSpellCost: " + SpellCost);
            str += ("\nSpellShape: " + SpellShape);

            return str; ;
        }

    }

    public partial class SpellManager : RoomObject
    {
        // resource file names
        const string spell_image_resource1 = "res://spell-effects.png";


        // tileset source IDs for the layers -- should be zero if only one tileset on the tilemaplayer
        int spell_effects_tileset_source_id = 0;

        // Our tilemap layers for our template
        public TileMapLayer spell_effects_map_layer { get; set; }

        /// <summary>
        /// The dictionaries that contain our spell data
        /// </summary>
        public System.Collections.Generic.Dictionary<SpellIdentifiers, SpellData> baseSpellData { get; set; } = new System.Collections.Generic.Dictionary<SpellIdentifiers, SpellData>();
        public System.Collections.Generic.Dictionary<SpellIdentifiers, BaseSpellObjectGraphics> spellObjectGraphicsDictionary { get; set; } = new System.Collections.Generic.Dictionary<SpellIdentifiers, BaseSpellObjectGraphics>();
        public System.Collections.Generic.Dictionary<SpellIdentifiers, Vector2I[]> spellAtlasArray { get; set; } = new System.Collections.Generic.Dictionary<SpellIdentifiers, Vector2I[]>();

        #region TilesetImage Vectors -- Used for atast array coords
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
        #endregion

        public SpellManager(TileMapLayer spell_effects)
        {
            // setup our layer constants
            spell_effects_map_layer = spell_effects;

            LoadData();
            CreateSpellGraphics();
            CreateSpellShapeObjects();
            CreateSpellAtlasArrays();
        }

        /// <summary>
        /// TODO:  this needs to be done differently
        /// The atlast array values for a given spell
        /// </summary>
        private void CreateSpellAtlasArrays()
        {
            spellAtlasArray.Clear();
            spellAtlasArray.Add(SpellIdentifiers.SPELL_FIRE_BOLT, spell_firebolt_effects_tiles);
            spellAtlasArray.Add(SpellIdentifiers.SPELL_LIGHTNING_BOLT, spell_lightningbolt_effects_tiles);
            spellAtlasArray.Add(SpellIdentifiers.SPELL_POISON_BOLT, spell_poisonbolt_effects_tiles);
            spellAtlasArray.Add(SpellIdentifiers.SPELL_FROST_BOLT, spell_frostbolt_effects_tiles);
            spellAtlasArray.Add(SpellIdentifiers.SPELL_EARTH_BOLT, spell_earthbolt_effects_tiles);
        }

        private void LoadData()
        {
            baseSpellData.Clear();
            baseSpellData.Add(SpellIdentifiers.SPELL_FIRE_BOLT, new SpellData("Fire Bolt", SpellIdentifiers.SPELL_FIRE_BOLT, 10, 100, 200, 500, new Size(5, 5), 10));
            baseSpellData.Add(SpellIdentifiers.SPELL_LIGHTNING_BOLT, new SpellData("Lightning Bolt", SpellIdentifiers.SPELL_LIGHTNING_BOLT, 1, 30, 200, 750, new Size(25, 2),10));
            baseSpellData.Add(SpellIdentifiers.SPELL_POISON_BOLT, new SpellData("Poison Bolt", SpellIdentifiers.SPELL_POISON_BOLT, 1, 10, 200, 300, new Size(12, 12), 10));
            baseSpellData.Add(SpellIdentifiers.SPELL_FROST_BOLT, new SpellData("Frost Bolt", SpellIdentifiers.SPELL_FROST_BOLT, 1, 10, 200, 300, new Size(16, 6), 10));
            baseSpellData.Add(SpellIdentifiers.SPELL_EARTH_BOLT, new SpellData("Earth Bolt", SpellIdentifiers.SPELL_EARTH_BOLT, 3, 15, 200, 200, new Size(20, 20), 10));
        }

        public void CreateSpellGraphics()
        {
            spellObjectGraphicsDictionary.Clear();
            List<BaseSpellObjectGraphics> spell_list = new List<BaseSpellObjectGraphics>();

            // create our spell
            spell_list.Add(new BaseSpellObjectGraphics(SpellIdentifiers.SPELL_FIRE_BOLT,
                spell_image_resource1, spell_effects_map_layer, spell_effects_tileset_source_id, spell_firebolt_effects_tiles));
            spell_list.Add(new BaseSpellObjectGraphics(SpellIdentifiers.SPELL_LIGHTNING_BOLT,
                spell_image_resource1, spell_effects_map_layer, spell_effects_tileset_source_id, spell_lightningbolt_effects_tiles));
            spell_list.Add(new BaseSpellObjectGraphics(SpellIdentifiers.SPELL_POISON_BOLT,
                spell_image_resource1, spell_effects_map_layer, spell_effects_tileset_source_id, spell_poisonbolt_effects_tiles));
            spell_list.Add(new BaseSpellObjectGraphics(SpellIdentifiers.SPELL_FROST_BOLT,
                spell_image_resource1, spell_effects_map_layer, spell_effects_tileset_source_id, spell_frostbolt_effects_tiles));
            spell_list.Add(new BaseSpellObjectGraphics(SpellIdentifiers.SPELL_EARTH_BOLT,
                spell_image_resource1, spell_effects_map_layer, spell_effects_tileset_source_id, spell_earthbolt_effects_tiles));

            // create our spell dictionary
            foreach (BaseSpellObjectGraphics new_spell in spell_list)
            {
                spellObjectGraphicsDictionary.Add(new_spell.ID, new_spell);
            }
        }

        /// <summary>
        /// Creates a physical GODOT object for the spell
        /// with a rectangular collision shape
        /// </summary>
        /// <returns></returns>
        private void CreateSpellShapeObjects()
        {
            foreach(var data in baseSpellData)
            {
                // create the node for the object
                var spell_shape = new RectangleShape2D();
                spell_shape.Size = new Vector2(data.Value.SpellSize.Width, data.Value.SpellSize.Height);
                data.Value.SpellShape = spell_shape;
            }
        }

    }
}
