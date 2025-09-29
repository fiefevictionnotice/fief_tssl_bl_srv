using System.Linq;
using DoFAdminTools.Helpers;
using TaleWorlds.MountAndBlade;

namespace DoFAdminTools.ChatCommands.AdminCommands;

public class RemoveHorsesCommand : AdminChatCommand
{
    public override string CommandText => "removeHorses";

    public override string Description => "Removes all unmounted horses.";

    public override bool Execute(NetworkCommunicator executor, string args)
    {
        if (Mission.Current?.MountsWithoutRiders.Count == 0)
        {
            Helper.SendMessageToPeer(executor, "No unmounted horses were found.");
            return false;
        }

        // collect all first as looping over keyset & removing directly would cause change in Collection during looping
        var agents = Mission.Current.MountsWithoutRiders
            .Where(pair => pair.Key != null)
            .Select(pair => pair.Key)
            .ToList();
            
        foreach (var agent in agents)
        {
            agent.FadeOut(true, true);
        }
            
        Helper.SendMessageToAllPeers($"{executor.UserName} removed all stray horses.");
        return true;
    }
}