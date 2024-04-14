using UnityEngine;

public class ServerSend
{
    //SENDS TCP PACKET TO THE SPECIFIC PLAYER
    private static void SendTCPDataToSomeone(int _clientID, Packet _packet)
    {
        _packet.InsertPacketLengthAtBeginning();
        Server.clientDictionary[_clientID].HukoTCP.SendTCPData(_packet);
    }

    //SENDS TCP PACKET TO EVERYONE
    private static void SendTCPDataToEveryone(Packet _packet)
    {
        _packet.InsertPacketLengthAtBeginning();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            Server.clientDictionary[i].HukoTCP.SendTCPData(_packet);
        }
    }

    //SENDS TCP PACKET TO EVERYONE EXCEPT SOMEONE (WITH SPECIFIC CLIENT ID)
    private static void SendTCPDataToEveryoneExceptSomeone(int _exceptClientID, Packet _packet)
    {
        _packet.InsertPacketLengthAtBeginning();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            if (i != _exceptClientID)
            {
                Server.clientDictionary[i].HukoTCP.SendTCPData(_packet);
            }
        }
    }

    //SENDS UDP PACKET TO EVERYONE
    private static void SendUDPDataToEveryone(Packet _packet)
    {
        _packet.InsertPacketLengthAtBeginning();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            Server.clientDictionary[i].HukoUDP.SendUDPData(_packet);
        }
    }
    //SENDS UDP PACKET TO THE SPECIFIC PLAYER
    private static void SendUDPDataToSomeone(int _clientID, Packet _packet)
    {
        _packet.InsertPacketLengthAtBeginning();
        Server.clientDictionary[_clientID].HukoUDP.SendUDPData(_packet);
    }

    //SENDS UDP PACKET TO EVERYONE EXCEPT SOMEONE (WITH SPECIFIC CLIENT ID)
    private static void SendUDPDataToEveryoneExceptSomeone(int _exceptClientID, Packet _packet)
    {
        _packet.InsertPacketLengthAtBeginning();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            if (i != _exceptClientID)
            {
                Server.clientDictionary[i].HukoUDP.SendUDPData(_packet);
            }
        }
    }

    #region Packets
    //SEND WELCOME MESSAGE TO THE HAVE SPECIFIC CLIENT INITIALIZE HIS UDP PACKET
    public static void SendPlayerWelcomeMessage(int _clientID, string _message)
    {
        using (Packet packet = new Packet((int)ServerPackets.sendPlayerWelcomeMessage))
        {
            packet.Write("Server");
            packet.Write(_message);
            packet.Write(_clientID);

            SendTCPDataToSomeone(_clientID, packet);
        }
    }

    //TELLS A CLIENT (WITH ID) TO SPAWN THE SPECIFIC PLAYER 
    public static void SpawnPlayer(int _clientID, Player _player)
    {
        using (Packet packet = new Packet((int)ServerPackets.spawnPlayer))
        {
            packet.Write(_player.playerID);
            packet.Write(_player.playerUsername);
            packet.Write(_player.transform.position);
            packet.Write(_player.transform.rotation);
            packet.Write(_player.ETHAccountAddress);

            SendTCPDataToSomeone(_clientID, packet);
        }
    }

    //SENDS A PLAYER'S UPDATED POSITION TO ALL PLAYERS
    public static void PlayerPositionToEveryone(Player _player)
    {
        using (Packet packet = new Packet((int)ServerPackets.playerPosition))
        {
            packet.Write(_player.playerID);
            packet.Write(_player.transform.position);

            SendUDPDataToEveryone(packet); //POSITION INFO CAN AFFORD LOSING SOME DATA (WE CAN USE UDP'S SPEED)
        }
    }

    //SENDS A PLAYER'S UPDATED ROTATION TO ALL PLAYERS EXCEPT YOURSELF (TO AVOID OVERROTATION)
    public static void PlayerRotationToEveryoneExceptYourself(Player _player)
    {
        using (Packet packet = new Packet((int)ServerPackets.playerRotation))
        {
            packet.Write(_player.playerID);
            packet.Write(_player.transform.rotation);

            SendUDPDataToEveryoneExceptSomeone(_player.playerID, packet); //ROTATION INFO CAN AFFORD LOSING SOME DATA (WE CAN USE UDP'S SPEED)
        }
    }

    //SENDS A PLAYER'S UPDATED ROTATION TO ALL PLAYERS EXCEPT YOURSELF (TO AVOID OVERROTATION)
    public static void PlayerDisconnected(int _playerID)
    {
        using (Packet packet = new Packet((int)ServerPackets.playerDisconnected))
        {
            packet.Write(_playerID);

            SendTCPDataToEveryone(packet);
        }
    }

    //SEND THE CORRESPONDING PLAYER'S HEALTH DATA TO EVERYONE
    public static void PlayerHealth(Player _player, int _objectIDWhoShootPlayer)
    {
        using (Packet packet = new Packet((int)ServerPackets.playerHealth))
        {
            packet.Write(_player.playerID);
            packet.Write(_player.playerCurrentHealth);
            packet.Write(_player.playerMaxHPMPLimit);
            packet.Write(_player.playerDeathExperience);
            packet.Write(_objectIDWhoShootPlayer);

            SendTCPDataToEveryone(packet);
        }
    }

    //SEND THE CORRESPONDING PLAYER'S MANA DATA TO EVERYONE
    public static void PlayerMana(Player _player)
    {
        using (Packet packet = new Packet((int)ServerPackets.playerMana))
        {
            packet.Write(_player.playerID);
            packet.Write(_player.playerCurrentMana);
            packet.Write(_player.playerMaxHPMPLimit);

            SendTCPDataToEveryone(packet);
        }
    }

    //SEND EVERYONE THE CORRESPONDING PLAYER'S HEALTH DATA TO EVERYONE
    public static void PlayerRespawned(Player _player)
    {
        using (Packet packet = new Packet((int)ServerPackets.playerRespawned))
        {
            packet.Write(_player.playerID);

            SendTCPDataToEveryone(packet);
        }
    }

    //SEND PACKET TO EVERYONE THAT CORRESPONDING PLAYER IS SHOOTING (SEND THEM THE ATTACK POSITION, ROTATION, ID...)
    public static void AttackSpawned(int attackObjectID, string creatorType, string attackType, Vector3 _shootingPosition, Quaternion _shootingRotation)
    {
        using (Packet packet = new Packet((int)ServerPackets.attackSpawned))
        {
            packet.Write(attackObjectID);
            packet.Write(creatorType);
            packet.Write(attackType);
            packet.Write(_shootingPosition);
            packet.Write(_shootingRotation);

            SendTCPDataToEveryone(packet);
        }
    }

    //SEND PACKET THAT TELLS THE PLAYERS THE POSITION OF AN ATTACK OBJECT
    public static void AttackPosition(Attack _attack)
    {
        using (Packet packet = new Packet((int)ServerPackets.attackPosition))
        {
            packet.Write(_attack.attackObjectID);
            packet.Write(_attack.transform.position);

            SendUDPDataToEveryone(packet); //POSITION INFO CAN AFFORD LOSING SOME DATA (WE CAN USE UDP'S SPEED)
        }
    }

    //SEND PACKET THAT TELLS THE PLAYERS THAT AN ATTACK OBJECT HAS BEEN DESTROYED
    public static void AttackDestroyed(Attack _attack)
    {
        using (Packet packet = new Packet((int)ServerPackets.attackDestroyed))
        {
            packet.Write(_attack.attackObjectID);

            SendTCPDataToEveryone(packet);
        }
    }

    //SEND PACKET THAT TELLS THE PLAYERS TO CREATE AN ITEMSPAWNER FIELD
    public static void ItemSpawnerCreated(int _playerID, int _itemSpawnerObjectID, Vector3 _itemSpawnerPosition, bool _hasItem, string _item_type)
    {
        using (Packet packet = new Packet((int)ServerPackets.itemSpawnerCreated))
        {
            packet.Write(_itemSpawnerObjectID);
            packet.Write(_itemSpawnerPosition);
            packet.Write(_hasItem);
            packet.Write(_item_type);

            SendTCPDataToSomeone(_playerID, packet);
        }
    }

    //SEND PACKET THAT TELLS THE PLAYERS THAT ITEM IS SPAWNED IN ITEMSPAWNER
    public static void ItemSpawned(int _itemObjectID, string _itemType)
    {
        using (Packet packet = new Packet((int)ServerPackets.itemSpawned))
        {
            packet.Write(_itemObjectID);
            packet.Write(_itemType);

            SendTCPDataToEveryone(packet);
        }
    }

    //SEND PACKET THAT TELLS THE PLAYERS THAT ITEM IS PICKED UP FROM THE ITEMSPAWNER
    public static void ItemPickedUp(int _playerID, int _itemObjectID)
    {
        using (Packet packet = new Packet((int)ServerPackets.itemPickedUp))
        {
            packet.Write(_itemObjectID);
            packet.Write(_playerID);

            SendTCPDataToEveryone(packet);
        }
    }

    //SEND PACKET THAT TELLS THE PLAYERS THAT A PROJECTILE HAS BEEN SPAWNED
    public static void ProjectileSpawned(Projectile _projectile, int _whoThrewProjectile)
    {
        using (Packet packet = new Packet((int)ServerPackets.projectileSpawned))
        {
            packet.Write(_whoThrewProjectile);
            packet.Write(_projectile.projectileID);
            packet.Write(_projectile.transform.position);


            SendTCPDataToEveryone(packet);
        }
    }

    //SEND PACKET THAT TELLS THE PLAYERS THE POSITION OF PROJECTILE
    public static void ProjectilePosition(Projectile _projectile)
    {
        using (Packet packet = new Packet((int)ServerPackets.projectilePosition))
        {
            packet.Write(_projectile.projectileID);
            packet.Write(_projectile.transform.position);

            SendUDPDataToEveryone(packet); //POSITION INFO CAN AFFORD LOSING SOME DATA (WE CAN USE UDP'S SPEED)
        }
    }

    //SEND PACKET THAT TELLS THE PLAYERS THAT A PROJECTILE HAS BEEN EXPLODED
    public static void ProjectileExploded(Projectile _projectile)
    {
        using (Packet packet = new Packet((int)ServerPackets.projectileExploded))
        {
            packet.Write(_projectile.projectileID);
            packet.Write(_projectile.transform.position);

            SendTCPDataToEveryone(packet);
        }
    }

    //SEND PACKET THAT TELLS THE PLAYERS TO SPAWN ENEMY
    public static void SpawnEnemy(Enemy _enemy)
    {
        using (Packet packet = new Packet((int)ServerPackets.spawnEnemy))
        {
            packet.Write(_enemy.enemyID);
            packet.Write(_enemy.transform.position);
            packet.Write(_enemy.enemyMaxHealth);
            packet.Write(_enemy.enemyWorldName);

            SendTCPDataToEveryone(packet);
        }
    }

    //SEND PACKET THAT TELLS THE NEWLY ONLINE PLAYER TO SPAWN ENEMY
    public static void SpawnEnemy(int _toNewPlayer, Enemy _enemy)
    {
        using (Packet packet = new Packet((int)ServerPackets.spawnEnemy))
        {
            packet.Write(_enemy.enemyID);
            packet.Write(_enemy.transform.position);
            packet.Write(_enemy.enemyMaxHealth);
            packet.Write(_enemy.enemyWorldName);

            SendTCPDataToSomeone(_toNewPlayer, packet);
        }
    }

    //SEND PACKET THAT TELLS THE ENEMY'S POSITION
    public static void EnemyPositionAndRotation(Enemy _enemy)
    {
        using (Packet packet = new Packet((int)ServerPackets.enemyPositionAndRotation))
        {
            packet.Write(_enemy.enemyID);
            packet.Write(_enemy.transform.position);
            packet.Write(_enemy.transform.rotation);

            SendUDPDataToEveryone(packet); //POSITION INFO CAN AFFORD LOSING SOME DATA (WE CAN USE UDP'S SPEED)
        }
    }

    //SEND PACKET THAT TELLS THE ENEMY'S HEALTH
    public static void EnemyHealth(Enemy _enemy, int _playerIDWhoShoot)
    {
        using (Packet packet = new Packet((int)ServerPackets.enemyHealth))
        {
            packet.Write(_enemy.enemyID);
            packet.Write(_enemy.enemyCurrentHealth);
            packet.Write(_enemy.enemyExperience);
            packet.Write(_playerIDWhoShoot);

            SendTCPDataToEveryone(packet);
        }
    }

    //SERVER SENDS PRIVATE CHAT MESSAGE TO SOMEONE
    public static void ServerSendPrivateMessageToSomeone(int _sendToPrivateClientID, string _message, string _messageType)
    {
        using (Packet packet = new Packet((int)ServerPackets.serverSendChatMessage))
        {
            packet.Write(_messageType);
            packet.Write(_message);

            SendTCPDataToSomeone(_sendToPrivateClientID, packet);
        }
    }

    //SERVER SENDS PRIVATE CHAT MESSAGE TO EVERYONE
    public static void ServerSendPrivateMessageToEveryone(string _message)
    {
        using (Packet packet = new Packet((int)ServerPackets.serverSendChatMessage))
        {
            packet.Write("Server");
            packet.Write(_message);

            SendTCPDataToEveryone(packet);
        }
    }

    //SERVER SENDS PRIVATE CHAT MESSAGE TO EVERYONE EXCEPT SOMEONE
    public static void ServerSendPrivateMessageToEveryoneExceptSomeone(int _exceptClientID, string _message) 
    {
        using (Packet packet = new Packet((int)ServerPackets.serverSendChatMessage))
        {
            packet.Write("Server");
            packet.Write(_message);

            SendTCPDataToEveryoneExceptSomeone(_exceptClientID, packet);
        }
    }

    //SEND CHAT MESSAGE PACKET THAT WILL BE WRITTEN EVERYONE'S CHAT SCREEN EXCEPT THE ONE SENT MESSAGE
    public static void ServerForwardChatMessage(int _senderID, string _message)
    {
        using (Packet packet = new Packet((int)ServerPackets.serverSendChatMessage))
        {
            packet.Write(Server.clientDictionary[_senderID].player.playerUsername);
            packet.Write(_message);

            SendTCPDataToEveryoneExceptSomeone(_senderID, packet);
        }
    }


    //SERVER FORWARDS CORRESPONDING PLAYER'S LEVEL TO EVERYONE
    public static void UpdatePlayerLevel(int _senderID, string _playerLevel, bool _levelEffectActivated)
    {
        using (Packet packet = new Packet((int)ServerPackets.updatePlayerLevel))
        {
            packet.Write(_senderID);
            packet.Write(_playerLevel);
            packet.Write(_levelEffectActivated);

            SendTCPDataToEveryone(packet);
        }
    }

    //NOTIFY PLAYERS WHEN SOMEONE'S SHIELD IS ACTIVATED OR DEACTIVATED
    public static void PlayerShieldOperations(int _senderID, bool _isActivated, string _shieldType)
    {
            using (Packet packet = new Packet((int)ServerPackets.playerShieldOperations))
            {
                packet.Write(_senderID);
                packet.Write(_isActivated);
                packet.Write(_shieldType);

                SendTCPDataToEveryone(packet);
            }
    }

    #endregion
}