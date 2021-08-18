using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public static Inventory instance = null;

    public int money;
    public int[] count;
    public int[] slottedItem;

    public Text[] hotbarSlotCount;
    public Text[] slotCount;

    public GameObject itemDescription;
    public Text itemNameText;
    public Text itemDescriptionText;

    [SerializeField] public Image[] slotImage;

    [SerializeField] public Image[] hotbarSlotImage;

    [SerializeField] private Image cursorImage;
    [SerializeField] private Text cursorText;

    [SerializeField] private ItemTable itemTable;
    [HideInInspector] public int cursorItemCount = 0;
    [HideInInspector] public int cursorSlottedItems = 0;

    [Serializable]
    struct Recipe
    {
        public string name;
        public int[] inputItemIndex;
        public int[] inputItemCount;

        public int[] outputItemIndex;
        public int[] outputItemCount;
    }

    [SerializeField] private Recipe[] craftingRecipes;

    void Start()
    {
        itemDescription.SetActive(false);

        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        itemTable = GameObject.FindGameObjectWithTag("ItemTable").GetComponent<ItemTable>();
    }

    public void GetClicked(int slotIndex)
    {
        int maxItemCount = slottedItem[slotIndex] > 100 ? 99 : 1;

        if (cursorSlottedItems == slottedItem[slotIndex])
        {
            int extraSpace = maxItemCount - count[slotIndex];
            int amountToAdd = cursorItemCount > extraSpace? extraSpace: cursorItemCount;

            count[slotIndex] += amountToAdd;
            cursorItemCount -= amountToAdd;
        }

        else
        {
            int tempCursorItemCount = cursorItemCount;
            int tempCursorSlottedItems = cursorSlottedItems;

            cursorItemCount = count[slotIndex];
            cursorSlottedItems = slottedItem[slotIndex];

            count[slotIndex] = tempCursorItemCount;
            slottedItem[slotIndex] = tempCursorSlottedItems;
        }
        

        UpdateSlot(slotIndex);
        UpdateCursor();

        ManageItemDescription(slottedItem[slotIndex]);
    }

    public void DropCursorItem()
    {
        itemTable.CreateItem(transform.position, cursorSlottedItems, cursorItemCount);
        cursorItemCount = 0;
        cursorSlottedItems = 0;
        UpdateCursor();
    }

    public void UpdateCursor()
    {
        cursorImage.enabled = cursorItemCount > 0;
        cursorSlottedItems = cursorItemCount <= 0 ? 0 : cursorSlottedItems;
        cursorImage.sprite = itemTable.itemSprite[cursorSlottedItems];
        cursorText.enabled = cursorItemCount > 1? true: false;
        cursorText.text = cursorItemCount.ToString();
    }

    public void EmptyCursor()
    {
        cursorSlottedItems = 0;
        cursorItemCount = 0;

        UpdateCursor();
    }
    public void UpdateSlot(int selectedSlotNumber)
    {
        //If the slot is empty...
        if (count[selectedSlotNumber] <= 0)
        {
            if (selectedSlotNumber < hotbarSlotImage.Length)
            {
                hotbarSlotCount[selectedSlotNumber].text = count[selectedSlotNumber].ToString();
                hotbarSlotCount[selectedSlotNumber].enabled = false;
                hotbarSlotImage[selectedSlotNumber].enabled = false;
            }

            slottedItem[selectedSlotNumber] = 0;
            count[selectedSlotNumber] = 0;
            slotCount[selectedSlotNumber].text = "";
            slotImage[selectedSlotNumber].enabled = false;
        }

        //If the slot is not empty...
        else
        {
            string countString = count[selectedSlotNumber] > 1? count[selectedSlotNumber].ToString(): "";
            Sprite itemSprite = itemTable.itemSprite[slottedItem[selectedSlotNumber]];

            if (selectedSlotNumber < hotbarSlotImage.Length)
            {
                hotbarSlotImage[selectedSlotNumber].sprite = itemSprite;
                hotbarSlotCount[selectedSlotNumber].text = countString;


                hotbarSlotImage[selectedSlotNumber].enabled = true;
                hotbarSlotCount[selectedSlotNumber].enabled = true;
            }

            slotCount[selectedSlotNumber].text = countString;
            slotImage[selectedSlotNumber].sprite = itemSprite;

            slotCount[selectedSlotNumber].enabled = true;
            slotImage[selectedSlotNumber].enabled = true;
        }

    }

    public bool AddItem(int itemIndex)
    {
        int maxItemCount = FindMaxItemCount(itemIndex);

        for (int i = 0; i < slotImage.Length; i++)
        {
            if (slottedItem[i] == 0 || slottedItem[i] == itemIndex && count[i] < maxItemCount)
            {
                slottedItem[i] = itemIndex;
                count[i]++;
                UpdateSlot(i);
                return true;
            }
        }
        return false;
    }


    //Will be used for drag and drop inventory
    public void RemoveItemFromSlot(int selectedSlotNumber, int amount)
    {
        count[selectedSlotNumber] -= amount;
        UpdateSlot(selectedSlotNumber);
    }

    public void DropItem(int selectedSlotNumber, int numberOfItemsToDrop)
    {
        count[selectedSlotNumber]--;
        itemTable.CreateItem(transform.position, slottedItem[selectedSlotNumber], numberOfItemsToDrop);
        UpdateSlot(selectedSlotNumber);
    }

    public void UpdateMoney(int amount)
    {
        GameObject[] moneyDisplay = GameObject.FindGameObjectsWithTag("MoneyDisplay");
        money += amount;
        foreach (GameObject t in moneyDisplay)
        {
            Text moneyDisplayText = t.GetComponent<Text>();
            moneyDisplayText.text = money.ToString();
        }
    }

    public void Craft(int craftingIndex)
    {
        int[] inputItemIndexToCraft = (int[]) craftingRecipes[craftingIndex].inputItemIndex.Clone();
        int[] inputItemCountToCraft = (int[])craftingRecipes[craftingIndex].inputItemCount.Clone();

        List<int> amountToRemoveToRemember = new List<int>();
        List<int> slotNumberToRemember = new List<int>();

        for (int i = 0; i < inputItemIndexToCraft.Length; i++)
        {
            for (int j = 0; j < slottedItem.Length; j++)
            {
                if (inputItemIndexToCraft[i] == slottedItem[j])
                {
                    int amountToRemove = count[j] >= inputItemCountToCraft[i] ? inputItemCountToCraft[i] : count[j];

                    inputItemCountToCraft[i] -= amountToRemove;

                    slotNumberToRemember.Add(j);
                    amountToRemoveToRemember.Add(amountToRemove);
                }
            }
        }

        foreach (int i in inputItemCountToCraft)
        {
            if (i > 0)
                return;
        }

        for(int i = 0; i < amountToRemoveToRemember.Count; i++)
        {
            count[slotNumberToRemember[i]] -= amountToRemoveToRemember[i];
            UpdateSlot(slotNumberToRemember[i]);
        }

        for (int i = 0; i < craftingRecipes[craftingIndex].outputItemIndex.Length; i++)
        {
            for (int j = 0; j < craftingRecipes[craftingIndex].outputItemCount[i]; j++)
                if (!AddItem(craftingRecipes[craftingIndex].outputItemIndex[i]))
                    itemTable.CreateItem(transform.position, craftingRecipes[craftingIndex].outputItemIndex[i], 1);
        }
    }

    //Attempts to remove item from inventory, will seek out item and return a bool
    public bool RemoveItem()
    {
        return false;
    }

    public int FindMaxItemCount(int itemIndex)
    {
        if (itemIndex <= 100)
            return 1;

        return 99;
    }

    private void UpdateAllSlots()
    {
        for(int i = 0; i < slottedItem.Length; i++)
        {
            UpdateSlot(i);
        }
    }

    public void UpdateItemDescription(int itemIndex)
    {
        itemNameText.text = itemTable.itemName[itemIndex];
        itemDescriptionText.text = itemTable.itemDescriptions[itemIndex];
    }

    public void ManageItemDescription(int itemIndex)
    {
        if (itemIndex != 0)
        {
            itemDescription.SetActive(true);
        }

        else
        {
            itemDescription.SetActive(false);
        }

        UpdateItemDescription(itemIndex);
    }

    public void DisplayItemDescriptions(int slotIndex)
    {
        ManageItemDescription(slottedItem[slotIndex]);
    }
}
