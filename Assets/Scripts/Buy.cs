using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Buy : MonoBehaviour
{
    [SerializeField] public int itemIndex;
    [SerializeField] public int cost;
    [SerializeField] private GameObject itemDisplay;
    [SerializeField] private Text costText;
    [SerializeField] private Text nameText;

    private ItemTable itemTable;
    private Inventory inventory;
    private int maxItemCount;

    void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
        itemTable = GameObject.FindGameObjectWithTag("ItemTable").GetComponent<ItemTable>();

        Image itemDisplayImage = itemDisplay.GetComponent<Image>();
        itemDisplayImage.sprite = itemTable.itemSprite[itemIndex];

        costText.text = cost.ToString();
        nameText.text = itemTable.itemName[itemIndex];
    }
    
    public void Purchase()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            for (int i = 0; i < 5; i++)
                if (inventory.money >= cost && inventory.AddItem(itemIndex))
                    inventory.UpdateMoney(-cost);
        }
        else if (inventory.money >= cost && inventory.AddItem(itemIndex))
            inventory.UpdateMoney(-cost);
    }
}
