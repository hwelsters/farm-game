using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Backpack : MonoBehaviour
{
    public static Backpack instance = null;
    private Inventory inventory;

    [SerializeField] private GameObject[] pages;
    [SerializeField] private GameObject backpackPage;
    [SerializeField] private GameObject backpackBack;

    [SerializeField] private Vector2 backpackNewPosition;
    [SerializeField] private Vector2 backpackBackNewPosition;

    private Vector2 backpackOriginalPosition;
    private Vector2 backpackBackOriginalPosition;


    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        backpackOriginalPosition = backpackPage.transform.localPosition;
        backpackBackOriginalPosition = backpackBack.transform.localPosition;

        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
    }

    void OnDisable()
    {
        inventory.DropCursorItem();
        inventory.itemDescription.SetActive(false);
    }

    public void ChangePage(int screen)
    {
        for (int i = 0; i < pages.Length; i++)
            pages[i].SetActive(false);

        if (screen == 0)
        {
            backpackBack.transform.localPosition = backpackBackOriginalPosition;
            backpackPage.transform.localPosition = backpackOriginalPosition;
        }
        else
        {
            backpackPage.SetActive(true);
            backpackBack.transform.localPosition = backpackBackNewPosition;
            backpackPage.transform.localPosition = backpackNewPosition;
        }

        pages[(int)screen].SetActive(true);
    }
}
