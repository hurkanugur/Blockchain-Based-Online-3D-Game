using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using UnityEngine.SceneManagement;

public class Client : MonoBehaviour
{
    public static Client instance;
    public static int MAX_DATA_SIZE = 1024; //MAX 1 KILOBYTE (1024 BYTE) DATA CAN BE RECEIVED

    public string IP;
    public int PORT;
    public int myID = 0;
    public TCP hukoTCP;
    public UDP hukoUDP;

    private bool isConnected = false;
    private delegate void PacketHandler(Packet _packet);
    private static Dictionary<int, PacketHandler> packetHandlers;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("[Error]: Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    public void OnApplicationQuit()
    {
        Disconnect(); //DISCONNECT THE PLAYER WHEN THE GAME IS CLOSED
    }

    //CLIENT ATTEMPTS TO CONNECT TO THE SERVER
    public void ConnectToServer(string _serverIP, int serverPort)
    {
        this.IP = _serverIP;
        this.PORT = serverPort;

        hukoTCP = new TCP();
        hukoUDP = new UDP();

        InitializeClientData();

        isConnected = true;
        hukoTCP.Connect(); //FIRSTLY CONNECT TCP. UDP WILL BE CONNECTED AFTER TCP IS DONE.
    }

    public class TCP
    {
        private NetworkStream networkStream;
        public TcpClient clientTCPSocket;
        private Packet receivedPacket;
        private byte[] receiveData;

        //ATTEMPS TO CONNECT TO THE SERVER BY USING TCP CONNECTION
        public void Connect()
        {

            clientTCPSocket = new TcpClient
            {
                ReceiveBufferSize = MAX_DATA_SIZE,
                SendBufferSize = MAX_DATA_SIZE
            };

            receiveData = new byte[MAX_DATA_SIZE];
            try
            {
                clientTCPSocket.BeginConnect(instance.IP, instance.PORT, TCPConnectionEstablished, clientTCPSocket);
            }
            catch (Exception)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }

        }

        //INITIALIZES NEWCOMER CLIENT'S TCP INFORMATION
        private void TCPConnectionEstablished(IAsyncResult _result)
        {
            clientTCPSocket.EndConnect(_result); //CONNECTION ESTABLISHED WITH SERVER

            if (!clientTCPSocket.Connected) //IF CLIENT COULDN'T CONNECT SUCCESSFULLY    
                return;

            networkStream = clientTCPSocket.GetStream(); //CONNECTS TO THE STREAM

            receivedPacket = new Packet(); //INITIALIZE THE PACKET

            //BEGINS TO READ DATA
            networkStream.BeginRead(receiveData, 0, MAX_DATA_SIZE, ReadTCPData, null);
        }

        //READS INCOMING TCP DATA FROM THE STREAM
        private void ReadTCPData(IAsyncResult _result)
        {
            try
            {
                int receivedPacketLength = networkStream.EndRead(_result);
                if (receivedPacketLength <= 0) //IF IT IS EMPTY THAT MEANS SOMETHING IS WRONG AND DISCONNTECT THE CLIENT
                {
                    instance.Disconnect();
                    return;
                }

                byte[] _data = new byte[receivedPacketLength];
                Array.Copy(receiveData, _data, receivedPacketLength); //COPY FROM ReceivedData ARRAY TO DATA ARRAY

                receivedPacket.ResetPacketContents(HandleTCPData(_data)); // RESET RECEIVED DATA IF IT WAS HANDLED
                //BEGIN TO READ NEW DATA
                networkStream.BeginRead(receiveData, 0, MAX_DATA_SIZE, ReadTCPData, null);
            }
            catch (Exception exception)
            {
                Console.WriteLine("[Error]: TCP data reading: " + exception);
                Disconnect();
            }
        }

        //SENDS DATA BY USING TCP
        public void SendTCPData(Packet _packet)
        {
            try
            {
                if (clientTCPSocket != null) //IF THE SPECIFIC SOCKET EXISTS, SEND THE DATA
                {
                    networkStream.BeginWrite(_packet.ConvertByteListToByteArray(), 0, _packet.GetLengthOfThePacketContents(), null, null); //SENDS DATA TO THE SERVER
                }
            }
            catch (Exception exception)
            {
                Debug.Log($"[Error]: TCP data reading " + exception);
            }
        }

        //PREPARES RECEIVED DATA, SO THAT IT WILL BE USED BY THE CONVENIENT PACKET HANDLER IN THE FUTURE
        private bool HandleTCPData(byte[] _data)
        {
            int receivedPacketLength = 0;
            receivedPacket.SetBytes(_data); //PUTS _data INTO receivedPacket
            if (receivedPacket.GetUnreadDataLengthInPacket() >= 4) //IF RECEIVED PACKET CONTAINS DATA
            {
                //CHECK IF PACKET CONTAINS DATA
                receivedPacketLength = receivedPacket.ReadInt();
                if (receivedPacketLength <= 0) //IF THERE IS NO DATA
                {
                    return true; //RESET receivedData TO BE REUSED IN THE FUTURE [TRUE MEANS RESET]
                }
            }

            //IF THE PACKET CONTAINS DATA AND PACKET LENGTH NOT EXCEED THE RECEIVED PACKET
            while (receivedPacketLength > 0 && receivedPacketLength <= receivedPacket.GetUnreadDataLengthInPacket())
            {
                byte[] packetBytes = receivedPacket.ReadByteArray(receivedPacketLength);
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet packet = new Packet(packetBytes))
                    {
                        int packetID = packet.ReadInt();
                        packetHandlers[packetID](packet); //CALL CONVENIENT PACKET HANDLER FOR THIS PACKET
                    }
                });

