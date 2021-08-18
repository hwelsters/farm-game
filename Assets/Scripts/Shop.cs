using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [Serializable]
    private struct ShopStock
    {
        [SerializeField] public int[] itemsSold;
        [SerializeField] public int[] itemsCost;
    }

    [SerializeField] private ShopStock[] shopStocks;
    [SerializeField] private ShopTable shopTable;

    [SerializeField] private GameObject shopUI;
    private GameObject backPackUI;
    private GameObject backPackWithoutBackground;
    private GameObject backPackBackUI;

    private Vector2 backPackUIOffset = new Vector2(-600,80);
    private Vector3 backPackUIOriginalPosition;

    private TileAim tileAim;
    private PlayerMovement playerMovement;

    public static bool shopIsActive = false;
    private bool playerInRange;



    void Start () 
    { 
        shopUI.SetActive(false);
        tileAim = GameObject.FindGameObjectWithTag("TileAim").GetComponent<TileAim>();
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();

        backPackUI = tileAim.backPackUI;
        backPackWithoutBackground = tileAim.backPackWithoutBackground;
        backPackBackUI = tileAim.backPackBackUI;

        backPackUIOriginalPosition = backPackWithoutBackground.transform.localPosition;
    }
    
    void TurnOnShop()
    {
        shopTable.GenerateShopItems(shopStocks[0].itemsSold, shopStocks[0].itemsCost);

        backPackUI.SetActive(true);
        backPackWithoutBackground.SetActive(true);
        backPackBackUI.SetActive(false);
        backPackWithoutBackground.transform.localPosition = (Vector2)backPackUIOriginalPosition + backPackUIOffset;

        shopUI.SetActive(true);

        playerMovement.cantMove = true;
        tileAim.canUseTool = false;
        tileAim.UIInProgress = true;

        shopIsActive = true;
    }

    void Update ()
    {
        if (playerInRange && Input.GetMouseButtonDown(1))
        {
            TurnOnShop();   
        }

        if (Input.GetKey(KeyCode.E) && shopIsActive)
            TurnOffShop();
    }

    public void TurnOffShop()
    {
        Invoke("RevertValues", 0.01f);

        backPackBackUI.SetActive(true);
        backPackUI.SetActive(false);

        backPackWithoutBackground.transform.localPosition = backPackUIOriginalPosition;

        shopIsActive = false;

        shopUI.SetActive(false);


    }

    void RevertValues()
    {
        tileAim.canUseTool = true;
        playerMovement.cantMove = false;

        tileAim.UIInProgress = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("TileAim")) 
        {
            playerInRange = true;
            CursorManager.SetCursorToHand();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("TileAim")) 
        {
            playerInRange = false;
            CursorManager.SetCursorBackToNormal();
        }
    }
}
