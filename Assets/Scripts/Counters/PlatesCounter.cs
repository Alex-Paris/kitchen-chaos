using System;
using System.Collections;
using UnityEngine;

public class PlatesCounter : BaseCounter
{
    public event EventHandler OnPlateSpawned;
    public event EventHandler OnPlateRemoved;

    [SerializeField] private KitchenObjectSO plateKitchenObjectSO;

    private Coroutine coroutine;
    private readonly float spawnPlateWaitTime = 4f;
    private int plateSpawnedAmount;
    private readonly int plateSpawnedAmountMax = 4;

    private void Start()
    {
        StartCoroutine();
    }

    public override void Interact(Player player)
    {
        // Verify if player isn't holding anything
        if (player.KitchenObject) return;

        // Verify if there's plate for to give to the player
        if (plateSpawnedAmount == 0) return;

        plateSpawnedAmount--;

        KitchenObject.SpawnKitchenObject(plateKitchenObjectSO, player);

        OnPlateRemoved?.Invoke(this, EventArgs.Empty);

        StartCoroutine(); // Give start to Coroutine to produce another plate
    }

    private IEnumerator CoroutineProcess()
    {
        while (plateSpawnedAmount < plateSpawnedAmountMax)
        {
            yield return new WaitForSeconds(spawnPlateWaitTime); // Wait for time before including new plate
            
            if (!LevelGameManager.Instance.IsGamePlaying()) continue;

            plateSpawnedAmount++;

            OnPlateSpawned?.Invoke(this, EventArgs.Empty);
        }

        StopCoroutine();
    }

    public void StartCoroutine()
    {
        // If coroutine already being executed, than let it go
        if (coroutine != null) return;

        coroutine = StartCoroutine(CoroutineProcess());
    }

    public void StopCoroutine()
    {
        if (coroutine == null) return;

        StopCoroutine(coroutine); // Stop the coroutine
        coroutine = null;
    }
}
