using Godot;
using ProjectDuhamel.scripts;
using System.Collections.Generic;
using System.Drawing;

namespace ProjectDuhamel.models.spells
{
    public partial class BaseSpellObjectGraphics : RoomObject
    {

        public SpellIdentifiers ID { get; set; } = SpellIdentifiers.SPELL_UNDEFINED;

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

        public BaseSpellObjectGraphics(SpellIdentifiers spell_id, string asset_path, TileMapLayer layer, 
            int tile_set_source_id, Vector2I[] atlas_coord)
        {

            this.ID = spell_id;

            this.AssetPath = asset_path;
            this.GraphicsLayer = layer;
            this.TileSetSourceId = tile_set_source_id;
            this.AtlasCoordArray = atlas_coord;
        }
    }
}
