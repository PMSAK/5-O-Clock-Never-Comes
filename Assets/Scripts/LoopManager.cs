using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoopManager : MonoBehaviour
{
    public static LoopManager Instance { get; private set; }

    [Header("Clock Settings")]
    [SerializeField] private int startHour = 4;
    [SerializeField] private int startMinute = 50;
    [SerializeField] private int resetHour = 5;
    [SerializeField] private int resetMinute = 0;

    [Tooltip("Real seconds for each in-game minute.")]
    [SerializeField] private float realSecondsPerGameMinute = 6f;

    [Header("Reset Settings")]
    [SerializeField] private string startingSceneName = "Level1";
    [SerializeField] private float resetDelay = 1f;

    private TMP_Text clockText;
    private float elapsedGameMinutes;
    private bool resetting;
    private bool clockStopped;
    private bool timerPaused;

    private int StartTotalMinutes => startHour * 60 + startMinute;
    private int ResetTotalMinutes => resetHour * 60 + resetMinute;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        FindClockText();
        UpdateClock();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Instance = null;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindClockText();
        UpdateClock();
    }

    private void FindClockText()
    {
        clockText = null;

        TMP_Text[] texts = Resources.FindObjectsOfTypeAll<TMP_Text>();

        foreach (TMP_Text textComponent in texts)
        {
            if (textComponent.gameObject.name == "LoopClockText"
                && textComponent.gameObject.scene.IsValid()
                && textComponent.gameObject.scene.isLoaded)
            {
                clockText = textComponent;
                break;
            }
        }

        if (clockText == null)
        {
            Debug.LogWarning(
                "LoopManager: LoopClockText was not found in the loaded scene."
            );
            return;
        }

        clockText.gameObject.SetActive(true);
        clockText.enabled = true;

        Debug.Log("LoopManager: Clock connected successfully.");
    }

    private void Update()
    {
        if (resetting || realSecondsPerGameMinute <= 0f || clockStopped || timerPaused)
            return;

        elapsedGameMinutes += Time.deltaTime / realSecondsPerGameMinute;

        int currentTotalMinutes =
            StartTotalMinutes + Mathf.FloorToInt(elapsedGameMinutes);

        if (currentTotalMinutes >= ResetTotalMinutes)
        {
            StartCoroutine(ResetLoop());
            return;
        }

        UpdateClock();
    }

    // Rewinds the in-game clock by the requested number of minutes.
    public void RewindTime(int minutes)
    {
        if (resetting || minutes <= 0)
            return;

        elapsedGameMinutes = Mathf.Max(
            0f,
            elapsedGameMinutes - minutes
        );

        UpdateClock();

        Debug.Log($"LoopManager: Time rewound by {minutes} in-game minutes.");
    }

    public void StopClock()
    {
        clockStopped = true;
        UpdateClock();

        Debug.Log("LoopManager: Clock stopped permanently for the ending.");
    }

    public void PauseTimer()
    {
        timerPaused = true;
    }

    public void ResumeTimer()
    {
        timerPaused = false;
    }

    private void UpdateClock()
    {
        if (clockText == null)
            return;

        int currentTotalMinutes =
            StartTotalMinutes + Mathf.FloorToInt(elapsedGameMinutes);

        int hour24 = (currentTotalMinutes / 60) % 24;
        int minute = currentTotalMinutes % 60;

        int displayHour = hour24 % 12;

        if (displayHour == 0)
            displayHour = 12;

        string period = hour24 >= 12 ? "PM" : "AM";

        clockText.text = $"{displayHour}:{minute:00} {period}";
    }

    private IEnumerator ResetLoop()
    {
        resetting = true;

        if (clockText != null)
            clockText.text = $"{resetHour}:{resetMinute:00} PM";

        yield return new WaitForSeconds(resetDelay);

        elapsedGameMinutes = 0f;

        SceneManager.LoadScene(startingSceneName);

        // OnSceneLoaded reconnects the clock.
        resetting = false;
    }
}
