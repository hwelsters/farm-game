using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    [SerializeField] private float newXPosition;
    [SerializeField] private float newYPosition;
    [SerializeField] private float newLeftLimit;
    [SerializeField] private float newRightLimit;
    [SerializeField] private float newTopLimit;
    [SerializeField] private float newBottonLimit;

    [SerializeField] private bool indoors;
    [SerializeField] private bool toMines;

    private Animator fadeAnimator;

    private GameObject mainCamera;
    private CameraController cameraController;

    [SerializeField] private bool clickToTeleport;

    private bool playerInRange;
    private PlayerMovement playerMovement;

    private bool teleporting = false;

    void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        cameraController = mainCamera.GetComponent<CameraController>();

        fadeAnimator = GameObject.FindGameObjectWithTag("Fade").GetComponent<Animator>();

        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(Transition(other));
        }

        if (other.CompareTag("TileAim") && clickToTeleport)
        {
            playerInRange = true;
            CursorManager.SetCursorToHand();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("TileAim"))
            playerInRange = false;

        if (other.CompareTag("TileAim") && clickToTeleport)
        {
            playerInRange = false;
            CursorManager.SetCursorBackToNormal();
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1) && clickToTeleport && playerInRange && !teleporting)
            StartCoroutine(Transition(GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>()));
    }

    IEnumerator Transition(Collider2D other)
    {
        teleporting = true;
        playerMovement.cantMove = true;

        fadeAnimator.SetTrigger("Leaving");
        yield return new WaitForSeconds(0.6f);

        ChangeLocation(other);
        
        teleporting = false;
        playerMovement.cantMove = false;
    }

    void ChangeLocation(Collider2D other)
    {
        Weather.instance.StopRaining();
        if (!indoors && DayCycle.weather == 2)
        {
            Weather.instance.StartRaining();
        }

        Vector3 newPosition = new Vector3(newXPosition, newYPosition, other.transform.position.z);
        other.transform.position = newPosition;
        cameraController.ChangeScreenLimit(newLeftLimit, newRightLimit, newBottonLimit, newTopLimit);

        cameraController.ChangeCameraPosition(newPosition);
        //mainCamera.transform.position = newPosition;

        MineGeneration mineGeneration = GameObject.FindGameObjectWithTag("Mines").GetComponent<MineGeneration>();

        MineGeneration.inMines = toMines;
        if (toMines)
        {
            MineGeneration.level++;
            mineGeneration.DestroyMines();
            mineGeneration.GenerateMines();
        }
        else
        {
            MineGeneration.level = 0;
            mineGeneration.DestroyMines();
        }
    }
    
}
