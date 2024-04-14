using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static Dictionary<int, PlayerManager> playerObjects = new Dictionary<int, PlayerManager>();
    public static Dictionary<int, AttackManager> attackObjects = new Dictionary<int, AttackManager>();
    public static Dictionary<int, ItemSpawner> itemSpawnerObjects = new Dictionary<int, ItemSpawner>();
    public static Dictionary<int, ProjectileManager> projectileObjects = new Dictionary<int, ProjectileManager>();
    public static Dictionary<int, EnemyManager> enemyObjects = new Dictionary<int, EnemyManager>();

    public GameObject mainPlayerPrefab;
    public GameObject otherPlayerPrefab;

    public GameObject itemSpawnerPrefab;

    public GameObject playerBasicBombPrefab;
    public GameObject playerBasicAttackPrefab;
    public GameObject enemyBasicAttackPrefab;

    public GameObject earthEnemyPrefab;
    public GameObject nightmareEnemyPrefab;
    public GameObject mirrorEnemyPrefab;
    public GameObject asylumEnemyPrefab;

    public void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Debug.Log("[Error]: Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    //SPAWNS A PLAYER
    //ID = PLAYER'S ID
    //USERNAME = PLAYER'S NAME
    //POSITION = PLAYER'S STARTING POSITION
    //ROTATION = PLAYER'S STARTING ROTATION
    public void SpawnPlayer(int _playerID, string _playerUsername, Vector3 _playerPosition, Quaternion _playerRotation, string _playerETHAccountAddress)
    {
        GameObject player;
        //IF THE CORRESPONDING PLAYER IS ME, THEN SPAWN MAIN_PLAYER_PREFAB
        if (_playerID == Client.instance.myID) 
            player = Instantiate(mainPlayerPrefab, _playerPosition, _playerRotation);
        //OR ELSE, SPAWN OTHER_PLAYER_PREFAB
        else
            player = Instantiate(otherPlayerPrefab, _playerPosition, _playerRotation);

        player.GetComponent<PlayerManager>().Initialize(_playerID, _playerUsername, _playerETHAccountAddress); //THEN INITIALIZE THE PLAYER
        playerObjects.Add(_playerID, player.GetComponent<PlayerManager>());
        ETHAccount.instance.UpdateOnlinePlayersETHAccounts(_playerID, _playerETHAccountAddress, true);
    }
    public void AttackSpawned(int _attackObjectID, string _creatorType, string _attackType, Vector3 _shootingPosition, Quaternion _shootingRotation)
    {
        GameObject basicAttackObject = null;
        if(_creatorType.Equals("Player"))
        {
            if(_attackType.Equals("basicAttack"))
                basicAttackObject = Instantiate(playerBasicAttackPrefab, _shootingPosition, _shootingRotation);
        }
        else if(_creatorType.Equals("Enemy"))
        {
            if (_attackType.Equals("basicAttack"))
                basicAttackObject = Instantiate(enemyBasicAttackPrefab, _shootingPosition, _shootingRotation);
        }
        
        basicAttackObject.GetComponent<AttackManager>().Initialize(_attackObjectID);
        attackObjects.Add(_attackObjectID, basicAttackObject.GetComponent<AttackManager>());
    }

    public void ItemSpawnerCreated(int _itemSpawnerObjectID, Vector3 _itemSpawnerPosition, bool _itemSpawnerHasItem, string _itemType)
    {
        GameObject itemSpawnerObject = Instantiate(itemSpawnerPrefab, _itemSpawnerPosition, itemSpawnerPrefab.transform.rotation);
        itemSpawnerObject.GetComponent<ItemSpawner>().Initialize(_itemSpawnerObjectID, _itemSpawnerHasItem, _itemType);
        itemSpawnerObjects.Add(_itemSpawnerObjectID, itemSpawnerObject.GetComponent<ItemSpawner>());
    }

    public void ProjectileSpawned(int _projectileObjectID, Vector3 _projectilePosition)
    {
        GameObject projectileObject = Instantiate(playerBasicBombPrefab, _projectilePosition, Quaternion.identity);
        projectileObject.GetComponent<ProjectileManager>().Initialize(_projectileObjectID);
        projectileObjects.Add(_projectileObjectID, projectileObject.GetComponent<ProjectileManager>());
    }

    public void SpawnEnemy(int _enemyID, Vector3 _enemyPosition, float _enemyMaxHP, string _enemyWorld)
    {
        GameObject enemyObject = null;
        if (_enemyWorld.Equals("Earth"))
        {
            enemyObject = Instantiate(earthEnemyPrefab, _enemyPosition, Quaternion.identity);
        }
        else if (_enemyWorld.Equals("Nightmare"))
        {
            enemyObject = Instantiate(nightmareEnemyPrefab, _enemyPosition, Quaternion.identity);
        }
        else if (_enemyWorld.Equals("Mirror"))
        {
            enemyObject = Instantiate(mirrorEnemyPrefab, _enemyPosition, Quaternion.identity);
        }
        else if (_enemyWorld.Equals("Asylum"))
        {
            enemyObject = Instantiate(asylumEnemyPrefab, _enemyPosition, Quaternion.identity);
        }

        enemyObject.GetComponent<EnemyManager>().Initialize(_enemyID, _enemyMaxHP, _enemyWorld);
        enemyObjects.Add(_enemyID, enemyObject.GetComponent<EnemyManager>());
    }
}
