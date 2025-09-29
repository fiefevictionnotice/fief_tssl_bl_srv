using TaleWorlds.MountAndBlade;

namespace DoFAdminTools.ChatCommands.AdminCommands;

public abstract class AdminChatCommand: ChatCommand
{
    public override bool CanExecute(NetworkCommunicator executor)
    {
        return executor.IsAdmin;
    }
}