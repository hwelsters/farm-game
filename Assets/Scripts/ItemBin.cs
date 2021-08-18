using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBin : MonoBehaviour
{
    private Animator animator;

    void Start ()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("PlayerInRange", false);
    }

    void OnTriggerEnter2D() { animator.SetBool("PlayerInRange", true); }

    void OnTriggerExit2D() { animator.SetBool("PlayerInRange", false); }
}
