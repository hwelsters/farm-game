using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class TileAim : MonoBehaviour
{
    class Item
    {
        public int itemIndex;
        public int itemCount;

        public Item(int itemIndex, int itemCount)
        {
            this.itemIndex = itemIndex;
            this.itemCount = itemCount;
        }
    }

    public RuleTile hoedSoil;
    public RuleTile wateredSoil;
    public Tilemap plowedSoil;
    public Tilemap plowable;
    public Tilemap wateredTilemap;

    public static List<Vector3Int> wateredSoilPositions = new List<Vector3Int>();

    private PlayerMovement player;
    private Inventory inventory;
    private Animator playerAnimator;
    private Animator toolAnimator;

    private int selectedGameObjectIndex;
    private float x, y;

    [SerializeField]
    private float xOffset;

    [SerializeField]
    private float yOffset;

    private Vector2 mousePosition;
    private Vector2 mouseWorldPosition;
    private int mouseXPosition;
    private int mouseYPosition;

    public int selectedSlotNumber = 0;
    public int itemIndexInSelectedSlot;

    public GameObject selectedSlotUI;

    private int axeDamage = 2;
    private int pickaxeDamage = 2;
    private float sizeOfInteractableArea = 0.1f;

    public GameObject[] plants;
    private int[][] plantGrowthSeasons;

    public static TileAim instance;

    [SerializeField] private Transform farmManager;

    private int fullWaterAmount = 20;
    private int waterAmount = 20;

    public bool canUseTool = true;

    public bool wasExhausted = false;

    [SerializeField] public int maxEnergy;
    [SerializeField] private Slider energySlider;
    private int energy;


    [SerializeField] private int energyCostOfAxe;
    [SerializeField] private int energyCostOfPickaxe;
    [SerializeField] private int energyCostOfFishingRod;
    [SerializeField] private int energyCostOfWateringCan;
    [SerializeField] private int energyCostOfHoe;


    private GameObject energyBar;

    private ItemTable itemTable;
    private int[] itemsSold;

    [SerializeField] public GameObject backPackUI;
    [SerializeField] private GameObject inventoryBarUI;
    [SerializeField] public GameObject backPackWithoutBackground;
    [SerializeField] public GameObject backPackBackUI;
    [SerializeField] private GameObject[] backpackPages;

    private bool backPackIsVisible = false;

    public bool UIInProgress;

    [SerializeField] public float maxFishingDistance;
    [SerializeField] private float fishingDistanceIncreaseRate;
    [SerializeField] private GameObject bobber;

    [SerializeField] private GameObject fishMarker;

    [SerializeField] private GameObject[] buildings;

    public bool[] talkedToNPCs = new bool[60];
    public bool[] giftedNPCToday = new bool[60];
    public int[] giftedNPCForEntireWeek = new int[60];

    public int[] NPCRelationshipPoints = new int[60];
    public bool[] NPCIntroduced = new bool[60];

    [HideInInspector] public List<Vector3Int> plowedSoilPositions = new List<Vector3Int>();

    [SerializeField] private Slider fishingSlider;
    [SerializeField] private Vector3 fishingSliderOffset;

    void Start()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
        playerAnimator = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
        toolAnimator = GameObject.FindGameObjectWithTag("ToolAnimator").GetComponent<Animator>();
        itemTable = GameObject.FindGameObjectWithTag("ItemTable").GetComponent<ItemTable>();

        itemsSold = new int[itemTable.itemDefaultValues.Length];

        energy = maxEnergy;

        backPackUI.SetActive(false);

        UpdateEnergy(0);

        fishingSlider.gameObject.SetActive(false);
    }

    void Update()
    {
        //Move this gameobject relative to the mouse and player's position
        mousePosition = Input.mousePosition;

        //Change mouse position to world position
        mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

        mouseWorldPosition.x = Mathf.Clamp(mouseWorldPosition.x, player.transform.position.x - 1, player.transform.position.x + 1);
        mouseWorldPosition.y = Mathf.Clamp(mouseWorldPosition.y, player.transform.position.y - 1, player.transform.position.y + 1);

        x = Mathf.Round(mouseWorldPosition.x) + xOffset;
        y = Mathf.Round(mouseWorldPosition.y) + yOffset;


        transform.position = new Vector2(x, y);

        selectedSlotNumber = GetSelectedSlotNumber();
        itemIndexInSelectedSlot = inventory.slottedItem[selectedSlotNumber];

        selectedSlotUI.transform.position = inventory.hotbarSlotImage[selectedSlotNumber].transform.position;

        if (Input.GetKeyDown(KeyCode.E) && !UIInProgress)
        {
            ChangeBackpackVisibility();
            
            if (!backPackIsVisible)
                inventory.DropCursorItem();
        }


        //Use Item In Inventory Slot
        if (Input.GetMouseButton(0) && canUseTool)
        {
            //MakePlayerFaceTileAim();
            ActivateTool();
        }

        if (Input.GetMouseButtonDown(0) && itemIndexInSelectedSlot == 5) 
        {
            StartCoroutine(Fish());
        }

        if (Input.GetMouseButton(1))
        {
            Harvest();

            if (itemIndexInSelectedSlot == 4)
                RefillWateringCan();

            ShipItem(selectedSlotNumber);

            PutItemIntoBuilding();

            //MakePlayerFaceTileAim();
        }
    }

    private IEnumerator Fish()
    {
        fishingSlider.gameObject.SetActive(true);
        playerAnimator.SetBool("Casting", true);
        toolAnimator.SetBool("Casting", true);
        canUseTool = false;

        player.cantMove = true;
        float fishingDistance = 0;
        float percentageOfDistance = 0;

        /*float facingX = cursorPositionInWorld.x - Mathf.Round(player.transform.position.x);
        float facingY = cursorPositionInWorld.y - Mathf.Round(player.transform.position.y);
        Vector3 fishingDirection = new Vector3
        (
            facingX,
            facingY,
            0
        );*/

        float facingX = player.yDirection != 0? 0 : player.xDirection;
        float facingY = player.yDirection;

        Vector3 fishingDirection = new Vector3
        (
            facingX,
            facingY,
            0
        );

        fishingDirection = Vector3.ClampMagnitude(fishingDirection, 1);
        
        Vector3 bobberPosition = fishingDirection * fishingDistance + player.transform.position;

        int multiplier = 1;

        GameObject posIndicator = Instantiate(fishMarker, bobberPosition, Quaternion.identity);
        while (!Input.GetMouseButtonUp(0))
        {
            percentageOfDistance += fishingDistanceIncreaseRate * multiplier;

            if (percentageOfDistance >= 1 || percentageOfDistance <= 0) multiplier *= -1;

            percentageOfDistance = Mathf.Clamp(percentageOfDistance, 0, 1f);
            fishingDistance = maxFishingDistance * percentageOfDistance;
            bobberPosition = fishingDirection * fishingDistance + player.transform.position;
            
            posIndicator.transform.position = bobberPosition;
        
            ManageFishingBar(percentageOfDistance);
            

            yield return null;
        }

        Destroy(posIndicator);

        UpdateEnergy(-energyCostOfFishingRod);

        toolAnimator.SetBool("Casted", true);
        playerAnimator.SetBool("Casted", true);

        toolAnimator.SetBool("Casting", false);
        playerAnimator.SetBool("Casting", false);

        float elapsedTime = 0f;

        while (elapsedTime < 0.2f)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Instantiate(bobber, bobberPosition, Quaternion.identity);
        fishingSlider.gameObject.SetActive(false);
    }

    public void ShipItem(int slotIndex)
    {
        Collider2D[] collider = Physics2D.OverlapCircleAll(transform.position, sizeOfInteractableArea);

        foreach (Collider2D c in collider)
        {
            if (c.CompareTag("ItemBin") && inventory.slottedItem[slotIndex] > 100)
            {
                itemsSold[inventory.slottedItem[slotIndex]] = inventory.count[slotIndex];
                inventory.RemoveItemFromSlot(slotIndex, inventory.count[slotIndex]);
            }
        }
    }

    public int CalculateEarnings()
    {
        int total = 0;
        for (int i = 0; i < itemsSold.Length; i++)
        {
            total += itemsSold[i] * itemTable.itemDefaultValues[i];
            itemsSold[i] = 0;
        }

        inventory.UpdateMoney(total);
        return total;
    }

    void MakePlayerFaceTileAim()
    {
        
        float facingX = x - Mathf.Round(player.transform.position.x);
        float facingY = y - Mathf.Round(player.transform.position.y);

        bool withinX = (int)x == (int) Mathf.Round(mouseWorldPosition.x);
        bool withinY = (int)y == (int) Mathf.Round(mouseWorldPosition.y);
        
        if (withinX && withinY)
        {
            PlayerMovement.instance.toolAnimator.SetFloat("Horizontal", facingX);
            PlayerMovement.instance.toolAnimator.SetFloat("Vertical", facingY);

            player.xDirection = facingX; 
            player.yDirection = facingY; 
        }
    }

    void MakePlayerFacePosition(Vector2 position)
    {
        player.xDirection = position.x - player.transform.position.x;
        player.yDirection = position.y - player.transform.position.y;

        if (player.yDirection > float.Epsilon || player.yDirection < float.Epsilon)
        {
            player.xDirection = 0;
        }
    }

    void ActivateTool()
    {
        switch (itemIndexInSelectedSlot)
        {
            case 1:
                Hoe();
                break;

            case 2:
                Chop();
                break;

            case 3:
                Mine();
                break;

            case 4:
                Water();
                break;

            case int n when (n >= 301 && n <= 400):
                //foreach(int n in plantGrowthSeasons[n - 301])
                //{
                //    if (n == DayCycle.season)
                //    {
                InstantiateObjectFromInventory(301, plants, true);
                //        break;
                //    }
                //}
                break;

            case int n when (n >= 701 && n <= 800):
                InstantiateObjectFromInventory(701, buildings, false);
                break;

            default:
                break;
        }
    }

    void Harvest()
    {
        Collider2D[] collider = Physics2D.OverlapCircleAll(transform.position, sizeOfInteractableArea);
        foreach (Collider2D c in collider)
        {
            CropPlants cropPlants = c.GetComponent<CropPlants>();
            if (cropPlants != null)
            {
                cropPlants.GetHarvested();
                break;
            }
        }
    }

    void RefillWateringCan()
    {
        Collider2D[] collider = Physics2D.OverlapCircleAll(transform.position, sizeOfInteractableArea);
        foreach (Collider2D c in collider)
        {
            if (c != null && c.CompareTag("Well"))
            {
                waterAmount = fullWaterAmount;
                playerAnimator.SetTrigger("Water");
                return;
            }
        }
    }

    void Water()
    {
        MakePlayerFaceTileAim();
        StartCoroutine(UseTool(0));
        playerAnimator.SetTrigger("Water");
        UpdateEnergy(-energyCostOfWateringCan);
    }

    void Chop()
    {
        StartCoroutine(UseTool(axeDamage));
        toolAnimator.SetTrigger("Axe");

        playerAnimator.SetTrigger("Hit");

        UpdateEnergy(-energyCostOfAxe);
    }

    void Mine()
    {
        StartCoroutine(UseTool(pickaxeDamage));
        toolAnimator.SetTrigger("Pickaxe");

        playerAnimator.SetTrigger("Hit");

        UpdateEnergy(-energyCostOfPickaxe);
    }

    IEnumerator UseTool(int damage)
    {
        MakePlayerFaceTileAim();
        Vector3Int toolAimPosition = new Vector3Int((int)transform.position.x, (int)transform.position.y - 1, (int)transform.position.z);
        Vector3 aimPosition = transform.position;
        canUseTool = false;
        player.cantMove = true;
        float waited = 0;

        while (waited < 0.3f)
        {
            waited += Time.deltaTime;
            yield return null;
        }

        Collider2D[] collider = Physics2D.OverlapCircleAll(aimPosition, sizeOfInteractableArea);

        if (itemIndexInSelectedSlot != 4)
        {
            foreach (Collider2D c in collider)
            {
                Hittable hittable = c != null ? c.GetComponent<Hittable>() : null;
                if (hittable != null)
                {
                    if (hittable.hittableIndex == itemIndexInSelectedSlot || hittable.hittableIndex == 0 && itemIndexInSelectedSlot != 4)
                        hittable.Hit(damage);
                    break;
                }
            }
        }

        //Play this if user uses watering can
        else
        {
            if (plowedSoil.GetTile(toolAimPosition) != null)
            {
                WaterTile(toolAimPosition);
            }
        }

        bool empty = true;

        //Play this if user uses hoe
        if (itemIndexInSelectedSlot == 1)
        {
            if (!CheckEmpty(aimPosition + Vector3.down))
                empty = false;


            if (plowable.GetTile(toolAimPosition) != null && empty && plowedSoil.GetTile(toolAimPosition) == null)
            {
                plowedSoil.SetTile(toolAimPosition, hoedSoil);
                plowedSoilPositions.Add(toolAimPosition);
                
                // 2 is the index number for rain
                if (DayCycle.weather == 2)
                {
                    WaterTile(toolAimPosition);
                }

                if (RandomChance(15)) itemTable.CreateItem(transform.position, 104, 1);
            }
        }

        //If User uses pickaxe
        if (itemIndexInSelectedSlot == 3)
        {
            if (empty)
            {
                wateredTilemap.SetTile(toolAimPosition, null);
                plowedSoil.SetTile(toolAimPosition, null);
            }
        }

        while (waited < 0.6f)
        {
            waited += Time.deltaTime;
            yield return null;
        }
        canUseTool = true;
        player.cantMove = false;
    }

    bool RandomChance(int percentChance)
    {
        int randomNumber = Random.Range(0, 100);
        if (randomNumber < percentChance) return true;
        return false;
    }


    bool CheckEmpty(Vector3 position)
    {
        Collider2D[] collider = Physics2D.OverlapCircleAll(position + Vector3.up, sizeOfInteractableArea);

        foreach (Collider2D c in collider)
            if (c.GetComponent<Hittable>() != null || c.GetComponent<CropPlants>())
                return false;

        return true;
    }

    void InstantiateObjectFromInventory(int baseNumber, GameObject[] objectArray, bool checkForSoil)
    {
        if (GameObject.FindGameObjectWithTag("FarmManager") == null)
            return;

        Vector3 instantiatePosition = new Vector3((int)transform.position.x, (int)transform.position.y - 1, (int)transform.position.z);

        if (!CheckEmpty(instantiatePosition))
            return;

        Vector3Int instantiatePositionInt = new Vector3Int((int)instantiatePosition.x, (int)instantiatePosition.y, (int)instantiatePosition.z);

        if (checkForSoil && plowedSoil.GetTile(instantiatePositionInt) == null)
            return;

        Vector3 offset = new Vector3(0f, 1f, 0f);
        inventory.RemoveItemFromSlot(selectedSlotNumber, 1);
        int objectIndex = itemIndexInSelectedSlot - baseNumber;
        GameObject objectToInstantiate = objectArray[objectIndex];
        GameObject instantiatedObject = Instantiate(objectToInstantiate, instantiatePosition + offset, Quaternion.identity);
        instantiatedObject.transform.SetParent(farmManager);
    }

    void Hoe()
    {
        StartCoroutine(UseTool(3));

        playerAnimator.SetTrigger("Hit");
        toolAnimator.SetTrigger("Hoe");

        UpdateEnergy(-energyCostOfHoe);
    }

    int GetSelectedSlotNumber()
    {
        int number = selectedSlotNumber;
        if (Input.GetKeyDown(KeyCode.Alpha1))
            number = 0;

        if (Input.GetKeyDown(KeyCode.Alpha2))
            number = 1;

        if (Input.GetKeyDown(KeyCode.Alpha3))
            number = 2;

        if (Input.GetKeyDown(KeyCode.Alpha4))
            number = 3;

        if (Input.GetKeyDown(KeyCode.Alpha5))
            number = 4;

        return number;
    }

    public void RenewEnergy()
    {
        int nextDayEnergy = (int)(wasExhausted ? maxEnergy * 0.6 : maxEnergy);
        UpdateEnergy(nextDayEnergy);

        wasExhausted = false;

    }

    private void PutItemIntoBuilding()
    {
        Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D[] collider = Physics2D.OverlapCircleAll(cursorPosition, sizeOfInteractableArea);

        foreach (Collider2D c in collider)
        {
            ProcessingBuilding processingBuilding = c.GetComponent<ProcessingBuilding>();
            if (processingBuilding != null)
            {
                processingBuilding.OutputItem();
                if (processingBuilding.InputItem(inventory.slottedItem[selectedSlotNumber], ref inventory.count[selectedSlotNumber]))
                {
                    inventory.UpdateSlot(selectedSlotNumber);
                }

                return;
            }
        }
    }

    public void UpdateEnergy(int changeInEnergy)
    {
        energy += changeInEnergy;

        energy = energy > maxEnergy ? maxEnergy : energy;

        if (energy < 0)
            wasExhausted = true;

        energySlider.maxValue = maxEnergy;

        energySlider.value = energy;
    }

    public void OnDialogStart()
    {
        canUseTool = false;
    }

    public void OnDialogEnd()
    {
        canUseTool = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Plants"))
        {
            CropPlants cropPlants = other.GetComponent<CropPlants>();
            if (cropPlants.CanBeHarvested())
                CursorManager.SetCursorToHand();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Plants")) CursorManager.SetCursorBackToNormal();
    }

    private void ManageFishingBar(float percentageOfDistance)
    {
        fishingSlider.transform.position = Camera.main.WorldToScreenPoint(player.transform.position + fishingSliderOffset);
        fishingSlider.value = percentageOfDistance;
    }

    private void ChangeBackpackVisibility()
    {
        backPackIsVisible = !backPackIsVisible;

        if (backPackIsVisible)
        {
            MakeBackpackVisible();
        }
        else
        {
            MakeBackpackInvisible();
        }
    }

    private void MakeBackpackInvisible()
    {
        foreach (GameObject page in backpackPages)
        {
            page.SetActive(false);
        }

        backpackPages[0].SetActive(true);
        canUseTool = true;

        backPackUI.SetActive(false);
    }

    private void MakeBackpackVisible()
    {
        backPackUI.SetActive(true);
        canUseTool = false;
    }

    public void WaterTile(Vector3Int position)
    {
        wateredTilemap.SetTile(position, wateredSoil);
        wateredSoilPositions.Add(position);
    }
}
