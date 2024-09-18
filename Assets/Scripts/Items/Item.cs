using UnityEngine;

namespace SL
{
    public class Item : ScriptableObject
    {
        [SerializeField] public int itemID { get; private set; }

        [Header("Item Information")]
        [SerializeField] public string itemName;
        [SerializeField] public Sprite itemIcon;
        [TextArea][SerializeField] public string itemDescription;

        public void SetItemID(int id)
        {
            itemID = id;
        }
    }
}
