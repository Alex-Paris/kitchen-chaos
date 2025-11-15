using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryResultUI : MonoBehaviour
{
    private const string POPUP = "Popup";
    
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Color successColor;
    [SerializeField] private Color failureColor;
    [SerializeField] private Sprite successSprite;
    [SerializeField] private Sprite failureSprite;

    private Animator animator;

    private void Awake()
    {
        animator  = GetComponent<Animator>();
    }

    private void Start()
    {
        gameObject.SetActive(false);
        DeliveryManager.Instance.OnRecipeFailed += DeliveryManager_OnRecipeFailed;
        DeliveryManager.Instance.OnRecipeCompleted += DeliveryManager_OnRecipeCompleted;
    }

    private void OnDestroy()
    {
        DeliveryManager.Instance.OnRecipeFailed -= DeliveryManager_OnRecipeFailed;
        DeliveryManager.Instance.OnRecipeCompleted -= DeliveryManager_OnRecipeCompleted;
    }
    
    private void DeliveryManager_OnRecipeFailed(object sender, EventArgs e)
    {
        backgroundImage.color = failureColor;
        iconImage.sprite = failureSprite;
        messageText.text = "DELIVERY\nFAILED";
        gameObject.SetActive(true);
        animator.SetTrigger(POPUP);
    }

    private void DeliveryManager_OnRecipeCompleted(object sender, DeliveryManager.OnRecipeChangedEventArgs e)
    {
        backgroundImage.color = successColor;
        iconImage.sprite =  successSprite;
        messageText.text = "DELIVERY\nSUCCESS";
        gameObject.SetActive(true);
        animator.SetTrigger(POPUP);
    }
}
