using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int playerID;
    public string playerUsername;
    public string levelText = "LVL 0";
    public string ETHAccountAddress = "0x";
    public int level = 0;

    public CharacterController characterController;

    //GAME FORCES
    private float gameGravity = -9.81f * 4.2f;
    private float movementSpeed = 15.0f;
    private float jumpSpeed = 20.0f;
    private float verticalVelocity = 0.0f;
    private bool[] movementKeys;

    //PLAYER HEALTH
    public float playerCurrentHealth;
    public float playerCurrentMana;
    public float playerMaxHPMPLimit = 100.0f;
    private readonly float respawnTimer = 5.0f;

    //RECOVERY
    private float baseHPRecovery;
    private float baseMPRecovery;

    //ITEM SPAWNER
    private readonly int inventoryLimit = 9; //PLAYER'S INVENTORY LIMIT
    public int currentItemAmount = 0; //PLAYER'S CURRENT NUMBER OF ITEMS

    //WHEN PLAYER DIES HE WILL GIVE OPPONENT THIS AMOUNT OF EXP
    public int playerDeathExperience;

    //SKILL MANA COSTS
    private float playerBasicAttackManaCost; // CONSUMES 2.5% MANA OF PLAYER'S TOTAL MANA

    //PLAYER'S SHIELD
    public ShieldManager shieldManager;

    public void Initialize(int _playerID, string _playerUsername, string _ethAccountAddress)
    {
        playerID = _playerID;
        playerUsername = _playerUsername;
        ETHAccountAddress = _ethAccountAddress;
        movementKeys = new bool[5];
        shieldManager = transform.Find("Shield").GetComponent<ShieldManager>();
    }

    public void Start()
    {
        gameGravity *= Time.fixedDeltaTime * Time.fixedDeltaTime;
        movementSpeed *= Time.fixedDeltaTime;
        jumpSpeed *= Time.fixedDeltaTime;

        transform.Find("Shield").GetComponent<ShieldManager>().Initialize(playerID);
        shieldManager.ActivateShield("Revive");
    }

    public void PlayerSetNewLevel(string _newLevelText)
    {
        level = int.Parse(_newLevelText.Substring(4)); //--> CONVERT STRING LEVEL TO INT LEVEL

        playerMaxHPMPLimit = level * 100;
        playerCurrentHealth = playerCurrentMana = playerMaxHPMPLimit;
        baseHPRecovery = playerMaxHPMPLimit * 0.03f / 100.0f;
        baseMPRecovery = playerMaxHPMPLimit * 0.05f / 100.0f;
        playerDeathExperience = 10 + level;
        playerBasicAttackManaCost = playerMaxHPMPLimit * 2.5f / 100.0f;
        ServerSend.PlayerHealth(this, playerID);
        ServerSend.PlayerMana(this);
    }

    //PROCESSES PLAYER INPUT AND MOVES THE PLAYER
    public void Update()
    {
        if (playerCurrentHealth <= 0) //IF THE PLAYER IS DEAD, MAKE SURE THAT S/HE CANNOT MOVE!
            return;
        else if (transform.position.y <= -50.0f) //IF THE PLAYER FALLS DOWN FROM THE WORLD, HE WILL DIE
        {
            Die();
            return;
        }

        if (playerCurrentHealth < playerMaxHPMPLimit)
            Recovery(baseHPRecovery, 0.0f); //PLAYER IS BEING RECOVERED
        if (playerCurrentMana < playerMaxHPMPLimit)
            Recovery(0.0f, baseMPRecovery); //PLAYER IS BEING RECOVERED

        Vector2 inputDirection = Vector2.zero;
        if (movementKeys[0])
            inputDirection.y += 1;
        if (movementKeys[1])
            inputDirection.y -= 1;
        if (movementKeys[2])
            inputDirection.x -= 1;
        if (movementKeys[3])
            inputDirection.x += 1;

        Move(inputDirection);

        movementKeys[0] = movementKeys[1] = movementKeys[2] = movementKeys[3] = false;
    }

    public void Recovery(float _HPRecoveryAmount, float _MPRecoveryAmount)
    {
        if ((playerCurrentHealth < playerMaxHPMPLimit) && (_HPRecoveryAmount != 0.0f))
        {
            if ((playerCurrentHealth + _HPRecoveryAmount) <= playerMaxHPMPLimit)
                playerCurrentHealth += _HPRecoveryAmount; //PLAYER GAINS HP
            else
                playerCurrentHealth = playerMaxHPMPLimit;

            ServerSend.PlayerHealth(this, playerID);
        }
        if ((playerCurrentMana < playerMaxHPMPLimit) && (_MPRecoveryAmount != 0.0f))
        {
            if ((playerCurrentMana + _MPRecoveryAmount) <= playerMaxHPMPLimit)
                playerCurrentMana += _MPRecoveryAmount; //PLAYER GAINS MP
            else
                playerCurrentMana = playerMaxHPMPLimit;

            ServerSend.PlayerMana(this);
        }
    }

    

    //CALCULATES THE PLAYER'S DESIRED MOVEMENT DIRECTION AND MOVES HIM
    private void Move(Vector2 _inputDirection)
    {
        try
        {
            Vector3 movementDirection = transform.right * _inputDirection.x + transform.forward * _inputDirection.y;
            movementDirection = movementDirection.normalized * movementSpeed;
            movementDirection.y = verticalVelocity;

            //IF THE CHARACTER TOUCHES TO THE GROUND, RESET HIS VERTICAL VELOCITY
            if (characterController.isGrounded)
            {
                verticalVelocity = 0.0f;
                if (movementKeys[4]) //IF JUMPING KEY IS PRESSED
                    verticalVelocity = jumpSpeed;
            }
            verticalVelocity += gameGravity;
            movementDirection.y = verticalVelocity;
            characterController.Move(movementDirection);

            ServerSend.PlayerPositionToEveryone(this);
            ServerSend.PlayerRotationToEveryoneExceptYourself(this);
        }
        catch (System.Exception) { characterController.enabled = true; }
    }

    //UPDATES THE PLAYER KEY INPUT WITH NEWLY RECEIVED KEY INPUT
    public void MovementDirection(bool[] _movementKeyPressed, Quaternion _rotation)
    {
        movementKeys = _movementKeyPressed; //NEW KEY INPUTS
        transform.rotation = _rotation; //NEW KEY ROTATION
    }

    //PLAYER ATTEMPTS TO ATTACK
    public void AttemptShooting(int _playerID, Vector3 _shootingPosition, Quaternion _shootingRotation)
    {
        if (playerCurrentHealth <= 0.0f || playerCurrentMana < playerBasicAttackManaCost) //IF THE PLAYER IS DEAD, HE IS NOT ALLOWED TO ATTEMPT TO SHOOT
            return;
        else
        {
            AttackSpawner.instance.BasicAttackSpawned(_playerID, this.tag, "basicAttack", _shootingPosition, _shootingRotation);
            playerCurrentMana -= playerBasicAttackManaCost;
            ServerSend.PlayerMana(this);
        }
    }

    //PLAYER ATTEMPTS TO THROW A PROJECTILE
    public void AttemptThrowingItem(Vector3 _throwingPosition, Quaternion _throwingRotation)
    {
        if (playerCurrentHealth <= 0.0f)
            return;
        if (currentItemAmount > 0)
        {
            ProjectileSpawner.instance.ProjectileSpawned("Player", playerID, _throwingPosition, _throwingRotation);
            currentItemAmount--;
        }
    }
    //PLAYER DIES
    public void Die()
    {
        TakeDamage(playerMaxHPMPLimit, playerID);
    }
    //PLAYER TAKES DAMAGE FROM THE OTHERS (IF HE DIES, HE WILL RESPAWN AT THE RESPAWNING POINT
    public void TakeDamage(float _receivedDamage, int _ObjectIDWhoShootPlayer)
    {
        if (playerCurrentHealth <= 0) //IF THE PLAYER HAS ALREADY 0 HEALTH, SKIP
            return;
        else if (shieldManager.playerShieldGameObject.activeInHierarchy)
            return;

        playerCurrentHealth -= _receivedDamage; //TAKES DAMAGE

        if (playerCurrentHealth <= 0) //IF THE PLAYER DIES
        {
            playerCurrentHealth = playerCurrentMana = 0;
            characterController.enabled = false;
            verticalVelocity = 0.0f;
            this.transform.position = NetworkManager.playerInitialSpawnPosition; //RESPAWN POINT
            ServerSend.PlayerMana(this);
            ServerSend.PlayerPositionToEveryone(this);
            StartCoroutine(Respawn()); //IENUMERATOR KINDA THINGS ARE BEING EXECUTED LIKE THIS
        }

        ServerSend.PlayerHealth(this, _ObjectIDWhoShootPlayer); //EVEN IF THE PLAYER DIES, WE WILL NOTIFY THIS VIA PLAYERHEALTH PACKET (BY SENDING 0 HEALTH)
    }

    //PLAYER RESPAWNS AFTER X SECONDS
    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnTimer); //PLAYER STAYS DEAD FOR X SECONDS
        Recovery(playerMaxHPMPLimit, playerMaxHPMPLimit);
        characterController.enabled = true;
        shieldManager.ActivateShield("Revive");
        ServerSend.PlayerRespawned(this);
    }

    //PLAYER TRIES TO PICK AN ITEM FROM ITEM SPAWNERS
    public bool AttemptPickupItem()
    {
        if (currentItemAmount >= inventoryLimit) //IF THE INVENTORY IS FULL, DON'T PICKUP !
            return false;
        //ELSE TAKE THE ITEM
        currentItemAmount++;
        return true;
    }
}