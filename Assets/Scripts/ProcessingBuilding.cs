using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcessingBuilding : MonoBehaviour
{
    [Serializable]
    struct Recipe
    {
        [Header("Input items")]
        public int inputItemIndex;
        public int inputAmount;

        [Header("Output items")]
        public int outputItemIndex;
        public int outputAmount; 
        public int waitTime;
    }

    [SerializeField] private Recipe[] recipes;

    private int processingItemIndex;

    private Inventory inventory;
    private ItemTable itemTable;

    private Animator animator;

    private bool processed = false;
    private bool processing = false;

    [SerializeField] private SpriteRenderer speechBubbleItemImage;

    private bool playerInRange = false;

    private void Start()
    {
        itemTable = GameObject.FindGameObjectWithTag("ItemTable").GetComponent<ItemTable>();
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();

        animator = GetComponent<Animator>();

        if (recipes.Length == 0)
            StartCoroutine(ProcessItem());

        processingItemIndex = 0;
    }

    public bool InputItem(int indexOfInsertedItem, ref int itemCount)
    {
        processingItemIndex = 0;

        if (!processing)
        {
            for (processingItemIndex = 0; processingItemIndex < recipes.Length; processingItemIndex++)
            {
                if (indexOfInsertedItem == recipes[processingItemIndex].inputItemIndex && itemCount >= recipes[processingItemIndex].inputAmount)
                {
                    itemCount -= recipes[processingItemIndex].inputAmount;
                    StartCoroutine(ProcessItem());
                    return true;
                }
            }
        }

        return false;
    }

    private IEnumerator ProcessItem ()
    {
        processing = true;
        animator.SetBool("Processing", true);

        yield return new WaitForSecondsRealtime(recipes[processingItemIndex].waitTime);

        animator.SetBool("Processed", true);
        animator.SetBool("Processing", false);
        
        processed = true;

        speechBubbleItemImage.sprite = itemTable.itemSprite[recipes[processingItemIndex].outputItemIndex];
        processing = false;
    }

    private void OnMouseOver()
    {
        if (processed)
            CursorManager.SetCursorToPlus();
        else
            CursorManager.SetCursorBackToNormal();
    }

    void OnMouseExit()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1) && playerInRange)
        {
            OutputItem();
            return;
        }
    }

    public bool OutputItem()
    {
        if (processed)
        {
            processed = false;
            animator.SetBool("Processed", false);
            for (int i = 0; i < recipes[processingItemIndex].outputAmount; i++)
            {
                itemTable.CreateItem(transform.position + new Vector3(0f, 0.5f, 0f), recipes[processingItemIndex].outputItemIndex, recipes[processingItemIndex].outputAmount);
            }

            if (recipes.Length == 0)
                StartCoroutine(ProcessItem());

            return true;
        }

        return false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("TileAim"))
            playerInRange = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("TileAim"))
            playerInRange = false;
    }
}
