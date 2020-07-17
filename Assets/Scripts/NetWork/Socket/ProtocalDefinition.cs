//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

/// <summary>
/// 网络数据类型:是ping命令还是普通message
/// </summary>
namespace ColaFramework.NetWork
{
    public enum eProtocalCommand
    {
        sc_ping = 0x1000,
        sc_message = 0x2000,
    }
}