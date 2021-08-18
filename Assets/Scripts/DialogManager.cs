using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Yarn.Unity;

public class DialogManager : MonoBehaviour
{
    [SerializeField] private Image speakerPortrait;
    [SerializeField] public TextMeshProUGUI txtDialog, txtSpeakerName;
    [SerializeField] public NPC currentNPC;
    
    public DialogueRunner dialogueRunner;
    private PlayerMovement playerMovement;
    private TileAim tileAim;

    public static DialogManager instance = null;

    Dictionary<string, SpeakerData> speakerDatabase = new Dictionary<string, SpeakerData>();

    private Animator dialogAnimator;

    public DayCycle dayCycle;

    void Start()
    {
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        tileAim = GameObject.FindGameObjectWithTag("TileAim").GetComponent<TileAim>();

        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        dialogAnimator = GetComponent<Animator>();


        dayCycle = GameObject.FindGameObjectWithTag("DayCycle").GetComponent<DayCycle>();
    }

    void Awake()
    {
        dialogueRunner.AddCommandHandler("SetSpeaker", SetSpeakerInfo);
        dialogueRunner.AddCommandHandler("Sleep", Sleep);
    }

    public void AddSpeaker (SpeakerData data)
    {
        if (speakerDatabase.ContainsKey(data.speakerName))
        {
            Debug.Log("Dab away the pain");
            return;
        }

        speakerDatabase.Add(data.speakerName, data);
    }

    public void SetSpeakerInfo(string[] info)
    {
        string speaker = info[0];
        string emotion = info.Length > 1 ? info[1].ToLower() : "happy";

        if (speakerDatabase.TryGetValue(speaker, out SpeakerData data))
        {
            speakerPortrait.sprite = data.GetEmotionPortrait(emotion);
            txtSpeakerName.text = data.speakerName;
        }
    }

    public void Sleep(string[] empty)
    {
        dayCycle.NewDay();
    }

    public void OnDialogStart()
    {
        dialogAnimator.SetBool("Talking", true);
        tileAim.OnDialogStart();
        playerMovement.OnDialogStart();
    }

    public void OnDialogEnd()
    {
        dialogAnimator.SetBool("Talking", false);
        tileAim.OnDialogEnd();
        playerMovement.OnDialogEnd();
    }
}
