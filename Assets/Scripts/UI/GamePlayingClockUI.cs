using UnityEngine;
using UnityEngine.UI;

public class GamePlayingClockUI : MonoBehaviour
{
    [SerializeField] private Image timer;

    private void Start()
    {
        Hide();
        LevelGameManager.Instance.OnStateChanged += LevelGameManager_OnStateChanged;
    }

    private void LevelGameManager_OnStateChanged(object sender, System.EventArgs e)
    {
        if (LevelGameManager.Instance.IsGamePlaying())
            Show();
        else
            Hide();
    }

    private void Update()
    {
        timer.fillAmount = LevelGameManager.Instance.GamePlayingTimerNormalized;
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
