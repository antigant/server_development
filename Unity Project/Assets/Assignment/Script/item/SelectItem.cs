using UnityEngine;

public class SelectItem : MonoBehaviour
{
    public int index;
    public GameObject dropImage;

    public void SetDropImage(bool active)
    {
        if (!Player.GetInstance().HasItem(index))
            return;
        dropImage.SetActive(active);
    }

    void OnDisable()
    {
        dropImage.SetActive(false);
    }
}
