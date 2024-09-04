using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDuhamel.models.inventory
{

    // the basic inventory class -- ideally will be used by monsters, rooms, chest, and the player
    public class Inventory
    {
        // number of items we can carry
        private int DEFAULT_INVENTORY_MAX = 30;

        public InventoryItem[] InventoryItems { get; set; } = null;

        public Inventory(int size)
        {
            InventoryItems = new InventoryItem[size];
        }
    }
}
