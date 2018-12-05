// Singleton class that the entire game can access
public class Player
{
    static Player instance = null;

    readonly int accountID;
    // name of the character in the game
    string playerName;
    // total number of item this can store
    int[] item;
    // curr available slot
    int currItemSlot;

    // Protect another object from being instantiate
    private Player(int accountID)
    {
        this.accountID = accountID;

        // can hold x amount of item id in it
        item = new int[10];
        // use -1 itemID to identify that the slot is empty
        for (int i = 0; i < item.Length; ++i)
            item[i] = -1;
        currItemSlot = 0;
    }

    // use this to get the player info
    public static Player GetInstance()
    {
        return instance;
    }

    // use this to init the player
    public static Player GetInstance(int accountID)
    {
        if (instance != null)
            return instance;
        return instance = new Player(accountID);
    }

    // setter
    public void SetPlayerName(string playerName)
    {
        this.playerName = playerName;
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

    public void AddItem(int itemID)
    {
        item[currItemSlot] = itemID;
        ++currItemSlot;
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
        }

        // using this loop to rearrange the item in the array
        for(int i = slotRemoved; i < item.Length - 1; ++i)
        {
            item[i] = item[++i];
            item[++i] = -1;
        }
    }
}