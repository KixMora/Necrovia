using System;
using UnityEngine;

public class WeaponUpgradeManager : MonoBehaviour
{
    public static WeaponUpgradeManager Instance { get; private set; }

    public event Action<WeaponData> OnWeaponUpgraded;

    private const string DamageLevelKeyPrefix = "weapon_damage_level_";
    private const string MagazineLevelKeyPrefix = "weapon_magazine_level_";
    private const string FireRateLevelKeyPrefix = "weapon_firerate_level_";
    private const string ReloadLevelKeyPrefix = "weapon_reload_level_";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public int GetDamageLevel(WeaponData weapon)
    {
        if (weapon == null || string.IsNullOrWhiteSpace(weapon.weaponId))
            return 0;

        string key = DamageLevelKeyPrefix + weapon.weaponId;
        return PlayerPrefs.GetInt(key, 0);
    }

    public int GetCurrentDamage(WeaponData weapon)
    {
        if (weapon == null)
            return 0;

        int level = GetDamageLevel(weapon);
        return weapon.damage + level * weapon.damagePerLevel;
    }

    public int GetNextDamage(WeaponData weapon)
    {
        if (weapon == null)
            return 0;

        int level = GetDamageLevel(weapon);

        if (level >= weapon.maxDamageLevel)
            return GetCurrentDamage(weapon);

        return weapon.damage + (level + 1) * weapon.damagePerLevel;
    }

    public int GetDamageUpgradeCost(WeaponData weapon)
    {
        if (weapon == null)
            return 0;

        int level = GetDamageLevel(weapon);
        return weapon.baseDamageUpgradeCost + level * weapon.baseDamageUpgradeCost;
    }

    public bool CanUpgradeDamage(WeaponData weapon)
    {
        if (weapon == null)
            return false;

        int level = GetDamageLevel(weapon);
        return level < weapon.maxDamageLevel;
    }

    public bool TryUpgradeDamage(WeaponData weapon)
    {
        if (weapon == null)
        {
            Debug.LogWarning("[WeaponUpgradeManager] Weapon is NULL");
            return false;
        }

        if (!CanUpgradeDamage(weapon))
        {
            Debug.Log("[WeaponUpgradeManager] Damage already maxed for: " + weapon.weaponName);
            return false;
        }

        int cost = GetDamageUpgradeCost(weapon);

        if (CurrencyManager.Instance == null)
        {
            Debug.LogError("[WeaponUpgradeManager] CurrencyManager.Instance is NULL");
            return false;
        }

        if (!CurrencyManager.Instance.TrySpendCurrency(cost))
        {
            Debug.Log("[WeaponUpgradeManager] Not enough scrap for upgrade. Cost: " + cost);
            return false;
        }

        int currentLevel = GetDamageLevel(weapon);
        int newLevel = currentLevel + 1;

        string key = DamageLevelKeyPrefix + weapon.weaponId;
        PlayerPrefs.SetInt(key, newLevel);
        PlayerPrefs.Save();

        Debug.Log("[WeaponUpgradeManager] Upgraded damage: " + weapon.weaponName + " to level " + newLevel);

        OnWeaponUpgraded?.Invoke(weapon);

        return true;
    }

    public void ResetWeaponUpgrades(WeaponData weapon)
    {
        if (weapon == null || string.IsNullOrWhiteSpace(weapon.weaponId))
            return;

        PlayerPrefs.DeleteKey(DamageLevelKeyPrefix + weapon.weaponId);
        PlayerPrefs.Save();

        OnWeaponUpgraded?.Invoke(weapon);
    }
    public int GetMagazineLevel(WeaponData weapon)
    {
        if (weapon == null || string.IsNullOrWhiteSpace(weapon.weaponId))
            return 0;

        return PlayerPrefs.GetInt(MagazineLevelKeyPrefix + weapon.weaponId, 0);
    }

    public int GetCurrentMagazineSize(WeaponData weapon)
    {
        if (weapon == null)
            return 0;

        int level = GetMagazineLevel(weapon);
        return weapon.magazineSize + level * weapon.magazinePerLevel;
    }

    public int GetNextMagazineSize(WeaponData weapon)
    {
        if (weapon == null)
            return 0;

        int level = GetMagazineLevel(weapon);

        if (level >= weapon.maxMagazineLevel)
            return GetCurrentMagazineSize(weapon);

        return weapon.magazineSize + (level + 1) * weapon.magazinePerLevel;
    }

    public int GetMagazineUpgradeCost(WeaponData weapon)
    {
        if (weapon == null)
            return 0;

        int level = GetMagazineLevel(weapon);
        return weapon.baseMagazineUpgradeCost + level * weapon.magazineUpgradeCostStep;
    }

    public bool CanUpgradeMagazine(WeaponData weapon)
    {
        if (weapon == null)
            return false;

        return GetMagazineLevel(weapon) < weapon.maxMagazineLevel;
    }

