using UnityEngine;

public class ServerHandle
{
    //SERVER RECEIVES A MESSAGE FROM THE CLIENT, THIS PACKET IS OPENED IN HERE
    public static void PlayerReceivedWelcomeMessage(int _fromActualClientID, Packet _packet)
    {
        int sentByClientID = _packet.ReadInt();
        string Username = _packet.ReadString();
        string userETHAccount = _packet.ReadString();
        int currentItemAmount = _packet.ReadInt();

        Debug.Log(Username + " [Player ID: " + _fromActualClientID + "] connected successfully via [" + Server.clientDictionary[_fromActualClientID].HukoTCP.clientSocket.Client.RemoteEndPoint + "].");
        
        if (_fromActualClientID != sentByClientID)
            Debug.Log(Username + " [Player ID: " + _fromActualClientID + "] has come with wrong ID [Player ID: " + sentByClientID + "]!");
        Server.clientDictionary[_fromActualClientID].ConnectNewPlayerToGame(Username, userETHAccount);
        Server.OnlinePlayersETHAccountsDictionary.Add(_fromActualClientID, userETHAccount);
        Server.clientDictionary[_fromActualClientID].player.currentItemAmount = currentItemAmount;
    }

    //SERVER RECEIVES CLIENT MOVEMENT INFO, THIS PACKET IS OPENED IN HERE
    public static void PlayerMovement(int _fromClientID, Packet _packet)
    {
        bool[] movementArray = new bool[_packet.ReadInt()];
        for (int i = 0; i < movementArray.Length; i++)
            movementArray[i] = _packet.ReadBool();
        Quaternion _rotation = _packet.ReadQuaternion();

        //CHECK IF THE CORRESPONDING OBJECT ID EXISTS
        if (Server.clientDictionary.ContainsKey(_fromClientID) == true)
            Server.clientDictionary[_fromClientID].player.MovementDirection(movementArray, _rotation);
    }

    //SERVER RECEIVES THAT CLIENT ATTEMPTED TO ATTACK, THIS PACKET IS OPENED IN HERE
    public static void PlayerShoot(int _fromClientID, Packet _packet)
    {
        int playerID = _packet.ReadInt();
        Vector3 shootingPosition = _packet.ReadVector3();
        Quaternion shootingRotation = _packet.ReadQuaternion();

        //CHECK IF THE CORRESPONDING OBJECT ID EXISTS
        if (Server.clientDictionary.ContainsKey(_fromClientID) == true)
            Server.clientDictionary[_fromClientID].player.AttemptShooting(playerID, shootingPosition, shootingRotation);
    }
    public static void PlayerThrowItem(int _fromClientID, Packet _packet)
    {
        Vector3 throwingPosition = _packet.ReadVector3();
        Quaternion throwingRotation = _packet.ReadQuaternion();

        //CHECK IF THE CORRESPONDING OBJECT ID EXISTS
        if (Server.clientDictionary.ContainsKey(_fromClientID) == true)
            Server.clientDictionary[_fromClientID].player.AttemptThrowingItem(throwingPosition, throwingRotation);
    }
    public static void PlayerSendChatMessage(int _fromClientID, Packet _packet)
    {
        string message  = _packet.ReadString();

        if (message.StartsWith("/trade ") == true)
        {
            try
            {
                string command = "/trade ";
                message = message.Substring(7);
                int toWhoID = int.Parse(message.Substring(0, message.IndexOf(" ")));
                message = message.Substring(message.IndexOf(" ") + 1);
                string operation = message;
                message = command + _fromClientID + " " + operation;

                if (Server.clientDictionary.ContainsKey(_fromClientID) == true && Server.clientDictionary.ContainsKey(toWhoID) == true)
                    ServerSend.ServerSendPrivateMessageToSomeone(toWhoID, message, "Trade");
            }catch(System.Exception)
            {
                if (Server.clientDictionary.ContainsKey(_fromClientID) == true)
                    ServerSend.ServerSendPrivateMessageToSomeone(_fromClientID, "The user is offline.", "Trade");
            }
        }
        else if(message.StartsWith("/transfer ") == true)
        {
            try
            {
                string temp = message.Substring(10);
                int toWhoID = int.Parse(temp.Substring(0, temp.IndexOf(" ")));

                if (Server.clientDictionary.ContainsKey(_fromClientID) == true && Server.clientDictionary.ContainsKey(toWhoID) == true)
                    ServerSend.ServerSendPrivateMessageToSomeone(toWhoID, message, "Transfer");
            }
            catch (System.Exception)
            {
                if (Server.clientDictionary.ContainsKey(_fromClientID) == true)
                    ServerSend.ServerSendPrivateMessageToSomeone(_fromClientID, "The user is offline.", "Transfer");
            }
        }
        else if(message.StartsWith("/inventory_current_item_number ") == true)
        {
            if (Server.clientDictionary.ContainsKey(_fromClientID) == true)
                Server.clientDictionary[_fromClientID].player.currentItemAmount = System.Convert.ToInt32(message.Substring(31));
        }
        else
        {
            //CHECK IF THE CORRESPONDING OBJECT ID EXISTS
            if (Server.clientDictionary.ContainsKey(_fromClientID) == true)
                ServerSend.ServerForwardChatMessage(_fromClientID, message);
        }
    }

    public static void PlayerLevelUp(int _fromClientID, Packet _packet) 
    {
        string characterLevel = _packet.ReadString();
        bool levelEffectActivated = _packet.ReadBool();

        //CHECK IF THE CORRESPONDING OBJECT ID EXISTS
        if (Server.clientDictionary.ContainsKey(_fromClientID) == true)
        {
            //IF CHARACTER LEVELED UP NEWLY AND NEW HP MP LIMIT IS NOT THE SAME AS THE INCOMING MAX LIMIT
            if (levelEffectActivated == false && Server.clientDictionary[_fromClientID].player.levelText != characterLevel)
                Server.clientDictionary[_fromClientID].player.PlayerSetNewLevel(characterLevel);
            ServerSend.UpdatePlayerLevel(_fromClientID, characterLevel, levelEffectActivated);
        }
    }

    public static void PlayerShieldActivated(int _fromClientID, Packet _packet)
    {
        string shieldType = _packet.ReadString();

        //CHECK IF THE CORRESPONDING OBJECT ID EXISTS
        if (Server.clientDictionary.ContainsKey(_fromClientID) == true)
            Server.clientDictionary[_fromClientID].player.shieldManager.ActivateShield(shieldType);
    }
}
