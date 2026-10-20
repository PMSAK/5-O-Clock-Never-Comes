using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SupervisorDoorTransition : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private string nextSceneName = "Level4";
    [SerializeField] private GameObject messageObject;
    [SerializeField] private float messageDuration = 2f;

    private bool transitioning;
    private Coroutine messageRoutine;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || transitioning)
            return;

        if (InventorySystem.Instance != null &&
            InventorySystem.Instance.HasItem("SupervisorA20"))
        {
            // Consume ONLY the Supervisor A20 card.
            InventorySystem.Instance.RemoveItem("SupervisorA20");

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
            Debug.LogWarning("Supervisor door message is not assigned!");
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
