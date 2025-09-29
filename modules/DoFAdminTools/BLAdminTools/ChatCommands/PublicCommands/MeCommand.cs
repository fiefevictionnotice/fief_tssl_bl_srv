using DoFAdminTools.Helpers;
using TaleWorlds.MountAndBlade;

namespace DoFAdminTools.ChatCommands.PublicCommands;

public class MeCommand: ChatCommand
{
    public override string CommandText => "me";
    public override string Description => "Prints information about yourself";

    public override bool CanExecute(NetworkCommunicator executor) => true;

    public override bool Execute(NetworkCommunicator executor, string args)
    {
        Helper.SendMessageToPeer(executor, $"Your PlayerID is {executor.VirtualPlayer.Id}");
        return true;
    }
}