using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpandWhenPointerIsOver : MonoBehaviour
{
    private float newSize = 0.78f;
    private float originalSize = 0.75f;
    private float growthSpeed = 0.001f;

    private Coroutine expandCoroutine = null;
    private Coroutine shrinkCoroutine = null;
    private Coroutine ploopCoroutine = null;

    private Inventory inventory;

    void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
    }

    public void Expand()
    {
        expandCoroutine = StartCoroutine(SmoothExpand());
    }

    public void Shrink()
    {
        inventory.itemDescription.SetActive(false);
        shrinkCoroutine = StartCoroutine(SmoothShrink());
    }

    private IEnumerator SmoothExpand()
    {
        if (shrinkCoroutine != null)
            StopCoroutine(shrinkCoroutine);

        transform.localScale = new Vector3(originalSize, originalSize, 1);

        while(transform.localScale.x <= newSize)
        {
            transform.localScale += new Vector3(growthSpeed, growthSpeed, 0);

            yield return null;
        }

        while(true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (ploopCoroutine != null)
                    StopCoroutine(ploopCoroutine);

                ploopCoroutine = StartCoroutine(Ploop()); 
            }

            yield return null;
        }
    }

    private IEnumerator SmoothShrink()
    {
        if (expandCoroutine != null)
            StopCoroutine(expandCoroutine);

        transform.localScale = new Vector3(newSize, newSize, 1);

        while (transform.localScale.x >= 0.75f)
        {
            transform.localScale -= new Vector3(growthSpeed, growthSpeed, 0);

            yield return null;
        }
    }


    private IEnumerator Ploop()
    {
        transform.localScale = new Vector3(newSize, newSize, 1);

        float prePloopSize = transform.localScale.x; 
        float ploopSize = transform.localScale.x * 1.1f;
        float ploopSpeed = 0.005f;

        Vector3 ploopVector = new Vector3(ploopSpeed, ploopSpeed, 0);

        while (transform.localScale.x <= ploopSize)
        {
            transform.localScale += ploopVector;

            yield return null;
        }

        while (transform.localScale.x >= prePloopSize)
        {
            transform.localScale -= ploopVector;

            yield return null;
        }
    }
}
