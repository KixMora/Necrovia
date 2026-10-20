using System;
using System.Collections.Generic;
using UnityEngine;

public class ZombieGroupController : MonoBehaviour
{
    [Header("Runtime")]
    [SerializeField] private List<ZombieAI> zombies = new List<ZombieAI>();

    private int aliveCount;

    public int CurrentGroupSize => zombies.Count;
    public int AliveCount => aliveCount;
    public bool IsCleared => aliveCount <= 0;

    public event Action<ZombieGroupController> OnGroupCleared;

    public void Initialize(List<ZombieAI> spawnedZombies)
    {
        CleanupSubscriptions();
        zombies.Clear();

        if (spawnedZombies == null)
        {
            aliveCount = 0;
            return;
        }

        for (int i = 0; i < spawnedZombies.Count; i++)
        {
            ZombieAI zombie = spawnedZombies[i];

            if (zombie == null)
                continue;

            zombies.Add(zombie);
            zombie.OnZombieDied += HandleZombieDied;
        }

        aliveCount = zombies.Count;
    }

    public void SetTarget(Transform target)
    {
        for (int i = 0; i < zombies.Count; i++)
        {
            if (zombies[i] != null)
                zombies[i].SetTarget(target);
        }
    }

    private void HandleZombieDied(ZombieAI deadZombie)
    {
        if (deadZombie == null)
            return;

        deadZombie.OnZombieDied -= HandleZombieDied;

        if (aliveCount > 0)
            aliveCount--;

        if (aliveCount <= 0)
            OnGroupCleared?.Invoke(this);
    }

    private void CleanupSubscriptions()
    {
        for (int i = 0; i < zombies.Count; i++)
        {
            if (zombies[i] != null)
                zombies[i].OnZombieDied -= HandleZombieDied;
        }
    }

    private void OnDestroy()
    {
        CleanupSubscriptions();
    }
}