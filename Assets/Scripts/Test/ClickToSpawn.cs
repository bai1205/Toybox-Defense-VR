using UnityEngine;

public class SpawnNextToAllTowers : MonoBehaviour
{
    [Header("��������")]
    public GameObject prefabToSpawn;    // Ҫ���ɵ�������
    public float spawnOffset = 2f;       // ��������ƫ����

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)) // ���� K ��
        {
            SpawnBesideAllTowers();
        }
    }

    void SpawnBesideAllTowers()
    {
        // �������д� Tower ��ǩ������
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");

        foreach (GameObject tower in towers)
        {
            if (tower != null)
            {
                // ���һ��ˮƽ����
                Vector3 spawnDirection = Random.onUnitSphere;
                spawnDirection.y = 0f;
                spawnDirection.Normalize();

                Vector3 spawnPosition = tower.transform.position + spawnDirection * spawnOffset;

                // ����Ԥ����
                if (prefabToSpawn != null)
                {
                    Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
                }
                else
                {
                    Debug.LogWarning("û������Ҫ���ɵ�Prefab��");
                }
            }
        }
    }
}
