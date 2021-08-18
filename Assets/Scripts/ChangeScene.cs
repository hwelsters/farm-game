using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    [SerializeField] private string nextLocation;

    private string currentLocation;

    [SerializeField] private Vector2 newPosition;
    [SerializeField] private float newLeftLimit;
    [SerializeField] private float newRightLimit;
    [SerializeField] private float newTopLimit;
    [SerializeField] private float newBottonLimit;

    private GameObject mainCamera;
    private CameraController cameraController;

    private PlayerMovement playerMovement;
    [SerializeField] private bool toFarm;

    private Animator fadeAnimator;

    void Start()
    {
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");

        cameraController = mainCamera.GetComponent<CameraController>();

        Scene currentScene = SceneManager.GetActiveScene();
        currentLocation = currentScene.name;

        fadeAnimator = GameObject.FindGameObjectWithTag("Fade").GetComponent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(TransitionToNextScene(other));
        }
    }

    IEnumerator TransitionToNextScene(Collider2D other)
    {
        fadeAnimator.SetTrigger("Leaving");
        yield return new WaitForSeconds(0.8f);
        ChangeActiveScene(other);
    }

    void ChangeActiveScene(Collider2D other)
    {
        playerMovement.RenderFarm(toFarm);

        SceneManager.LoadScene(nextLocation);

        other.transform.position = newPosition;

        cameraController.ChangeScreenLimit(newLeftLimit, newRightLimit, newBottonLimit, newTopLimit);
        cameraController.transform.position = newPosition;
    }
}
