using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClickToChangeScene : MonoBehaviour
{
    private bool playerInRange = false;
    private GameObject player;
    private CameraController cameraController;

    [SerializeField] private string nextScene;

    [SerializeField] private float newXPosition;
    [SerializeField] private float newYPosition;

    [SerializeField] private float newLeftLimit;
    [SerializeField] private float newRightLimit;
    [SerializeField] private float newTopLimit;
    [SerializeField] private float newBottomLimit;

    [SerializeField] private bool advanceInMines;
    [SerializeField] private bool toOverworld;

    [SerializeField] private Texture2D cursorSprite;

    private Animator fadeAnimator;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        cameraController = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();

        fadeAnimator = GameObject.FindGameObjectWithTag("Fade").GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("TileAim")) playerInRange = true;
    }

    private void OnMouseEnter()
    {
        Cursor.SetCursor(cursorSprite, Vector2.zero, CursorMode.Auto);
    }

    private void OnMouseExit()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("TileAim")) playerInRange = false;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && playerInRange)
        {
            StartCoroutine(TransitionBetweenScenes());
        }
    }

    private IEnumerator TransitionBetweenScenes()
    {
        Transform farmManager = PlayerMovement.instance.farmManager.transform;
        
        foreach (Transform child in farmManager)
            child.gameObject.SetActive(false);

        fadeAnimator.SetTrigger("Leaving");
        yield return new WaitForSeconds(0.8f);
        ChangeActiveScene();
    }

    private void ChangeActiveScene()
    {
        if (advanceInMines)
        {
            MineManager.level++;
        }

        else
        {
            MineManager.level = 0;
        }

        FarmManager.instance.gameObject.SetActive(toOverworld);
        Vector2 newPosition = new Vector2(newXPosition, newYPosition);
        cameraController.transform.position = newPosition;
        player.transform.position = newPosition;

        cameraController.ChangeScreenLimit(newLeftLimit, newRightLimit, newBottomLimit, newTopLimit);

        SceneManager.LoadScene(nextScene);
    }
}
