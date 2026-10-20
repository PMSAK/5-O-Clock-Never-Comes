using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToLevel1Door : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private string targetSceneName = "Level1";

    private bool triggered;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered || !other.CompareTag("Player"))
            return;

        triggered = true;

        Debug.Log("Returning to Level 1...");

        SceneManager.LoadScene(targetSceneName);
    }
}

