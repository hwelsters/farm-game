using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanDropLoot : MonoBehaviour
{
    [Serializable]
    class Item
    {
        public int itemIndex;
        public int itemCount;
    }

    private ItemTable itemTable;
    [SerializeField] private Item[] itemsToDrop;
    [SerializeField] private bool destroyWhenClicked;

    private bool playerInRange;

    void Start()
    {
        itemTable = GameObject.FindGameObjectWithTag("ItemTable").GetComponent<ItemTable>();
    }

    void OnTriggerEnter2D(Collider2D other) {if (other.CompareTag("Player")) playerInRange = true;}

    void OnTriggerExit2D(Collider2D other) { if (other.CompareTag("Player")) playerInRange = false; }

    void OnMouseExit()
    {
        CursorManager.SetCursorBackToNormal();
    }

    void OnMouseOver()
    {
        CursorManager.SetCursorToHand();
        if (Input.GetMouseButtonDown(1) && playerInRange)
        {
            DropLoot();

            if (destroyWhenClicked)
            {
                Destroy(gameObject);
                CursorManager.SetCursorBackToNormal();
            }
        }
    }

    private void DropLoot()
    {
        foreach (Item n in itemsToDrop)
            itemTable.CreateItem(transform.position, n.itemIndex, n.itemCount);
    }
}
