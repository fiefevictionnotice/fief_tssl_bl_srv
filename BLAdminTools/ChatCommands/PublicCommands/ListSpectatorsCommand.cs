using DoFAdminTools.Helpers;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace DoFAdminTools.ChatCommands.PublicCommands;

public class ListSpectatorsCommand: ChatCommand
{
    public override string CommandText => "specs";
    public override string Description => "Lists all players currently spectating the game";
    public override bool CanExecute(NetworkCommunicator executor) => true;
    
    public override bool Execute(NetworkCommunicator executor, string args)
    {
        var specTeam = Mission.Current.SpectatorTeam;
        
        Helper.SendMessageToPeer(executor, "----Spectators:----");
        foreach (var peer in GameNetwork.NetworkPeers)
        {
            if (!peer.IsSynchronized)
                continue;
            
            var missionPeer = peer.GetComponent<MissionPeer>();
            if (missionPeer == null)
                continue;
            
            if (missionPeer.Team is null || missionPeer.Team == specTeam)
                Helper.SendMessageToPeer(executor, peer.UserName);
        }
        Helper.SendMessageToPeer(executor, "---------------------");

        return true;
    }
}