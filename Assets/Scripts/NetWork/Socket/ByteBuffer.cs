//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using System;
using LuaInterface;

namespace ColaFramework.NetWork
{
    /// <summary>
    /// ByteBuffer Lua-C#网络字节缓冲类
    /// 用于接收网络数据并且在lua - C#之间交换
    /// </summary>
    public class ByteBuffer
    {

        MemoryStream stream = null;
        BinaryWriter writer = null;
        BinaryReader reader = null;

        public ByteBuffer()
        {
            stream = new MemoryStream();
            writer = new BinaryWriter(stream);
        }

        public ByteBuffer(byte[] data)
        {
            if (data != null)
            {
                stream = new MemoryStream(data);
                reader = new BinaryReader(stream);
            }
            else
            {
                stream = new MemoryStream();
                writer = new BinaryWriter(stream);
            }
        }

        public void Close()
        {
            if (writer != null) writer.Close();
            if (reader != null) reader.Close();

            stream.Close();
            writer = null;
            reader = null;
            stream = null;
        }

        public void WriteByte(byte v)
        {
            writer.Write(v);
        }

        public void WriteInt(int v)
        {
            writer.Write(v);
        }

        public void WriteuInt(uint v)
        {
            writer.Write(v);
        }

        public void WriteShort(short v)
        {
            writer.Write(v);
        }

        public void WriteuShort(ushort v)
        {
            writer.Write(v);
        }

        public void WriteLong(long v)
        {
            writer.Write(v);
        }

        public void WriteuLong(ulong v)
        {
            writer.Write(v);
        }

        public void WriteFloat(float v)
        {
            byte[] temp = BitConverter.GetBytes(v);
            Array.Reverse(temp);
            writer.Write(BitConverter.ToSingle(temp, 0));
        }

        public void WriteDouble(double v)
        {
            byte[] temp = BitConverter.GetBytes(v);
            Array.Reverse(temp);
            writer.Write(BitConverter.ToDouble(temp, 0));
        }

        public void WriteBoolean(bool v)
        {
            writer.Write(v);
        }

        public void WriteString(string v)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(v);
            writer.Write((ushort)bytes.Length);
            writer.Write(bytes);
        }

        public void WriteBytes(byte[] v)
        {
            writer.Write(v.Length);
            writer.Write(v);
        }

        public void WriteBuffer(LuaByteBuffer strBuffer)
        {
            WriteBytes(strBuffer.buffer);
        }

        public byte ReadByte()
        {
            return reader.ReadByte();
        }

        public int ReadInt()
        {
            return (int)reader.ReadInt32();
        }

        public uint ReaduInt()
        {
            return reader.ReadUInt32();
        }

        public short ReadShort()
        {
            return reader.ReadInt16();
        }

        public ushort ReaduShort()
        {
            return reader.ReadUInt16();
        }

        public long ReadLong()
        {
            return reader.ReadInt64();
        }

        public ulong ReaduLong()
        {
            return reader.ReadUInt64();
        }

        public float ReadFloat()
        {
            byte[] temp = BitConverter.GetBytes(reader.ReadSingle());
            Array.Reverse(temp);
            return BitConverter.ToSingle(temp, 0);
        }

        public double ReadDouble()
        {
            byte[] temp = BitConverter.GetBytes(reader.ReadDouble());
            Array.Reverse(temp);
            return BitConverter.ToDouble(temp, 0);
        }

        public string ReadString()
        {
            ushort len = ReaduShort();
            byte[] buffer = new byte[len];
            buffer = reader.ReadBytes(len);
            return Encoding.UTF8.GetString(buffer);
        }

        public byte[] ReadBytes()
        {
            int len = ReadInt();
            return reader.ReadBytes(len);
        }

        public LuaByteBuffer ReadBuffer()
        {
            byte[] bytes = ReadBytes();
            return new LuaByteBuffer(bytes);
        }

        private byte[] flip(byte[] _data)
        {
            if (BitConverter.IsLittleEndian)
                Array.Reverse(_data);
            return _data;
        }

        public byte[] ToBytes()
        {
            writer.Flush();
            return stream.ToArray();
        }

        public void Flush()
        {
            stream.Position = 0;
            writer.Flush();
            stream.Flush();
        }
    }
}