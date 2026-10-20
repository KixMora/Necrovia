using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieSpawner : MonoBehaviour
{
    [Header("Zombie Prefab")]
    public ZombieAI zombiePrefab;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    [Header("Base Group Size Per Point")]
    public int minGroupSizePerPoint = 2;
    public int maxGroupSizePerPoint = 4;

    [Header("Wave Scaling")]
    public int extraZombiesPerPointEveryWaves = 2;
    public int zombiesPerPointGrowth = 1;
    public int addSpawnPointEveryWaves = 2;
    public int maxPointsPerWave = 99;

    [Header("Spawn Distances")]
    public float spawnRadius = 2f;
    public float minDistanceFromPlayer = 10f;

    [Header("References")]
    public Transform player;

    public ZombieGroupController SpawnZombieGroup(int waveNumber)
    {
        if (zombiePrefab == null)
        {
            Debug.LogWarning("ZombieSpawner: zombiePrefab не назначен.");
            return null;
        }

        List<Transform> validPoints = GetValidSpawnPoints();

        if (validPoints.Count == 0)
        {
            Debug.LogWarning("ZombieSpawner: нет подходящих точек спавна.");
            return null;
        }

        List<Transform> pointsForThisWave = SelectPointsForWave(validPoints, waveNumber);

        if (pointsForThisWave.Count == 0)
        {
            Debug.LogWarning("ZombieSpawner: не удалось выбрать точки для волны.");
            return null;
        }

        GameObject groupObject = new GameObject("ZombieGroup");
        groupObject.transform.position = transform.position;

        ZombieGroupController groupController = groupObject.AddComponent<ZombieGroupController>();
        List<ZombieAI> spawnedZombies = new List<ZombieAI>();

        int bonusPerPoint = GetBonusZombiesPerPoint(waveNumber);

        for (int p = 0; p < pointsForThisWave.Count; p++)
        {
            Transform point = pointsForThisWave[p];

            int minCount = minGroupSizePerPoint + bonusPerPoint;
            int maxCount = maxGroupSizePerPoint + bonusPerPoint;

            if (maxCount < minCount)
                maxCount = minCount;

            int groupSize = Random.Range(minCount, maxCount + 1);

            for (int i = 0; i < groupSize; i++)
            {
                Vector3 spawnPos = GetSpawnPositionAroundPoint(point.position);
                ZombieAI zombie = Instantiate(zombiePrefab, spawnPos, Quaternion.identity, groupObject.transform);

                if (player != null)
                    zombie.SetTarget(player);

                spawnedZombies.Add(zombie);
            }
        }

        groupController.Initialize(spawnedZombies);

        if (player != null)
            groupController.SetTarget(player);

        return groupController;
    }

    private int GetBonusZombiesPerPoint(int waveNumber)
    {
        if (extraZombiesPerPointEveryWaves <= 0)
            return 0;

        int waveIndex = Mathf.Max(0, waveNumber - 1);
        return (waveIndex / extraZombiesPerPointEveryWaves) * zombiesPerPointGrowth;
    }

    private List<Transform> SelectPointsForWave(List<Transform> validPoints, int waveNumber)
    {
        List<Transform> shuffled = new List<Transform>(validPoints);
        Shuffle(shuffled);

        int pointsToUse = GetPointsCountForWave(shuffled.Count, waveNumber);

        List<Transform> result = new List<Transform>();

        for (int i = 0; i < pointsToUse && i < shuffled.Count; i++)
            result.Add(shuffled[i]);

        return result;
    }

    private int GetPointsCountForWave(int availablePoints, int waveNumber)
    {
        if (availablePoints <= 0)
            return 0;

        int pointsToUse = 1;

        if (addSpawnPointEveryWaves > 0)
        {
            int waveIndex = Mathf.Max(0, waveNumber - 1);
            pointsToUse += waveIndex / addSpawnPointEveryWaves;
        }

        pointsToUse = Mathf.Min(pointsToUse, availablePoints);
        pointsToUse = Mathf.Min(pointsToUse, maxPointsPerWave);
        pointsToUse = Mathf.Max(pointsToUse, 1);

        return pointsToUse;
    }

    private List<Transform> GetValidSpawnPoints()
    {
        List<Transform> validPoints = new List<Transform>();

        if (spawnPoints == null || spawnPoints.Length == 0)
            return validPoints;

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            Transform point = spawnPoints[i];

            if (point == null)
                continue;

            if (player == null)
            {
                validPoints.Add(point);
                continue;
            }

            float distanceToPlayer = Vector3.Distance(point.position, player.position);

            if (distanceToPlayer >= minDistanceFromPlayer)
                validPoints.Add(point);
        }

        if (validPoints.Count == 0)
        {
            Transform farthestPoint = GetFarthestPoint();
            if (farthestPoint != null)
                validPoints.Add(farthestPoint);
        }

        return validPoints;
    }

    private Transform GetFarthestPoint()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
            return null;

        float farthestDistance = -1f;
        Transform farthestPoint = null;

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            Transform point = spawnPoints[i];

            if (point == null)
                continue;

            if (player == null)
                return point;

            float distanceToPlayer = Vector3.Distance(point.position, player.position);

            if (distanceToPlayer > farthestDistance)
            {
                farthestDistance = distanceToPlayer;
                farthestPoint = point;
            }
        }

        return farthestPoint;
    }

    private Vector3 GetSpawnPositionAroundPoint(Vector3 center)
    {
        const int maxAttempts = 12;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
            Vector3 candidate = center + new Vector3(randomCircle.x, 0f, randomCircle.y);

            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, 2f, NavMesh.AllAreas))
                return hit.position;
        }

        if (NavMesh.SamplePosition(center, out NavMeshHit fallbackHit, 2f, NavMesh.AllAreas))
            return fallbackHit.position;

        return center;
    }

    private void Shuffle(List<Transform> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            Transform temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (spawnPoints == null)
            return;

        Gizmos.color = Color.yellow;

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (spawnPoints[i] == null)
                continue;

            Gizmos.DrawWireSphere(spawnPoints[i].position, spawnRadius);
        }

        if (player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(player.position, minDistanceFromPlayer);
        }
    }
}