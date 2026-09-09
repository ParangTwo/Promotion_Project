using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("추적 대상")]
    [SerializeField] private Transform target; // Player Transform 연결
    [SerializeField] private float smoothSpeed = 5f;

    [Header("맵 이동 제한 범위 (Min / Max)")]
    [SerializeField] private bool useBounds = false;
    [SerializeField] private Vector2 minBounds; // 맵의 좌측 하단 좌표
    [SerializeField] private Vector2 maxBounds; // 맵의 우측 상단 좌표

    private void LateUpdate()
    {
        if (target == null) return;

        // 플레이어의 X, Y 좌표 추적 (카메라 Z값 -10 고정)
        Vector3 targetPosition = new Vector3(target.position.x, target.position.y, transform.position.z);

        // 맵 범위 제한이 켜져있다면 카메라 이동 위치 가두기
        if (useBounds)
        {
            targetPosition.x = Mathf.Clamp(targetPosition.x, minBounds.x, maxBounds.x);
            targetPosition.y = Mathf.Clamp(targetPosition.y, minBounds.y, maxBounds.y);
        }

        // 부드러운 카메라 이동
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
    }
}