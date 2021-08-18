using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pickup : MonoBehaviour
{
    private Inventory inventory;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;

    public int itemIndex;
    public string itemName;

    private Transform player;
    private PlayerMovement playerStats;
    public float itemSpeed = 1f;

    private float maxXVelocity = 0.03f;
    private float maxYVelocity = 0.08f;
    private float xVelocity;
    private float yVelocity;

    public float overrideXVelocity = -999;
    public float overrideYVelocity = -999;

    private float gravity = 0.004f;
    private bool hasPlopped = false;

    public bool globallyAttracted = false;
    public float playerGravity = 0f;

    private void Start()
    {
        if (itemIndex == 0)
            Destroy(gameObject);

        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();

        player = GameObject.FindGameObjectWithTag("Player").transform;

        spriteRenderer = GetComponent<SpriteRenderer>();

        xVelocity = Random.Range(-maxXVelocity, maxXVelocity);
        yVelocity = maxYVelocity;

        boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.enabled = false;
        StartCoroutine(Plop());
    }


    void FixedUpdate()
    {
        //Move towards the player
        float sqrDistanceFromPlayer = (transform.position - player.position).sqrMagnitude;

        if (sqrDistanceFromPlayer < playerStats.sqrItemAttractionRadius && hasPlopped || globallyAttracted)
        {
            Vector3 newPosition = Vector3.MoveTowards(transform.position, player.position, itemSpeed * Time.deltaTime);
            transform.position = newPosition;
        }


        itemSpeed += playerGravity;
    }


    //This coroutine will make the item go plop plop
    public IEnumerator Plop()
    {
        int originalSortingOrder = spriteRenderer.sortingOrder;

        Vector3 originalPosition = transform.position;

        float itemHeight = 0;
        float xDistance = 0;
        float finalYDistance = Random.Range(-0.1f, 0f);
        bool ploppedHeightReached = false;
        while (itemHeight >= finalYDistance || !ploppedHeightReached)
        {
            transform.position = new Vector3(originalPosition.x + xDistance, originalPosition.y + itemHeight, originalPosition.z);
            xDistance += xVelocity;
            itemHeight += yVelocity;
            yVelocity -= gravity;

            ploppedHeightReached = yVelocity <= 0 ? true : false;

            yield return null;
        }

        hasPlopped = true;

        boxCollider.enabled = true;

        spriteRenderer.sortingLayerName = "Wall";

        spriteRenderer.sortingOrder = originalSortingOrder;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //If the player has space in his inventory, destroy this gameobject after updating inventory slot
        if (other.CompareTag("Player"))
        {
            playerGravity = 0f;
            itemSpeed = 1f;
            if (hasPlopped && inventory.AddItem(itemIndex))
            {
                Destroy(gameObject);
                return;
            }
        }
    }

    void FallIntoPond()
    {
        Destroy(gameObject);
    }
}
