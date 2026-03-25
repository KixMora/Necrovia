using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    public float maxHp = 30f;
    public float hp;

    public Transform target;
    private NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        hp = maxHp;
    }

    void Update()
    {
        if (target != null)
            agent.SetDestination(target.position);
    }

    public void TakeDamage(float dmg)
    {
        hp -= dmg;
        if (hp <= 0f)
            Die();
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
