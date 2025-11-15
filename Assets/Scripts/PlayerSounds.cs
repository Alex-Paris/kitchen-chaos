using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    private Player player;
    private float footstepTimer;
    private readonly float footstepTimerMax = .1f;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void Update()
    {
        if (!player.IsWalking) return;

        footstepTimer -= Time.deltaTime;

        if (footstepTimer > 0f) return;

        footstepTimer = footstepTimerMax;

        float volume = 1f;
        SoundManager.Instance.PlayFootstepsSount(player.transform.position, volume);
    }
}
