using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class DoorLevelTransition : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private string nextSceneName = "Level2";
    [SerializeField] private GameObject messageObject;
    [SerializeField] private float messageDuration = 2f;

    private bool transitioning;
    private Coroutine messageRoutine;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || transitioning)
            return;

        if (InventorySystem.Instance != null &&
            InventorySystem.Instance.HasItem("Keycard"))
        {
            // Consume ONLY the normal keycard.
            InventorySystem.Instance.RemoveItem("Keycard");

            transitioning = true;
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            ShowLockedMessage();
        }
    }

    private void ShowLockedMessage()
    {
        if (messageObject == null)
        {
            Debug.LogWarning("Door message is not assigned!");
            return;
        }

        if (messageRoutine != null)
            StopCoroutine(messageRoutine);

        messageRoutine = StartCoroutine(ShowMessage());
    }

    private IEnumerator ShowMessage()
    {
        messageObject.SetActive(true);

        yield return new WaitForSeconds(messageDuration);

        if (messageObject != null)
            messageObject.SetActive(false);

        messageRoutine = null;
    }
}
