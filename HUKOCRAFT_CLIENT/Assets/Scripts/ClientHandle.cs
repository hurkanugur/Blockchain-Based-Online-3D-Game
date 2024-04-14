using System.Net;
using UnityEngine;

public class ClientHandle : MonoBehaviour //HANDLE THE PACKETS THAT HAVE COME FROM SERVER
{
    //GET THE WELCOME MESSAGE THAT HAS COME FROM THE SERVER
    public static void PlayerReceivedWelcomeMessage(Packet _packet)
    {
        string serverName = _packet.ReadString();
        string message = _packet.ReadString();
        int myID = _packet.ReadInt();

        Client.instance.myID = myID;
        ClientSend.PlayerWelcomeMessageAsync();
        GameChat.instance.SendChatMessage(serverName, message);

        //SINCE WE HAVE THE CLIENT'S ID, CONNECT UDP CONNECTION
        Client.instance.hukoUDP.Connect(((IPEndPoint)Client.instance.hukoTCP.clientTCPSocket.Client.LocalEndPoint).Port);
    }

    //SPAWN THE PLAYERS' WHO HAVE JUST COME TO THE GAME PACKET RECEIVED
    public static void SpawnPlayer(Packet _packet)
    {
        int clientID = _packet.ReadInt();
        string username = _packet.ReadString();
        Vector3 playerPosition = _packet.ReadVector3();
        Quaternion playerRotation = _packet.ReadQuaternion();
        string playerETHAccountAddress = _packet.ReadString();

        //CHECK IF THE CORRESPONDING OBJECT ID DOES NOT EXIST (YOU CAN ALSO CHECK VIA OBJECT DICTIONARY)
        if (GameManager.playerObjects.ContainsKey(clientID) == false)
        {
            GameManager.instance.SpawnPlayer(clientID, username, playerPosition, playerRotation, playerETHAccountAddress);
            if(GameManager.playerObjects.ContainsKey(Client.instance.myID))
            {
                if (ExperienceManager.instance.isExp100 == true)
                    ClientSend.PlayerLevelUp(ExperienceManager.instance.levelText.text, true);
                else
                    ClientSend.PlayerLevelUp(ExperienceManager.instance.levelText.text, false);
            }
        }
    }

    //RECEIVE PLAYERS' POSITION INFO PACKET RECEIVED
    public static void PlayerPosition(Packet _packet)
    {
        int playerID = _packet.ReadInt();
        Vector3 playerPosition = _packet.ReadVector3();

        //CHECK IF THE CORRESPONDING OBJECT ID EXISTS (YOU CAN ALSO CHECK VIA OBJECT DICTIONARY)
        if (GameManager.playerObjects.TryGetValue(playerID, out PlayerManager playerManager))
            playerManager.transform.position = playerPosition;
    }

    //RECEIVE PLAYERS' ROTATION INFO PACKET RECEIVED
    public static void PlayerRotation(Packet _packet)
    {
        int playerID = _packet.ReadInt();
        Quaternion playerRotation = _packet.ReadQuaternion();

        //CHECK IF THE CORRESPONDING OBJECT ID EXISTS (YOU CAN ALSO CHECK VIA OBJECT DICTIONARY)
        if (GameManager.playerObjects.TryGetValue(playerID, out PlayerManager playerManager))
            playerManager.transform.rotation = playerRotation;
    }

    //PLAYERS WHO DISCONNECTED FROM THE GAME PACKET RECEIVED
    public static void PlayerDisconnected(Packet _packet)
    {
        int disconnectedPlayerID = _packet.ReadInt();

        //CHECK IF THE CORRESPONDING OBJECT ID EXISTS
        if (GameManager.playerObjects.ContainsKey(disconnectedPlayerID) == true)
        {
            Destroy(GameManager.playerObjects[disconnectedPlayerID].gameObject); //USE THAT ID TO DESTROY THAT CLIENT'S GAME OBJECT
            GameManager.playerObjects.Remove(disconnectedPlayerID); //AND REMOVE THE DISCONNECTED PLAYER FROM THE DICTIONARY
            ETHAccount.instance.UpdateOnlinePlayersETHAccounts(disconnectedPlayerID, "", false);
        }
    }

