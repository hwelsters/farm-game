using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bobber : MonoBehaviour
{
    private bool hasPlopped;
    private float timeUntilFishBite;
    private float elapsedTime = 0;
    [SerializeField] private float minWaitTime;
    [SerializeField] private float maxWaitTime;
    [SerializeField] private float timeBeforeFishEscapes;
    [SerializeField] private float sizeOfInteractableArea;

    private bool fishOnHook = false;
    private int indexOfFishToCatch = -1;

    private Animator animator;
    private ItemTable itemTable;


    private PlayerMovement player;
    private TileAim tileAim;
    private Animator playerAnimator;
    private Animator toolAnimator;

    private bool inWater;

    public float finalYDistance;
    public float finalXDistance;

    void Start()
    {
        playerAnimator = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
        toolAnimator = GameObject.FindGameObjectWithTag("ToolAnimator").GetComponent<Animator>();

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        tileAim = GameObject.FindGameObjectWithTag("TileAim").GetComponent<TileAim>();

        animator = GetComponent<Animator>();
        itemTable = GameObject.FindGameObjectWithTag("ItemTable").GetComponent<ItemTable>();
        timeUntilFishBite = Random.Range(minWaitTime, maxWaitTime);

        CheckForFishableArea();
    }

    void CheckForFishableArea ()
    {
        Collider2D[] collider = Physics2D.OverlapCircleAll(transform.position, sizeOfInteractableArea);

        foreach (Collider2D c in collider)
        {
            FishableArea fishableArea = c.GetComponent<FishableArea>();
            if (fishableArea != null)
            {
                animator.SetBool("Bobbing", true);
                indexOfFishToCatch = fishableArea.GetFish();

                inWater = true;

                break;
            }
        }

        if (indexOfFishToCatch == -1)
            ReelItIn();
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= timeUntilFishBite && elapsedTime <= timeUntilFishBite + timeBeforeFishEscapes && inWater)
            fishOnHook = true;

        else
            fishOnHook = false;

        if (elapsedTime > timeUntilFishBite + timeBeforeFishEscapes)
        {
            elapsedTime = 0;
            timeUntilFishBite = Random.Range(minWaitTime, maxWaitTime);
        }

        animator.SetBool("Caught", fishOnHook);

        if (Input.GetMouseButtonDown(0))
            ReelItIn();
    }

    // You're probably wondering why I named it reel it in. You will never know what Amine
    void ReelItIn()
    {
        if (fishOnHook && inWater)
        {
            Pickup item = itemTable.CreateAndReturnItem(transform.position, indexOfFishToCatch).GetComponent<Pickup>();
            
            Vector3 towardsPlayerDirection = new Vector3
            (
                player.transform.position.x - transform.position.x,
                player.transform.position.y - transform.position.y,
                0
            );

            item.globallyAttracted = true;
            item.itemSpeed = 4f;
            item.playerGravity = 1f;
        }


        tileAim.canUseTool = true;
        player.cantMove = false;

        toolAnimator.SetBool("Casted", false);
        playerAnimator.SetBool("Casted", false);

        Destroy(gameObject);


    }
}
