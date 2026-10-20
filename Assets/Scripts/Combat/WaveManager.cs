using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    
    [Header("Wave Settings")]
    public float timeBetweenWaves = 3f;
    [SerializeField] private int currentWave = 0;
    public bool autoStart = true;
    [Header("Wave Rewards")]
    [SerializeField] private int baseScrapRewardPerWave = 10;
    [SerializeField] private int extraScrapPerWave = 5;

    [Header("References")]
    public ZombieSpawner[] spawners;
    public Transform player;
    public TMP_Text statusText;

    private readonly List<ZombieGroupController> activeGroups = new List<ZombieGroupController>();

    private bool waveInProgress;
    private bool waitingForNextWave;
    private float nextWaveCountdown;

    public int CurrentWave => currentWave;
    public bool WaveInProgress => waveInProgress;
    public bool WaitingForNextWave => waitingForNextWave;
    public float NextWaveCountdown => nextWaveCountdown;
    public int EnemiesAliveInWave => GetAliveZombieCount();

    private void OnEnable()
    {
        ZombieAI.OnAnyZombieDied += HandleAnyZombieDied;
    }

    private void OnDisable()
    {
        ZombieAI.OnAnyZombieDied -= HandleAnyZombieDied;
    }

    private void Start()
    {
        AssignPlayerToSpawners();
        UpdateStatusText();

        if (autoStart)
            StartCoroutine(BeginNextWaveRoutine());
    }

    private void Update()
    {
        if (waitingForNextWave)
        {
            nextWaveCountdown -= Time.deltaTime;

            if (nextWaveCountdown <= 0f)
            {
                waitingForNextWave = false;
                nextWaveCountdown = 0f;
                StartNextWave();
            }
        }
    }

    private void AssignPlayerToSpawners()
    {
        if (spawners == null)
            return;

        for (int i = 0; i < spawners.Length; i++)
        {
            if (spawners[i] != null)
                spawners[i].player = player;
        }
    }

    private IEnumerator BeginNextWaveRoutine()
    {
        waitingForNextWave = true;
        nextWaveCountdown = timeBetweenWaves;
        UpdateStatusText();

        while (nextWaveCountdown > 0f)
            yield return null;

        waitingForNextWave = false;
        StartNextWave();
    }

    public void StartNextWave()
    {
        if (waveInProgress)
            return;

        if (spawners == null || spawners.Length == 0)
        {
            Debug.LogWarning("WaveManager: не назначены spawners.");
            return;
        }

        currentWave++;
        waveInProgress = true;

        activeGroups.Clear();

        for (int i = 0; i < spawners.Length; i++)
        {
            ZombieSpawner spawner = spawners[i];

            if (spawner == null)
                continue;

            spawner.player = player;

            ZombieGroupController group = spawner.SpawnZombieGroup(currentWave);

            if (group != null)
            {
                group.OnGroupCleared += HandleGroupCleared;
                activeGroups.Add(group);
            }
        }

        UpdateStatusText();
    }

    private void HandleGroupCleared(ZombieGroupController clearedGroup)
    {
        if (clearedGroup != null)
            clearedGroup.OnGroupCleared -= HandleGroupCleared;

        UpdateStatusText();

        if (GetAliveZombieCount() <= 0 && waveInProgress)
        {
            // Сохраняем прогресс миссии
            if (MissionRuntimeTracker.Instance != null)
            {
                MissionRuntimeTracker.Instance.SetWaveSurvived(currentWave);
            }

            // Аналитика волны
            if (MissionManager.Instance != null &&
                MissionManager.Instance.SelectedMission != null)
            {
                AnalyticsManager.Instance?.TrackWaveReached(
                    MissionManager.Instance.SelectedMission,
                    currentWave
                );
            }

            // Выдаём награду за волну
            GiveWaveReward();

            Debug.Log("[WaveManager] Wave survived: " + currentWave);

            waveInProgress = false;

            StartCoroutine(BeginNextWaveRoutine());
        }
    }

    private void HandleAnyZombieDied(ZombieAI deadZombie)
    {
        UpdateStatusText();
    }

    private int GetAliveZombieCount()
    {
        int alive = 0;

        for (int i = 0; i < activeGroups.Count; i++)
        {
            ZombieGroupController group = activeGroups[i];

            if (group != null)
                alive += group.AliveCount;
        }

        return alive;
    }
    private int GetWaveScrapReward()
    {
        int waveNumber = Mathf.Max(1, currentWave);

        return baseScrapRewardPerWave +
            (waveNumber - 1) * extraScrapPerWave;
    }
    private void GiveWaveReward()
    {
        int reward = GetWaveScrapReward();

        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.AddCurrency(
                reward,
                ScrapEarnSource.WaveCompleted
            );
        }

        Debug.Log("[WaveManager] Wave reward: +" + reward);
    }
    private void UpdateStatusText()
    {
        if (statusText == null)
            return;

        if (waveInProgress)
        {
            statusText.text = $"Wave: {currentWave}\nZombies Left: {GetAliveZombieCount()}";
        }
        else if (waitingForNextWave)
        {
            statusText.text = $"Next wave in: {Mathf.CeilToInt(nextWaveCountdown)}";
        }
        else if (currentWave <= 0)
        {
            statusText.text = "Prepare yourself";
        }
        else
        {
            statusText.text = $"Wave {currentWave} cleared";
        }
    }
}