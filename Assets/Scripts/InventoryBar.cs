using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryBar : MonoBehaviour
{
    public static InventoryBar instance;

    void Start()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }
}
