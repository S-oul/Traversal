using ENet6;
using Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static Protocols.Protocole;
public class Client : MonoBehaviour
{
    public TMP_InputField hostIP_Field;
    public TMP_InputField playerName_Field;

    private Host enetHost = null;
    private Peer? serverPeer = null;

    public void Connect(string addressString)
    {
        Address address = new Address();

        // Pour localhost, on force IPv4 pour éviter la mauvaise résolution sur ::1
        AddressType type = (addressString == "localhost") ? AddressType.IPv4 : AddressType.Any;

        if (!address.SetHost(type, addressString))
        {
            Debug.LogError("Échec de résolution pour : " + addressString);
            return;
        }

        address.Port = 25565;
        print("Tentative de connexion vers " + address.GetIP());

        if (enetHost != null)
        {
            enetHost.Dispose();
            enetHost = null;
        }

        enetHost = new Host();
        enetHost.Create(address.Type, 1, 0);

        serverPeer = enetHost.Connect(address, 0);
    }
    void Start()
    {
        if (!ENet6.Library.Initialize())
            throw new Exception("Failed to initialize ENet");
    }

    public void ConnectToWrittenIP()
    {
        Connect(hostIP_Field.text);
    }

    private void OnApplicationQuit()
    {
        ENet6.Library.Deinitialize();
    }

    void FixedUpdate()
    {
        ENet6.Event evt = new ENet6.Event();
        if (enetHost != null && enetHost.Service(0, out evt) > 0)
        {
            do
            {
                switch (evt.Type)
                {
                    case ENet6.EventType.None:
                        {
                            print("None ?");
                        }
                        break;

                    case ENet6.EventType.Connect:
                        {
                            print("Connect");

                            //Our packet 
                            PlayerInfoPacket infoPacket = new()
                            {
                                name = playerName_Field.text
                            };

                            List<byte> data = new List<byte>();
                            infoPacket.Serialize(ref data);
                            
                            //Enet Packets
                            Packet packet = default;
                            packet.Create(data.ToArray(), PacketFlags.Reliable);
                            serverPeer.Value.Send(0, ref packet);

                            break;
                        }

                    case ENet6.EventType.Disconnect:
                        print("Disconnect");
                        serverPeer = null;
                        Destroy(this);
                        break;

                    case ENet6.EventType.Receive:
                        {
                            byte[] bytes = new byte[evt.Packet.Length];
                            evt.Packet.CopyTo(bytes);
                            HandleFromServer(bytes);
                            break;
                        }

                    case ENet6.EventType.Timeout:
                        print("Timeout");
                        break;
                }
            }
            while (enetHost.CheckEvents(out evt) > 0);
        }
    }
    void HandleFromServer(byte[] bytes)
    {
        List<byte> data = bytes.ToList();
        int offset = 0;
        Opcode opcode = (Opcode)Deserialize_Uint8(data, ref offset);

        switch (opcode)
        {
            // case Opcode.:
            //     {

            //         break;
            //     }
        }

                
        
    }




    void print(string str)
    {
        MonoBehaviour.print("[CLIENT] " + str);
    }
}
