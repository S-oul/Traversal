using System;
using System.Collections.Generic;
using System.Linq;
using ENet6;
using static Protocols.Protocole;
using TMPro;
using UnityEngine;
using Event = ENet6.Event;
using EventType = ENet6.EventType;

public class Server : MonoBehaviour
{
    public static Server Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else DestroyImmediate(this);
    }
    public TextMeshProUGUI textLogger;
    class ServerData
    {
        public ENet6.Host host;
    };

    private Dictionary<Peer,string> _connectedPeers = new();

    private ServerData _serverData = new ServerData();
    private UInt32 nextTick = ENet6.Library.Time;

    private void Start()
    {
        _connectedPeers.Clear();

        // Initialisation d'enet
        if (!ENet6.Library.Initialize())
        {
            throw new Exception("failed to initialize enet");
        }

        // On planifie la libération d'enet à la fin de la fonction

        ENet6.Address address = ENet6.Address.BuildAny(AddressType.IPv6);

        address.Port = 25565;

        _serverData.host = new Host();
        _serverData.host.Create(AddressType.Any, address, 10, 10, 0, 0);
        if (!_serverData.host.IsSet)
        {
            throw new Exception("Failed to create ENet host (is port free?)");
        }

        print("Server is Runnin' on port : " + address.Port);
    }

    // Update is called once per frame
    void Update()
    {
        // On récupère le temps actuel
        UInt32 now = ENet6.Library.Time;

        // On gère les événements ENet
        while (_serverData.host.Service(1, out Event eNetEvent) > 0)
        {
            do
            {
                switch (eNetEvent.Type)
                {
                    // Un nouveau joueur s'est connecté
                    case EventType.Connect:
                        print($"Peer # {eNetEvent.Peer.ID} connected!");
                        break;

                    // Un joueur s'est déconnecté
                    case EventType.Disconnect:
                        _connectedPeers.Remove(eNetEvent.Peer);
                        print($"Peer # {eNetEvent.Peer.ID} disconnected!");
                        break;

                    // On a reçu des données d'un joueur
                    case EventType.Receive:
                        {
                            byte[] bytes = new byte[eNetEvent.Packet.Length];
                            eNetEvent.Packet.CopyTo(bytes);
                            HandleFromClient(eNetEvent.Peer, bytes);

                            eNetEvent.Packet.Dispose();
                        }
                        break;
                }
            }
            while (_serverData.host.Service(1, out eNetEvent) > 0);
        }

        // Serveur or Physique ?
        if (now >= nextTick)
        {
            Tick(ref _serverData);
            //17 ms
            nextTick += 17;
        }

    }

    private void HandleFromClient(Peer peer, byte[] bytes)
    {
        List<byte> data = bytes.ToList();
        int offset = 0;
        Opcode opcode = (Opcode)Deserialize_Uint8(data, ref offset);

        switch (opcode)
        {
            case Opcode.C_PlayerInfo:
                {
                    packet_PlayerInfoPacket info = packet_PlayerInfoPacket.Deserialize(data, offset);
                        _connectedPeers[peer] = info.name;
                    print("peer " + peer + " is " + info.name);
                    break;
                }
            case Opcode.C_PlayerStart:
                {
                    foreach (var kvp in _connectedPeers)
                    {
                        Peer OtherPeer = kvp.Key;

                        packet_ServerStartedGame packet = new();
                        List<byte> data2 = new List<byte>();
                        packet.Serialize(ref data2);

                        Packet EnetPacket = default;
                        EnetPacket.Create(data2.ToArray(), PacketFlags.Reliable);

                        OtherPeer.Send(0,ref EnetPacket);
                    }
                    print("From peer " + peer + " -> Game Started");
                    break;
                }
            case Opcode.A_PlayerMove:
                {
                    Packet EnetPacket = default;
                    EnetPacket.Create(bytes, PacketFlags.Reliable);

                    foreach (var kvp in _connectedPeers)
                    {
                        Peer OtherPeer = kvp.Key;
                        OtherPeer.Send(0,ref EnetPacket);
                    }
                    break;
                }
        }
    }
    

    //Tick function
    private void Tick(ref ServerData servData)
    {
        //print("Tick");
        //Server Update on Fixed DeltaTime (now and Tick)
    }

    private void OnApplicationQuit()
    {
        ShutdownServer();
    }

    private void OnDestroy()
    {
        ShutdownServer();
    }

    private void ShutdownServer()
    {
        if (_serverData?.host != null && _serverData.host.IsSet)
        {
            foreach (var kvp in _connectedPeers)
            {
                Peer peer = kvp.Key;
                peer.DisconnectNow(0);
            }
            _connectedPeers.Clear();

            _serverData.host.Flush();
            _serverData.host.Dispose();
            _serverData.host = null;
            print("Host disposed.");
        }

        Library.Deinitialize();
        print("ENet library deinitialized.");
    }









    string stack = "";
    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }



    void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (stack.Length > 2000) ClearConsole();
        logString = logString.Split("UnityEngine")[0];
        textLogger.text = "> " + logString + "\n" + stack;
        stack = logString + "\n" + stack;
    }

    public void ClearConsole()
    {
        stack = "";
        print("\nConsole Cleared");
    }

    void print(string str)
    {
        MonoBehaviour.print("[SERVER] " + str);
    }
}
