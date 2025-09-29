using System;
using NetworkMessages.FromServer;
using TaleWorlds.MountAndBlade;

namespace DoFAdminTools.Helpers;

public static class Helper
{
    private const string Prefix = "[DAT] ";
    private const string WarningPrefix = Prefix + "[WARNING] ";
    private const string ErrorPrefix = Prefix + "[ERROR] ";

    public static void Print(string message)
    {
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine(Prefix + message);
        Console.ResetColor();
    }

    public static void PrintWarning(string message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(WarningPrefix + message);
        Console.ResetColor();
    }
        
    public static void PrintError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(ErrorPrefix + message);
        Console.ResetColor();
    }
        
    public static void SendMessageToAllPeers(string message)
    {
        GameNetwork.BeginBroadcastModuleEvent();
        GameNetwork.WriteMessage(new ServerMessage(Prefix + message));
        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.IncludeUnsynchronizedClients);
    }

    public static void SendMessageToPeer(NetworkCommunicator peer, string message)
    {
        GameNetwork.BeginModuleEventAsServer(peer);
        GameNetwork.WriteMessage(new ServerMessage(Prefix + message));
        GameNetwork.EndModuleEventAsServer();
    }

    public static void SendAdminMessageToAllPeers(string message, bool isBroadcast = false)
    {        
        GameNetwork.BeginBroadcastModuleEvent();
        GameNetwork.WriteMessage(new ServerAdminMessage(message, isBroadcast));
        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
    }
}