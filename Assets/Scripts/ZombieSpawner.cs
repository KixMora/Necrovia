using System;
using UnityEngine;
using UnityEngine.AI;

public class ZombieSpawner : MonoBehaviour
{
    [Serializable]
    public class ZombieType
    {
        public string name;
        public GameObject prefab;

        [Tooltip("Чем больше вес — тем чаще выбирается тип.")]
        [Min(0f)] public float weight = 1f;

        [Tooltip("С какой волны этот тип может появляться.")]
        [Min(1)] public int minWave = 1;
    }

    [Header("References")]
    public Transform player;                 // сюда перетащи PlayerRoot
    public Transform[] spawnPoints;          // точки спавна

    [Header("Zombie Types")]
    public ZombieType[] types;               // 3 префаба: normal/strong/brute

    [Header("Wave Settings")]
    [Min(1)] public int currentWave = 1;
    [Min(1)] public int zombiesPerWave = 8;
    [Min(0.05f)] public float timeBetweenSpawns = 0.5f;

    [Header("NavMesh Spawn Fix")]
    public bool snapToNavMesh = true;
    public float navMeshSampleRadius = 2f;

    private int spawnedThisWave = 0;
    private float nextSpawnTime = 0f;

    void Update()
    {
        if (!CanSpawnNow()) return;

        if (Time.time >= nextSpawnTime)
        {
            nextSpawnTime = Time.time + timeBetweenSpawns;
            SpawnOne();
        }
    }

    bool CanSpawnNow()
    {
        if (player == null) return false;
        if (spawnPoints == null || spawnPoints.Length == 0) return false;
        if (types == null || types.Length == 0) return false;
        if (spawnedThisWave >= zombiesPerWave) return false;
        return true;
    }

    void SpawnOne()
    {
        Transform sp = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
        GameObject prefab = PickRandomPrefabForWave(currentWave);
        if (prefab == null) return;

        Vector3 spawnPos = sp.position;
        Quaternion spawnRot = sp.rotation;

        if (snapToNavMesh && NavMesh.SamplePosition(spawnPos, out NavMeshHit hit, navMeshSampleRadius, NavMesh.AllAreas))
            spawnPos = hit.position;

        GameObject z = Instantiate(prefab, spawnPos, spawnRot);

        // Назначаем таргет всем зомби
        ZombieAI ai = z.GetComponent<ZombieAI>();
        if (ai == null) ai = z.GetComponentInChildren<ZombieAI>();
        if (ai != null) ai.target = player;

        spawnedThisWave++;
    }

    GameObject PickRandomPrefabForWave(int wave)
    {
        float total = 0f;

        for (int i = 0; i < types.Length; i++)
        {
            var t = types[i];
            if (t == null || t.prefab == null) continue;
            if (wave < t.minWave) continue;
            if (t.weight <= 0f) continue;
            total += t.weight;
        }

        if (total <= 0f) return null;

        float r = UnityEngine.Random.Range(0f, total);
        float acc = 0f;

        for (int i = 0; i < types.Length; i++)
        {
            var t = types[i];
            if (t == null || t.prefab == null) continue;
            if (wave < t.minWave) continue;
            if (t.weight <= 0f) continue;

            acc += t.weight;
            if (r <= acc)
                return t.prefab;
        }

        // на всякий случай
        return types[0].prefab;
    }

    // Вызывай это, когда хочешь начать новую волну
    public void StartWave(int waveNumber, int countInWave)
    {
        currentWave = Mathf.Max(1, waveNumber);
        zombiesPerWave = Mathf.Max(1, countInWave);
        spawnedThisWave = 0;
        nextSpawnTime = Time.time;
    }

    // Удобно для кнопки/отладки из инспектора
    [ContextMenu("Start Next Wave")]
    public void StartNextWave()
    {
        currentWave++;
        int newCount = zombiesPerWave + 2; // пример роста сложности
        StartWave(currentWave, newCount);
    }
}
