using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTable : MonoBehaviour
{
    [SerializeField] public Sprite[] itemSprite;
    [SerializeField] public string[] itemName;
    [SerializeField] public int[] itemDefaultValues;
    [TextArea] [SerializeField] public string[] itemDescriptions;
    [SerializeField] public int[] itemHealthPoints;
    [SerializeField] public int[] itemEnergyPoints;

    [SerializeField] GameObject baseItem;

    public static ItemTable itemTable;

    void Start()
    {
        if (itemTable == null)
            itemTable = this;
        else if (itemTable != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void CreateItem (Vector3 position, int itemIndex, int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject instantiatedItem = Instantiate(baseItem, position, Quaternion.identity);

            SpriteRenderer spriteRenderer = instantiatedItem.GetComponent<SpriteRenderer>();
            Pickup pickup = instantiatedItem.GetComponent<Pickup>();

            pickup.itemIndex = itemIndex;
            spriteRenderer.sprite = itemSprite[itemIndex];
        }
    }

    public GameObject CreateAndReturnItem (Vector3 position, int itemIndex)
    {
        GameObject instantiatedItem = Instantiate(baseItem, position, Quaternion.identity);

        SpriteRenderer spriteRenderer = instantiatedItem.GetComponent<SpriteRenderer>();
        Pickup pickup = instantiatedItem.GetComponent<Pickup>();

        pickup.itemIndex = itemIndex;
        spriteRenderer.sprite = itemSprite[itemIndex];

        return instantiatedItem;
    }
}
