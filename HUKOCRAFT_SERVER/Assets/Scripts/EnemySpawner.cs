using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static Dictionary<int, EnemySpawner> enemySpawnerObjects = new Dictionary<int, EnemySpawner>();
    public static Dictionary<int, Enemy> enemyObjects = new Dictionary<int, Enemy>();
    
    public static int nextEnemySpawnerID = 1;
    public int enemySpawnerID;

    private readonly int maxEnemyNumber = 1;
    private int currentEnemyNumber;

    public static int nextEnemyID = 1;

    public GameObject earthEnemyPrefab;
    public GameObject nightmareEnemyPrefab;
    public GameObject mirrorEnemyPrefab;
    public GameObject asylumEnemyPrefab;
    public float enemySpawnfrequency = 10f;

    public void Start()
    {
        currentEnemyNumber = 0;
        enemySpawnerID = nextEnemySpawnerID;
        nextEnemySpawnerID++;
        enemySpawnerObjects.Add(enemySpawnerID, this);

        StartCoroutine(RespawnEnemy());
    }

    public void DestroyEnemyObject(int _enemyObjectID, int _playerIDWhoShoot)
    {
        if (enemyObjects.ContainsKey(_enemyObjectID) == true)
        {
            //BY USING ENEMYHEALTH PACKET, PLAYER WILL DELETE THE ENEMY WITH 0 HEALTH
            ServerSend.EnemyHealth(enemyObjects[_enemyObjectID], _playerIDWhoShoot);

            Destroy(enemyObjects[_enemyObjectID].gameObject);
            enemyObjects.Remove(_enemyObjectID);
            currentEnemyNumber--;
            StartCoroutine(RespawnEnemy());
        }
    }

    public void EnemySpawned(Vector3 _enemyPosition) //RETURNS THE REFERANCE TO THE PLAYER
    {
        int EnemyID = nextEnemyID;
        nextEnemyID++;
        currentEnemyNumber++;
        string enemyWorld = string.Empty;

        GameObject enemyObject = null;
        if (_enemyPosition.x >= -100 && _enemyPosition.x < 100) //EARTH ENEMY
        {
            enemyObject = Instantiate(earthEnemyPrefab, _enemyPosition, Quaternion.identity);
            enemyWorld = "Earth";
        }
        else if (_enemyPosition.x >= 100 && _enemyPosition.x < 300) //NIGHTMARE ENEMY
        {
            enemyObject = Instantiate(nightmareEnemyPrefab, _enemyPosition, Quaternion.identity);
            enemyWorld = "Nightmare";
        }
        else if(_enemyPosition.x >= 300 && _enemyPosition.x < 500)  //MIRROR ENEMY
        {
            enemyObject = Instantiate(mirrorEnemyPrefab, _enemyPosition, Quaternion.identity);
            enemyWorld = "Mirror";
        }
        else if(_enemyPosition.x >= 500 && _enemyPosition.x < 700) //ASYLUM ENEMY
        {
            enemyObject = Instantiate(earthEnemyPrefab, _enemyPosition, Quaternion.identity);
            enemyWorld = "Asylum";
        }
        
        enemyObject.GetComponent<Enemy>().Initialize(EnemyID, enemySpawnerID, enemyWorld);
        enemyObjects.Add(EnemyID, enemyObject.GetComponent<Enemy>());
        ServerSend.SpawnEnemy(enemyObjects[EnemyID]);
    }

    private IEnumerator RespawnEnemy()
    {
        yield return new WaitForSeconds(enemySpawnfrequency);

        if (currentEnemyNumber < maxEnemyNumber)
            EnemySpawned(transform.position);
    }
}