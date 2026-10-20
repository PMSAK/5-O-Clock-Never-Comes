using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class FileRackPuzzle : MonoBehaviour
{
    [Header("Puzzle UI")]
    [SerializeField] private GameObject puzzlePanel;
    [SerializeField] private TMP_Text questionText;
    [SerializeField] private TMP_InputField answerInput;
    [SerializeField] private Button submitButton;
    [SerializeField] private TMP_Text feedbackText;

    [Header("Puzzle Settings")]
    [SerializeField] private string correctAnswer = "CLOCK";
    [SerializeField] private string computerCodeToReveal = "7391";

    [Header("NPC Safety")]
    [SerializeField] private Transform[] npcs;
    [SerializeField] private float npcSafeDistance = 2.5f;

    [Header("Player")]
    [SerializeField] private PlayerMovement playerMovement;

    private bool playerNearby;
    private bool panelOpen;
    private bool puzzleSolved;

    private void Start()
    {
        if (puzzlePanel != null)
            puzzlePanel.SetActive(false);

        if (questionText != null)
        {
            questionText.text =
                "I have hands but cannot clap. " +
                "I have a face but cannot speak. What am I?";
        }

        if (submitButton != null)
            submitButton.onClick.AddListener(SubmitAnswer);
    }

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (playerNearby && !panelOpen && !puzzleSolved &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (AreNPCsNearby())
            {
                if (feedbackText != null)
                    feedbackText.text =
                        "STAFF ARE NEARBY! WAIT UNTIL THEY LEAVE.";

                return;
            }

            OpenPuzzle();
        }

        if (panelOpen && Keyboard.current.escapeKey.wasPressedThisFrame)
            ClosePuzzle();
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
            ClosePuzzle();
    }

    private bool AreNPCsNearby()
    {
        if (npcs == null)
            return false;

        foreach (Transform npc in npcs)
        {
            if (npc == null)
                continue;

            float distance = Vector3.Distance(npc.position, transform.position);

            if (distance < npcSafeDistance)
                return true;
        }

        return false;
    }

    private void OpenPuzzle()
    {
        if (puzzlePanel == null)
        {
            Debug.LogWarning("FileRackPuzzle: Puzzle Panel is not assigned.");
            return;
        }

        if (LoopManager.Instance != null)
            LoopManager.Instance.PauseTimer();

        panelOpen = true;
        puzzlePanel.SetActive(true);

        if (feedbackText != null)
            feedbackText.text = "";

        if (answerInput != null)
        {
            answerInput.text = "";
            answerInput.interactable = true;
            answerInput.ActivateInputField();
        }

        if (submitButton != null)
            submitButton.interactable = true;

        if (playerMovement != null)
            playerMovement.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void SubmitAnswer()
    {
        if (!panelOpen || puzzleSolved || answerInput == null)
            return;

        if (AreNPCsNearby())
        {
            if (feedbackText != null)
                feedbackText.text =
                    "STAFF ARE NEARBY! YOU CANNOT ACCESS THESE FILES.";

            return;
        }

        if (answerInput.text.Trim().ToUpperInvariant() !=
            correctAnswer.Trim().ToUpperInvariant())
        {
            if (feedbackText != null)
                feedbackText.text = "INCORRECT ANSWER. TRY AGAIN.";

            answerInput.ActivateInputField();
            return;
        }

        puzzleSolved = true;

        if (feedbackText != null)
        {
            feedbackText.text =
                "ACCESS GRANTED!\nFINAL COMPUTER CODE: " +
                computerCodeToReveal;
        }

        answerInput.interactable = false;

        if (submitButton != null)
            submitButton.interactable = false;

        Debug.Log("File rack puzzle solved. Computer code: " + computerCodeToReveal);
    }

    private void ClosePuzzle()
    {
        if (LoopManager.Instance != null)
            LoopManager.Instance.ResumeTimer();

        panelOpen = false;

        if (puzzlePanel != null)
            puzzlePanel.SetActive(false);

        if (playerMovement != null)
            playerMovement.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDestroy()
    {
        if (submitButton != null)
            submitButton.onClick.RemoveListener(SubmitAnswer);
    }
}
