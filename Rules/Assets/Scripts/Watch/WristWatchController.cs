using UnityEngine;

public class WristWatchController : MonoBehaviour
{
    [Header("=== œ–»¬ﬂ« ¿ ===")]
    [SerializeField] private GameObject watchUI;
    [SerializeField] private Animator wristAnimator;
    [SerializeField] private string showWatchTrigger = "ShowWatch";
    [SerializeField] private string hideWatchTrigger = "HideWatch";

    [Header("=== Õ¿—“–Œ… » ===")]
    [SerializeField] private bool pauseGameWhenOpen = false;

    private bool isWatchVisible = false;
    private PlayerControl playerControl;
    private WatchUI watchUIComponent;
    private float originalTimeScale;

    private void Awake()
    {
        playerControl = FindFirstObjectByType<PlayerControl>();
        watchUIComponent = watchUI?.GetComponent<WatchUI>();
    }

    private void Start()
    {
        if (watchUI != null)
            watchUI.SetActive(false);
    }

    private void OnEnable()
    {
        if (playerControl != null)
        {
            playerControl.OnToggleWatch += ToggleWatch;
        }
    }

    private void OnDisable()
    {
        if (playerControl != null)
        {
            playerControl.OnToggleWatch -= ToggleWatch;
        }
    }

    private void ToggleWatch()
    {
        isWatchVisible = !isWatchVisible;

        if (isWatchVisible)
            OpenWatch();
        else
            CloseWatch();
    }

    private void OpenWatch()
    {
        originalTimeScale = Time.timeScale;

        if (pauseGameWhenOpen)
            Time.timeScale = 0f;

        if (wristAnimator != null)
            wristAnimator.SetTrigger(showWatchTrigger);

        if (wristAnimator != null)
            Invoke(nameof(ShowUI), 0.3f);
        else
            ShowUI();
    }

    private void ShowUI()
    {
        if (watchUI != null)
        {
            watchUI.SetActive(true);
            if (watchUIComponent != null)
                watchUIComponent.OpenWatch(); 
        }
    }

    private void CloseWatch()
    {
        if (watchUI != null)
        {
            if (watchUIComponent != null)
                watchUIComponent.CloseWatch();
            watchUI.SetActive(false);
        }

        if (wristAnimator != null)
            wristAnimator.SetTrigger(hideWatchTrigger);

        Time.timeScale = originalTimeScale;
    }
}