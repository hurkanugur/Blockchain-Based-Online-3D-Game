using UnityEngine;

public class Attack : MonoBehaviour
{
    public int creatorID; //THE PERSON WHO CREATED THE ATTACK SKILL (EITHER ENEMY OR PLAYER)
    public string creatorType; //EITHER PLAYER OR ENEMY
    public string attackType; //basicAttack, ultiAttack etc.
    public int attackObjectID;

    //FOR PLAYER ATTACKS
    private float playerAttackDamageBasic; //50 + (PLAYER'S LEVEL * 2)
    private readonly float playerAttackSpeedBasic = 100000.0f;
    //FOR ENEMY ATTACKS
    private float enemyAttackDamageBasic; //DEPENDS ON THE ENEMY TYPE
    private readonly float enemyAttackSpeedBasic = 50000.0f;

    //DESTROYING COORDINATE LIMITS
    private readonly float worldLimit_Z = 100.0f;
    private readonly float worldLimit_Y = 100.0f;
    private readonly float[,] worldLimit_X = new float[,] { { 90f, 110f }, { 290f, 310f }, { 490f, 510f } };

    public void Initialize(int _creatorID, string _creatorType, string _attackType, int _attackObjectID, Vector3 _movementVector)
    {
        creatorID = _creatorID;
        creatorType = _creatorType;
        attackType = _attackType;
        attackObjectID = _attackObjectID;

        if (creatorType.Equals("Player") == true)
        {
            if (attackType.Equals("basicAttack"))
            {
                playerAttackDamageBasic = 50.0f + (Server.clientDictionary[creatorID].player.level * 2);
                this.gameObject.GetComponent<Rigidbody>().AddForce(_movementVector * playerAttackSpeedBasic * Time.deltaTime, ForceMode.Force);
            }
        }
        else if (creatorType.Equals("Enemy") == true)
        {
            if (attackType.Equals("basicAttack"))
            {
                if (EnemySpawner.enemyObjects[creatorID].enemyWorldName.Equals("Earth"))
                {
                    enemyAttackDamageBasic = 50.0f;
                    this.gameObject.GetComponent<Rigidbody>().AddForce(_movementVector * enemyAttackSpeedBasic * Time.deltaTime, ForceMode.Force);
                }
                else if (EnemySpawner.enemyObjects[creatorID].enemyWorldName.Equals("Nightmare"))
                {
                    enemyAttackDamageBasic = 100.0f;
                    this.gameObject.GetComponent<Rigidbody>().AddForce(_movementVector * enemyAttackSpeedBasic * Time.deltaTime, ForceMode.Force);
                }
                else if (EnemySpawner.enemyObjects[creatorID].enemyWorldName.Equals("Mirror"))
                {
                    //CURRENTLY NOT USED, SINCE THE BOSS ATTACKS WITH BOMBS (PROJECTILE)
                    //enemyAttackDamageBasic = 100.0f; 
                    //this.gameObject.GetComponent<Rigidbody>().AddForce(_movementVector * enemyAttackSpeedBasic * Time.deltaTime, ForceMode.Force);
                }
                else if (EnemySpawner.enemyObjects[creatorID].enemyWorldName.Equals("Asylum"))
                {
                    enemyAttackDamageBasic = 0f;
                    this.gameObject.GetComponent<Rigidbody>().AddForce(_movementVector * enemyAttackSpeedBasic * Time.deltaTime, ForceMode.Force);
                }
            }

        }
    }

