using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    private void Start()
    {
        string spawnName = PlayerSpawnData.nextSpawnPointName;
        if (string.IsNullOrEmpty(spawnName)) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        GameObject spawnPoint = GameObject.Find(spawnName);
        if (spawnPoint == null) return;

        player.transform.position = spawnPoint.transform.position;

        // 用完清掉
        PlayerSpawnData.nextSpawnPointName = "";
    }
}
