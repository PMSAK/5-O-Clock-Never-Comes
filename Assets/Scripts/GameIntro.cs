using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameIntro : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text bodyText;
    [SerializeField] private Button nextButton;
    [SerializeField] private TMP_Text buttonText;

    [Header("Game")]
    [SerializeField] private string gameplayScene = "Level1";

    private int currentPage;

    private readonly string[] titles =
    {
        "Another Day at the Office",
        "4:45 PM",
        "Not So Fast",
        "Something Is Wrong",
        "Get Out",
        "Instructions"
    };

    private readonly string[] messages =
    {
        "You are a 9-to-5 office worker. Every day is the same: endless paperwork, endless meetings, and a clock that never seems to move fast enough.",

        "Fifteen minutes until freedom. You've packed your things and you're ready to leave. Just one last walk through the office.",

        "The exit is locked. Apparently, you need an access card. Of course you do. Nothing at this place is ever simple.",

        "The clock strikes 5:00 PM. Everything resets. You're back where you started, but you remember what happened.",

        "Find the keycards, avoid suspicious coworkers and leave before the clock strikes five. You don't have much time.",

        "Go near objects you find interesting or want to just look at :), press 'E' to interact and 'Esc' to close"
    };

    private void Start()
    {
        if (nextButton != null)
            nextButton.onClick.AddListener(NextPage);

        ShowPage();
    }

    private void ShowPage()
    {
        titleText.text = titles[currentPage];
        bodyText.text = messages[currentPage];

        if (buttonText != null)
        {
            buttonText.text =
                currentPage == titles.Length - 1
                ? "START GAME"
                : "NEXT";
        }
    }

    public void NextPage()
    {
        currentPage++;

        if (currentPage >= titles.Length)
        {
            SceneManager.LoadScene(gameplayScene);
            return;
        }

        ShowPage();
    }

    private void OnDestroy()
    {
        if (nextButton != null)
            nextButton.onClick.RemoveListener(NextPage);
    }
}

