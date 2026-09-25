using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemySpawner : MonoBehaviour
{
    [Header("스폰 대상 설정")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Tilemap floorTilemap;

    [Header("스폰 거리 및 제한 설정")]
    [SerializeField] private float minSpawnDistance = 3f;
    [SerializeField] private float maxSpawnDistance = 8f;
    [SerializeField] private float spawnInterval = 1.0f;

    [Header("스테이지 목표 적 수")]
    [SerializeField] private int totalSpawnTarget = 20;
    private int spawnedCount = 0;
    private int killedCount = 0;

    [Header("시도 횟수 제한")]
    [SerializeField] private int maxAttempts = 50;

    private void Start()
    {
        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
            }
            else
            {
                PlayerMovement player = FindFirstObjectByType(typeof(PlayerMovement)) as PlayerMovement;
                if (player != null) playerTransform = player.transform;
            }
        }

        UpdateEnemyUI();
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (spawnedCount < totalSpawnTarget)
        {
            yield return new WaitForSeconds(spawnInterval);
            TrySpawnEnemy();
        }
    }

    private void TrySpawnEnemy()
    {
        if (enemyPrefab == null || playerTransform == null || floorTilemap == null) return;

        for (int i = 0; i < maxAttempts; i++)
        {
            Vector2 randomDir = Random.insideUnitCircle.normalized;
            float randomDist = Random.Range(minSpawnDistance, maxSpawnDistance);
            Vector3 spawnPosition = playerTransform.position + (Vector3)(randomDir * randomDist);

            Vector3Int cellPosition = floorTilemap.WorldToCell(spawnPosition);

            if (floorTilemap.HasTile(cellPosition))
            {
                Vector3 finalSpawnPos = floorTilemap.GetCellCenterWorld(cellPosition);
                GameObject enemyObj = Instantiate(enemyPrefab, finalSpawnPos, Quaternion.identity);

                // 스폰된 적 사망 시 이벤트를 연동 (typeof 방식으로 안전하게 컴포넌트 탐색)
                EnemyHealth enemyHealth = enemyObj.GetComponent(typeof(EnemyHealth)) as EnemyHealth;
                if (enemyHealth != null)
                {
                    enemyHealth.OnEnemyDied += OnEnemyKilled;
                }

                spawnedCount++;
                return; // 💡 void 함수이므로 return으로 루프 종료
            }
        }
    }

    private void OnEnemyKilled()
    {
        killedCount++;
        UpdateEnemyUI();
    }

    private void UpdateEnemyUI()
    {
        int remaining = totalSpawnTarget - killedCount;
        if (remaining < 0) remaining = 0;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateRemainingEnemyText(remaining);
        }
    }
}