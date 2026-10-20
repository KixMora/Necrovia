using UnityEngine;

public class ZombieAnimator : MonoBehaviour
{
    public Animator animator;
    private bool isDead = false;

    void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    public void SetMoveSpeed(float speed)
    {
        if (animator == null || isDead) return;
        animator.SetFloat("Speed", speed);
    }

    public void PlayAttack()
    {
        if (animator == null || isDead) return;
        animator.SetTrigger("Attack");
    }

    public void PlayDeath()
    {
        if (animator == null || isDead) return;

        isDead = true;
        animator.SetBool("IsDead", true);
        animator.SetTrigger("Die");
    }
}