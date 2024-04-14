using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;

public class Client
{
    public static int MAX_DATA_SIZE = 1024; //MAX 1 KILOBYTE (1024 BYTE) DATA CAN BE RECEIVED

    public int clientID;
    public Player player;
    public TCP HukoTCP;
    public UDP HukoUDP;

    public Client(int _clientId)
    {
        clientID = _clientId;
        HukoTCP = new TCP(clientID);
        HukoUDP = new UDP(clientID);
    }

    public class TCP
    {
        private readonly int ClientID;
        private NetworkStream networkStream;
        public TcpClient clientSocket;
        private Packet receivedPacket;
        private byte[] receivedData;

        public TCP(int _id)
        {
            ClientID = _id;
        }

        //INITIALIZES NEWCOMER CLIENT'S TCP INFORMATION
        public void Connect(TcpClient _clientSocket)
        {
            clientSocket = _clientSocket;
            clientSocket.ReceiveBufferSize = MAX_DATA_SIZE;
            clientSocket.SendBufferSize = MAX_DATA_SIZE;

            networkStream = clientSocket.GetStream();

            receivedPacket = new Packet(); //CREATES PACKET
            receivedData = new byte[MAX_DATA_SIZE]; //CREATES MAX SIZED BYTE ARRAY TO HOLD DATA

            //BEGINS TO READ THE STREAM FOR NEW INCOMING DATAS
            networkStream.BeginRead(receivedData, 0, MAX_DATA_SIZE, ReadTCPData, null);

            ServerSend.SendPlayerWelcomeMessage(ClientID, "Welcome to the server!");
        }

        //RECEIVES & READS THE DATA FROM THE STREAM [TCP]
        private void ReadTCPData(IAsyncResult _asyncResult)
        {
            try
            {
                int receivedDataLength = networkStream.EndRead(_asyncResult);

                if (receivedDataLength <= 0) //IF IT IS EMPTY THAT MEANS SOMETHING IS WRONG AND DISCONNTECT THE CLIENT
                {
                    Server.clientDictionary[ClientID].Disconnect();
                    return;
                }

                byte[] data = new byte[receivedDataLength];
                Array.Copy(this.receivedData, data, receivedDataLength); //COPY FROM ReceivedData ARRAY TO DATA ARRAY

                receivedPacket.ResetPacketContents(HandleTCPData(data)); // RESET RECEIVED DATA IF IT WAS HANDLED
                                                                         //BEGIN TO READ NEW DATA
                networkStream.BeginRead(this.receivedData, 0, MAX_DATA_SIZE, this.ReadTCPData, null);
            }
            catch (Exception exception)
            {
                Debug.Log("[Error]: TCP data reading: " + exception);
                Server.clientDictionary[ClientID].Disconnect();
            }
        }

        //SENDS THE DATA TO THE SPECIFIC CLIENT [TCP]
        public void SendTCPData(Packet _packet)
        {
            try
            {
                if (clientSocket != null) //IF THE SPECIFIC SOCKET EXISTS, SEND THE DATA
                {
                    networkStream.BeginWrite(_packet.ConvertByteListToByteArray(), 0, _packet.GetLengthOfThePacketContents(), null, null);
                }
            }
            catch (Exception exception)
            {
                Debug.Log("[Error]: TCP data sending to player " + ClientID + " " + exception);
            }
        }

        //PREPARES RECEIVED DATA, SO THAT IT WILL BE USED BY THE CONVENIENT PACKET HANDLER IN THE FUTURE
        private bool HandleTCPData(byte[] _data)
        {
            int receivedPacketLength = 0;
            this.receivedPacket.SetBytes(_data); //PUTS _data INTO receivedPacket

            if (this.receivedPacket.GetUnreadDataLengthInPacket() >= 4) //IF RECEIVED PACKET CONTAINS DATA
            {
                //CHECK IF PACKET CONTAINS DATA
                receivedPacketLength = this.receivedPacket.ReadInt();
                if (receivedPacketLength <= 0) //IF THERE IS NO DATA
                {
                    return true; //RESET receivedData TO BE REUSED IN THE FUTURE [TRUE MEANS RESET]
                }
            }

            //IF THE PACKET CONTAINS DATA AND PACKET LENGTH NOT EXCEED THE RECEIVED PACKET
            while (receivedPacketLength > 0 && receivedPacketLength <= this.receivedPacket.GetUnreadDataLengthInPacket())
            {
                byte[] packetData = this.receivedPacket.ReadByteArray(receivedPacketLength);
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet packet = new Packet(packetData))
                    {
                        int packetID = packet.ReadInt();
                        Server.packetHandlers[packetID](ClientID, packet); //CALL CONVENIENT PACKET HANDLER FOR THIS PACKET
                    }
                });

