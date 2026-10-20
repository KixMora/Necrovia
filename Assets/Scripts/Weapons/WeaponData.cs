using UnityEngine;

public enum WeaponFireMode
{
    SemiAuto,
    FullAuto
}

public enum WeaponShootType
{
    SingleBullet,
    Shotgun
}

[CreateAssetMenu(menuName = "Necrovia/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("Info")]
    public string weaponId;
    public string weaponName;

    [Header("Visual")]
    public Sprite icon;
    public GameObject weaponModelPrefab;
    [Header("Shop")]
    public bool unlockedByDefault = false;
    public int buyCost = 100;

    [Header("Animation")]
    public AnimatorOverrideController animatorOverride;
    
    [Header("Shooting")]
    public WeaponFireMode fireMode = WeaponFireMode.FullAuto;
    public WeaponShootType shootType = WeaponShootType.SingleBullet;

    public int damage = 10;
    public float fireRate = 8f;
    public float range = 100f;
    
    [Header("Damage Upgrade")]
    public int baseDamageUpgradeCost = 50;
    public int damageUpgradeCostStep = 50;
    public int maxDamageLevel = 5;
    public int damagePerLevel = 3;

    [Header("Magazine Upgrade")]
    public int baseMagazineUpgradeCost = 60;
    public int magazineUpgradeCostStep = 60;
    public int maxMagazineLevel = 5;
    public int magazinePerLevel = 3;

    [Header("Fire Rate Upgrade")]
    public int baseFireRateUpgradeCost = 70;
    public int fireRateUpgradeCostStep = 70;
    public int maxFireRateLevel = 5;
    public float fireRatePerLevel = 0.5f;

    [Header("Reload Upgrade")]
    public int baseReloadUpgradeCost = 70;
    public int reloadUpgradeCostStep = 70;
    public int maxReloadLevel = 5;
    public float reloadReductionPerLevel = 0.15f;
    public float minReloadTime = 0.5f;

    [Header("Shotgun")]
    public int pellets = 7;
    public float spreadAngle = 8f;

    [Header("Ammo")]
    public int magazineSize = 30;
    public float reloadTime = 1.5f;

    [Header("Effects")]
    public GameObject tracerPrefab;
    public float tracerLifeTime = 0.05f;
    public ParticleSystem muzzleFlashPrefab;

    [Header("Audio")]
    public AudioClip shootClip;
    public AudioClip reloadClip;
    public AudioClip emptyClip;
}