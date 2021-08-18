using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionRendererSorter : MonoBehaviour
{
    [SerializeField]
    private int sortingOrderBase = 0;
    [SerializeField]
    private int offset = 0;
    private Renderer myRenderer;

    void Awake()
    {
        myRenderer = gameObject.GetComponent<Renderer>();
    }

    void LateUpdate()
    {
        myRenderer.sortingOrder = (int)(sortingOrderBase - transform.position.y - offset);
    }
}