                receivedPacketLength = 0; //RESET PACKET LENGTH
                if (this.receivedPacket.GetUnreadDataLengthInPacket() >= 4) //IF RECEIVED PACKET CONTAINS ANOTHER PACKETS
                {
                    receivedPacketLength = this.receivedPacket.ReadInt();
                    if (receivedPacketLength <= 0) //IF PACKET CONTAINS NO DATA
                    {
                        return true; //RESET receivedData TO BE REUSED IN THE FUTURE [TRUE MEANS RESET]
                    }
                }
            }

            if (receivedPacketLength <= 1)
            {
                return true; //RESET receivedData TO BE REUSED IN THE FUTURE [TRUE MEANS RESET]
            }

            return false;
        }

        //CLOSES THE TCP CONNECTION AND CLEANS THE DATA
        public void Disconnect()
        {
            clientSocket.Close();
            networkStream = null;
            receivedPacket = null;
            receivedData = null;
            clientSocket = null;
           
        }
    }

    public class UDP
    {
        public IPEndPoint clientAddress;
        private readonly int clientID;

        public UDP(int _id)
        {
            clientID = _id;
        }

        //INITIALIZES NEWCOMER UDP CLIENT
        public void Connect(IPEndPoint _clientAddress)
        {
            clientAddress = _clientAddress;
        }

        /*
            UDPDATA STARTS BEING READ IN SERVER.cs, THEN IT DIRECTS TO THE CORRESPONDING USER'S "HandleUDPData" METHOD
        */

        //SEND UDP PACKET TO THE SPECIFIC CLIENT
        public void SendUDPData(Packet _packet)
        {
            try
            {
                if (clientAddress != null) //IF THE CLIENT ADDRESS EXISTS, SEND THE UDP PACKET
                {
                    Server.udpClient.BeginSend(_packet.ConvertByteListToByteArray(), _packet.GetLengthOfThePacketContents(), clientAddress, null, null);
                }
            }
            catch (Exception exception)
            {
                Debug.Log("[Error]: UDP sending to " + clientAddress + " " + exception);
            }
        }

        //PREPARES RECEIVED DATA, SO THAT IT WILL BE USED BY THE CONVENIENT PACKET HANDLER IN THE FUTURE
        public void HandleUDPData(Packet _packet)
        {
            int packetLength = _packet.ReadInt();
            byte[] packetData = _packet.ReadByteArray(packetLength);

            ThreadManager.ExecuteOnMainThread(() =>
            {
                using (Packet packet = new Packet(packetData))
                {
                    int packetID = packet.ReadInt();
                    Server.packetHandlers[packetID](clientID, packet); //CALL CONVENIENT PACKET HANDLER FOR THIS PACKET
                }
            });
        }
        //CLOSES THE UDP CONNECTION AND CLEANS THE DATA
        public void Disconnect()
        {
            clientAddress = null;
        }
    }

    //SENDS THE NEW PLAYER TO ALL PLAYERS (EXCEPT HIMSELF)
    public void ConnectNewPlayerToGame(string _newPlayerName, string _ethAccountAddress)
    {
        player = NetworkManager.instance.InstantiatePlayer();
        player.Initialize(clientID, _newPlayerName, _ethAccountAddress);

        //INFORM OTHER PLAYERS THAT NEW PLAYER HAS COME !
        foreach (Client searchClients in Server.clientDictionary.Values)
        {
            if (searchClients.player != null)
            {
                if (searchClients.clientID != clientID) //INFORM OTHER PLAYERS (EXCEPT YOURSELF) THAT NEW PLAYER HAS CONNECTED
                {
                    ServerSend.SpawnPlayer(clientID, searchClients.player);
                }
            }
        }

        //SENDS THE NEW PLAYER TO ALL PLAYERS (INCLUDING HIMSELF)
        foreach (Client searchClients in Server.clientDictionary.Values)
        {
            if (searchClients.player != null)
            {
                ServerSend.SpawnPlayer(searchClients.clientID, player);
            }
        }

        //TELL THE NEW PLAYER TO CREATE EXISTING ITEMSPAWNERS
        foreach(ItemSpawner searchItemSpawner in ItemSpawner.itemSpawnerObjects.Values)
        {
            ServerSend.ItemSpawnerCreated(clientID, searchItemSpawner.itemSpawnerObjectID, searchItemSpawner.transform.position, searchItemSpawner.itemSpawnerHasItem, searchItemSpawner.itemType);
        }

        //SEND THE NEW PLAYER TO SPAWN EXISTING ENEMIES
        foreach(Enemy searchEnemy in EnemySpawner.enemyObjects.Values)
        {
            ServerSend.SpawnEnemy(clientID, searchEnemy);
        }

        ServerSend.ServerSendPrivateMessageToEveryoneExceptSomeone(clientID, _newPlayerName + " is now online.");
    }

    //DISCONNECTS THE CLIENT
    private void Disconnect()
    {
        Debug.Log(HukoTCP.clientSocket.Client.RemoteEndPoint + " has disconnected.");

        ThreadManager.ExecuteOnMainThread(() =>
        {
            ServerSend.ServerSendPrivateMessageToEveryoneExceptSomeone(clientID, player.playerUsername + " is offline.");
            UnityEngine.Object.Destroy(player.gameObject);
            player = null;
        }); 

        HukoTCP.Disconnect();
        HukoUDP.Disconnect();

        Server.OnlinePlayersETHAccountsDictionary.Remove(clientID);
        ServerSend.PlayerDisconnected(clientID);
    }
}
