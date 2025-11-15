using System;
using UnityEngine;

public class StoveCounterSound : MonoBehaviour
{
    [SerializeField] private StoveCounter stoveCounter;

    private AudioSource audioSource;
    private float warningSoudTimer;
    private bool playWarningSound;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        stoveCounter.OnStoveBeingUsed += StoveCounter_OnStoveBeingUsed;
        stoveCounter.OnProgressChanged += StoveCounter_OnProgressChanged;
    }

    private void OnDestroy()
    {
        stoveCounter.OnStoveBeingUsed -= StoveCounter_OnStoveBeingUsed;
        stoveCounter.OnProgressChanged -= StoveCounter_OnProgressChanged;
    }

    private void Update()
    {
        if (!playWarningSound) return;
        
        warningSoudTimer -= Time.deltaTime;
        if (warningSoudTimer <= 0f)
        {
            float warningSoundTimerMax = .2f;
            warningSoudTimer = warningSoundTimerMax;
            SoundManager.Instance.PlayWarningSound(stoveCounter.transform.position);
        }
    }

    private void StoveCounter_OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e)
    {
        float burnShowProgressAmount = .5f;
        playWarningSound = stoveCounter.IsFried() && e.progressNormalized > burnShowProgressAmount;
    }

    private void StoveCounter_OnStoveBeingUsed(object sender, StoveCounter.OnStoveBeingUsedEventArgs e)
    {
        if (e.stoveBeingUsed)
            audioSource.Play();
        else
            audioSource.Pause();
    }
}
