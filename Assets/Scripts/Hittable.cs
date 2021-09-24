using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Hittable : MonoBehaviour
{
    [Serializable]
    class Item
    {
        public int itemCount;
        public int itemIndex;
        public int itemDropChance;
    }

    private ItemTable itemTable;

    // 1 is tree, 2 is rock
    [SerializeField] public int hittableIndex;

    [SerializeField] private int hp = 0;

    [SerializeField] private Item[] itemsToDrop;

    [SerializeField] public int debrisIndex;

    private float duration = 0.3f;
    private float magnitude = 0.02f;

    [SerializeField] private GameObject deathAnimation;

    void Start ()
    {
        itemTable = GameObject.FindGameObjectWithTag("ItemTable").GetComponent<ItemTable>();
    }

    public void Hit(int damage)
    {
        hp -= damage;
        StartCoroutine(Shake(duration, magnitude));
        
        if (hp <= 0)
        {
            GetDestroyed();
        }
    }

    private IEnumerator Shake (float duration, float magnitude)
    {
        Vector3 originalPosition = transform.position;

        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;

            transform.position = new Vector3(originalPosition.x + x, originalPosition.y, originalPosition.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.position = originalPosition;
    }

    private bool RandomChance(int chance)
    {
        return Random.Range(0, 101) <= chance;
    }

    public void GetDestroyed()
    {
        foreach (Item n in itemsToDrop)
        {
            if (RandomChance(n.itemDropChance))
            {
                itemTable.CreateItem(transform.position, n.itemIndex, n.itemCount);
            }
        }

        if (MineGeneration.inMines)
        {
            MineGeneration mines = GameObject.FindGameObjectWithTag("Mines").GetComponent<MineGeneration>();
            mines.MakeLadderAppear(transform.position);
        }

        if (deathAnimation != null) Instantiate(deathAnimation, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) GetDestroyed();
    }
}
