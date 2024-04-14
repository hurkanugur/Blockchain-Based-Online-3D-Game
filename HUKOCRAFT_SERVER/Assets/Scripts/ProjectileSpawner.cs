using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpawner : MonoBehaviour  
{
    public static ProjectileSpawner instance;
    public static Dictionary<int, Projectile> projectileObjects = new Dictionary<int, Projectile>();
    private static int nextProjectileObjectID = 1;

    public GameObject basicBombPrefab;

    public void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Debug.Log("[Error]: ProjectileSpawner Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    public void DestroyProjectileObject(int _projectileObjectID)
    {
        if(projectileObjects.ContainsKey(_projectileObjectID) == true)
        {
            ServerSend.ProjectileExploded(projectileObjects[_projectileObjectID]);

            Destroy(projectileObjects[_projectileObjectID].gameObject);
            projectileObjects.Remove(_projectileObjectID);
        }
    }

    public void ProjectileSpawned(string _creatorType, int _creatorIDWhoThrewProjectile, Vector3 _throwingPosition, Quaternion _throwingRotation)
    {
        int projectileObjectID = nextProjectileObjectID;
        nextProjectileObjectID++;

        GameObject basicProjectileObject = Instantiate(basicBombPrefab, _throwingPosition, _throwingRotation);
        basicProjectileObject.GetComponent<Projectile>().Initialize(_creatorType, _creatorIDWhoThrewProjectile, projectileObjectID, basicProjectileObject.transform.forward);
        projectileObjects.Add(projectileObjectID, basicProjectileObject.GetComponent<Projectile>());

        ServerSend.ProjectileSpawned(projectileObjects[projectileObjectID], projectileObjects[projectileObjectID].creatorID);
    }
}
