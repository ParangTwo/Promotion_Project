using UnityEngine;
using UnityEngine.InputSystem; // 신버전 Input System 사용

public class WeaponController : MonoBehaviour
{
    [Header("무기 연결")]
    [SerializeField] private Transform weaponPivot; // 무기 회전 중심
    [SerializeField] private Transform muzzle;      // 총알 발사 위치
    [SerializeField] private GameObject bulletPrefab; // 총알 프리팹

    [Header("사격 설정")]
    [SerializeField] private float fireRate = 0.2f;  // 발사 간격(초)
    [SerializeField] private float bulletSpeed = 15f;

    private float nextFireTime = 0f;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Mouse.current == null) return;

        RotateWeaponToMouse();

        // 신버전 마우스 좌클릭 감지 (Mouse.current.leftButton)
        if (Mouse.current.leftButton.isPressed && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            Shoot();
        }
    }

    // 마우스 커서 방향으로 무기 회전
    private void RotateWeaponToMouse()
    {
        // 신버전 마우스 커서 위치 가져오기
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(mouseScreenPos);

        Vector2 aimDirection = (mousePos - weaponPivot.position).normalized;

        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        weaponPivot.rotation = Quaternion.Euler(0, 0, angle);

        // 캐릭터가 좌측을 볼 때 무기가 뒤집히지 않도록 Y축 반전
        Vector3 localScale = Vector3.one;
        if (angle > 90f || angle < -90f)
        {
            localScale.y = -1f;
        }
        else
        {
            localScale.y = 1f;
        }
        weaponPivot.localScale = localScale;
    }

    private void Shoot()
    {
        if (bulletPrefab == null || muzzle == null) return;

        // 총알 생성 및 날아가기
        GameObject bullet = Instantiate(bulletPrefab, muzzle.position, muzzle.rotation);
        if (bullet.TryGetComponent(out Rigidbody2D rb))
        {
            rb.linearVelocity = muzzle.right * bulletSpeed;
        }
    }
}