using Godot;
using ProjectDuhamel.scripts;
using System.Collections.Generic;
using System.Drawing;

namespace ProjectDuhamel.models.spells
{


    public partial class BaseSpellObjectGraphics : RoomObjects
    {
        public string SpellName { get; set; } = "Unnamed Spell";

        public SpellIdentifiers ID { get; set; } = SpellIdentifiers.SPELL_UNDEFINED;
        public float SpellSpeed { get; set; } = 40.0f;

        public Size SpellSize { get; set; } = new Size(5, 5);

        /// <summary>
        /// The shape of the CollisionShape2D object for this object
        /// </summary>
        public RectangleShape2D SpellShape { get; set; } = new RectangleShape2D();

        // Contains the texture(s) for the spell
        public List<Texture2D> Texture { get; set; } = new List<Texture2D>();
        // The tilemape layer that the graphic will be drawn on.
        public TileMapLayer GraphicsLayer { get; set; } = null;

        /// <summary>
        /// Parameterless constructor
        /// </summary>
        public BaseSpellObjectGraphics()
        {
                
        }

        public BaseSpellObjectGraphics(string spell_name, float spell_speed, Size spell_size, SpellIdentifiers spell_id, string asset_path, TileMapLayer layer, 
            int tile_set_source_id, Vector2I[] atlas_coord)
        {
            this.SpellName = spell_name;
            this.SpellSpeed = spell_speed;
            this.SpellSize = spell_size;
            this.ID = spell_id;

            this.AssetPath = asset_path;
            this.GraphicsLayer = layer;
            this.TileSetSourceId = tile_set_source_id;
            this.AtlasCoordArray = atlas_coord;

            this.SpellShape = CreateSpellObject();
        }

        /// <summary>
        /// Creates a physical GODOT object for the spell
        /// with a rectangular collision shape
        /// </summary>
        /// <returns></returns>
        private RectangleShape2D CreateSpellObject()
        {
            // create the node for the object
            var spell_shape = new RectangleShape2D();
            spell_shape.Size = new Vector2((float)SpellSize.Width, SpellSize.Height);

            return spell_shape;
        }
    }
}
