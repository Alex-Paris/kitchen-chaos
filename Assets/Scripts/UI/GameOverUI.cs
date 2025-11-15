using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI recipesDeliveredText;

    private void Start()
    {
        Hide();
        LevelGameManager.Instance.OnStateChanged += LevelGameManager_OnStateChanged;
    }

    private void LevelGameManager_OnStateChanged(object sender, System.EventArgs e)
    {
        if (LevelGameManager.Instance.IsGameOver())
        {
            recipesDeliveredText.text = DeliveryManager.Instance.SuccessfulRecipesAmout.ToString();
            Show();
        }
        else
            Hide();
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
