using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public GameObject[] dust;
    private int dustTimer;

    public static PlayerMovement instance = null;

    public float moveSpeed = 0.1f;

    public int luck;

    public Rigidbody2D rb;
    private Animator playerAnimator;
    public Animator toolAnimator;

    public Vector2 movement;

    public float xDirection;
    public float yDirection;

    public bool cantMove = false;

    public float sqrItemAttractionRadius = 5f;

    public GameObject farmManager;

    void Awake()
    {
        //Destroy extra players
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

        toolAnimator = GameObject.FindGameObjectWithTag("ToolAnimator").GetComponent<Animator>();
        playerAnimator = GetComponent<Animator>();

        farmManager = GameObject.FindGameObjectWithTag("FarmManager");
    }


    void Update()
    {
        //toolAnimator.SetFloat("Horizontal", xDirection);
        //toolAnimator.SetFloat("Vertical", yDirection);

        playerAnimator.SetFloat("Horizontal", xDirection);
        playerAnimator.SetFloat("Vertical", yDirection);

        toolAnimator.SetFloat("Horizontal", xDirection);
        toolAnimator.SetFloat("Vertical", yDirection);

        playerAnimator.SetFloat("Speed", movement.sqrMagnitude);

        if (cantMove)
        {
            movement = new Vector2(0, 0);
            return;
        }

        //Player Input
        movement.x = Mathf.Ceil( Input.GetAxisRaw("Horizontal") );
        movement.y = Mathf.Ceil( Input.GetAxisRaw("Vertical") );

        //Ensure Player moves at consistent speed
        movement = Vector2.ClampMagnitude(movement, 1);

        movement *= moveSpeed;

        if (movement.sqrMagnitude > 0)
        {
            //Control Animations
            xDirection = movement.x;
            yDirection = movement.y;


            //Create Dust
            dustTimer++;

            if (dustTimer >= 50)
            {
                int dustIndex = Random.Range(0, dust.Length - 1);
                Instantiate(dust[dustIndex], transform.position, Quaternion.identity);
                dustTimer = 0;
            }

        }

        else
            dustTimer = 100;
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * Time.deltaTime);
    }

    public void RenderFarm(bool inFarm)
    {
        farmManager.SetActive(inFarm);
    }

    public void OnDialogStart()
    {
        cantMove = true;
    }

    public void OnDialogEnd()
    {
        cantMove = false;
    }
}