    //PLAYERS' CURRENT HEALTH INFO PACKET RECEIVED
    public static void PlayerHealth(Packet _packet)
    {
        int playerID = _packet.ReadInt();
        float playerCurrentHealth = _packet.ReadFloat(); //GET CORRESPONDING PLAYER'S HEALTH VALUE AT THAT MOMENT
        float playerMaxHPMPLimit = _packet.ReadFloat(); //GET CORRESPONDING PLAYER'S MAXIMUM HEALTH VALUE
        int playerDeathExperience = _packet.ReadInt();
        int objectIDWhoShootPlayer = _packet.ReadInt();

        //CHECK IF THE CORRESPONDING OBJECT ID EXISTS
        if (GameManager.playerObjects.ContainsKey(playerID) == true)
            GameManager.playerObjects[playerID].PlayerSetHealth(playerCurrentHealth, playerMaxHPMPLimit, playerDeathExperience, objectIDWhoShootPlayer);
    }

    //PLAYERS' CURRENT MANA INFO PACKET RECEIVED
    public static void PlayerMana(Packet _packet)
    {
        int playerID = _packet.ReadInt();
        float playerCurrentMana = _packet.ReadFloat(); //GET CORRESPONDING PLAYER'S MANA VALUE AT THAT MOMENT
        float playerMaxHPMPLimit = _packet.ReadFloat(); //GET CORRESPONDING PLAYER'S MAXIMUM MANA VALUE

        //CHECK IF THE CORRESPONDING OBJECT ID EXISTS
        if (GameManager.playerObjects.ContainsKey(playerID) == true)
            GameManager.playerObjects[playerID].PlayerSetMana(playerCurrentMana, playerMaxHPMPLimit);
    }

    //PLAYERS' REBOR PACKET RECEIVED
    public static void PlayerRespawned(Packet _packet)
    {
        int playerID = _packet.ReadInt();

        //CHECK IF THE CORRESPONDING OBJECT ID EXISTS
        if (GameManager.playerObjects.ContainsKey(playerID) == true)
            GameManager.playerObjects[playerID].PlayerRespawns();
    }

    //PLAYERS' ATTACK INFO PACKET RECEIVED
    public static void AttackSpawned(Packet _packet)
    {
        int attackObjectID = _packet.ReadInt();
        string creatorType = _packet.ReadString();
        string attackType = _packet.ReadString();
        Vector3 shootingPosition = _packet.ReadVector3();
        Quaternion shootingRotation = _packet.ReadQuaternion();

        //CHECK IF THE CORRESPONDING OBJECT ID DOES NOT EXIST (YOU CAN ALSO CHECK VIA OBJECT DICTIONARY)
        if (GameManager.attackObjects.ContainsKey(attackObjectID) == false)
            GameManager.instance.AttackSpawned(attackObjectID, creatorType, attackType, shootingPosition, shootingRotation);
    }

    //OPEN THE PACKET THAT TELLS YOU THE POSITION OF A CERTAIN ATTACK OBJECT BY THIS PACKET
    public static void AttackPosition(Packet _packet)
    {
        int attackObjectID = _packet.ReadInt();
        Vector3 attackObjectPosition = _packet.ReadVector3();

        //CHECK IF THE CORRESPONDING OBJECT ID EXISTS (YOU CAN ALSO CHECK VIA OBJECT DICTIONARY)
        if (GameManager.attackObjects.TryGetValue(attackObjectID, out AttackManager attackManager))
            attackManager.transform.position = attackObjectPosition;
    }

    //OPEN THE PACKET THAT TELLS YOU AN ATTACK OBJECT HAS BEEN DESTROYED BY THIS PACKET
    public static void AttackDestroyed(Packet _packet)
    {
        int attackObjectID = _packet.ReadInt();

        //CHECK IF THE CORRESPONDING OBJECT ID EXISTS
        if (GameManager.attackObjects.ContainsKey(attackObjectID) == true)
            GameManager.attackObjects[attackObjectID].DestroyAttackObject();
    }

