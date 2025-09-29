using DoFAdminTools.Helpers;
using System.Reflection;
using TaleWorlds.MountAndBlade;

namespace DoFAdminTools.ChatCommands.AdminCommands;

public class ExtendWarmupCommand : AdminChatCommand
{
    public override string CommandText => "extendwarmup";

    public override string Description => "Resets the warmup timer.";
    
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
            Helper.SendMessageToPeer(executor, $"You can only extend the warmup duration during the warmup phase.");
            return false;
        }
        
        multiplayerWarmupComponent
            .GetPropertyInfo("WarmupState")
            .SetValue(multiplayerWarmupComponent, MultiplayerWarmupComponent.WarmupStates.InProgress);

        multiplayerWarmupComponent
            .GetFieldValue<MultiplayerTimerComponent>("_timerComponent")?
            .StartTimerAsServer(MultiplayerWarmupComponent.TotalWarmupDuration);
        
        Helper.SendMessageToAllPeers($"{executor.UserName} extended the warmup duration!");

        return true;
    }
}