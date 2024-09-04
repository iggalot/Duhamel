using System.Collections.Generic;

namespace ProjectDuhamel.models.inventory
{
    public class InventoryManager
    {


        // the list of items being carried and by whom / what
        private Dictionary<string, Inventory> InventoryDictionary { get; set; } = new Dictionary<string, Inventory>();

        public InventoryManager()
        {

        }   

        /// <summary>
        /// Adds an entry to the inventory dictionary
        /// </summary>
        /// <param name="player"></param>
        /// <param name="inventory"></param>
        public void AddPlayerEntryToDictionary(Player player, Inventory inventory)
        {
            if(player != null && inventory != null)
            {
                InventoryDictionary.Add(player.Name, inventory);
            }
        }

        /// <summary>
        /// Returns an entry from the inventory dictionary
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public Inventory GetInventoryEntry(Player player)
        {
            if(player == null)
            {
                return null;
            } else
            {
                if(InventoryDictionary.ContainsKey(player.Name) == false)
                {
                    return null;
                } else
                {
                    return InventoryDictionary[player.Name];
                }
            }
        }

        /// <summary>
        /// Removes an entry from the inventory dictionary
        /// </summary>
        /// <param name="player"></param>
        public void RemoveEntryFromDictionary(Player player)
        {
            if (player == null)
            {
                return;
            }
            else
            {
                if (InventoryDictionary.ContainsKey(player.Name) == false)
                {
                    return;
                }
                else
                {
                    InventoryDictionary.Remove(player.Name);
                    return;
                }
            }
        }
    }
}
