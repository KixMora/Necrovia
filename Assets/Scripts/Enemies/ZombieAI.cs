using System;
using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    [Header("Reward")]
    [SerializeField] private int currencyReward = 5;
    [Header("Stats")]
    public int maxHp = 3;
    public int damage = 1;
    public float attackRange = 1.6f;
    public float attackCooldown = 1f;

    [Header("Movement")]
    public float stopDistance = 1.1f;

    [Header("Death")]
    public float destroyDelay = 2.5f;

    [Header("Target")]
    public Transform target;

    private int currentHp;
    private float nextAttackTime;
    private bool isDead;

    private NavMeshAgent agent;
    private PlayerHealth playerHealth;
    private Animator animator;

    public event Action<ZombieAI> OnZombieDied;
    public static event Action<ZombieAI> OnAnyZombieDied;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        currentHp = maxHp;

        if (agent != null)
            agent.stoppingDistance = stopDistance;
    }

    private void Start()
    {
        RefreshTargetReferences();
    }

    private void Update()
    {
        if (isDead)
            return;

        if (target == null)
        {
            TryFindPlayer();
            RefreshTargetReferences();

            if (target == null)
                return;
        }

        if (playerHealth == null)
            RefreshTargetReferences();

        float distance = Vector3.Distance(transform.position, target.position);

        bool inAttackRange = distance <= attackRange;

        if (agent != null && agent.enabled && agent.isOnNavMesh)
        {
            agent.stoppingDistance = stopDistance;

            if (inAttackRange)
                agent.ResetPath();
            else
                agent.SetDestination(target.position);
        }

        if (animator != null)
        {
            float speed = 0f;

            if (!inAttackRange && agent != null)
                speed = agent.velocity.magnitude;

            animator.SetFloat("Speed", speed);
        }

        if (inAttackRange && Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + attackCooldown;

            PlayAttackAnimation();

            if (playerHealth != null)
                playerHealth.TakeDamage(damage);
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        RefreshTargetReferences();
    }

    private void PlayAttackAnimation()
    {
        if (animator == null)
            return;

        animator.SetTrigger("Attack");
    }

    private void RefreshTargetReferences()
    {
        if (target == null)
        {
            playerHealth = null;
            return;
        }

        playerHealth = target.GetComponent<PlayerHealth>();

        if (playerHealth == null)
            playerHealth = target.GetComponentInParent<PlayerHealth>();

        if (playerHealth == null)
            playerHealth = target.GetComponentInChildren<PlayerHealth>();
    }

    private void TryFindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
            target = playerObject.transform;
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead)
            return;

        currentHp -= damageAmount;

        if (currentHp <= 0)
            Die();
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;
        
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.AddCurrency(currencyReward, ScrapEarnSource.ZombieKill);
        }

        if (FloatingScrapTextSpawner.Instance != null)
        {
            FloatingScrapTextSpawner.Instance.Show(transform.position + Vector3.up * 1.5f, currencyReward);
        }

        if (agent != null)
        {
            if (agent.isOnNavMesh)
                agent.ResetPath();

            agent.enabled = false;
        }

        Collider[] colliders = GetComponentsInChildren<Collider>();
        for (int i = 0; i < colliders.Length; i++)
            colliders[i].enabled = false;

        if (animator != null)
        {
            animator.SetFloat("Speed", 0f);
            animator.SetTrigger("Die");
        }
        
        if (MissionRuntimeTracker.Instance != null)
        {
            MissionRuntimeTracker.Instance.AddZombieKill();
        }
        
        OnZombieDied?.Invoke(this);
        OnAnyZombieDied?.Invoke(this);

        Destroy(gameObject, destroyDelay);
    }
}