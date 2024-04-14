using System.Collections.Generic;
using UnityEngine;

public class AttackSpawner : MonoBehaviour
{
    public static AttackSpawner instance;
    public static Dictionary<int, Attack> attackObjects = new Dictionary<int, Attack>();
    private static int nextAttackObjectID = 1;

    public GameObject playerBasicAttackPrefab;
    public GameObject enemyBasicAttackPrefab;

    public void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Debug.Log("[Error]: AttackSpawner Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    public void DestroyAttackObject(int _attackObjectID)
    {
        if (attackObjects.ContainsKey(_attackObjectID) == true)
        {
            ServerSend.AttackDestroyed(attackObjects[_attackObjectID]);

            Destroy(attackObjects[_attackObjectID].gameObject);
            attackObjects.Remove(_attackObjectID);
        }
    }

    public void BasicAttackSpawned(int _creatorID, string creatorType, string attackType, Vector3 _shootingPosition, Quaternion _shootingRotation)
    {
        int attackObjectID = nextAttackObjectID;
        nextAttackObjectID++;

        GameObject basicAttackObject = null;
        if (creatorType.Equals("Player"))
        {
            if (attackType.Equals("basicAttack"))
                basicAttackObject = Instantiate(playerBasicAttackPrefab, _shootingPosition, _shootingRotation);
        }
        else if (creatorType.Equals("Enemy"))
        {
            if (attackType.Equals("basicAttack"))
                basicAttackObject = Instantiate(enemyBasicAttackPrefab, _shootingPosition, _shootingRotation);
        }

        basicAttackObject.GetComponent<Attack>().Initialize(_creatorID, creatorType, attackType, attackObjectID, basicAttackObject.transform.forward);
        attackObjects.Add(attackObjectID, basicAttackObject.GetComponent<Attack>());

        ServerSend.AttackSpawned(attackObjectID, creatorType, attackType, _shootingPosition, _shootingRotation);
    }
}