                receivedPacketLength = 0; //RESET PACKET LENGTH
                if (receivedPacket.GetUnreadDataLengthInPacket() >= 4) //IF RECEIVED PACKET CONTAINS ANOTHER PACKETS
                {
                    receivedPacketLength = receivedPacket.ReadInt();
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
        private void Disconnect()
        {
            instance.Disconnect();

            networkStream = null;
            receivedPacket = null;
            receiveData = null;
            clientTCPSocket = null;
        }
    }

    public class UDP
    {
        public UdpClient socket;
        public IPEndPoint endPoint;

        public UDP()
        {
            endPoint = new IPEndPoint(IPAddress.Parse(instance.IP), instance.PORT);
        }

        //INITIALIZES NEWCOMER UDP CLIENT
        public void Connect(int _localPort)
        {
            socket = new UdpClient(_localPort);

            socket.Connect(endPoint);
            socket.BeginReceive(ReadUDPData, null);

            using (Packet _packet = new Packet())
            {
                SendUDPData(_packet);
            }
        }

        //SEND UDP PACKET TO THE SPECIFIC CLIENT
        public void SendUDPData(Packet _packet)
        {
            try
            {
                _packet.InsertIntAtBeginning(instance.myID); //INSERT THE CLIENT'S ID AT THE START OF THE PACKET
                if (socket != null) //IF THE CLIENT ADDRESS EXISTS, SEND THE UDP PACKET
                {
                    socket.BeginSend(_packet.ConvertByteListToByteArray(), _packet.GetLengthOfThePacketContents(), null, null);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("[Error]: UDP sending to the server " + exception);
            }
        }

        //READ UDP PACKET COMING FROM SERVER
        private void ReadUDPData(IAsyncResult _result)
        {
            try
            {
                byte[] packetData = socket.EndReceive(_result, ref endPoint);
                socket.BeginReceive(ReadUDPData, null);

                if (packetData.Length < 4)
                {
                    instance.Disconnect();
                    return;
                }
                HandleUDPData(packetData);
            }
            catch
            {
                Disconnect();
            }
        }

        //PREPARES RECEIVED DATA, SO THAT IT WILL BE USED BY THE CONVENIENT PACKET HANDLER IN THE FUTURE
        private void HandleUDPData(byte[] _data)
        {
            using (Packet packet = new Packet(_data))
            {
                int packetLength = packet.ReadInt();
                _data = packet.ReadByteArray(packetLength);
            }

            ThreadManager.ExecuteOnMainThread(() =>
            {
                using (Packet _packet = new Packet(_data))
                {
                    int packetID = _packet.ReadInt();
                    packetHandlers[packetID](_packet); //CALL CONVENIENT PACKET HANDLER FOR THIS PACKET
                }
            });
        }

        //CLOSES THE UDP CONNECTION AND CLEANS THE DATA
        private void Disconnect()
        {
            instance.Disconnect();
            endPoint = null;
            socket = null;
        }
    }

    //INITIALIZES DATAS THAT ARE SENT FROM SERVER TO CLIENT (RECEIVED DATA)
    private void InitializeClientData()
    {
        packetHandlers = new Dictionary<int, PacketHandler>()
        {
            { (int)ServerPackets.sendPlayerWelcomeMessage, ClientHandle.PlayerReceivedWelcomeMessage },
            { (int)ServerPackets.spawnPlayer, ClientHandle.SpawnPlayer },
            { (int)ServerPackets.playerPosition, ClientHandle.PlayerPosition },
            { (int)ServerPackets.playerRotation, ClientHandle.PlayerRotation },
            { (int)ServerPackets.playerDisconnected, ClientHandle.PlayerDisconnected },
            { (int)ServerPackets.playerHealth, ClientHandle.PlayerHealth },
            { (int)ServerPackets.playerMana, ClientHandle.PlayerMana },
            { (int)ServerPackets.playerRespawned, ClientHandle.PlayerRespawned },
            { (int)ServerPackets.attackSpawned, ClientHandle.AttackSpawned },
            { (int)ServerPackets.attackPosition, ClientHandle.AttackPosition },
            { (int)ServerPackets.attackDestroyed, ClientHandle.AttackDestroyed },
            { (int)ServerPackets.itemSpawnerCreated, ClientHandle.ItemSpawnerCreated },
            { (int)ServerPackets.itemSpawned, ClientHandle.ItemSpawned },
            { (int)ServerPackets.itemPickedUp, ClientHandle.ItemPickedUp },
            { (int)ServerPackets.spawnProjectile, ClientHandle.ProjectileSpawned },
            { (int)ServerPackets.projectilePosition, ClientHandle.ProjectilePosition },
            { (int)ServerPackets.projectileExploded, ClientHandle.ProjectileExploded },
            { (int)ServerPackets.spawnEnemy, ClientHandle.SpawnEnemy },
            { (int)ServerPackets.enemyPositionAndRotation, ClientHandle.EnemyPositionAndRotation },
            { (int)ServerPackets.enemyHealth, ClientHandle.EnemyHealth },
            { (int)ServerPackets.serverSendChatMessage, ClientHandle.ServerSendChatMessage },
            { (int)ServerPackets.updatePlayerLevel, ClientHandle.UpdatePlayerLevel } ,
            { (int)ServerPackets.playerShieldOperations, ClientHandle.PlayerShieldOperations }
        };
        Debug.Log("Initialized packets.");
    }

    //DISCONNECTS THE CLIENT
    private void Disconnect()
    {
        try
        {
            if (isConnected)
            {
                isConnected = false;
                hukoTCP.clientTCPSocket.Close();
                hukoUDP.socket.Close();
                Debug.Log("Disconnected from server.");
            }
        }catch(Exception)
        {
            Debug.Log("Exception thrown in Client.cs -> Disconnect()");
        }
    }
}
