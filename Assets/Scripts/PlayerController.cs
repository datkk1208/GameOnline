using UnityEngine;
using Fusion;

public class PlayerMovement : NetworkBehaviour
{
    private CharacterController _controller;
    public float speed = 2f;
    private Animator _animator;
    public bool jumpPressed;
    float gravity = -9.8f;
    Vector3 _velocity;
    public float jumpForce = 5f;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!HasStateAuthority) return;

        // Bắt nút Space
        if (Input.GetKeyDown(KeyCode.Space) && _controller.isGrounded)
        {
            jumpPressed = true;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority) return;

        // 1. Trọng lực & Nhảy
        if (_controller.isGrounded)
        {
            _velocity.y = -2f; // Ép nhẹ xuống để giữ isGrounded ổn định
            if (jumpPressed)
            {
                _velocity.y = jumpForce;
                _animator.SetTrigger("Jump"); // Gọi animation nhảy
                jumpPressed = false;          // Reset nút
            }
        }
        else
        {
            _velocity.y += gravity * Runner.DeltaTime;
        }

        // 2. Lấy Input
        float inputH = Input.GetAxis("Horizontal");
        float inputV = Input.GetAxis("Vertical");
        Vector3 inputDir = new Vector3(inputH, 0, inputV).normalized;

        _animator.SetFloat("Speed", inputDir.magnitude);

        // 3. Di chuyển
        if (inputDir.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
            Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Runner.DeltaTime);

            Vector3 moveDir = targetRotation * Vector3.forward;
            _controller.Move(moveDir * speed * Runner.DeltaTime);
        }

        // 4. Áp dụng trọng lực
        _controller.Move(_velocity * Runner.DeltaTime);
    }
}