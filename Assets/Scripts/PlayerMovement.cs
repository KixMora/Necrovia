using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 6f;
    private Animator animator;

    private Rigidbody rb;
    private Vector3 moveInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        moveInput = Vector3.zero;

        if (Keyboard.current == null) return;

        if (Keyboard.current.wKey.isPressed) moveInput += Vector3.forward;
        if (Keyboard.current.sKey.isPressed) moveInput += Vector3.back;
        if (Keyboard.current.aKey.isPressed) moveInput += Vector3.left;
        if (Keyboard.current.dKey.isPressed) moveInput += Vector3.right;

        moveInput = moveInput.normalized;

        if (animator != null)
            animator.SetFloat("Speed", moveInput.magnitude);
    }


    void FixedUpdate()
    {
        // Движение по XZ
        Vector3 v = rb.linearVelocity;
        v.x = moveInput.x * moveSpeed;
        v.z = moveInput.z * moveSpeed;
        v.y = 0f; // чтобы не "прыгало" по Y
        rb.linearVelocity = v;

        // Поворот тоже через Rigidbody (важно!)
        if (moveInput.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveInput, Vector3.up);
            rb.MoveRotation(targetRot);
        }
    }


}
