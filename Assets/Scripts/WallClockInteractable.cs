using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class WallClockInteractable : MonoBehaviour
{
    [Header("Clock Settings")]
    [SerializeField] private int minutesToRewind = 2;

    [Header("Interaction")]
    [SerializeField] private string interactionMessage =
        "Press E to rewind time";

    [SerializeField] private TMP_Text interactionText;

    private bool playerNearby;
    private bool used;

    private void Start()
    {
        if (interactionText != null)
            interactionText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!playerNearby || used || Keyboard.current == null)
            return;

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            UseClock();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerNearby = true;

        if (!used && interactionText != null)
        {
            interactionText.text = interactionMessage;
            interactionText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerNearby = false;

        if (interactionText != null)
            interactionText.gameObject.SetActive(false);
    }

    private void UseClock()
    {
        if (LoopManager.Instance == null)
        {
            Debug.LogWarning("Wall clock: LoopManager was not found.");
            return;
        }

        used = true;

        LoopManager.Instance.RewindTime(minutesToRewind);

        if (interactionText != null)
        {
            interactionText.text = "TIME REWOUND!";
            interactionText.gameObject.SetActive(true);
        }

        Debug.Log($"Wall clock used. Rewound {minutesToRewind} minutes.");
    }
}

