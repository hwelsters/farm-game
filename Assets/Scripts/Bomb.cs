using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private float radiusOfExplosion;
    [SerializeField] private float secondsBeforeExploding;
    [SerializeField] private int damageDealt;

    void Start()
    {
        Invoke("Explode", secondsBeforeExploding);
    }

    void Explode()
    {
        Collider2D[] collider = Physics2D.OverlapCircleAll(transform.position, radiusOfExplosion);

        foreach (Collider2D c in collider)
            if (c.CompareTag("Debris"))
                c.GetComponent<Hittable>().Hit(damageDealt);
            

        Destroy(gameObject);
    }
}
