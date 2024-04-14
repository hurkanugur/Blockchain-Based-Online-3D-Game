using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int projectileID;
    public Rigidbody rigidBody;
    public int creatorID; //PLAYER WHO THROW THE PROJECTILE
    public string creatorType;
    private readonly float initialForceMagnitude = 800.0f;
    private readonly float explosionRadius = 15.0f;
    private float explosionDamage;
    private readonly float projectileTimer = 5.0f;

    //DESTROYING COORDINATE LIMITS
    private readonly float worldLimit_Z = 100.0f;
    private readonly float worldLimit_Y = 100.0f;
    private readonly float[,] worldLimit_X = new float[,] { { 90f, 110f }, { 290f, 310f }, { 490f, 510f } };

    //INITIALIZES THE PROJECTILE
    public void Initialize(string _creatorType, int _creatorIDWhoThrewItem, int _projectileObjectID, Vector3 _initialDirection)
    {
        creatorType = _creatorType;
        creatorID = _creatorIDWhoThrewItem;
        projectileID = _projectileObjectID;

        if (creatorType.Equals("Player"))
            explosionDamage = 200.0f + (Server.clientDictionary[creatorID].player.level * 10);
        else if (creatorType.Equals("Enemy"))
        {
            explosionDamage = 100.0f;
            rigidBody.useGravity = false;
        }


        Vector3 initialForce = _initialDirection * initialForceMagnitude;

        rigidBody.AddForce(initialForce); //ADD INITIAL FORCE TO THROW THE THROWABLE ITEM
        StartCoroutine(ExplodeAfterTime());
    }

    public void Update()
    {
        //IF THE ENEMY IS STILL ALIVE, MAKE FLYING ENEMYS' ATTACKS WILL BE NOT STABLE (GOES RANDOMLY LEFT/RIGHT)
        if (EnemySpawner.enemyObjects.ContainsKey(creatorID))
        {
            if (EnemySpawner.enemyObjects[creatorID].enemyWorldName.Equals("Mirror"))
            {
                if (Random.Range(0, 2) % 2 == 0)
                    gameObject.GetComponent<Rigidbody>().AddForce(gameObject.transform.right * 2000 * Time.deltaTime, ForceMode.Force);
                else
                    gameObject.GetComponent<Rigidbody>().AddForce(-gameObject.transform.right * 2000 * Time.deltaTime, ForceMode.Force);
            }
        }

        //IF THE PROJECTILE REACHES AT THE LIMIT OF THE MAP, DELETE THEM
        if ((transform.position.y >= worldLimit_Y || transform.position.y <= -worldLimit_Y)
            || (transform.position.z >= worldLimit_Z || transform.position.z <= -worldLimit_Z)
            || (transform.position.x <= -90 || transform.position.x >= 690) //MOST LEFT & RIGHT LIMITS
            || (transform.position.x >= worldLimit_X[0, 0] && transform.position.x < worldLimit_X[0, 1])
            || (transform.position.x >= worldLimit_X[1, 0] && transform.position.x < worldLimit_X[1, 1])
            || (transform.position.x >= worldLimit_X[2, 0] && transform.position.x < worldLimit_X[2, 1]))
        {
            Explode();
        }
        else //ELSE SEND EVERYONE PROJECTILE'S POSITION
        {
            ServerSend.ProjectilePosition(this);
        }

    }

    //IF THE PROJECTILE HITS SOMETHING, IT WILL EXPLODE IMMEDIATELLY
    public void OnCollisionEnter(Collision collision)
    {
        bool destroyProjectileObject = true;
        if (collision.collider.CompareTag("Player") && collision.collider.name.Equals("Shield"))
        {
            //IF SHIELD ID IS THE SAME AS PLAYER ID, THAT MEANS PLAYER SPAWNED PROJECTILE FROM INSIDE OF THE SHIELD
            if (collision.collider.GetComponent<ShieldManager>().shieldID == creatorID)
            {
                //DO NOTHING (BECAUSE IT MEANS THAT THE PLAYER SHOOTS FROM INSIDE OF HIS SHIELD
                destroyProjectileObject = false;
            }
        }
        //DO NOT DESTROY THE PROJECTILE OBJECT WHEN IT COLLIDES WITH ANOTHER ATTACK OBJECTS
        else if (collision.collider.CompareTag("EnemyBasicAttack") || collision.collider.CompareTag("PlayerBasicAttack") || collision.collider.CompareTag("PlayerBasicBomb"))
        {
            destroyProjectileObject = false;
        }

        if (destroyProjectileObject == true)
            Explode();
    }

    //AFTER THROWING THE PROJECTILE AND NOT COLLIDE ANYTHING, IT WILL EXPLODE AFTER X MINUTES
    private IEnumerator ExplodeAfterTime()
    {
        yield return new WaitForSeconds(projectileTimer);
        Explode();
    }

    //WHEN THE PROJECTILE COLLIDE WITH SOMETHING, IT WILL DAMAGE TO NEARBY PLAYERS 
    private void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider searchCollider in colliders)
        {
            Vector3 directionVector = searchCollider.transform.position - this.transform.position;
            if (creatorType.Equals("Player"))
            {
                if (searchCollider.CompareTag("Player")) //THE PLAYERS TOUCHED THE BOMB WILL BE AFFECTED!
                {
                    //IF THE PROJECTILE HITS THE SHIELD, NOT THE PLAYER
                    if (searchCollider.name.Equals("Shield"))
                    {
                        //DO NOTHING TO THE SHIELD
                    }
                    //IF THE PROJECTILE HITS THE PLAYER'S HIMSELF
                    else
                    {
                        //IF SHIELD IS NOT ACTIVATED, THEN HIT THE PLAYER
                        if (searchCollider.GetComponent<Player>().shieldManager.isShieldActivated == false)
                        {
                            searchCollider.GetComponent<Player>().TakeDamage(explosionDamage, creatorID);
                            searchCollider.GetComponent<ForceManager>().AddHukoImpact(directionVector, 50); //PUSH AWAY
                        } 
                    }
                }
                else if (searchCollider.CompareTag("Enemy"))
                {
                    searchCollider.GetComponent<Enemy>().TakeDamage(explosionDamage, creatorID);
                    searchCollider.GetComponent<ForceManager>().AddHukoImpact(directionVector, 50); //PUSH AWAY
                }

            }
            else if (creatorType.Equals("Enemy"))
            {
                if (searchCollider.CompareTag("Player")) //THE PLAYERS TOUCHED THE BOMB WILL BE AFFECTED!
                {
                    if (searchCollider.name.Equals("Shield"))
                    {
                        //DO NOTHING TO THE SHIELD
                    }
                    else
                    {
                        //IF SHIELD IS NOT ACTIVATED, THEN HIT THE PLAYER
                        if (searchCollider.GetComponent<Player>().shieldManager.isShieldActivated == false)
                        {
                            searchCollider.GetComponent<Player>().TakeDamage(explosionDamage, -1); //(-1) ID REPRESENTS THAT AN ENEMY KILLED PLAYER
                            searchCollider.GetComponent<ForceManager>().AddHukoImpact(directionVector, 50); //PUSH AWAY
                        }
                    }
                }
            }
        }
        //NOTIFY THE CLIENT TO OPERATE EXPLOSION EFFECT AND THEN DELETE THE BOMB OBJECT
        ProjectileSpawner.instance.DestroyProjectileObject(projectileID);
    }
}