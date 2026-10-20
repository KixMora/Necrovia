using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealthUI : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public Image healthFill;
    public TMP_Text healthText;

    void Update()
    {
        if (playerHealth == null)
            return;

        if (healthFill != null)
            healthFill.fillAmount = playerHealth.GetHealthPercent();

        if (healthText != null)
            healthText.text = "HP: " + Mathf.CeilToInt(playerHealth.GetCurrentHp()) + " / " + Mathf.CeilToInt(playerHealth.GetMaxHp());
    }
}