using DoFAdminTools.Helpers;
using System.Reflection;
using TaleWorlds.MountAndBlade;

namespace DoFAdminTools.ChatCommands.AdminCommands;

public class EndWarmupCommand : AdminChatCommand
{
    public override string CommandText => "endwarmup";

    public override string Description => "Sets the warmup timer to 30 seconds.";

    public override bool CanExecute(NetworkCommunicator executor)
    {
        return base.CanExecute(executor) && Mission.Current.GetMissionBehavior<MultiplayerWarmupComponent>() != null;
    }

    public override bool Execute(NetworkCommunicator executor, string args)
    {
        MultiplayerWarmupComponent multiplayerWarmupComponent =
            Mission.Current.GetMissionBehavior<MultiplayerWarmupComponent>();
        
        if (multiplayerWarmupComponent == null || !multiplayerWarmupComponent.IsInWarmup)
        {
            Helper.SendMessageToPeer(executor, $"You can only end the warmup during the warmup phase.");
            return false;
        }

        multiplayerWarmupComponent.EndWarmupProgress();
        Helper.SendMessageToAllPeers($"{executor.UserName} reduced the warmup duration.");

        return true;
    }
}