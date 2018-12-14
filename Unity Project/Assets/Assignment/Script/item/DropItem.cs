using UnityEngine;

public class DropItem : Photon.MonoBehaviour
{
    public int index;
    public GameObject dropImage;

    string itemPrefabName = "Cubeprefab";

    // when player click on this
    public void Drop()
    {
        int itemID = Player.GetInstance().GetItemID(index);
        Player.GetInstance().GetInventory().UpdateItem("U1PDATE", itemID);
        Player.GetInstance().RemoveItem(itemID);

        object[] objs = new object[1];
        objs[0] = itemID;

        PhotonNetwork.Instantiate(itemPrefabName, transform.position, Quaternion.identity, 0, objs);
        dropImage.SetActive(false);

        AudioManager.instance.PlaySFX(1);

        Debug.Log("item_id dropped:" + itemID);
    }
}