    //OPEN THE PACKET THAT ORDERS YOU TO CREATE AN ITEM SPAWNER BY THIS PACKET
    public static void ItemSpawnerCreated(Packet _packet)
    {
        int itemSpawnerObjectID = _packet.ReadInt();
        Vector3 itemSpawnerPosition = _packet.ReadVector3();
        bool hasItem = _packet.ReadBool();
        string itemType = _packet.ReadString();

        //CHECK IF THE CORRESPONDING OBJECT ID DOES NO EXIST
        if (GameManager.itemSpawnerObjects.ContainsKey(itemSpawnerObjectID) == false)
            GameManager.instance.ItemSpawnerCreated(itemSpawnerObjectID, itemSpawnerPosition, hasItem, itemType);
    }

    //OPEN THE PACKET THAT TELLS YOU ITEM SPAWNER SPAWNED AN ITEM BY THIS PACKET
    public static void ItemSpawned(Packet _packet)
    {
        int itemSpawnerObjectID = _packet.ReadInt();
        string itemType = _packet.ReadString();

        //CHECK IF THE CORRESPONDING OBJECT ID EXISTS
        if (GameManager.itemSpawnerObjects.ContainsKey(itemSpawnerObjectID) == true)
            GameManager.itemSpawnerObjects[itemSpawnerObjectID].ItemSpawned(itemType);
    }

    //OPEN THE PACKET THAT TELLS YOU SOMEONE TOOK AN ITEM FROM AN ITEM SPAWNER BY THIS PACKET
    public static void ItemPickedUp(Packet _packet)
    {
        int itemSpawnerObjectID = _packet.ReadInt();
        int playerIDWhoTookItem = _packet.ReadInt();

        //CHECK IF THE CORRESPONDING OBJECT ID EXISTS
        if (GameManager.itemSpawnerObjects.ContainsKey(itemSpawnerObjectID) == true)
            GameManager.itemSpawnerObjects[itemSpawnerObjectID].ItemPickedUp(playerIDWhoTookItem);
    }

    //OPEN THE PACKET THAT TELLS YOU SOMEONE THREW A PROJECTILE BY THIS PACKET
    public static void ProjectileSpawned(Packet _packet)
    {
        int playerWhoThrew = _packet.ReadInt();
        int projectileID = _packet.ReadInt();
        Vector3 projectilePosition = _packet.ReadVector3();

        //CHECK IF THE CORRESPONDING OBJECT ID DOES NOT EXIST
        if (GameManager.projectileObjects.ContainsKey(projectileID) == false)
        {
            GameManager.instance.ProjectileSpawned(projectileID, projectilePosition);
            if (playerWhoThrew == Client.instance.myID)
                Inventory.instance.currentItemAmount--;
        }
    }

    //OPEN THE PACKET THAT TELLS YOU THE POSITION OF A CERTAIN PROJECTILE BY THIS PACKET
    public static void ProjectilePosition(Packet _packet)
    {
        int projectileID = _packet.ReadInt();
        Vector3 projectilePosition = _packet.ReadVector3();

        //CHECK IF THE CORRESPONDING OBJECT ID EXISTS (YOU CAN ALSO CHECK VIA OBJECT DICTIONARY)
        if (GameManager.projectileObjects.TryGetValue(projectileID, out ProjectileManager projectileManager))
        {
            try
            {
                projectileManager.transform.position = projectilePosition;
            }
            catch (System.Exception) { }
        }
            
    }

    //OPEN THE PACKET THAT TELLS YOU PROJECTILE HAS BEEN EXPLODED BY THIS PACKET
    public static void ProjectileExploded(Packet _packet)
    {
        int projectileID = _packet.ReadInt();
        Vector3 projectilePosition = _packet.ReadVector3();

        //CHECK IF THE CORRESPONDING OBJECT ID EXISTS
        if (GameManager.projectileObjects.ContainsKey(projectileID) == true)
            GameManager.projectileObjects[projectileID].Explode(projectilePosition);
    }

    public static void SpawnEnemy(Packet _packet)
    {
        int enemyID = _packet.ReadInt();
        Vector3 enemyPosition = _packet.ReadVector3();
        float enemyMaxHP = _packet.ReadFloat();
        string enemyWorld = _packet.ReadString();

        //CHECK IF THE CORRESPONDING OBJECT ID DOES NOT EXIST
        if (GameManager.enemyObjects.ContainsKey(enemyID) == false)
            GameManager.instance.SpawnEnemy(enemyID, enemyPosition, enemyMaxHP, enemyWorld);
    }

