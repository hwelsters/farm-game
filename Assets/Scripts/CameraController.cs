using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    [SerializeField]
    float leftLimit;

    [SerializeField]
    float rightLimit;

    [SerializeField]
    float topLimit;

    [SerializeField]
    float bottomLimit;

    private Transform player;

    public float dampTime = 0.4f;

    private Vector3 playerPosition;
    private Vector3 velocity = Vector3.zero;

    public static CameraController instance;

    void Start()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        player = GameObject.FindGameObjectWithTag("Player").transform;
        DontDestroyOnLoad(gameObject);
    }

    public void ChangeCameraPosition(Vector3 newPosition)
    {
        newPosition = new Vector3
        (
            Mathf.Clamp(newPosition.x, leftLimit, rightLimit),
            Mathf.Clamp(newPosition.y, bottomLimit, topLimit),
            transform.position.z
        );
        
        transform.position = newPosition;
        FixedUpdate();
    }

    void FixedUpdate()
    {
        playerPosition = new Vector3(player.position.x, player.position.y, -10f);
        transform.position = Vector3.SmoothDamp(transform.position, playerPosition, ref velocity, dampTime);

        transform.position = new Vector3
        (
            Mathf.Clamp(transform.position.x, leftLimit, rightLimit), 
            Mathf.Clamp(transform.position.y, bottomLimit, topLimit),
            transform.position.z
        );
    }

    public void ChangeScreenLimit(float leftLimit, float rightLimit, float bottomLimit, float topLimit)
    {
        this.leftLimit = leftLimit;
        this.rightLimit = rightLimit;
        this.bottomLimit = bottomLimit;
        this.topLimit = topLimit;
    }
}
