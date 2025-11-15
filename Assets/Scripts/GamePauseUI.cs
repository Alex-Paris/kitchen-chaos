using UnityEngine;
using UnityEngine.UI;

public class GamePauseUI : MonoBehaviour
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button optionsButton;

    private void Awake()
    {
        resumeButton.onClick.AddListener(() => LevelGameManager.Instance.TogglePauseGame());
        mainMenuButton.onClick.AddListener(() => Loader.Load(Loader.Scene.MainMenuScene));
        optionsButton.onClick.AddListener(() => {
            Hide();
            OptionsUI.Instance.Show(Show);
        });
    }

    private void Start()
    {
        LevelGameManager.Instance.OnGamePausedChanged += LevelGameManager_OnGamePausedChanged;

        Hide();
    }

    private void LevelGameManager_OnGamePausedChanged(object sender, LevelGameManager.OnGamePausedChangedEventArgs e)
    {
        if (e.isGamePaused)
            Show();
        else
            Hide();
    }

    private void Show()
    {
        gameObject.SetActive(true);

        resumeButton.Select();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
