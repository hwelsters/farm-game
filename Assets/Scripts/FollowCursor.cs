using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FollowCursor : MonoBehaviour
{
    void Start()
    {
        GetComponent<Image>().enabled = false;
    }

    void Update()
    {
        transform.position = Input.mousePosition;       
    }
}
