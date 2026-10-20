using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class FinalDoorTransition : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private string nextSceneName = "Ending";
    [SerializeField] private GameObject messageObject;
    [SerializeField] private float messageDuration = 2f;

    private bool transitioning;
    private Coroutine messageRoutine;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || transitioning)
            return;

        if (InventorySystem.Instance == null)
        {
            ShowMessage("Inventory unavailable.");
            return;
        }

        bool hasOriginalKeycard =
            InventorySystem.Instance.HasItem("Keycard");

        bool hasFinalKeycard =
            InventorySystem.Instance.HasItem("FinalKeycard");

        if (hasOriginalKeycard && hasFinalKeycard)
        {
            transitioning = true;

            if (LoopManager.Instance != null)
                LoopManager.Instance.StopClock();

            SceneManager.LoadScene(nextSceneName);
        }
        else if (!hasOriginalKeycard && !hasFinalKeycard)
        {
            ShowMessage("ACCESS DENIED. Both keycards are required.");
        }
        else if (!hasOriginalKeycard)
        {
            ShowMessage("ACCESS DENIED. Original keycard required.");
        }
        else
        {
            ShowMessage("ACCESS DENIED. Final keycard required.");
        }
    }

    private void ShowMessage(string message)
    {
        if (messageObject == null)
        {
            Debug.Log(message);
            return;
        }

        if (messageRoutine != null)
            StopCoroutine(messageRoutine);

        messageRoutine = StartCoroutine(DisplayMessage(message));
    }

    private IEnumerator DisplayMessage(string message)
    {
        messageObject.SetActive(true);

        var text = messageObject.GetComponent<TMPro.TMP_Text>();

        if (text == null)
            text = messageObject.GetComponentInChildren<TMPro.TMP_Text>();

        if (text != null)
            text.text = message;

        yield return new WaitForSeconds(messageDuration);

        if (messageObject != null)
            messageObject.SetActive(false);

        messageRoutine = null;
    }
}
