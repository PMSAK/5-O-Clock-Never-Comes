using UnityEngine;
using TMPro;

public class EndingScreenMessage : MonoBehaviour
{
    [SerializeField] private TMP_Text endingText;

    private void Start()
    {
        if (endingText == null)
            return;

        endingText.text = "Finally work is over.... Time to relax";
        endingText.gameObject.SetActive(true);
    }
}

