using System;
using UnityEngine;

public enum ScrapEarnSource
{
    ZombieKill,
    WaveCompleted,
    MissionCompleted,
    Debug
}

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }
    public event System.Action<int, ScrapEarnSource> OnScrapEarned;

    private const string CurrencyKey = "PlayerCurrency";

    public int CurrentCurrency { get; private set; }

    public event Action<int> OnCurrencyChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadCurrency();
    }
    public void AddCurrency(int amount, ScrapEarnSource source)
    {
        if (amount <= 0)
            return;

        CurrentCurrency += amount;
        SaveCurrency();

        OnCurrencyChanged?.Invoke(CurrentCurrency);
        OnScrapEarned?.Invoke(amount, source);

        Debug.Log("[CurrencyManager] Added scrap: " + amount + " | Source: " + source + " | Total: " + CurrentCurrency);
    }
    private void LoadCurrency()
    {
        CurrentCurrency = PlayerPrefs.GetInt(CurrencyKey, 0);
        OnCurrencyChanged?.Invoke(CurrentCurrency);

        Debug.Log("[CurrencyManager] Loaded currency: " + CurrentCurrency);
    }
    public void AddCurrency(int amount)
    {
        AddCurrency(amount, ScrapEarnSource.Debug);
    }
    // public void AddCurrency(int amount)
    // {
    //     if (amount <= 0)
    //         return;

    //     CurrentCurrency += amount;
    //     SaveCurrency();

    //     OnCurrencyChanged?.Invoke(CurrentCurrency);

    //     Debug.Log("[CurrencyManager] Added currency: " + amount + ". Total: " + CurrentCurrency);
    // }

    public bool TrySpendCurrency(int amount)
    {
        if (amount <= 0)
            return true;

        if (CurrentCurrency < amount)
        {
            Debug.Log("[CurrencyManager] Not enough currency. Need: " + amount + ", have: " + CurrentCurrency);
            return false;
        }

        CurrentCurrency -= amount;
        SaveCurrency();

        OnCurrencyChanged?.Invoke(CurrentCurrency);

        Debug.Log("[CurrencyManager] Spent currency: " + amount + ". Total: " + CurrentCurrency);
        return true;
    }

    public void SetCurrency(int amount)
    {
        CurrentCurrency = Mathf.Max(0, amount);
        SaveCurrency();

        OnCurrencyChanged?.Invoke(CurrentCurrency);
    }

    public void ResetCurrency()
    {
        CurrentCurrency = 0;
        SaveCurrency();

        OnCurrencyChanged?.Invoke(CurrentCurrency);

        Debug.Log("[CurrencyManager] Currency reset");
    }

    private void SaveCurrency()
    {
        PlayerPrefs.SetInt(CurrencyKey, CurrentCurrency);
        PlayerPrefs.Save();
    }
}