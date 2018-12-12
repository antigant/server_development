using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public GameObject inventory;
    public GameObject[] items = new GameObject[Player.GetInstance().GetInventorySize()];
    Image[] itemImages;
    // only spawning cubes for this assignment, so can do this
    public Sprite[] defaultSprite = new Sprite[2];

    void Awake()
    {
        PhotonNetwork.OnEventCall += UpdateItemReceive;
        PhotonNetwork.OnEventCall += InitInventoryReceive;
    }

    void Start ()
    {
        for (int i = 0; i < Player.GetInstance().GetInventorySize(); ++i)
            itemImages[i] = items[i].GetComponent<Image>();

        // make sure this is set to false
        inventory.SetActive(false);
        Player.GetInstance().SetInventory(this);

        // init the inventory when player log into the game
        InitInventory();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKey(KeyCode.I))
            InventoryState();

        if (Input.GetKey(KeyCode.Alpha1))
            SpawnItem();
	}

    // open/close inventory
    void InventoryState()
    {
        inventory.SetActive(!inventory.GetActive());
    }

    // make the look of invetory to have items start from index 0
    public void InventoryLook()
    {
        // just gonna hard code here first
        // set the cube image
        for (int i = 0; i < Player.GetInstance().GetInventorySize(); ++i)
            itemImages[i].sprite = defaultSprite[1];

        for (int i = Player.GetInstance().GetInventorySize() - 1; i >= Player.GetInstance().GetItemCount(); --i)
            itemImages[i].sprite = defaultSprite[0];
    }

    // spawn item in inventory
    void SpawnItem()
    {
        // inserting into database
        UpdateItem("INSERT"); 
    }

    void InitInventoryReceive(byte eventCode, object content, int senderID)
    {
        if (eventCode != (byte)EvCode.INIT_INVENTORY || senderID > 0)
            return;

        int[] itemIDs = (int[])content;
        for(int i = 0; i < itemIDs.Length; ++i)
            Player.GetInstance().AddItem(itemIDs[i]);
    }

    void InitInventory()
    {
        byte evCode = (byte)EvCode.INIT_INVENTORY;
        bool reliable = true;

        string content = Player.GetInstance().GetAccountID().ToString();
        PhotonNetwork.RaiseEvent(evCode, content, reliable, null);
    }

    void UpdateItemReceive(byte eventCode, object content, int senderID)
    {
        if (eventCode != (byte)EvCode.LOGIN || senderID > 0)
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
    void UpdateItem(string messageType, int itemID = 0)
    {
        byte evCode = (byte)EvCode.UPDATE_ITEM;

        string item_id = itemID.ToString();
        if (itemID <= 0)
            item_id = "NULL";

        string[] content = { messageType, Player.GetInstance().GetAccountID().ToString(), item_id };
        bool reliable = true;
        PhotonNetwork.RaiseEvent(evCode, content, reliable, null);
    }
}
