using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;

public class Server
{
    public static int MaxPlayers { get; private set; }
    public static int Port { get; private set; }
    public static Dictionary<int, Client> clientDictionary = new Dictionary<int, Client>();
    public static Dictionary<int, string> OnlinePlayersETHAccountsDictionary = new Dictionary<int, string>();
    public delegate void PacketHandler(int _fromClient, Packet _packet);
    public static Dictionary<int, PacketHandler> packetHandlers;

    private static TcpListener tcpListener;
    public static UdpClient udpClient;

    //SERVER IS STARTING
    public static void Start(int _maxPlayers, int _port)
    {
        MaxPlayers = _maxPlayers;
        Port = _port;

        Debug.Log("Server is being started...");
        InitializeServerData();

        //LISTEN TO THE TCP CLIENTS
        tcpListener = new TcpListener(IPAddress.Any, Port); //SERVER IS GONNA LISTEN ANY IP IN ANY PORT
        tcpListener.Start();
        tcpListener.BeginAcceptTcpClient(ServerAcceptsNewTCPClients, null); //SERVER BEGINS TO ACCEPT THE CLIENT

        //LISTEN TO THE UDP CLIENTS
        udpClient = new UdpClient(Port); //SERVER IS GONNA LISTEN TO SPECIFIC PORT
        udpClient.BeginReceive(ServerAcceptsNewUDPClients, null); //SERVER IS GONNA BEGIN TO ACCEPT THE CLIENTS

        Debug.Log("Server started on port " + Port);
    }

    //HANDLES NEW TCP CONNECTIONS IN HERE
    private static void ServerAcceptsNewTCPClients(IAsyncResult _asyncResult)
    {
        TcpClient newClient = tcpListener.EndAcceptTcpClient(_asyncResult); //ACCEPTS THE NEW CLIENT AND ENDS LISTENING
        tcpListener.BeginAcceptTcpClient(ServerAcceptsNewTCPClients, null);  //STARTS LISTENING THE NEW CLIENTS
        Debug.Log("New player connected from: " + newClient.Client.RemoteEndPoint);

        for (int i = 1; i <= MaxPlayers; i++) //STORE NEW CLIENTS IN clientDictionary
        {
            if (clientDictionary[i].HukoTCP.clientSocket == null)
            {
                clientDictionary[i].HukoTCP.Connect(newClient);
                return;
            }
        }
        Debug.Log("The client from " + newClient.Client.RemoteEndPoint + "failed to connect!");
    }

    //HANDLES NEW UDP CLIENTS HERE
    private static void ServerAcceptsNewUDPClients(IAsyncResult _asyncResult)
    {
        try
        {
            IPEndPoint clientAddress = new IPEndPoint(IPAddress.Any, 0);
            byte[] udpData = udpClient.EndReceive(_asyncResult, ref clientAddress);
            udpClient.BeginReceive(ServerAcceptsNewUDPClients, null);

            if (udpData.Length < 4)
                return;

            //START OPENING THE UDP PACKET (UNLIKE THE TCP PACKET, UDP IS OPENNED HERE)
            using (Packet udpPacket = new Packet(udpData))
            {
                int clientID = udpPacket.ReadInt();

                if (clientID == 0)
                    return;
                if (clientDictionary[clientID].HukoUDP.clientAddress == null)
                {
                    clientDictionary[clientID].HukoUDP.Connect(clientAddress); //THAT MEANS THIS IS A NEW CONNECTION AND SAVE IT !
                    return;
                }
                //START HANDLE INCOMING UDP PACKET
                if (clientDictionary[clientID].HukoUDP.clientAddress.ToString() == clientAddress.ToString())
                {
                    clientDictionary[clientID].HukoUDP.HandleUDPData(udpPacket);
                }
            }
        }
        catch (Exception exception)
        {
            Debug.Log("[Error]: UDP receiving " + exception);
        }
    }

    //INITIALIZES DATAS THAT ARE SENT FROM CLIENT TO SERVER (RECEIVED DATA)
    private static void InitializeServerData()
    {
        for (int i = 1; i <= MaxPlayers; i++)
            clientDictionary.Add(i, new Client(i)); //SERVER-SIDE IP IS GIVEN TO THE CLIENTS

        packetHandlers = new Dictionary<int, PacketHandler>() //INITIALIZE THE PACKET TYPES THAT CAN COME FROM CLIENTS
        {
            { (int)ClientPackets.playerReceivedWelcomeMessage, ServerHandle.PlayerReceivedWelcomeMessage },
            { (int)ClientPackets.playerMovement, ServerHandle.PlayerMovement },
            { (int)ClientPackets.playerShoot, ServerHandle.PlayerShoot },
            { (int)ClientPackets.playerThrowItem, ServerHandle.PlayerThrowItem },
            { (int)ClientPackets.playerSendChatMessage, ServerHandle.PlayerSendChatMessage },
            { (int)ClientPackets.playerLevelUp, ServerHandle.PlayerLevelUp },
            { (int)ClientPackets.playerShieldActivated, ServerHandle.PlayerShieldActivated }
        };
        Debug.Log("Initialized packets.");
    }

    public static void Stop()
    {
        tcpListener.Stop();
        udpClient.Close();
    }
}
