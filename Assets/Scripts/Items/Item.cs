using UnityEngine;

namespace SL
{
    public class Item : ScriptableObject
    {
        [Header("Item Information")]
        [SerializeField] private int itemID;
        [SerializeField] private string itemName;
        [SerializeField] private Sprite itemIcon;
        [TextArea][SerializeField] private string itemDescription;
    }
}
