//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System.IO;
using System;
using UnityEngine;

namespace ColaFramework.NetWork
{

    //常量数据
    public class Constants
    {
        //消息：数据总长度(4byte) + 协议类型(4byte) + 数据(N byte)
        public static int HEAD_DATA_LEN = 4;
        public static int HEAD_TYPE_LEN = 4;
        public static int HEAD_LEN //8byte
        {
            get { return HEAD_DATA_LEN + HEAD_TYPE_LEN; }
        }

        public static readonly int PING_PROTO_CODE = 1; //ping protocol的id
    }

    public enum NetErrorEnum
    {
        BeginConnectError = 1,
        ConnnectedError = 2,
        SendMsgError = 3,
        ReceiveError = 4,
    }

    /// <summary>
    /// 网络数据结构
    /// </summary>
    [System.Serializable]
    public struct sSocketData
    {
        public byte[] data;
        public int protocal;
        public int buffLength;
        public int dataLength;
    }

    /// <summary>
    /// 网络数据缓存器，
    /// </summary>
    [System.Serializable]
    public class DataBuffer
    {//自动大小数据缓存器
        private int minBuffLen;
        private byte[] buff;
        private int curBuffPosition;
        private int buffLength = 0;
        private int dataLength;
        private int protocal;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="minBuffLen">最小缓冲区大小</param>
        public DataBuffer(int minBuffLen = 1024)
        {
            if (minBuffLen <= 0)
            {
                this.minBuffLen = 1024;
            }
            else
            {
                this.minBuffLen = minBuffLen;
            }
            buff = new byte[this.minBuffLen];
        }

        /// <summary>
        /// 添加缓存数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="dataLen"></param>
        public void AddBuffer(byte[] data, int dataLen)
        {
            if (dataLen > buff.Length - curBuffPosition)//超过当前缓存
            {
                byte[] tmpBuff = new byte[curBuffPosition + dataLen];
                Array.Copy(buff, 0, tmpBuff, 0, curBuffPosition);
                Array.Copy(data, 0, tmpBuff, curBuffPosition, dataLen);
                buff = tmpBuff;
                tmpBuff = null;
            }
            else
            {
                Array.Copy(data, 0, buff, curBuffPosition, dataLen);
            }
            curBuffPosition += dataLen;//修改当前数据标记
        }

        /// <summary>
        /// 更新数据长度
        /// </summary>
        public void UpdateDataLength()
        {
            if (dataLength == 0 && curBuffPosition >= Constants.HEAD_LEN)
            {
                byte[] tmpDataLen = new byte[Constants.HEAD_DATA_LEN];
                Array.Copy(buff, 0, tmpDataLen, 0, Constants.HEAD_DATA_LEN);
                buffLength = BitConverter.ToInt32(tmpDataLen, 0);

                byte[] tmpProtocalType = new byte[Constants.HEAD_TYPE_LEN];
                Array.Copy(buff, Constants.HEAD_DATA_LEN, tmpProtocalType, 0, Constants.HEAD_TYPE_LEN);
                protocal = BitConverter.ToUInt16(tmpProtocalType, 0);

                dataLength = buffLength - Constants.HEAD_LEN;
            }
        }

        /// <summary>
        /// 获取一条可用数据，返回值标记是否有数据
        /// </summary>
        /// <param name="tmpSocketData"></param>
        /// <returns></returns>
        public bool GetData(out sSocketData tmpSocketData)
        {
            tmpSocketData = new sSocketData();

            if (buffLength <= 0)
            {
                UpdateDataLength();
            }

            if (buffLength > 0 && curBuffPosition >= buffLength)
            {
                tmpSocketData.buffLength = buffLength;
                tmpSocketData.dataLength = dataLength;
                tmpSocketData.protocal = protocal;
                tmpSocketData.data = new byte[dataLength];
                Array.Copy(buff, Constants.HEAD_LEN, tmpSocketData.data, 0, dataLength);
                curBuffPosition -= buffLength;
                byte[] tmpBuff = new byte[curBuffPosition < minBuffLen ? minBuffLen : curBuffPosition];
                Array.Copy(buff, buffLength, tmpBuff, 0, curBuffPosition);
                buff = tmpBuff;


                buffLength = 0;
                dataLength = 0;
                protocal = 0;
                return true;
            }
            return false;
        }

    }
}