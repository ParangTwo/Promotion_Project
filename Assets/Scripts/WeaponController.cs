using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponController : MonoBehaviour
{
    [Header("무기 연결")]
    [SerializeField] private Transform weaponPivot;
    [SerializeField] private Transform muzzle;
    [SerializeField] private GameObject bulletPrefab;

    [Header("캐릭터 뒤집기 설정")]
    [SerializeField] private SpriteRenderer characterSprite; // 💡 Inspector에서 직접 드래그 연결

    [Header("사격 설정")]
    [SerializeField] private float fireRate = 0.2f;
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

        RotateWeaponAndCharacter();

        if (Mouse.current.leftButton.isPressed && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            Shoot();
        }
    }

    private void RotateWeaponAndCharacter()
    {
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(mouseScreenPos);

        Vector2 aimDirection = (mousePos - weaponPivot.position).normalized;

        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        weaponPivot.rotation = Quaternion.Euler(0, 0, angle);

        bool isLeft = angle > 90f || angle < -90f;

        Vector3 localScale = Vector3.one;
        localScale.y = isLeft ? -1f : 1f;
        weaponPivot.localScale = localScale;

        if (characterSprite != null)
        {
            characterSprite.flipX = isLeft;
        }
    }

    private void Shoot()
    {
        if (bulletPrefab == null || muzzle == null) return;

        GameObject bullet = Instantiate(bulletPrefab, muzzle.position, muzzle.rotation);
        if (bullet.TryGetComponent(out Rigidbody2D rb))
        {
            rb.linearVelocity = muzzle.right * bulletSpeed;
        }
    }
}