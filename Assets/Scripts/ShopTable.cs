using System.Collections;
using UnityEngine;
using System;

public class ShopTable : MonoBehaviour
{
    [SerializeField] private GameObject baseShopBar;
    [SerializeField] private Transform contentTransform;

    void Start() { }

    public virtual void GenerateShopItems(in int[] itemsSold, in int[] itemsCost)
    {
        DeletePreviousItems();
        int positionIndex = 0;
        for (positionIndex = 0; positionIndex < itemsSold.Length; positionIndex++)
        {
            CreateShopBar(itemsSold[positionIndex], itemsCost[positionIndex], positionIndex);
        }
    }

    private void CreateShopBar(int itemIndex, int itemCost, int positionIndex)
    {
        GameObject instantiatedShopBar = Instantiate(baseShopBar, transform.position + new Vector3(0, 169 - positionIndex * 117, 0), Quaternion.identity);
        Buy buy = instantiatedShopBar.GetComponent<Buy>();
        buy.itemIndex = itemIndex;
        buy.cost = itemCost;

        instantiatedShopBar.transform.SetParent(contentTransform);
    }

    public virtual void PickShopStock()
    {

    }

    private void DeletePreviousItems()
    {
        foreach (Transform stock in contentTransform)
        {
            Destroy(stock.gameObject);
        }
    }
}
