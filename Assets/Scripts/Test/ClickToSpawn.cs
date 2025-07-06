using UnityEngine;

public class SpawnNextToAllTowers : MonoBehaviour
{
    [Header("生成设置")]
    public GameObject prefabToSpawn;    // 要生成的新物体
    public float spawnOffset = 2f;       // 距离塔的偏移量

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)) // 按下 K 键
        {
            SpawnBesideAllTowers();
        }
    }

    void SpawnBesideAllTowers()
    {
        // 查找所有带 Tower 标签的物体
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");

        foreach (GameObject tower in towers)
        {
            if (tower != null)
            {
                // 随机一个水平方向
                Vector3 spawnDirection = Random.onUnitSphere;
                spawnDirection.y = 0f;
                spawnDirection.Normalize();

                Vector3 spawnPosition = tower.transform.position + spawnDirection * spawnOffset;

                // 生成预制体
                if (prefabToSpawn != null)
                {
                    Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
                }
                else
                {
                    Debug.LogWarning("没有设置要生成的Prefab！");
                }
            }
        }
    }
}
