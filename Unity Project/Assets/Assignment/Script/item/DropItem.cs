using UnityEngine;

public class DropItem : Photon.MonoBehaviour
{
    public int index;
    public GameObject dropImage;

    string itemPrefabName = "Cube Collider";

    // when player click on this
    public void Drop()
    {
        int itemID = Player.GetInstance().GetItemID(index);
        Player.GetInstance().GetInventory().UpdateItem("UPDATE", itemID);

        object[] objs = new object[1];
        objs[0] = itemID;

        PhotonNetwork.Instantiate(itemPrefabName, transform.position, Quaternion.identity, 0, objs);
        dropImage.SetActive(false);
        Debug.Log(itemID);
    }
}
