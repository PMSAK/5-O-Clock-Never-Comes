using UnityEngine;
using TMPro;
using System.Collections;

public class KeyCardPickup : MonoBehaviour
{
    [Header("Keycard")]
    [SerializeField] private GameObject keycardObject;
    [SerializeField] private Sprite keycardIcon;

    [Header("NPC Detection")]
    [SerializeField] private Transform[] npcs;
    [SerializeField] private float npcAwayDistance = 2f;

    [Header("UI Message")]
    [SerializeField] private TMP_Text warningText;
    [SerializeField] private string warningMessage =
        "An NPC is nearby! Wait until they leave.";
    [SerializeField] private float warningDuration = 2f;

    private bool collected;
    private Coroutine warningCoroutine;

    private void Start()
    {
        if (warningText != null)
            warningText.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (collected || !other.CompareTag("Player"))
            return;

        if (keycardObject == null)
        {
            Debug.LogWarning("Keycard object is not assigned!");
            return;
        }

        foreach (Transform npc in npcs)
        {
            if (npc == null)
                continue;

            float distance = Vector3.Distance(
                npc.position,
                keycardObject.transform.position
            );

            if (distance < npcAwayDistance)
            {
                ShowWarning();
                return;
            }
        }

        if (InventorySystem.Instance == null)
        {
            Debug.LogWarning("InventorySystem is missing from the scene!");
            return;
        }

        bool added = InventorySystem.Instance.AddItem(
            "Keycard",
            keycardIcon
        );

        if (added)
        {
            collected = true;
            keycardObject.SetActive(false);
            HideWarning();

            Debug.Log("Keycard collected!");
        }
    }

    private void ShowWarning()
    {
        if (warningText == null)
            return;

        warningText.text = warningMessage;
        warningText.gameObject.SetActive(true);

        if (warningCoroutine != null)
            StopCoroutine(warningCoroutine);

        warningCoroutine = StartCoroutine(HideWarningAfterDelay());
    }

    private IEnumerator HideWarningAfterDelay()
    {
        yield return new WaitForSeconds(warningDuration);

        if (warningText != null)
            warningText.gameObject.SetActive(false);

        warningCoroutine = null;
    }

    private void HideWarning()
    {
        if (warningCoroutine != null)
        {
            StopCoroutine(warningCoroutine);
            warningCoroutine = null;
        }

        if (warningText != null)
            warningText.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (warningCoroutine != null)
            StopCoroutine(warningCoroutine);
    }
}
