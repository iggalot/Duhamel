using Godot;
using ProjectDuhamel.models.monsters;
using ProjectDuhamel.scripts;
using System.Collections.Generic;
using System.Drawing;

namespace ProjectDuhamel.models.spells
{
    public partial class BaseMonsterObjectGraphics : MonsterObject
    {
        public string MonsterName { get; set; } = "Unnamed Monster";

        public MonsterRaceIdentifiers ID { get; set; } = MonsterRaceIdentifiers.MONSTER_RACE_UNDEFINED;
        public float MonsterSpeed { get; set; } = 0.0f;

        /// <summary>
        /// The hitbox size of the monster -- can also be used to scale the graphic
        /// </summary>
        public Size MonsterSize { get; set; } = new Size(5, 5);

        /// <summary>
        /// The shape of the CollisionShape2D object for this object
        /// </summary>
        public RectangleShape2D MonsterShape { get; set; } = new RectangleShape2D();

        // Contains the texture(s) for the spell
        public List<Texture2D> Texture { get; set; } = new List<Texture2D>();
        // The tilemape layer that the graphic will be drawn on.
        public TileMapLayer GraphicsLayer { get; set; } = null;

        /// <summary>
        /// Parameterless constructor
        /// </summary>
        public BaseMonsterObjectGraphics()
        {
                
        }

        public BaseMonsterObjectGraphics(string monster_name, float monster_speed, Size monster_size, 
            MonsterRaceIdentifiers monster_id, string asset_path, TileMapLayer layer, 
            int tile_set_source_id, Vector2I[] atlas_coord)
        {
            this.MonsterName = monster_name;
            this.MonsterSpeed = monster_speed;
            this.MonsterSize = monster_size;
            this.ID = monster_id;

            this.AssetPath = asset_path;
            this.GraphicsLayer = layer;
            this.TileSetSourceId = tile_set_source_id;
            this.AtlasCoordArray = atlas_coord;

            this.MonsterShape = CreateMonsterObject();
        }

        /// <summary>
        /// Creates a physical GODOT object for the spell
        /// with a rectangular collision shape
        /// </summary>
        /// <returns></returns>
        private RectangleShape2D CreateMonsterObject()
        {
            // create the node for the object
            var monster_shape = new RectangleShape2D();
            monster_shape.Size = new Vector2((float)MonsterSize.Width, MonsterSize.Height);

            return monster_shape;
        }
    }
}
