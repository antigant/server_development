using UnityEngine;

// Singleton class that the entire game can access
public class Player
{
    static Player instance = null;

    readonly int accountID;
    // name of the character in the game
    string playerName;
    // store the item_id of the item
    int[] item;
    // number of item in the arary item (can technically use currItemSlot but I will just create another var for future expansion)
    int itemCount;
    // curr available slot
    int currItemSlot;

    // last position
    Vector3 position;
    Vector3 petPosition;

    // inventory
    Inventory inventory;

    // Protect another object from being instantiate
    private Player(int id, string name)
    {
        accountID = id;
        playerName = name;

        // can hold x amount of item id in it
        item = new int[9];
        // use -1 itemID to identify that the slot is empty
        for (int i = 0; i < item.Length; ++i)
            item[i] = -1;
        currItemSlot = 0;

        // Delete this later
        petPosition = Vector3.zero;
    }

    // use this to get the player info
    public static Player GetInstance()
    {
        return instance;
    }

    // use this to init the player
    public static Player GetInstance(int id, string name, bool force = false)
    {
        if (instance != null && !force)
            return instance;
        return instance = new Player(id, name);
    }

    // setter
    public void SetPosition(Vector3 pos)
    {
        position = pos;
    }

    public void SetPetPosition(Vector3 pos)
    {
        petPosition = pos;
    }

    public void SetInventory(Inventory invent)
    {
        inventory = invent;
    }

    // getter
    public string GetPlayerName()
    {
        return playerName;
    }

    public int GetAccountID()
    {
        return accountID;
    }

    public Vector3 GetPosition()
    {
        return position;
    }

    public Vector3 GetPetPosition()
    {
        return petPosition;
    }

    public Inventory GetInventroy()
    {
        return inventory;
    }

    // returns the total number of items
    public int GetItemCount()
    {
        return itemCount;
    }

    public int GetInventorySize()
    {
        return item.Length;
    }

    public void AddItem(int itemID)
    {
        item[currItemSlot] = itemID;
        ++currItemSlot;
        ++itemCount;

        inventory.InventoryLook();
    }
    
    public void RemoveItem(int itemID)
    {
        // slot number of the item removed
        int slotRemoved = item.Length; // init as item.length so that if the item is not found, 2nd loop will not run
        // using this loop to remove the specify item
        for(int i = 0; i < item.Length; ++i)
        {
            if (item[i] != itemID)
                continue;
            // found the ID of the item
            item[i] = -1; // "remove" the item
            slotRemoved = i;
            --currItemSlot;
            --itemCount;
        }

        // using this loop to rearrange the item in the array
        for (int i = slotRemoved; i < item.Length - 1; ++i)
        {
            item[i] = item[++i];
            item[++i] = -1;
        }

        // return if no item is deleted, won't update the inventory
        if (slotRemoved >= item.Length)
            return;

        inventory.InventoryLook();
    }
}