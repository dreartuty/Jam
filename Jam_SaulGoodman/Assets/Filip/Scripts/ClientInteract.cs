using Unity.Netcode;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.UI;

public class ClientInteract : NetworkBehaviour
{
    public LayerMask interactLayer;
    public float interactRange;
    public Transform middle;
    public Items currentItem;
    public Image itemDisplay;
    public Sprite defaultIcon;
    public DialogueUI dialogueUI;
    public KeyCode interact, giveItem, closeUI;
    public Image highlight;
    public Sprite[] highlightStages;
    public int evidencePoints = 0;

    public override void OnNetworkSpawn()
    {
        GameObject canvas = GameObject.Find("Canvas");
        itemDisplay = canvas.transform.GetChild(2).GetComponent<Image>();
        highlight = canvas.transform.GetChild(1).GetComponent<Image>();   
    }
    void Update()
    {
        if (currentItem != null)
        {
            itemDisplay.sprite = currentItem.itemIcon;
        }
        else
        {
            itemDisplay.sprite = defaultIcon;
        }

        bool doInteract = Input.GetKeyDown(interact) && dialogueUI.isOpened == false;
        bool doGiveItem = Input.GetKeyDown(giveItem) && dialogueUI.isOpened == false;
        bool doCloseUI = Input.GetKeyDown(closeUI) && dialogueUI.isOpened == true;

        if (doCloseUI)
        {
            dialogueUI.EndDialogue();
        }

        // check for closest valid target
        Collider[] validNPCs = Physics.OverlapSphere(middle.position, interactRange, interactLayer);
        float range = Mathf.Infinity;
        GameObject validNPC = null;
        foreach (Collider c in validNPCs)
        {
            float distance = Vector3.Distance(middle.position, c.transform.position);
            if (range > distance)
            {
                range = distance;
                validNPC = c.gameObject;
            }
        }
        highlight.gameObject.SetActive(validNPC != null);
        if (validNPC == null) return;
        profile NPC = validNPC.GetComponent<profile>();
        PickupItem pickup = validNPC.GetComponent<PickupItem>();
        if (NPC != null)
        {
            bool know = false;
            know = true;

            /*if (NetworkManager.Singleton.LocalClient.ClientId == 0 && NPC.CrimeKnowledge == profile.Whitness.player0)
            {
                know = true;
            }
            else if (NetworkManager.Singleton.LocalClient.ClientId == 1 && NPC.CrimeKnowledge == profile.Whitness.player1)
            {
                know = true;
            }*/

            highlight.GetComponent<RectTransform>().position = Camera.main.WorldToScreenPoint(validNPC.transform.position + Vector3.up * 1.5f);

            bool canGiveItem = know && NPC.Profile.favouriteItem == currentItem;
            Sprite oldSprite = highlight.sprite;
            highlight.sprite = highlightStages[canGiveItem ? 1 : 0];
            if (oldSprite != highlight.sprite) highlight.SetNativeSize();

            // give item to the npc
            if (doGiveItem)
            {
                if (canGiveItem)
                {
                    currentItem = null;
                    NPC.wasGivenItem = true;
                    doInteract = true;
                    evidencePoints++;
                }
            }

            // interacting with the npc
            if (doInteract)
            {
                if (know && (NPC.wasGivenItem || NPC.Profile.favouriteItem == null))
                {
                    dialogueUI.StartDialogue(NPC.Profile.satisfiedDialogue);
                }
                else if (know && !NPC.wasGivenItem)
                {
                    dialogueUI.StartDialogue(NPC.Profile.needItemDialogue);
                }
                else
                {
                    dialogueUI.StartDialogue(NPC.Profile.notKnowingDialogue);
                }
            }
        }
        else if (pickup != null)
        {
            highlight.GetComponent<RectTransform>().position = Camera.main.WorldToScreenPoint(validNPC.transform.position + Vector3.up * 0.5f);
            Sprite oldSprite = highlight.sprite;
            highlight.sprite = highlightStages[0];
            if (oldSprite != highlight.sprite) highlight.SetNativeSize();
            if (doInteract)
            {
                currentItem = pickup.Pickup(currentItem);
            }
        }
    }
}
