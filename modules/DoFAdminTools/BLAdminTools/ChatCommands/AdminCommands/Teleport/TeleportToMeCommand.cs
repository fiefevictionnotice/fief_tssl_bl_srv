using DoFAdminTools.Helpers;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace DoFAdminTools.ChatCommands.AdminCommands.Teleport;

public class TeleportToMeCommand : AdminChatCommand
{
    public override string CommandText => "tptome";
    public override string UsageDescription => $"{base.UsageDescription} (PLAYERNAME)";

    public override string Description =>
        "Teleports all players whose names contain the given text to you, or all players if none is given.";

    public override bool Execute(NetworkCommunicator executor, string args)
    {
        if (executor.ControlledAgent is null)
        {
            Helper.SendMessageToPeer(executor, "You need to be alive to use this command.");
            return false;
        }

        Vec3 curPos = executor.ControlledAgent.Position;
        bool checkPlayerName = args.Length > 0;

        int count = 0;
        foreach (NetworkCommunicator peer in GameNetwork.NetworkPeers)
        {
            if (peer == executor)
                continue;
                
            if (peer.ControlledAgent is null)
                continue;
                
            // if a parameter was provided, it must be part of the players name.
            if (checkPlayerName && !(peer.UserName.Contains(args)))
                continue;
                
            peer.ControlledAgent.TeleportToPosition(curPos);
            count++;
        }

        if (count > 0)
        {
            Helper.SendMessageToAllPeers($"{executor.UserName} teleported {count} other player(s) to their position.");
                
            return true;
        }

        Helper.SendMessageToPeer(executor, "No player found with matching name.");
        return false;
    }
}