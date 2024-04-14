using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum EnemyState
    {
        enemyIdle,
        enemyPatrol,
        enemyChase,
        enemyAttack
    }

    public int enemyID; //CURRENT ENEMY'S ID
    public string enemyWorldName;
    private int enemySpawnerID; //ID OF THE ENEMY SPAWNER OBJECT WHICH SPAWNED THIS ENEMY

    public Player targetPlayer;
    public Transform enemyShootingPlace;
    public CharacterController enemyController;

    private float attackSpeedDelay;
    private float gravity = -9.81f * 4.2f; //GRAVITY THAT AFFECTS THE ENEMY
    private float patrolSpeed = 5.0f; //LOOKING FOR PLAYER WALKING SPEED
    private float chaseSpeed = 8.0f; //PLAYER DETECTED AND BEING CHASED SPEED
    private float detectionRange; //WITHIN THIS RANGE, ENEMY STARTS CHASING
    private float shootRange; //WITHIN THIS RANGE, ENEMY STARTS ATTACKING
    private float patrolDuration = 3f; //WALKING TIME (FOR NOTHING)
    private float idleDuration = 1f; //RESTING TIME (AFTER PATROL)
    private float verticalVelocity = 0;

    private EnemyState enemyState;
    public float enemyCurrentHealth;
    public float enemyMaxHealth; //DEPENDS ON THE ENEMY TYPE
    private float baseHPRecovery; // 1% HP RECOVERY PER SECOND

    private bool isPatrolRoutineRunning;
    private bool isEnemyAttacking = false;

    public int enemyExperience; //DEPENDS ON THE ENEMY TYPE

    //WORLD LIMITS                                           EARTH       NIGHTMARE      MIRROR       ASYLUM 
    private readonly float[,] worldLimit = new float[,] { { -40, 40 }, { 130, 270 }, { 330, 470 }, { 555, 645 } };

    public void Initialize(int _enemyID, int _enemySpawnerID, string _enemyWorldName)
    {
        enemyID = _enemyID;
        enemySpawnerID = _enemySpawnerID;
        enemyWorldName = _enemyWorldName;

        if (enemyWorldName.Equals("Earth"))
        {
            enemyExperience = 5;
            enemyMaxHealth = 200.0f;
        }
        else if (enemyWorldName.Equals("Nightmare"))
        {
            enemyExperience = 20;
            enemyMaxHealth = 2000.0f;
        }
        else if (enemyWorldName.Equals("Mirror"))
        {
            enemyExperience = 1000;
            enemyMaxHealth = 30000.0f;
        }
        else if (enemyWorldName.Equals("Asylum"))
        {
            enemyExperience = 100;
            enemyMaxHealth = 5000.0f;
        }
    }

    public void Start()
    {
        enemyState = EnemyState.enemyPatrol;

        chaseSpeed *= Time.fixedDeltaTime;
        patrolSpeed *= Time.fixedDeltaTime;
        gravity *= Time.fixedDeltaTime * Time.fixedDeltaTime;

        enemyCurrentHealth = enemyMaxHealth;
        baseHPRecovery = enemyCurrentHealth * 0.05f / 100.0f;

        if (enemyWorldName.Equals("Earth"))
        {
            detectionRange = 35.0f;
            shootRange = 25.0f;
            attackSpeedDelay = 0.4f;
        }
        else if (enemyWorldName.Equals("Nightmare"))
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, 15f, gameObject.transform.position.z);
            detectionRange = 60f;
            shootRange = 40f;
            attackSpeedDelay = 0.3f;
        }
        else if (enemyWorldName.Equals("Mirror"))
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, 20f, gameObject.transform.position.z);
            detectionRange = 100f;
            shootRange = 80f;
            attackSpeedDelay = 0.2f;
        }
        else if (enemyWorldName.Equals("Asylum"))
        {
            detectionRange = 0f;
            shootRange = 0f;
            attackSpeedDelay = 1f;
            patrolSpeed = 2.0f;
            chaseSpeed = 0.0f;
            idleDuration = 0.0f;
            patrolDuration = 1.0f;
        }
    }

    public void Update()
    {
        //ACCORDING TO THE STATE, ENEMY BEHAVIOR CHANGES
        switch (enemyState)
        {
            case EnemyState.enemyIdle: //ENEMY STOPS MOVING
                LookForPlayer();
                break;
            case EnemyState.enemyPatrol: //ENEMY WALKS AROUND
                if (!LookForPlayer())
                    Patrol();
                break;
            case EnemyState.enemyChase: //ENEMY CHASES A PLAYER IN HIS DETECTION RANGE
                Chase();
                break;
            case EnemyState.enemyAttack: //ENEMY ATTACKS A PLAYER
                Attack();
                break;
            default:
                break;
        }


        //ENEMY IS BEING RECOVERED
        if ((enemyCurrentHealth > 0.0f) && (enemyCurrentHealth < enemyMaxHealth))
        {
            enemyCurrentHealth += baseHPRecovery;
            ServerSend.EnemyHealth(this, -1);
        }

    }

    private bool LookForPlayer()
    {
        foreach (Client searchClient in Server.clientDictionary.Values)
        {
            if (searchClient.player != null)
            {
                Vector3 playerDistance = searchClient.player.transform.position - this.transform.position;
                if (playerDistance.magnitude <= detectionRange) //IF THE PLAYER IS IN THE DETECTION RANGE
                {
                    targetPlayer = searchClient.player.GetComponent<Player>(); //MAKE CORRESPONDING PLAYER TARGET
                    if (isPatrolRoutineRunning) //STOP THE PATROL COROUTINE (IF IT IS RUNNING)
                    {
                        isPatrolRoutineRunning = false;
                        StopCoroutine(StartPatrol());
                    }
                    //SET THE ENEMY STATE AS CHASE
                    enemyState = EnemyState.enemyChase;
                    return true;

                }
            }
        }
        return false;
    }

    private void Patrol()
    {
        //IF PATROL COROUTINE IS NOT ACTIVE, ACTIVATE IT
        if (!isPatrolRoutineRunning)
            StartCoroutine(StartPatrol());
        //MAKE ENEMY WALK THROUGH SPECIFIC DIRECTION
        Move(transform.forward, patrolSpeed);
    }

    private IEnumerator StartPatrol()
    {
        isPatrolRoutineRunning = true;
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        transform.forward = new Vector3(randomDirection.x, 0f, randomDirection.y); //DETERMINE RANDOM DIRECTION IN 2D

        yield return new WaitForSeconds(patrolDuration); //WALK AROUND FOR X SECONDS

        enemyState = EnemyState.enemyIdle; //THEN STOP AND HAVE A REST

        yield return new WaitForSeconds(idleDuration); //RESTING TIME FOR X SECONDS

        enemyState = EnemyState.enemyPatrol; //REPEAT THE PROCESS
        isPatrolRoutineRunning = false;
    }

    private void Chase()
    {
        if (EnemyCanSeeTarget()) //IF THE ENEMY CAN SEE THE TARGET
        {
            Vector3 playerDirection = targetPlayer.transform.position - this.transform.position;
            if (playerDirection.magnitude <= shootRange) //IF THE TARGET IS IN THE SHOOTING RANGE
                enemyState = EnemyState.enemyAttack; //CHANGE CURRENT STATE TO ATTACK STATE
            else
                Move(playerDirection, chaseSpeed); //ELSE, CHASE THE TARGET
        }
        else
        {
            //IF THE ENEMY CAN'T SEE THE TARGET, STOP CHASING IT, START PATROLLING
            targetPlayer = null;
            enemyState = EnemyState.enemyPatrol;
        }
    }

    private void Attack()
    {
        if (EnemyCanSeeTarget())//IF THE ENEMY CAN SEE THE TARGET
        {
            Vector3 playerDirection = targetPlayer.transform.position - this.transform.position;
            EnemyLooksAtPlayerTarget();
            if (playerDirection.magnitude <= shootRange) //IF THE TARGET IS IN THE SHOOTING RANGE
            {
                if ((isEnemyAttacking == false) || attackSpeedDelay == 0f)
                {
                    isEnemyAttacking = true;
                    StartCoroutine(AttemptShooting());
                } 
            }
            else //ELSE, CHASE THE TARGET
                Move(playerDirection, chaseSpeed);
        }
        else
        {
            //IF THE ENEMY CAN'T SEE THE TARGET, STOP ATTACKING IT, START PATROLLING
            targetPlayer = null;
            enemyState = EnemyState.enemyPatrol;
        }
    }
    private IEnumerator AttemptShooting()
    {
        Shoot(); //ATTACK
        yield return new WaitForSeconds(attackSpeedDelay); //TO CAST THE NEXT ATTACK, WAIT X SECONDS (ATTACK SPEED)
        isEnemyAttacking = false;
    }

    private void EnemyLooksAtPlayerTarget()
    {
        if (enemyWorldName.Equals("Nightmare") || enemyWorldName.Equals("Mirror")) //FLYING ENEMY LOOKS AT THE PLAYER
            this.transform.LookAt(targetPlayer.transform);
        else //WALKING ENEMY LOOKS AT THE PLAYER
        {
            Vector3 direction = (targetPlayer.transform.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            this.transform.rotation = lookRotation;
        }

        enemyShootingPlace.LookAt(targetPlayer.transform);
    }

    private void Shoot()
    {
        if (enemyCurrentHealth <= 0.0f) //IF THE ENEMY IS DEAD, HE IS NOT ALLOWED TO ATTEMPT TO SHOOT
            return;
        else if (targetPlayer != null) //ELSE CREATE THE ATTACK
        {
            //IF THE TARGET EXISTS, LOOK AT HIM AND CHANGE SHOOTING PLACE'S DIRECTION TO AIM IT
            EnemyLooksAtPlayerTarget();
            if(enemyWorldName.Equals("Mirror")) //THE BOSS THROWS BOMB
                ProjectileSpawner.instance.ProjectileSpawned("Enemy", enemyID, enemyShootingPlace.position, enemyShootingPlace.rotation);
            else
            AttackSpawner.instance.BasicAttackSpawned(enemyID, this.tag, "basicAttack", enemyShootingPlace.position, enemyShootingPlace.rotation);
        }
    }

    private bool EnemyCanSeeTarget()
    {
        if (targetPlayer == null) //IF THERE IS NO TARGET, RETURN FALSE
            return false;

        //IF THE TARGET EXISTS, LOOK AT HIM AND CHANGE SHOOTING PLACE'S DIRECTION TO AIM IT
        EnemyLooksAtPlayerTarget();
        ServerSend.EnemyPositionAndRotation(this); //NOTIFY THE CLIENT ABOUT THESE ROTATION CHANGES

        //DRAWS AN INVISIBLE VECTOR FROM enemyShootingPlace TO Target's transform position. (VECTOR'S LENGTH IS detectionRange)
        if (Physics.Raycast(enemyShootingPlace.position, targetPlayer.transform.position - this.transform.position, out RaycastHit _hit, detectionRange))
            if (_hit.collider.CompareTag("Player")) //IF THE VECTOR HITS AN OBSTICLE WITH "PLAYER" TAG, THAT MEANS ENEMY CAN SEE THE TARGET
                return true;
        return false;
    }

    //IF AI FLYS AWAY FROM THE LIMITS, PREVENT IT TO STUCK
    private void PreventAIFallFromTheWorld(float _lowerLimit, float _upperLimit, char limitAxis)
    {
        if(limitAxis == 'z')
        {
            float distance1 = Mathf.Abs(transform.position.z - _lowerLimit);
            float distance2 = Mathf.Abs(transform.position.z - _upperLimit);
            if (distance1 < distance2) transform.position = new Vector3(transform.position.x, transform.position.y, _lowerLimit + 2.0f);
            else transform.position = new Vector3(transform.position.x, transform.position.y, _upperLimit - 2.0f);
        }
        else if(limitAxis == 'x')
        {
            float distance1 = Mathf.Abs(transform.position.x - _lowerLimit);
            float distance2 = Mathf.Abs(transform.position.x - _upperLimit);
            if (distance1 < distance2) transform.position = new Vector3(_lowerLimit + 2.0f, transform.position.y, transform.position.z);
            else transform.position = new Vector3(_upperLimit - 2.0f, transform.position.y, transform.position.z);
        }
    }

    private void Move(Vector3 _movementDirection, float _movementSpeed)
    {
        if (enemyWorldName.Equals("Earth")) //AI WONT CRASH TO THE WALLS
        {
            if (transform.position.z <= -40 || transform.position.z >= 40) //AI WONT CRASH TO THE WALLS
            {
                //IF AI FLYS AWAY FROM THE LIMITS, PREVENT IT TO STUCK
                PreventAIFallFromTheWorld(-40f, 40f, 'z');
                _movementDirection.z = -_movementDirection.z;
            }
            if (transform.position.x <= worldLimit[0, 0] || transform.position.x >= worldLimit[0, 1])
            {
                //IF AI FLYS AWAY FROM THE LIMITS, PREVENT IT TO STUCK
                PreventAIFallFromTheWorld(worldLimit[0, 0], worldLimit[0, 1], 'x');
                _movementDirection.x = -_movementDirection.x;
            }
                
        }
        else if (enemyWorldName.Equals("Nightmare")) //AI WONT CRASH TO THE WALLS
        {
            if (transform.position.z <= -70 || transform.position.z >= 70) //AI WONT CRASH TO THE WALLS
            {
                //IF AI FLYS AWAY FROM THE LIMITS, PREVENT IT TO STUCK
                PreventAIFallFromTheWorld(-70f, 70f, 'z');
                _movementDirection.z = -_movementDirection.z;
            }
            if (transform.position.x <= worldLimit[1, 0] || transform.position.x >= worldLimit[1, 1])
            {
                //IF AI FLYS AWAY FROM THE LIMITS, PREVENT IT TO STUCK
                PreventAIFallFromTheWorld(worldLimit[1, 0], worldLimit[1, 1], 'x');
                _movementDirection.x = -_movementDirection.x;
            }
        }
        else if (enemyWorldName.Equals("Mirror")) //AI WONT CRASH TO THE WALLS
        {
            if (transform.position.z <= -70 || transform.position.z >= 70) //AI WONT CRASH TO THE WALLS
            {
                //IF AI FLYS AWAY FROM THE LIMITS, PREVENT IT TO STUCK
                PreventAIFallFromTheWorld(-70f, 70f, 'z');
                _movementDirection.z = -_movementDirection.z;
            }
            if (transform.position.x <= worldLimit[2, 0] || transform.position.x >= worldLimit[2, 1])
            {
                //IF AI FLYS AWAY FROM THE LIMITS, PREVENT IT TO STUCK
                PreventAIFallFromTheWorld(worldLimit[2, 0], worldLimit[2, 1], 'x');
                _movementDirection.x = -_movementDirection.x;
            }
        }
        else if (enemyWorldName.Equals("Asylum")) //AI WONT CRASH TO THE WALLS
        {
            if (transform.position.z <= -40 || transform.position.z >= 40) //AI WONT CRASH TO THE WALLS
            {
                //IF AI FLYS AWAY FROM THE LIMITS, PREVENT IT TO STUCK
                PreventAIFallFromTheWorld(-40f, 40f, 'z');
                _movementDirection.z = -_movementDirection.z;
            }
            if (transform.position.x <= worldLimit[3, 0] || transform.position.x >= worldLimit[3, 1])
            {
                PreventAIFallFromTheWorld(worldLimit[3, 0], worldLimit[3, 1], 'x');
                _movementDirection.x = -_movementDirection.x;
            }
        }

        //ENEMY MOVES ACCORDING TO THE PHYSIC RULES
        _movementDirection.y = 0f;
        this.transform.forward = _movementDirection;
        Vector3 movementVector = transform.forward * _movementSpeed;

        if (enemyController.isGrounded)
            verticalVelocity = 0f;
        verticalVelocity += gravity;

        if (enemyWorldName.Equals("Nightmare") || enemyWorldName.Equals("Mirror")) //GRAVITY WON'T AFFECT FLYING OBJECTS
            verticalVelocity = 0f;

        movementVector.y = verticalVelocity;

        enemyController.Move(movementVector);

        ServerSend.EnemyPositionAndRotation(this);
    }

    public void TakeDamage(float _receivedDamage, int _playerIDWhoShoot)
    {
        enemyCurrentHealth -= _receivedDamage;
        if (enemyCurrentHealth <= 0.0f) //IF ENEMY'S HEALTH IS <= 0, DESTROY THE ENEMY
        {
            enemyCurrentHealth = 0.0f;
            EnemySpawner.enemySpawnerObjects[enemySpawnerID].DestroyEnemyObject(enemyID, _playerIDWhoShoot);
        }
        else
            ServerSend.EnemyHealth(this, _playerIDWhoShoot);
    }
}
