using UnityEngine;

namespace SL
{
    public class Item : ScriptableObject
    {
        [SerializeField] public int itemID { get; private set; }

        [Header("Item Information")]
        [SerializeField] private string itemName;
        [SerializeField] private Sprite itemIcon;
        [TextArea][SerializeField] private string itemDescription;

        public void SetItemID(int id)
        {
            itemID = id;
        }
    }
}
