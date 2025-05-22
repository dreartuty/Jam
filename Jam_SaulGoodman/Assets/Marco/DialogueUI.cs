using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class DialogueUI : MonoBehaviour
{
    public GameObject panel;
    public Image portraitImage;
    public TMP_Text nameText;
    public TMP_Text dialogueText;
    [HideInInspector] public bool isOpened = false;

    public Dialogue currentDialogue; // The current Dialogue being shown
    private int currentLine; // The index of the current line to be shown

    //Trigger function for the start of an interaction with an NPC
    public void StartDialogue(Dialogue dialogue)
    {
        currentDialogue = dialogue;
        currentLine = 0;
        isOpened = true;
        panel.SetActive(true);
        ShowLine();
    }

    //function that showcases the current line to be told(by an NPC)
    public void ShowLine()
    {
        if (currentLine >= currentDialogue.lines.Length)
        {
            EndDialogue();//End the Convo
            return;
        }


        //NPC profile visualisation Setup
        var line = currentDialogue.lines[currentLine];
        nameText.text = currentDialogue.profile.npcName;
        nameText.color = currentDialogue.profile.nameColor;
        portraitImage.sprite = currentDialogue.profile.portrait;
        dialogueText.text = line.line;


        currentLine++; //Increments the line
    }

    //Ends dialogue
    public void EndDialogue()
    {
        panel.SetActive(false);
        currentDialogue = null;
        isOpened = false;
    }
    //This function needs to be hooked to the "Next" button in the UI
    public void OnNextButton()
    {
        ShowLine();
    }
}