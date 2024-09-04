using Godot;
using ProjectDuhamel.scripts;
using System.Collections.Generic;

namespace ProjectDuhamel.models.items
{
    public partial class BaseItemObjectGraphics : RoomObject
    {
        public int ID { get; set; } = -1;

        // Contains the textures for the item
        public List<Texture2D> Textures { get; set; } = new List<Texture2D>();

        // The tilemap image layer that the graphic will be drawn on.
        public TileMapLayer GraphicsLayer { get; set; } = null;

        public float ScaleFactor { get; set; } = 1.0f;

        /// <summary>
        /// Parameterless constructor
        /// </summary>
        public BaseItemObjectGraphics()
        {
                
        }

        public BaseItemObjectGraphics(int item_id, string asset_path, TileMapLayer layer, int tile_set_source_id, Vector2I[] atlas_coord)
        {
            this.ID = item_id;

            this.AssetPath = asset_path;
            this.GraphicsLayer = layer;
            this.TileSetSourceId = tile_set_source_id;
            this.AtlasCoordArray = atlas_coord;

            Textures = MakeTextures();
            GD.Print("Made " + Textures.Count + " textures");
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
            // try and resize the image?
            loaded_image.GetRegion(new Rect2I(0, 0, 16, 16));


            var texture = new ImageTexture();
            texture.SetImage(loaded_image);


            for (int i = 0; i < AtlasCoordArray.Length; i++)
            {
                // 2. create a new atlas texture
                var atlas_texture = new AtlasTexture();

                // 3. set the new atlas_texture's image atlas to the texture we loaded
                atlas_texture.Atlas = texture;

                // 4. get the sub region within the atlast texture based on the coordinate in the atlas image --
                //    if there is more than one sub region, this will be a random one

                var region = new Rect2(AtlasCoordArray[i].X * 16, AtlasCoordArray[i].Y * 16, 16, 16);
                atlas_texture.Region = region;

                //GD.Print("atlas_texture" + atlas_texture);

                temp_list.Add(atlas_texture);
            }

            //GD.Print("temp_list: " + temp_list.Count);

            return temp_list;
        }

        // Returns a random texture from our list of processed textures
        public Texture2D GetTextureRandom()
        {
            var rng = new RandomNumberGenerator();
            return this.Textures[rng.RandiRange(0, this.Textures.Count - 1)];
        }

        // Returns a specified texture
        public Texture2D GetTexture(int index = 0)
        {
            GD.Print("Texture count: " + this.Textures.Count);
            if (index >= 0 && index < this.Textures.Count)
            {
                return this.Textures[index];
            }
            else
            {
                GD.Print("Invalid index (" + index + ") passed to GetTexture() in BaseItemObjectGraphics.cs");
                return GD.Load("res://icon.svg") as Texture2D;
                // TODO:  Load a placeholder instead?
            }
        }

        /// <summary>
        /// Returns a copy of this object
        /// </summary>
        /// <returns></returns>
        public BaseItemObjectGraphics Copy()
        {
            return new BaseItemObjectGraphics
            {
                ID = this.ID,
                AssetPath = this.AssetPath,
                GraphicsLayer = this.GraphicsLayer,
                TileSetSourceId = this.TileSetSourceId,
                AtlasCoordArray = this.AtlasCoordArray,

                Textures = this.Textures
            };
        }
    }
}
