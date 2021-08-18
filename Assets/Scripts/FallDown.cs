using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallDown : MonoBehaviour
{
    public IEnumerator Fall()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        int direction = player.transform.position.x - transform.position.x > 0? -1 : 1;
        
        float velocity = 0;
        float gravity = 1 * direction;

        float rotation = 0;

        while (transform.eulerAngles.z < 90 && transform.eulerAngles.z > -90)
        {
            velocity += gravity;
            rotation += velocity;
            transform.localEulerAngles = new Vector3(0,0,rotation);
            yield return null;
        }

        Destroy(gameObject);
    }
}