    public bool TryUpgradeMagazine(WeaponData weapon)
    {
        if (weapon == null)
            return false;

        if (!CanUpgradeMagazine(weapon))
            return false;

        int cost = GetMagazineUpgradeCost(weapon);

        if (CurrencyManager.Instance == null)
            return false;

        if (!CurrencyManager.Instance.TrySpendCurrency(cost))
            return false;

        int newLevel = GetMagazineLevel(weapon) + 1;

        PlayerPrefs.SetInt(MagazineLevelKeyPrefix + weapon.weaponId, newLevel);
        PlayerPrefs.Save();

        OnWeaponUpgraded?.Invoke(weapon);

        return true;
    }
    public int GetFireRateLevel(WeaponData weapon)
    {
        if (weapon == null || string.IsNullOrWhiteSpace(weapon.weaponId))
            return 0;

        return PlayerPrefs.GetInt(FireRateLevelKeyPrefix + weapon.weaponId, 0);
    }

    public float GetCurrentFireRate(WeaponData weapon)
    {
        if (weapon == null)
            return 0f;

        int level = GetFireRateLevel(weapon);
        return weapon.fireRate + level * weapon.fireRatePerLevel;
    }

    public float GetNextFireRate(WeaponData weapon)
    {
        if (weapon == null)
            return 0f;

        int level = GetFireRateLevel(weapon);

        if (level >= weapon.maxFireRateLevel)
            return GetCurrentFireRate(weapon);

        return weapon.fireRate + (level + 1) * weapon.fireRatePerLevel;
    }

    public int GetFireRateUpgradeCost(WeaponData weapon)
    {
        if (weapon == null)
            return 0;

        int level = GetFireRateLevel(weapon);
        return weapon.baseFireRateUpgradeCost + level * weapon.fireRateUpgradeCostStep;
    }

    public bool CanUpgradeFireRate(WeaponData weapon)
    {
        if (weapon == null)
            return false;

        return GetFireRateLevel(weapon) < weapon.maxFireRateLevel;
    }

    public bool TryUpgradeFireRate(WeaponData weapon)
    {
        if (weapon == null)
            return false;

        if (!CanUpgradeFireRate(weapon))
            return false;

        int cost = GetFireRateUpgradeCost(weapon);

        if (CurrencyManager.Instance == null)
            return false;

        if (!CurrencyManager.Instance.TrySpendCurrency(cost))
            return false;

        int newLevel = GetFireRateLevel(weapon) + 1;

        PlayerPrefs.SetInt(FireRateLevelKeyPrefix + weapon.weaponId, newLevel);
        PlayerPrefs.Save();

        OnWeaponUpgraded?.Invoke(weapon);

        return true;
    }
    public int GetReloadLevel(WeaponData weapon)
    {
        if (weapon == null || string.IsNullOrWhiteSpace(weapon.weaponId))
            return 0;

        return PlayerPrefs.GetInt(ReloadLevelKeyPrefix + weapon.weaponId, 0);
    }

    public float GetCurrentReloadTime(WeaponData weapon)
    {
        if (weapon == null)
            return 0f;

        int level = GetReloadLevel(weapon);
        float value = weapon.reloadTime - level * weapon.reloadReductionPerLevel;

        return Mathf.Max(weapon.minReloadTime, value);
    }

    public float GetNextReloadTime(WeaponData weapon)
    {
        if (weapon == null)
            return 0f;

        int level = GetReloadLevel(weapon);

        if (level >= weapon.maxReloadLevel)
            return GetCurrentReloadTime(weapon);

        float value = weapon.reloadTime - (level + 1) * weapon.reloadReductionPerLevel;

        return Mathf.Max(weapon.minReloadTime, value);
    }

    public int GetReloadUpgradeCost(WeaponData weapon)
    {
        if (weapon == null)
            return 0;

        int level = GetReloadLevel(weapon);
        return weapon.baseReloadUpgradeCost + level * weapon.reloadUpgradeCostStep;
    }

    public bool CanUpgradeReload(WeaponData weapon)
    {
        if (weapon == null)
            return false;

        return GetReloadLevel(weapon) < weapon.maxReloadLevel;
    }

    public bool TryUpgradeReload(WeaponData weapon)
    {
        if (weapon == null)
            return false;

        if (!CanUpgradeReload(weapon))
            return false;

        int cost = GetReloadUpgradeCost(weapon);

        if (CurrencyManager.Instance == null)
            return false;

        if (!CurrencyManager.Instance.TrySpendCurrency(cost))
            return false;

        int newLevel = GetReloadLevel(weapon) + 1;

        PlayerPrefs.SetInt(ReloadLevelKeyPrefix + weapon.weaponId, newLevel);
        PlayerPrefs.Save();

        OnWeaponUpgraded?.Invoke(weapon);

        return true;
    }
}
