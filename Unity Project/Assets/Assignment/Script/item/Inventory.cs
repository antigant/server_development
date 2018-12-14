using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : Photon.MonoBehaviour
{
    public GameObject inventory;
    public Image[] itemImages = new Image[9];
    // only spawning cubes for this assignment, so can do this
    public Sprite[] defaultSprite = new Sprite[2];

    void Start ()
    {
        // make sure this is set to false
        inventory.SetActive(false);
        Player.GetInstance().SetInventory(this);

        // init the inventory when player log into the game
        StartCoroutine(ProcessInventory());
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.I))
            InventoryState();

        if (Input.GetKeyDown(KeyCode.Alpha1))
            SpawnItem();
	}

    IEnumerator ProcessInventory()
    {
        yield return new WaitForSeconds(2.0f);
        InitInventory();
    }

    // open/close inventory
    void InventoryState()
    {
        inventory.SetActive(!inventory.GetActive());
    }

    // make the look of invetory to have items start from index 0
    public void InventoryLook()
    {
        // set all the item box in inventory to the box sprite
        for(int i = 0; i < Player.GetInstance().GetInventorySize(); ++i)
            itemImages[i].sprite = defaultSprite[0];

        // set the cube image
        for (int i = 0; i < Player.GetInstance().GetItemCount(); ++i)
            itemImages[i].sprite = defaultSprite[1];
    }

    // spawn item in inventory
    void SpawnItem()
    {
        // inserting into database
        UpdateItem("INSERT"); 
    }

    public static void InitInventoryReceive(byte eventCode, object content, int senderID)
    {
        if (eventCode != (byte)EvCode.INIT_INVENTORY || senderID > 0)
            return;

        int[] itemIDs = (int[])content;
        //Item[] items = new Item[itemIDs.Length];
        //for (int i = 0; i < itemIDs.Length; ++i)
        //    items[i].id = itemIDs[i];
        Player.GetInstance().SetItems(itemIDs);
    }

    void InitInventory()
    {
        byte evCode = (byte)EvCode.INIT_INVENTORY;
        bool reliable = true;

        string content = Player.GetInstance().GetAccountID().ToString();
        PhotonNetwork.RaiseEvent(evCode, content, reliable, null);
    }

    public static void UpdateItemReceive(byte eventCode, object content, int senderID)
    {
        if (eventCode != (byte)EvCode.UPDATE_ITEM || senderID > 0)
            return;

        string[] message = (string[])content;
        if(message[0][0] == 'I')
        {
            // add into Player item inventory
            int itemID = System.Convert.ToInt32(message[1]);
            Player.GetInstance().AddItem(itemID);
        }
    }

    // calls the plugin to store it in the database
    public void UpdateItem(string messageType, int itemID = 0, int account_id = 0)
    {
        byte evCode = (byte)EvCode.UPDATE_ITEM;

        string item_id = itemID.ToString();
        if (itemID <= 0)
            item_id = "NULL";

        string[] content = { messageType, Player.GetInstance().GetAccountID().ToString(), item_id, account_id.ToString() };
        bool reliable = true;
        PhotonNetwork.RaiseEvent(evCode, content, reliable, null);
    }
}
