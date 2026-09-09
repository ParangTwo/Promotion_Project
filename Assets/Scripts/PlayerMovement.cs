using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("이동 설정")]
    [SerializeField] private float moveSpeed = 7f;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // WASD 키 입력 감지
        if (Keyboard.current != null)
        {
            float moveX = 0f;
            float moveY = 0f;

            if (Keyboard.current.aKey.isPressed) moveX -= 1f; // A: 좌측
            if (Keyboard.current.dKey.isPressed) moveX += 1f; // D: 우측
            if (Keyboard.current.sKey.isPressed) moveY -= 1f; // S: 하단
            if (Keyboard.current.wKey.isPressed) moveY += 1f; // W: 상단

            moveInput = new Vector2(moveX, moveY).normalized;
        }
    }

    private void FixedUpdate()
    {
        // 물리 기반 부드러운 위치 이동
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }
}