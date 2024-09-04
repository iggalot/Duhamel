using Godot;
using ProjectDuhamel.models.monsters;
using ProjectDuhamel.scripts;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDuhamel.models.items
{
    public enum ItemTypes
    {
        ITEM_TYPE_NONE = -1,
        ITEM_TYPE_QUEST = 0,
        ITEM_TYPE_CURRENCY = 1,
        ITEM_TYPE_CONSUMABLE = 2,
        ITEM_TYPE_EQUIPMENT = 3,
        ITEM_TYPE_TOOL = 4,
        ITEM_TYPE_CRAFTING = 5,
        ITEM_POTION = 6,
        ITEM_KEY = 7,
        ITEM_CONTAINER = 8,
        ITEM_ROOM_DECORATION = 9,
        ITEM_DOOR = 10,
        ITEM_TRAP = 11

    }

    public partial class ItemData : GodotObject
    {
        public int ItemID { get; set; } 
        public string ItemName { get; set; } = String.Empty;
        public int ItemDurability { get; set; }
        public string ItemEffect { get; set; } = String.Empty;
        public ItemTypes ItemType { get; set; }
        public int ItemBaseCost { get; set; }
        public RectangleShape2D ItemShape { get; set; }
        public Size ItemSize { get; set; }

        public float ItemScaleFactor { get; set; } = 1.0f;

        public ItemData()
        {
                
        }

        public ItemData(int itemID, string itemName, int itemDurability, string itemEffect, ItemTypes itemType, int itemBaseCost, Size itemSize, RectangleShape2D itemShape=null)
        {
            ItemID = itemID;
            ItemName = itemName;
            ItemDurability = itemDurability;
            ItemEffect = itemEffect;
            ItemType = itemType;
            ItemBaseCost = itemBaseCost;
            ItemShape = itemShape;
            ItemSize = itemSize;
        }

        public ItemData Copy()
        {
            return new ItemData
            {
                ItemID = this.ItemID,
                ItemName = this.ItemName,
                ItemDurability = this.ItemDurability,
                ItemEffect = this.ItemEffect,
                ItemType = this.ItemType,
                ItemBaseCost = this.ItemBaseCost,
                ItemShape = this.ItemShape,
                ItemSize = this.ItemSize
            };
        }

        public override string ToString()
        {
            string str = String.Empty;
            str += ("\nItemID: " + ItemID);
            str += ("\nItemName: " + ItemName);
            str += ("\nItemDurability: " + ItemDurability);
            str += ("\nItemEffect: " + ItemEffect);
            str += ("\nItemType: " + ItemType);
            str += ("\nItemBaseCost: " + ItemBaseCost);
            str += ("\nItemShape: " + ItemShape);
            str += ("\nItemSize: " + ItemSize);

            return str;
        }
    }

    public partial class ItemManager : RoomObject
    {
        // resource file names
        const string item_image_resource1 = "res://assets/character_and_tileset/Dungeon_Tileset.png";

        // tileset source IDs for the layers -- should be zero if only one tileset on the tilemap layer
        int item_images_tileset_source_id = 0;

        // our tilemap layers for our template
        public TileMapLayer monster_map_layer { get; set; }

        /// <summary>
        /// The dictionaries for our item data
        /// </summary>
        public TileMapLayer item_map_layer { get; set; }

        public System.Collections.Generic.Dictionary<int, ItemData> baseItemData { get; set; } = new System.Collections.Generic.Dictionary<int, ItemData>();
        public System.Collections.Generic.Dictionary<int, BaseItemObjectGraphics> itemObjectGraphicsDictionary { get; set; } = new System.Collections.Generic.Dictionary<int, BaseItemObjectGraphics>();
        public System.Collections.Generic.Dictionary<int, Vector2I[]> itemAtlastArray { get; set; } = new System.Collections.Generic.Dictionary<int, Vector2I[]>();

        #region TilesetImage Vectors -- Used for atlas array coords
        Vector2I[] item_images_tiles =
        {
            new Vector2I(4, 8),
            new Vector2I(4, 9),

            new Vector2I(6, 8),
            new Vector2I(6, 9),
            new Vector2I(6, 10),
            new Vector2I(6, 11),

            new Vector2I(7, 7),

            new Vector2I(0, 9),
        };

        public ItemManager(TileMapLayer layer)
        {
            // setup our layer constants
            item_map_layer = layer;

            CreateAllItems();
        }

        private void CreateAllItems()
        {
            CreateAllItemData();
            CreateAllItemGraphics();
            CreateAllItemShapeObjects();
            CreateAllItemAtlasArrays();
        }

        /// <summary>
        /// Create the item atlast array.  ID numbers need to agree with other ID numbers in the item data functions
        /// </summary>
        private void CreateAllItemAtlasArrays()
        {
            itemAtlastArray.Clear();
            itemAtlastArray.Add(0, item_images_tiles);
        }

        /// <summary>
        /// Creates all the items that are known by the game
        /// </summary>
        private void CreateAllItemData()
        {
            baseItemData.Clear();
            baseItemData.Add(0, new ItemData(0, "Sword", 10, "Sword Effect", ItemTypes.ITEM_TYPE_EQUIPMENT, 10, new Size(5, 5)));
            baseItemData.Add(1, new ItemData(1, "Shield", 10, "Shield Effect", ItemTypes.ITEM_TYPE_EQUIPMENT, 10, new Size(5, 5)));
            baseItemData.Add(2, new ItemData(2, "Potion", 10, "Potion Effect", ItemTypes.ITEM_TYPE_CONSUMABLE, 10, new Size(5, 5)));
            baseItemData.Add(3, new ItemData(3, "Ring", 10, "Ring Effect", ItemTypes.ITEM_TYPE_EQUIPMENT, 10, new Size(5, 5)));
            baseItemData.Add(4, new ItemData(4, "Amulet", 10, "Amulet Effect", ItemTypes.ITEM_TYPE_EQUIPMENT, 10, new Size(5, 5)));
            baseItemData.Add(5, new ItemData(5, "Boots", 10, "Boots Effect", ItemTypes.ITEM_TYPE_EQUIPMENT, 10, new Size(5, 5)));
            baseItemData.Add(6, new ItemData(6, "Gloves", 10, "Gloves Effect", ItemTypes.ITEM_TYPE_EQUIPMENT, 10, new Size(5, 5)));
            baseItemData.Add(7, new ItemData(7, "Chest", 10, "Chest Effect", ItemTypes.ITEM_TYPE_EQUIPMENT, 10, new Size(5, 5)));
            baseItemData.Add(8, new ItemData(8, "Cape", 10, "Cape Effect", ItemTypes.ITEM_TYPE_EQUIPMENT, 10, new Size(5, 5)));
            baseItemData.Add(9, new ItemData(9, "Helm", 10, "Helm Effect", ItemTypes.ITEM_TYPE_EQUIPMENT, 10, new Size(5, 5)));
            baseItemData.Add(10, new ItemData(10, "Small coins", 10, "Coin Effect", ItemTypes.ITEM_TYPE_CURRENCY, 10, new Size(5, 5)));
        }

        private void CreateAllItemShapeObjects()
        {
            itemObjectGraphicsDictionary.Clear();
            List<BaseItemObjectGraphics> item_list = new List<BaseItemObjectGraphics>
            {
                new BaseItemObjectGraphics(0, item_image_resource1, item_map_layer, item_images_tileset_source_id, item_images_tiles),
                new BaseItemObjectGraphics(1, item_image_resource1, item_map_layer, item_images_tileset_source_id, item_images_tiles),
                new BaseItemObjectGraphics(2, item_image_resource1, item_map_layer, item_images_tileset_source_id, item_images_tiles),
                new BaseItemObjectGraphics(3, item_image_resource1, item_map_layer, item_images_tileset_source_id, item_images_tiles),
                new BaseItemObjectGraphics(4, item_image_resource1, item_map_layer, item_images_tileset_source_id, item_images_tiles),
                new BaseItemObjectGraphics(5, item_image_resource1, item_map_layer, item_images_tileset_source_id, item_images_tiles),
                new BaseItemObjectGraphics(6, item_image_resource1, item_map_layer, item_images_tileset_source_id, item_images_tiles),
                new BaseItemObjectGraphics(7, item_image_resource1, item_map_layer, item_images_tileset_source_id, item_images_tiles),
                new BaseItemObjectGraphics(8, item_image_resource1, item_map_layer, item_images_tileset_source_id, item_images_tiles),
                new BaseItemObjectGraphics(9, item_image_resource1, item_map_layer, item_images_tileset_source_id, item_images_tiles),
                new BaseItemObjectGraphics(10, item_image_resource1, item_map_layer, item_images_tileset_source_id, item_images_tiles)
            };

            // create our item dictionary
            foreach(BaseItemObjectGraphics new_item in item_list)
            {
                this.itemObjectGraphicsDictionary.Add(new_item.ID, new_item);
            }
        }

        private void CreateAllItemGraphics()
        {
            foreach (var data in baseItemData)
            {
                //create the node for the object
                var item_shape = new RectangleShape2D();
                item_shape.Size = new Vector2(data.Value.ItemSize.Width, data.Value.ItemSize.Height);
                data.Value.ItemShape = item_shape;  
            }
        }

        public ItemData CreateSingleItemData(int item_id)
        {
            var data = baseItemData[item_id].Copy();  // make a copy of the data

            // TODO::  Add manipulators off of the base dictionary item -- scale, graphics, etc...

            // create the shape obj for this monster
            var item_shape = new RectangleShape2D();
            item_shape.Size = new Vector2(data.ItemSize.Width * data.ItemScaleFactor, data.ItemSize.Height * data.ItemScaleFactor);
            data.ItemShape = item_shape;
            return data;
        }
        #endregion
    }
}
