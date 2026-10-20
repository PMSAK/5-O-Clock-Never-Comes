using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class ComputerPuzzle : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject securityPanel;
    [SerializeField] private TMP_InputField codeInput;
    [SerializeField] private Button submitButton;
    [SerializeField] private TMP_Text feedbackText;

    [Header("Authorization")]
    [SerializeField] private string correctCode = "4826";
    [SerializeField] private Sprite supervisorKeycardIcon;

    [Header("Player")]
    [SerializeField] private PlayerMovement playerMovement;

    private bool playerInside;
    private bool panelOpen;

    private void Start()
    {
        if (securityPanel != null)
            securityPanel.SetActive(false);

        if (playerMovement == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player != null)
                playerMovement = player.GetComponent<PlayerMovement>();
        }

        if (submitButton != null)
            submitButton.onClick.AddListener(SubmitCode);
        else
            Debug.LogError("ComputerPuzzle: Submit Button is not assigned!");

        if (securityPanel == null || codeInput == null ||
            feedbackText == null)
        {
            Debug.LogError(
                "ComputerPuzzle: Assign the Security Panel, Code Input, " +
                "and Feedback Text in the Inspector."
            );
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

        if (playerInside && !panelOpen &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            OpenPanel();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInside = false;

        if (panelOpen)
            ClosePanel();
    }

    private void OpenPanel()
    {
        if (securityPanel == null)
            return;

        panelOpen = true;
        securityPanel.SetActive(true);

        // Pause the game timer.
        if (LoopManager.Instance != null)
            LoopManager.Instance.PauseTimer();

        if (playerMovement != null)
            playerMovement.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (codeInput != null)
        {
            codeInput.interactable = true;
            codeInput.readOnly = false;
            codeInput.text = "";
            codeInput.Select();
            codeInput.ActivateInputField();
        }

        Debug.Log("Security terminal opened. Timer paused.");
    }

    public void SubmitCode()
    {
        if (!panelOpen || codeInput == null)
            return;

        string enteredCode = codeInput.text.Trim();

        if (enteredCode != correctCode)
        {
            if (feedbackText != null)
                feedbackText.text = "ACCESS DENIED - INVALID CODE";

            codeInput.text = "";
            codeInput.ActivateInputField();
            return;
        }

        if (InventorySystem.Instance == null)
        {
            if (feedbackText != null)
                feedbackText.text = "INVENTORY SYSTEM UNAVAILABLE";

            return;
        }

        bool added = InventorySystem.Instance.AddItem(
            "SupervisorA20",
            supervisorKeycardIcon
        );

        if (!added)
        {
            if (feedbackText != null)
                feedbackText.text = "INVENTORY FULL - MAKE SPACE";

            return;
        }

        if (feedbackText != null)
            feedbackText.text = "ACCESS GRANTED - A20 KEYCARD ISSUED";

        Debug.Log("Supervisor A20 keycard added to inventory.");

        // Close the panel and resume the timer.
        ClosePanel();
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

    private void OnDestroy()
    {
        if (submitButton != null)
            submitButton.onClick.RemoveListener(SubmitCode);

        // Avoid leaving the timer paused if this object is destroyed.
        if (panelOpen && LoopManager.Instance != null)
            LoopManager.Instance.ResumeTimer();
    }
}
