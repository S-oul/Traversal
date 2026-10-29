using System;
using System.Collections.Generic;
using UnityEngine;
using System.Net;

namespace Protocols
{
public static class Protocole
{
#region Base Serialize
    public static void Serialize_Uint8(ref List<byte> byteArray, byte value)
    {
        byteArray.Add(value);
    }
    public static void Serialize_Int8(ref List<byte> byteArray, sbyte value)
    {
        Serialize_Uint8(ref byteArray, (byte)value);
    }
    public static void Serialize_Uint16(ref List<byte> byteArray, ushort value)
    {
        short htons = IPAddress.HostToNetworkOrder((short)value);
        byte[] ar = BitConverter.GetBytes(htons);
        byteArray.AddRange(ar);
    }
    public static void Serialize_int16(ref List<byte> byteArray, short value)
    {
        Serialize_Uint16(ref byteArray, (ushort)value);
    }
    public static void Serialize_Uint32(ref List<byte> byteArray, uint value)
    {
        int htoni = IPAddress.HostToNetworkOrder((int)value);
        byte[] ar = BitConverter.GetBytes(htoni);
        byteArray.AddRange(ar);
    }
    public static void Serialize_int32(ref List<byte> byteArray, int value)
    {
        Serialize_Uint32(ref byteArray, (uint)value);
    }
    public static void Serialize_Uint64(ref List<byte> byteArray, ulong value)
    {
        long htonl = IPAddress.HostToNetworkOrder((long)value);
        byte[] ar = BitConverter.GetBytes(htonl);
        byteArray.AddRange(ar);
    }
    public static void Serialize_int64(ref List<byte> byteArray, long value)
    {
        Serialize_Uint64(ref byteArray, (ulong)value);
    }
    public static void Serialize_f(ref List<byte> byteArray, float value)
    {
        byte[] bytes = BitConverter.GetBytes(value);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes);
        }
        byteArray.AddRange(bytes);
    }
    public static void Serialize_str(ref List<byte> byteArray, string value)
    {
        Serialize_Uint32(ref byteArray, (uint)value.Length);
        for (int i = 0; i < value.Length; i++)
        {
            byte[] ar = BitConverter.GetBytes(value[i]);
            byteArray.AddRange(ar);
        }
    }
    public static void Serialize_color(ref List<byte> byteArray, Color value)
    {
        Serialize_f(ref byteArray, value.r);
        Serialize_f(ref byteArray, value.g);
        Serialize_f(ref byteArray, value.b);
        Serialize_f(ref byteArray, value.a);
    }
#endregion
    
#region Base Deserialize
    public static byte Deserialize_Uint8(List<byte> byteArray, ref int offset)
    {
        byte value = byteArray[offset];
        offset++;
        return value;
    }
    public static sbyte Deserialize_int8(List<byte> byteArray, ref int offset)
    {
        byte val = Deserialize_Uint8(byteArray, ref offset);
        return unchecked((sbyte)val);
    }
    public static ushort Deserialize_Uint16(List<byte> byteArray, ref int offset)
    {
        short value;
        byte[] ar = new byte[2];
        for (int i = 0; i < 2; i++)
        {
            ar[i] = byteArray[offset + i];
        }
        value = BitConverter.ToInt16(ar);
        ushort ntohs = (ushort)IPAddress.NetworkToHostOrder(value);
        offset += 2;
        return ntohs;
    }
    public static short Deserialize_int16(List<byte> byteArray, ref int offset)
    {
        ushort value = Deserialize_Uint16(byteArray, ref offset);
        return unchecked((short)value);
    }
    public static uint Deserialize_Uint32(List<byte> byteArray, ref int offset)
    {
        int value;
        byte[] ar = new byte[4];
        for (int i = 0; i < 4; i++)
        {
            ar[i] = byteArray[offset + i];
        }
        value = BitConverter.ToInt32(ar);
        uint ntohi = (uint)IPAddress.NetworkToHostOrder(value);
        offset += 4;
        return ntohi;
    }
    public static int Deserialize_int32(List<byte> byteArray, ref int offset)
    {
        uint value = Deserialize_Uint32(byteArray, ref offset);
        return unchecked((int)value);
    }
    public static ulong Deserialize_Uint64(List<byte> byteArray, ref int offset)
    {
        long value;
        byte[] ar = new byte[8];
        for (int i = 0; i < 8; i++)
        {
            ar[i] = byteArray[offset + i];
        }
        value = BitConverter.ToInt64(ar);
        ulong ntohl = (ulong)IPAddress.NetworkToHostOrder(value);
        offset += 8;
        return ntohl;
    }
    public static long Deserialize_int64(List<byte> byteArray, ref int offset)
    {
        ulong value = Deserialize_Uint64(byteArray, ref offset);
        return unchecked((long)value);
    }
    public static float Deserialize_f(List<byte> byteArray, ref int offset)
    {
        float val;
        byte[] ar = new byte[4];
        for (int i = 0; i < 4; i++)
        {
            ar[i] = byteArray[offset + i];
        }

        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(ar);
        }
        val = BitConverter.ToSingle(ar);
        offset += sizeof(Single);
        return val;
    }
    public static string Deserialize_str(List<byte> byteArray, ref int offset)
    {
        uint length = Deserialize_Uint32(byteArray, ref offset);
        char[] str = new char[length];
        for (int i = 0; i < length; i++)
        {
            int j = i * 2;
            str[i] = (char)byteArray[j + offset];
        }
        string val = new string(str);
        offset += (int)length*2;
        return val;
    }
    public static Color Deserialize_color(List<byte> byteArray, ref int offset)
    {
        Color c;
        c.r = Deserialize_f(byteArray, ref offset);
        c.g = Deserialize_f(byteArray, ref offset);
        c.b = Deserialize_f(byteArray, ref offset);
        c.a = Deserialize_f(byteArray, ref offset);
        return c;
    }
