using Godot;
using ProjectDuhamel.scripts;
using System.Collections.Generic;

namespace ProjectDuhamel.models.spells
{


    public partial class BaseSpellObjectGraphics : RoomObjects
    {
        public string SpellName { get; set; } = "Unnamed Spell";
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

        public BaseSpellObjectGraphics(string spell_name, SpellIdentifiers spell_id, string asset_path, TileMapLayer layer, 
            int tile_set_source_id, Vector2I[] atlas_coord)
        {
            SpellName = spell_name;
            ID = spell_id;

            AssetPath = asset_path;
            GraphicsLayer = layer;
            TileSetSourceId = tile_set_source_id;
            AtlasCoordArray = atlas_coord;

            SpellShape = CreateSpellObject();
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
            spell_shape.Size = new Vector2((float)8, 8);


            //// For historic purposes -- how to manually create a character body 2D from code
            //// remember to assign a shape2D object to the collision objects
            
            //// create a new collision shape
            //CollisionShape2D collision_shape = new CollisionShape2D();
            //collision_shape.Shape = shape;
            //collision_shape.Rotation = 0;

            //CharacterBody2D char_body = new CharacterBody2D();
            //char_body.AddChild(collision_shape);
            //char_body.Position = new Vector2(0, 0);
            //char_body.ZIndex = 3;

            return spell_shape;
        }
    }
}
