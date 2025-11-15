using UnityEngine;

public class StoveCounterVisual : MonoBehaviour
{
    [SerializeField] private StoveCounter stoveCounter;
    [SerializeField] private GameObject stoveOnGameObject;
    [SerializeField] private GameObject particlesGameObject;

    private void Start()
    {
        stoveCounter.OnStoveBeingUsed += StoveCounter_OnStoveBeingUsed;
    }

    private void StoveCounter_OnStoveBeingUsed(object sender, StoveCounter.OnStoveBeingUsedEventArgs e)
    {
        stoveOnGameObject.SetActive(e.stoveBeingUsed);
        particlesGameObject.SetActive(e.stoveBeingUsed);
    }
}
