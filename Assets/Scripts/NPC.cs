using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;
using TMPro;

public class NPC : MonoBehaviour
{
    [SerializeField] private int NPCIndex;

    [SerializeField] private string introNode;

    [SerializeField] private string[] springDialogNodes;
    [SerializeField] private string[] summerDialogNodes;
    [SerializeField] private string[] autumnDialogNodes;
    [SerializeField] private string[] winterDialogNodes;

    [SerializeField] private string loveItemNode;
    [SerializeField] private string likeItemNode;
    [SerializeField] private string neutralItemNode;
    [SerializeField] private string dislikeItemNode;
    [SerializeField] private string hateItemNode;

    [SerializeField] private int[] loveItems;
    [SerializeField] private int[] likeItems;
    [SerializeField] private int[] neutralItems;
    [SerializeField] private int[] dislikeItems;
    [SerializeField] private int[] hateItems;

    private HashSet<int> loveItemsHashSet;
    private HashSet<int> likeItemsHashSet;
    private HashSet<int> neutralItemsHashSet;
    private HashSet<int> dislikeItemsHashSet;
    private HashSet<int> hateItemsHashSet;

    [SerializeField] int birthday;

    [SerializeField] private YarnProgram yarnDialog;

    

    private bool playerInRange;

    private TileAim tileAim;
    private PlayerMovement playerMovement;
    private Inventory inventory;
    private DayCycle dayCycle;
    private DialogManager dialogManager;

    [SerializeField] private SpeakerData speakerData;

    void Start()
    {
        tileAim = GameObject.FindGameObjectWithTag("TileAim").GetComponent<TileAim>();

        dayCycle = GameObject.FindGameObjectWithTag("DayCycle").GetComponent<DayCycle>();

        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();

        dialogManager = GameObject.FindGameObjectWithTag("DialogManager").GetComponent<DialogManager>();

        dialogManager.dialogueRunner.Add(yarnDialog);

        dialogManager.AddSpeaker(speakerData);

        MoveArraysToHashSets();
    }

    void MoveArraysToHashSets()
    {
        loveItemsHashSet = new HashSet<int>(loveItems);
        likeItemsHashSet = new HashSet<int>(likeItems);
        neutralItemsHashSet = new HashSet<int>(neutralItems);
        dislikeItemsHashSet = new HashSet<int>(dislikeItems);
        hateItemsHashSet = new HashSet<int>(hateItems);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("TileAim"))
            playerInRange = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("TileAim"))
            playerInRange = false;
    }

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1) && playerInRange)
        {
            Talk();
            tileAim.NPCRelationshipPoints[NPCIndex] += 2;
        }

        if (PlayerHoldingGift() && CanBeGifted())
        {
            CursorManager.SetCursorToGift();
        }
        else if (!TalkedToday())
        {
            CursorManager.SetCursorToDialog();
        }
        else 
        {
            CursorManager.SetCursorBackToNormal();
        }
    }

    void OnMouseExit()
    {
        CursorManager.SetCursorBackToNormal();
    }

    bool PlayerHoldingGift()
    {
        return ((tileAim.itemIndexInSelectedSlot >= 100 && tileAim.itemIndexInSelectedSlot <= 700) || tileAim.itemIndexInSelectedSlot >= 801);
    }

    void Talk()
    {
        if (PlayerHoldingGift() && CanBeGifted())
        {
            ReceiveGift(tileAim.itemIndexInSelectedSlot);
            tileAim.giftedNPCToday[NPCIndex] = true;
            tileAim.giftedNPCForEntireWeek[NPCIndex]++;
        }

        else if (!TalkedToday())
        {
            string dialog = PickDialogFromArray();
            dialogManager.dialogueRunner.StartDialogue(dialog);
            tileAim.talkedToNPCs[NPCIndex] = true;
        }

        else
        {
            return;
        }

        StartCoroutine(Bounce());
    }

    public void ReceiveGift(int itemIndex)
    {
        string dialog = "";
        int relationshipPointsGained;
        if (CalibrateGift(in loveItemsHashSet, 20, itemIndex, out relationshipPointsGained)) { dialog = loveItemNode; }
        else if (CalibrateGift(in likeItemsHashSet, 10, itemIndex, out relationshipPointsGained)) { dialog = likeItemNode; }
        else if (CalibrateGift(in neutralItemsHashSet, 5, itemIndex, out relationshipPointsGained)) { dialog = neutralItemNode; }
        else if (CalibrateGift(in dislikeItemsHashSet, -10, itemIndex, out relationshipPointsGained)) { dialog = dislikeItemNode; }
        else { dialog = hateItemNode; }

        inventory.RemoveItemFromSlot(tileAim.selectedSlotNumber, 1);

        dialogManager.dialogueRunner.StartDialogue(dialog);
    }

    private bool CalibrateGift(in HashSet<int> itemsHashSet, int relationshipPointsGiven, int giftedItemIndex, out int relationshipPointsGained)
    {
        int muliplier = 1;
        if (itemsHashSet.Contains(giftedItemIndex))
        {
            relationshipPointsGained = relationshipPointsGiven * muliplier;
            return true;
        }

        relationshipPointsGained = 0;
        return false;
    }

    private string[] PickDialogArray()
    {
        switch(dayCycle.season)
        {
            case 0: return springDialogNodes;
            case 1: return summerDialogNodes;
            case 2: return autumnDialogNodes;
            case 3: return winterDialogNodes;
        }

        return null;
    }

    private IEnumerator Bounce()
    {
        float newWidth = 1.05f;
        float bounceSpeed = 0.5f;

        while (transform.localScale.x <= newWidth)
        {
            transform.localScale += new Vector3(bounceSpeed * Time.deltaTime, -bounceSpeed * Time.deltaTime, 0);
            yield return null;
        }

        while (transform.localScale.x >= 1)
        {
            transform.localScale += new Vector3(-bounceSpeed * Time.deltaTime, bounceSpeed * Time.deltaTime, 0);
            yield return null;
        }

        transform.localScale = new Vector2(1f, 1f);
    }

    string PickDialogFromArray()
    {
        string[] dialogArray = PickDialogArray();
        return dialogArray[DayCycle.day & dialogArray.Length];
    }

    private bool TalkedToday()
    {
        return tileAim.talkedToNPCs[NPCIndex];
    }

    private bool CanBeGifted()
    {
        return tileAim.giftedNPCForEntireWeek[NPCIndex] < 2 && !tileAim.giftedNPCToday[NPCIndex];
    }
}