    public void Update()
    {
        /*//IF THE ENEMY IS STILL ALIVE, MAKE FLYING ENEMYS' ATTACKS WILL BE NOT STABLE (GOES RANDOMLY LEFT/RIGHT)
        if (EnemySpawner.enemyObjects.ContainsKey(creatorID))
        {
            if (EnemySpawner.enemyObjects[creatorID].enemyWorldName.Equals("Nightmare"))
            {
                if (Random.Range(0, 2) % 2 == 0)
                    gameObject.GetComponent<Rigidbody>().AddForce(gameObject.transform.right * 2000 * Time.deltaTime, ForceMode.Force);
                else
                    gameObject.GetComponent<Rigidbody>().AddForce(-gameObject.transform.right * 2000 * Time.deltaTime, ForceMode.Force);
            }
        }*/

        //IF THE FIREBALL REACHES AT THE LIMIT OF THE MAP, DELETE THEM
        if ((transform.position.y >= worldLimit_Y || transform.position.y <= -worldLimit_Y)
            || (transform.position.z >= worldLimit_Z || transform.position.z <= -worldLimit_Z)
            || (transform.position.x <= -90 || transform.position.x >= 690) //MOST LEFT & RIGHT LIMITS
            || (transform.position.x >= worldLimit_X[0, 0] && transform.position.x < worldLimit_X[0, 1])
            || (transform.position.x >= worldLimit_X[1, 0] && transform.position.x < worldLimit_X[1, 1])
            || (transform.position.x >= worldLimit_X[2, 0] && transform.position.x < worldLimit_X[2, 1]))
        {
            AttackSpawner.instance.DestroyAttackObject(attackObjectID);
        }
        else //ELSE SEND EVERYONE ATTACK'S POSITION
        {
            ServerSend.AttackPosition(this);
        }
    }

    /*public void OnCollisionExit(Collision collision)
    {

    }*/
    public void OnCollisionEnter(Collision collision)
    {
        bool destroyAttackObject = true;
        Vector3 directionVector = this.transform.forward;

        //IF THE BASIC ATTACK HITS, DELETE THE FIREBALL
        if (creatorType.Equals("Player") && collision.collider.tag == "Player")
        {
            if (collision.collider.name.Equals("Shield"))
            {
                if (collision.collider.GetComponent<ShieldManager>().shieldID == creatorID)
                {
                    //DO NOTHING (BECAUSE IT MEANS THAT THE PLAYER SHOOTS FROM INSIDE OF HIS SHIELD
                    destroyAttackObject = false;
                }
            }
            else if (attackType.Equals("basicAttack"))
            {
                collision.collider.GetComponent<ForceManager>().AddHukoImpact(directionVector, 30); //vector, push force
                collision.collider.GetComponent<Player>().TakeDamage(playerAttackDamageBasic, creatorID);
            }

        }
        else if (creatorType.Equals("Player") && collision.collider.tag == "Enemy")
        {
            if (attackType.Equals("basicAttack"))
            {
                collision.collider.GetComponent<ForceManager>().AddHukoImpact(directionVector, 30); //vector, push force
                collision.collider.GetComponent<Enemy>().TakeDamage(playerAttackDamageBasic, creatorID);
            }
        }
        else if (creatorType.Equals("Enemy") && collision.collider.tag == "Player")
        {
            if (collision.collider.name.Equals("Shield"))
            {
                //JUST DESTROY THE GAME OBJECT, DON'T PUSH ANYTHING OR ANYONE
            }
            else if (attackType.Equals("basicAttack"))
            {
                collision.collider.GetComponent<ForceManager>().AddHukoImpact(directionVector, 30); //vector, push force
                collision.collider.GetComponent<Player>().TakeDamage(enemyAttackDamageBasic, -1); //(-1) ID REPRESENTS THAT AN ENEMY KILLED PLAYER
            }
        }
        //DO NOT DESTROY THE PROJECTILE OBJECT WHEN IT COLLIDES WITH ANOTHER ATTACK OBJECTS
        else if (collision.collider.CompareTag("EnemyBasicAttack") || collision.collider.CompareTag("PlayerBasicAttack") || collision.collider.CompareTag("PlayerBasicBomb"))
        {
            destroyAttackObject = false;
        }

        if (destroyAttackObject == true)
            AttackSpawner.instance.DestroyAttackObject(attackObjectID); //NOTIFY CLIENT AND DELETE THE OBJECT
    }
}
