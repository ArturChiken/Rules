using TMPro;
using UnityEngine;

public class OnScreenTimeDisplayTest : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI dateText;

    private void Start()
    {
        if (timeText == null)
            timeText = GetComponent<TextMeshProUGUI>();

        if (GameDateTime.Instance != null)
        {
            GameDateTime.Instance.OnDateTimeChanged += UpdateDisplay;
            UpdateDisplay();
        }
    }

    private void UpdateDisplay()
    {
        if (timeText != null)
            timeText.text = GameDateTime.Instance.TimeString;
        if (dateText != null)
            dateText.text = GameDateTime.Instance.DateString;
    }

    private void OnDestroy()
    {
        if (GameDateTime.Instance != null)
            GameDateTime.Instance.OnDateTimeChanged -= UpdateDisplay;
    }
}