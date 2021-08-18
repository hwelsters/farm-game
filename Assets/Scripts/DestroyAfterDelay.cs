using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterDelay : MonoBehaviour
{
    [SerializeField] private float seconds;

    void Start()
    {
        Invoke("DestroyObject", seconds);
    }

    void DestroyObject()
    { 
        Destroy(gameObject);
    }
}
