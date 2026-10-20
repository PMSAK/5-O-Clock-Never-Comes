using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

public class FinalComputerPuzzle : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject securityPanel;
    [SerializeField] private TMP_InputField codeInput;
    [SerializeField] private Button submitButton;
    [SerializeField] private TMP_Text feedbackText;

    [Header("Authorization")]
    [SerializeField] private string correctCode = "7391";
    [SerializeField] private Sprite finalKeycardIcon;

    [Header("Player")]
    [SerializeField] private PlayerMovement playerMovement;

    private bool playerNearby;
    private bool puzzleSolved;
    private bool panelOpen;

    private void Start()
    {
        if (securityPanel != null)
            securityPanel.SetActive(false);

        if (submitButton != null)
            submitButton.onClick.AddListener(SubmitCode);

        if (InventorySystem.Instance != null &&
            InventorySystem.Instance.HasItem("FinalKeycard"))
        {
            puzzleSolved = true;
        }
    }

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (panelOpen && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            ClosePanel();
            return;
        }

        if (!playerNearby || puzzleSolved || panelOpen)
            return;

        if (Keyboard.current.eKey.wasPressedThisFrame)
            OpenPanel();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerNearby = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerNearby = false;

        if (panelOpen)
            ClosePanel();
    }

    private void OpenPanel()
    {
        if (securityPanel == null)
        {
            Debug.LogWarning(
                "Final computer: Security Panel is not assigned."
            );
            return;
        }

        panelOpen = true;
        securityPanel.SetActive(true);

        // Pause the game timer.
        if (LoopManager.Instance != null)
            LoopManager.Instance.PauseTimer();

        if (feedbackText != null)
            feedbackText.text = "Enter the final authorization code.";

        if (playerMovement != null)
            playerMovement.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (submitButton != null)
            submitButton.interactable = true;

        if (codeInput != null)
        {
            codeInput.text = "";
            codeInput.Select();
            codeInput.ActivateInputField();
        }

        Debug.Log("Final computer panel opened. Timer paused.");
    }

    private void ClosePanel()
    {
        panelOpen = false;

        if (securityPanel != null)
            securityPanel.SetActive(false);

        if (playerMovement != null)
            playerMovement.enabled = true;

        // Resume the game timer.
        if (LoopManager.Instance != null)
            LoopManager.Instance.ResumeTimer();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void SubmitCode()
    {
        if (!panelOpen || puzzleSolved || codeInput == null)
            return;

        if (codeInput.text.Trim() != correctCode)
        {
            if (feedbackText != null)
                feedbackText.text =
                    "ACCESS DENIED. Invalid authorization code.";

            codeInput.Select();
            codeInput.ActivateInputField();
            return;
        }

        if (InventorySystem.Instance == null)
        {
            if (feedbackText != null)
                feedbackText.text = "INVENTORY SYSTEM UNAVAILABLE.";

            Debug.LogWarning("InventorySystem is missing.");
            return;
        }

        if (InventorySystem.Instance.HasItem("FinalKeycard"))
        {
            puzzleSolved = true;
            ClosePanel();
            return;
        }

        if (finalKeycardIcon == null)
        {
            if (feedbackText != null)
                feedbackText.text = "KEYCARD DATA UNAVAILABLE.";

            Debug.LogWarning("Final keycard icon is not assigned.");
            return;
        }

        bool added = InventorySystem.Instance.AddItem(
            "FinalKeycard",
            finalKeycardIcon
        );

        if (added)
        {
            puzzleSolved = true;

            Debug.Log("Final keycard awarded!");

            // Closing the panel also resumes the timer.
            ClosePanel();
        }
        else
        {
            if (feedbackText != null)
                feedbackText.text =
                    "INVENTORY FULL. Make room for the keycard.";
        }
    }

    private void OnDestroy()
    {
        if (submitButton != null)
            submitButton.onClick.RemoveListener(SubmitCode);

        // Prevent the timer from remaining paused if this object is destroyed.
        if (panelOpen && LoopManager.Instance != null)
            LoopManager.Instance.ResumeTimer();
    }
}
