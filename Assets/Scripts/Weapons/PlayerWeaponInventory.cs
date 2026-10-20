using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponInventory : MonoBehaviour
{
    [Header("All Weapons")]
    [SerializeField] private List<WeaponData> allWeapons = new List<WeaponData>();

    [Header("Start")]
    [SerializeField] private int startWeaponIndex = 0;

    private readonly List<WeaponData> ownedWeapons = new List<WeaponData>();

    public WeaponData CurrentWeapon { get; private set; }
    public int CurrentWeaponIndex { get; private set; }

    public event Action<WeaponData> OnWeaponChanged;

    private void Start()
    {
        RefreshOwnedWeapons();

        if (ownedWeapons.Count == 0)
        {
            Debug.LogWarning("[PlayerWeaponInventory] No owned weapons available.");
            OnWeaponChanged?.Invoke(null);
            return;
        }

        EquipWeapon(startWeaponIndex);
    }

    private void BuildOwnedWeaponsList()
    {
        ownedWeapons.Clear();

        if (allWeapons == null || allWeapons.Count == 0)
        {
            Debug.LogWarning("[PlayerWeaponInventory] All Weapons list is empty.");
            return;
        }

        foreach (WeaponData weapon in allWeapons)
        {
            if (weapon == null)
                continue;

            bool owned = false;

            if (WeaponShopManager.Instance != null)
            {
                owned = WeaponShopManager.Instance.IsWeaponOwned(weapon);
            }
            else
            {
                // Если магазина нет, хотя бы стартовые оружия должны быть доступны.
                owned = weapon.unlockedByDefault;
            }

            if (owned)
                ownedWeapons.Add(weapon);
        }

        // Защита: если вдруг ничего не куплено, но есть оружие по умолчанию,
        // добавляем первое unlockedByDefault оружие.
        if (ownedWeapons.Count == 0)
        {
            foreach (WeaponData weapon in allWeapons)
            {
                if (weapon == null)
                    continue;

                if (weapon.unlockedByDefault)
                {
                    ownedWeapons.Add(weapon);
                    break;
                }
            }
        }
    }

    public void RefreshOwnedWeapons()
    {
        WeaponData previousWeapon = CurrentWeapon;

        BuildOwnedWeaponsList();

        if (ownedWeapons.Count == 0)
        {
            CurrentWeapon = null;
            CurrentWeaponIndex = 0;
            OnWeaponChanged?.Invoke(null);
            return;
        }

        if (previousWeapon != null && ownedWeapons.Contains(previousWeapon))
        {
            CurrentWeaponIndex = ownedWeapons.IndexOf(previousWeapon);
            CurrentWeapon = previousWeapon;
        }
        else
        {
            CurrentWeaponIndex = Mathf.Clamp(startWeaponIndex, 0, ownedWeapons.Count - 1);
            CurrentWeapon = ownedWeapons[CurrentWeaponIndex];
        }

        Debug.Log("[PlayerWeaponInventory] Owned weapons count: " + ownedWeapons.Count);
        Debug.Log("[PlayerWeaponInventory] Current weapon: " + CurrentWeapon.weaponName);

        OnWeaponChanged?.Invoke(CurrentWeapon);
    }

    public void EquipWeapon(int index)
    {
        if (ownedWeapons == null || ownedWeapons.Count == 0)
        {
            Debug.LogWarning("[PlayerWeaponInventory] No owned weapons to equip.");
            return;
        }

        if (index < 0 || index >= ownedWeapons.Count)
        {
            Debug.LogWarning("[PlayerWeaponInventory] Invalid weapon index: " + index);
            return;
        }

        CurrentWeaponIndex = index;
        CurrentWeapon = ownedWeapons[index];

        Debug.Log("[PlayerWeaponInventory] Equipped weapon: " + CurrentWeapon.weaponName);

        OnWeaponChanged?.Invoke(CurrentWeapon);
    }

    public void EquipNextWeapon()
    {
        if (ownedWeapons == null || ownedWeapons.Count == 0)
            return;

        int nextIndex = CurrentWeaponIndex + 1;

        if (nextIndex >= ownedWeapons.Count)
            nextIndex = 0;

        EquipWeapon(nextIndex);
    }

    public void EquipPreviousWeapon()
    {
        if (ownedWeapons == null || ownedWeapons.Count == 0)
            return;

        int previousIndex = CurrentWeaponIndex - 1;

        if (previousIndex < 0)
            previousIndex = ownedWeapons.Count - 1;

        EquipWeapon(previousIndex);
    }

    public List<WeaponData> GetAllWeapons()
    {
        return allWeapons;
    }

    public List<WeaponData> GetOwnedWeapons()
    {
        return ownedWeapons;
    }

    public bool HasWeapon(WeaponData weapon)
    {
        if (weapon == null)
            return false;

        return ownedWeapons.Contains(weapon);
    }
}