    public static void EnemyPositionAndRotation(Packet _packet)
    {
        int enemyID = _packet.ReadInt();
        Vector3 enemyPosition = _packet.ReadVector3();
        Quaternion enemyRotation = _packet.ReadQuaternion();

        //CHECK IF THE CORRESPONDING OBJECT ID EXISTS (YOU CAN ALSO CHECK VIA OBJECT DICTIONARY)
        if (GameManager.enemyObjects.TryGetValue(enemyID, out EnemyManager enemyManager))
        {
            enemyManager.transform.position = enemyPosition;
            enemyManager.transform.rotation = enemyRotation;
        }
    }

    public static void EnemyHealth(Packet _packet)
    {
        int enemyID = _packet.ReadInt();
        float enemyHealth = _packet.ReadFloat();
        int enemyExperience = _packet.ReadInt();
        int playerIDWhoShoot = _packet.ReadInt();

        //CHECK IF THE CORRESPONDING OBJECT ID EXISTS
        if (GameManager.enemyObjects.ContainsKey(enemyID) == true)
            GameManager.enemyObjects[enemyID].EnemySetHealth(enemyHealth, enemyExperience, playerIDWhoShoot);
    }

    //GET THE CHAT MESSAGE THAT SERVER DELIVERED TO YOU FROM OTHER PLAYERS
    public static void ServerSendChatMessage(Packet _packet)
    {
        string senderName = _packet.ReadString();
        string message = _packet.ReadString();

        if (senderName.Equals("Trade") == true)
        {
            GameChat.instance.SendTradeRequest("OTHER", message, "ToBeDeterminedInTheFunc");
        }
        else if (senderName.Equals("Transfer") == true)
        {
            string operation = message.Substring(0, message.IndexOf(" ")); //REPRESENTS "/transfer"
            message = message.Substring(message.IndexOf(" ") + 1);
            operation += " " + message.Substring(0, message.IndexOf(" ")); //REPRESENTS "/transfer RECEIVER_ID"
            message = message.Substring(message.IndexOf(" ") + 1);
            operation += " " + message.Substring(0, message.IndexOf(" ")); //REPRESENTS "/transfer RECEIVER_ID RECEIVE"
            message = message.Substring(message.IndexOf(" ") + 1);
            string component = message.Substring(0, message.IndexOf(" ")); //COMPONENT
            message = message.Substring(message.IndexOf(" ") + 1);
            string value = message; //VALUE
            try
            {
                TradeMenu.instance.TradeMenuEventHandler(operation, component, value);
            }
            catch (System.Exception) { }
        }
        else
            GameChat.instance.SendChatMessage(senderName, message);
    }

    //SERVER FORWARDS CORRESPONDING PLAYER'S LEVEL TO EVERYONE
    public static void UpdatePlayerLevel(Packet _packet)
    {
        int playerID = _packet.ReadInt();
        string playerLevelText = _packet.ReadString();
        bool levelEffectActivated = _packet.ReadBool();

        if (GameManager.playerObjects.ContainsKey(playerID) == true)
        {
            GameManager.playerObjects[playerID].UpdatePlayerLevel(playerLevelText, levelEffectActivated);
        }
    }


    //OPEN THE PACKET THAT TELLS YOU SOMEONE ACTIVATED OR DEACTIVATED HIS SHIELD
    public static void PlayerShieldOperations(Packet _packet)
    {
        int playerID = _packet.ReadInt();
        bool isActivated = _packet.ReadBool();
        string shieldType = _packet.ReadString();

        //CHECK IF THE CORRESPONDING OBJECT ID DOES NOT EXIST
        if (GameManager.playerObjects.ContainsKey(playerID) == true)
        {
            GameManager.playerObjects[playerID].ChangeShieldState(isActivated, shieldType);
            if (playerID == Client.instance.myID)
            {
                if(shieldType.Equals("Normal") == true)
                    Inventory.instance.currentItemAmount--;
            }
        }
    }
}
