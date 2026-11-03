using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    public void Open()
    {
        Debug.Log("Treasure chest opened!");
        gameObject.SetActive(false);
    }
}
