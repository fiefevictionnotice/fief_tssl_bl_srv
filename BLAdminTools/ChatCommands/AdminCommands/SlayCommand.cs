using DoFAdminTools.Helpers;
using TaleWorlds.MountAndBlade;

namespace DoFAdminTools.ChatCommands.AdminCommands;

public class SlayCommand : AdminChatCommand
{
    public override string CommandText => "slay";
    public override string UsageDescription => $"{base.UsageDescription} (PLAYERNAME)";

    public override string Description =>
        "Kills any player whose name contains the given name, or all players if no name is given.";
    
    public override bool Execute(NetworkCommunicator executor, string args)
    {
        bool checkName = args.Length > 0;
        int counter = 0;
        
        foreach (var peer in GameNetwork.NetworkPeers)
        {
            if (checkName && !peer.UserName.Contains(args))
                continue;
            
            if (peer.ControlledAgent == null)
                continue;
            
            peer.ControlledAgent.FadeOut(true, true);
            counter++;
        }
        
        if (checkName)
            Helper.SendMessageToAllPeers($"{executor.UserName} has slain {counter} players!");
        else
            Helper.SendMessageToAllPeers($"{executor.UserName} has slain everyone!");
        
        return true;
    }
}