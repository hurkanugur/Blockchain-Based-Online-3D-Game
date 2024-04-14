using UnityEngine;

public class ClientSend : MonoBehaviour
{
    //SENDS TCP PACKET TO THE SERVER
    private static void SendTCPData(Packet _packet)
    {
        _packet.InsertPacketLengthAtBeginning();
        Client.instance.hukoTCP.SendTCPData(_packet);
    }

    //SENDS UDP PACKET TO THE SERVER
    private static void SendUDPData(Packet _packet)
    {
        _packet.InsertPacketLengthAtBeginning();
        Client.instance.hukoUDP.SendUDPData(_packet);
    }

    #region Packets
    //INFORM THE SERVER THAT YOU RECEIVED ITS WELCOME MESSAGE
    public static void PlayerWelcomeMessageAsync()
    {
        using (Packet packet = new Packet((int)ClientPackets.playerReceivedWelcomeMessage))
        {
            packet.Write(Client.instance.myID);
            packet.Write(MenuManager.instance.usernameField.text);
            packet.Write(ETHAccount.instance.GetMyWalletAddress());
            packet.Write(Inventory.instance.currentItemAmount);

            SendTCPData(packet);
        }
    }

    //SEND PLAYER'S POSITION TO THE SERVER
    public static void PlayerMovement(bool[] _inputs)
    {
        using (Packet packet = new Packet((int)ClientPackets.playerMovement))
        {
            packet.Write(_inputs.Length);
            foreach (bool _input in _inputs)
            {
                packet.Write(_input);
            }
            packet.Write(GameManager.playerObjects[Client.instance.myID].transform.rotation);

            SendUDPData(packet);
        }
    }

    //SEND PLAYER SHOOT TO THE SERVER
    public static void PlayerShoot(int _playerID, Vector3 _shootingPosition, Quaternion _shootingRotation)
    {
        using (Packet packet = new Packet((int)ClientPackets.playerShoot))
        {
            packet.Write(_playerID);
            packet.Write(_shootingPosition);
            packet.Write(_shootingRotation);

            SendTCPData(packet);
        }
    }

    //SEND PLAYER THROWING ITEM INFO TO THE SERVER
    public static void PlayerThrowItem(Vector3 _throwingPosition, Quaternion _throwingRotation)
    {
        using (Packet packet = new Packet((int)ClientPackets.playerThrowItem))
        {
            packet.Write(_throwingPosition);
            packet.Write(_throwingRotation);

            SendTCPData(packet);
        }
    }

    //SEND YOUR CHAT MESSAGE TO THE SERVER TO BE DELIVERED TO OTHER PLAYERS
    public static void PlayerSendChatMessage(string _message)
    {
        using (Packet packet = new Packet((int)ClientPackets.playerSendChatMessage))
        {
            packet.Write(_message);

            SendTCPData(packet);
        }
    }

    //NOTIFY SERVER THAT YOU LEVEL UP
    public static void PlayerLevelUp(string _playerLevelText, bool _levelEffectActivated)
    {
        using (Packet packet = new Packet((int)ClientPackets.playerLevelUp))
        {
            packet.Write(_playerLevelText);
            packet.Write(_levelEffectActivated);

            SendTCPData(packet);
        }
    }

    //SEND PLAYER THROWING ITEM INFO TO THE SERVER
    public static void PlayerShieldActivated(string shieldType)
    {
        using (Packet packet = new Packet((int)ClientPackets.playerShieldActivated))
        {
            packet.Write(shieldType);

            SendTCPData(packet);
        }
    }
    #endregion
}
