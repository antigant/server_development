using UnityEngine;

// Singleton class that the entire game can access
public class Player
{
    static Player instance;

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
    Vector3 forward;

    // inventory
    Inventory inventory;

    // Protect another object from being instantiate
    private Player(int id, string name)
    {
        ResetPlayer();
        accountID = id;
        playerName = name;
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

    public void ResetPlayer()
    {
        instance = null;
        playerName = "";
        item = null;
        inventory = null;

        currItemSlot = 0;
        itemCount = 0;

        // Delete this later
        //position = Vector3.zero;
        petPosition = Vector3.zero;
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

    public void SetForward(Vector3 fw)
    {
        forward = fw;
    }

    public void SetInventory(Inventory invent)
    {
        inventory = invent;
    }

    public void SetItems(int[] itemIDs)
    {
        item = itemIDs;
        // init itemCount to be the correct value
        for (int i = 0; i < item.Length; ++i)
        {
            if (item[i] == -1)
                break;
            ++itemCount;
            ++currItemSlot;
        }
        inventory.InventoryLook();
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

    public Inventory GetInventory()
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

    // get the item id on that current index
    public int GetItemID(int index = -1)
    {
        if (index <= -1)
            return -1;
        return item[index];
    }

    public Vector3 GetForward()
    {
        return forward;
    }

    // check if there's item
    public bool HasItem(int index)
    {
        return item[index] > -1;
    }

    public void AddItem(int itemID)
    {
        // do not add if item is more than the what it can hold LOL stupid mistake
        if (itemCount > item.Length)
            return;
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
            item[i] = item[i + 1];
            item[i + 1] = -1;
        }

        // return if no item is deleted, won't update the inventory
        if (slotRemoved >= item.Length)
            return;

        inventory.InventoryLook();
    }
}