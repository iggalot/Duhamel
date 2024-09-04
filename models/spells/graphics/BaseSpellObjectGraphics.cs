using Godot;
using ProjectDuhamel.models.monsters;
using ProjectDuhamel.scripts;
using System.Collections.Generic;

namespace ProjectDuhamel.models.spells
{
    public partial class BaseSpellObjectGraphics : RoomObject
    {

        public SpellIdentifiers ID { get; set; } = SpellIdentifiers.SPELL_UNDEFINED;

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

            Texture = MakeTextures();
        }

        private List<Texture2D> MakeTextures()
        {
            var temp_list = new List<Texture2D>();

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
            var error = loaded_image.Load(AssetPath);
            if (error != Error.Ok)
            {
                GD.Print("Error loading image: " + error);
            }
            loaded_image.GetRegion(new Rect2I(16, 16, 16, 16));

            var texture = new ImageTexture();
            texture.SetImage(loaded_image);


            for(int i = 0; i < AtlasCoordArray.Length; i++)
            {
                // 2. create a new atlas texture
                var atlas_texture = new AtlasTexture();

                // 3. set the new atlas_texture's image atlas to the texture we loaded
                atlas_texture.Atlas = texture;

                // 4. get the sub region within the atlast texture based on the coordinate in the atlast image --
                //    if there is more than one sub region, this will be a random one

                var region = new Rect2(AtlasCoordArray[i].X * 16, AtlasCoordArray[i].Y * 16, 16, 16);
                atlas_texture.Region = region;
                temp_list.Add(atlas_texture);
            }

            return temp_list;
        }

        // Returns a random texture from our list of processed textures
        public Texture2D GetTextureRandom()
        {
            var rng = new RandomNumberGenerator();
            return Texture[rng.RandiRange(0, Texture.Count - 1)];
        }

        // Returns a specified texture
        public Texture2D GetTexture(int index=0)
        {
            if(index >= 0 && index < Texture.Count)
            {
                return Texture[index];
            }
            else
            {
                GD.Print("Invalid index (" + index + ") passed to GetTexture() in BaseSpellObjectGraphics.cs");
                return GD.Load("res://icon.svg") as Texture2D;
                // TODO:  Load a placeholder instead?
            }
        }

        /// <summary>
        /// Returns a copy of this object
        /// </summary>
        /// <returns></returns>
        public BaseSpellObjectGraphics Copy()
        {
            return new BaseSpellObjectGraphics
            {
                ID = this.ID,
                AssetPath = this.AssetPath,
                GraphicsLayer = this.GraphicsLayer,
                TileSetSourceId = this.TileSetSourceId,
                AtlasCoordArray = this.AtlasCoordArray
            };
        }
    }
}
