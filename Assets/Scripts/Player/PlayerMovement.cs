using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;

    [Header("Mobile Input")]
    [SerializeField] private MobileJoystick moveJoystick;
    [SerializeField] private MobileJoystick aimJoystick;

    [Header("Visual")]
    public Transform visual;

    [Header("Rotation")]
    [SerializeField] private bool rotateByAim = true;
    [SerializeField] private bool rotateByMovementWhenNotAiming = true;

    private Rigidbody rb;
    private Animator animator;
    private Vector3 moveInput;
    private Vector3 aimInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        ReadMobileInput();
        RotatePlayer();
        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void ReadMobileInput()
    {
        moveInput = Vector3.zero;
        aimInput = Vector3.zero;

        if (moveJoystick != null)
        {
            Vector2 move = moveJoystick.Direction;

            moveInput = new Vector3(move.x, 0f, move.y);
            moveInput = Vector3.ClampMagnitude(moveInput, 1f);
        }

        if (aimJoystick != null)
        {
            Vector2 aim = aimJoystick.Direction;

            aimInput = new Vector3(aim.x, 0f, aim.y);
            aimInput = Vector3.ClampMagnitude(aimInput, 1f);
        }
    }

    private void MovePlayer()
    {
        Vector3 delta = moveInput * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + delta);
    }

    private void RotatePlayer()
    {
        if (visual == null)
            return;

        Vector3 lookDirection = Vector3.zero;

        if (rotateByAim && aimInput.sqrMagnitude > 0.01f)
        {
            lookDirection = aimInput;
        }
        else if (rotateByMovementWhenNotAiming && moveInput.sqrMagnitude > 0.01f)
        {
            lookDirection = moveInput;
        }

        if (lookDirection.sqrMagnitude > 0.01f)
        {
            visual.rotation = Quaternion.LookRotation(lookDirection.normalized, Vector3.up);
        }
    }

    private void UpdateAnimator()
    {
        if (animator == null || visual == null)
            return;

        Vector3 localMove = visual.InverseTransformDirection(moveInput);

        animator.SetFloat("MoveX", localMove.x);
        animator.SetFloat("MoveY", localMove.z);
        animator.SetFloat("Speed", moveInput.magnitude);
    }
}