#endregion

    public enum Opcode : byte
    {
        C_PlayerInfo,
        C_PlayerStart,
        C_PlayerPlay,

        S_ServerStartedGame,
        S_ServerValidatePlayer,
        S_Seed
        
    }


#region packets

    public struct packet_PlayerInfoPacket
    {
        static Opcode opcode = Opcode.C_PlayerInfo;

        public string name;

        public void Serialize(ref List<byte> byteArray)
        {
            Serialize_Uint8(ref byteArray, (byte)opcode);
            Serialize_str(ref byteArray, name);

        }
        public static packet_PlayerInfoPacket Deserialize(List<byte> byteArray, int offset)
        {
            packet_PlayerInfoPacket packet;
            packet.name = Deserialize_str(byteArray, ref offset);
            return packet;
        }
    };

    public struct packet_PlayerStartGame
    {
        static Opcode opcode = Opcode.C_PlayerStart;

        public void Serialize(ref List<byte> byteArray)
        {
            Serialize_Uint8(ref byteArray, (byte)opcode);

        }
    }

    public struct packet_PlayerTurn
    {
        static Opcode opcode = Opcode.C_PlayerPlay;
        
        public UInt16 turnNumber;
        // Numerotation Vg4 (Vanguard to g4)
        public string move;


        public void Serialize(ref List<byte> byteArray)
        {
            Serialize_Uint8(ref byteArray, (byte)opcode);
            Serialize_Uint16(ref byteArray, turnNumber);
            Serialize_str(ref byteArray, move);

        }

        public static packet_PlayerTurn Deserialize(List<byte> byteArray, int offset)
        {
            packet_PlayerTurn packet;
            packet.turnNumber = Deserialize_Uint16(byteArray, ref offset);
            packet.move = Deserialize_str(byteArray, ref offset);
            return packet;
        }
    }

    

    //SERVER


    public struct packet_ServerStartedGame
    {
        static Opcode opcode = Opcode.S_ServerStartedGame;

        public void Serialize(ref List<byte> byteArray)
        {
            Serialize_Uint8(ref byteArray, (byte)opcode);

        }
    }
    public struct packet_SeedPacket
    {
        static Opcode opcode = Opcode.S_Seed;

        public int seed;
        
        public void Serialize(ref List<byte> byteArray)
        {
            Serialize_Uint8(ref byteArray, (byte)opcode);
            Serialize_int32(ref byteArray, seed);
        }
        
        public static packet_SeedPacket Deserialize(List<byte> byteArray, int offset)
        {
            packet_SeedPacket packet;
            packet.seed = Deserialize_int32(byteArray, ref offset);
            return packet;
        }
    }

#endregion
}
}