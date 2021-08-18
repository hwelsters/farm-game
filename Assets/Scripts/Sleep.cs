using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sleep : MonoBehaviour
{
    private DayCycle dayCycle;

    private DialogManager dialogManager;

    [SerializeField] private SpeakerData speakerData;

    [SerializeField] YarnProgram yarnDialog;

    [SerializeField] string introNode;

    void Start()
    {
        dayCycle = GameObject.FindGameObjectWithTag("DayCycle").GetComponent<DayCycle>();

        dialogManager = GameObject.FindGameObjectWithTag("DialogManager").GetComponent<DialogManager>();

        dialogManager.dialogueRunner.Add(yarnDialog);

        dialogManager.AddSpeaker(speakerData);
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            dialogManager.dialogueRunner.StartDialogue(introNode);
        }
    }
}
