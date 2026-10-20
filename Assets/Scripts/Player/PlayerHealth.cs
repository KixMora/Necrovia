using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public float maxHp = 100f;
    private float currentHp;

    private bool isDead = false;
    // private DeathScreenUI deathScreenUI;

    void Awake()
    {
        currentHp = maxHp;
        // deathScreenUI = FindFirstObjectByType<DeathScreenUI>();
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHp -= damage;
        currentHp = Mathf.Max(currentHp, 0f);

        if (DamageBloodOverlay.Instance != null)
            DamageBloodOverlay.Instance.ShowDamage();

        Debug.Log("Player took damage: " + damage + " | HP: " + currentHp);

        if (currentHp <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("Player died");

        PlayerMovement movement = GetComponent<PlayerMovement>();
        if (movement != null)
            movement.enabled = false;

        PlayerShooting shooting = GetComponent<PlayerShooting>();
        if (shooting != null)
            shooting.enabled = false;

        if (MissionManager.Instance != null && MissionManager.Instance.SelectedMission != null)
        {
            AnalyticsManager.Instance?.TrackMissionFail(MissionManager.Instance.SelectedMission);
        }

        // if (deathScreenUI != null)
        //     deathScreenUI.ShowDeathScreen();
        if (MissionResultUI.Instance != null)
        {
            MissionResultUI.Instance.ShowFailed();
        }
    }

    public float GetCurrentHp()
    {
        return currentHp;
    }

    public float GetMaxHp()
    {
        return maxHp;
    }

    public float GetHealthPercent()
    {
        if (maxHp <= 0f) return 0f;
        return currentHp / maxHp;
    }

    public bool IsDead()
    {
        return isDead;
    }
}