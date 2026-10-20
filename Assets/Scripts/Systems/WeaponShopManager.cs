using System;
using UnityEngine;

public class WeaponShopManager : MonoBehaviour
{
    public static WeaponShopManager Instance { get; private set; }

    public event Action<WeaponData> OnWeaponPurchased;

    private const string WeaponOwnedKeyPrefix = "weapon_owned_";

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

    public bool IsWeaponOwned(WeaponData weapon)
    {
        if (weapon == null)
            return false;

        if (weapon.unlockedByDefault)
            return true;

        if (string.IsNullOrWhiteSpace(weapon.weaponId))
        {
            Debug.LogWarning("[WeaponShopManager] Weapon has empty weaponId: " + weapon.weaponName);
            return false;
        }

        string key = WeaponOwnedKeyPrefix + weapon.weaponId;
        return PlayerPrefs.GetInt(key, 0) == 1;
    }

    public bool CanBuyWeapon(WeaponData weapon)
    {
        if (weapon == null)
            return false;

        if (IsWeaponOwned(weapon))
            return false;

        if (CurrencyManager.Instance == null)
            return false;

        return CurrencyManager.Instance.CurrentCurrency >= weapon.buyCost;
    }

    public bool TryBuyWeapon(WeaponData weapon)
    {
        if (weapon == null)
        {
            Debug.LogWarning("[WeaponShopManager] Weapon is NULL");
            return false;
        }

        if (IsWeaponOwned(weapon))
        {
            Debug.Log("[WeaponShopManager] Weapon already owned: " + weapon.weaponName);
            return false;
        }

        if (CurrencyManager.Instance == null)
        {
            Debug.LogError("[WeaponShopManager] CurrencyManager.Instance is NULL");
            return false;
        }

        if (!CurrencyManager.Instance.TrySpendCurrency(weapon.buyCost))
        {
            Debug.Log("[WeaponShopManager] Not enough scrap to buy: " + weapon.weaponName);
            return false;
        }

        SetWeaponOwned(weapon, true);

        Debug.Log("[WeaponShopManager] Purchased weapon: " + weapon.weaponName);

        OnWeaponPurchased?.Invoke(weapon);

        return true;
    }

    public void SetWeaponOwned(WeaponData weapon, bool owned)
    {
        if (weapon == null || string.IsNullOrWhiteSpace(weapon.weaponId))
            return;

        string key = WeaponOwnedKeyPrefix + weapon.weaponId;
        PlayerPrefs.SetInt(key, owned ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void ResetWeaponOwnership(WeaponData weapon)
    {
        if (weapon == null || string.IsNullOrWhiteSpace(weapon.weaponId))
            return;

        string key = WeaponOwnedKeyPrefix + weapon.weaponId;
        PlayerPrefs.DeleteKey(key);
        PlayerPrefs.Save();

        OnWeaponPurchased?.Invoke(weapon);
    }
}