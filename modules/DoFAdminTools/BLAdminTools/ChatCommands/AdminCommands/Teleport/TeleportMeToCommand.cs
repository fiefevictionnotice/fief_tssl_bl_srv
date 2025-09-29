using DoFAdminTools.Helpers;
using TaleWorlds.MountAndBlade;

namespace DoFAdminTools.ChatCommands.AdminCommands.Teleport;

public class TeleportMeToCommand : AdminChatCommand
{
    public override string CommandText => "tpmeto";
    public override string UsageDescription => $"{base.UsageDescription} [PLAYERNAME]";

    public override string Description =>
        "Teleports you to the first player whose name contains the given text.";

    public override bool Execute(NetworkCommunicator executor, string args)
    {
        if (executor.ControlledAgent is null)
        {
            Helper.SendMessageToPeer(executor, "You need to be alive to use this command.");
            return false;
        }

        if (args.Length == 0)
        {
            Helper.SendMessageToPeer(executor, "You need to provide a name.");
            return false;
        }
            
        foreach (NetworkCommunicator peer in GameNetwork.NetworkPeers)
        {
            if (peer == executor)
                continue;
                
            if (peer.UserName.Contains(args) && peer.ControlledAgent != null)
            {
                executor.ControlledAgent.TeleportToPosition(peer.ControlledAgent.Position);
                Helper.SendMessageToAllPeers($"{executor.UserName} teleported themself to {peer.UserName}.");
                    
                return true;
            }
        }
            
        Helper.SendMessageToPeer(executor, "No player found with matching name.");
        return false;
    }
}