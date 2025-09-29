using System.Linq;
using DoFAdminTools.Helpers;
using TaleWorlds.MountAndBlade;

namespace DoFAdminTools.ChatCommands.PublicCommands;

public class HelpCommand: ChatCommand
{
    private readonly ChatCommand[] _registeredCommands;
    
    public override string CommandText => "help";
    public override string UsageDescription => $"{base.UsageDescription} (COMMANDNAME)";
    public override string Description => "Prints this.";

    public HelpCommand()
    {
        // this help command isn't part of the list at time of creation, so add it manually
        var commands = ChatCommandHandler.Instance.Commands;
        int registeredAmount = commands.Length;
        
        _registeredCommands = new ChatCommand[registeredAmount + 1];
        for (int i = 0; i < registeredAmount; i++)
        {
            _registeredCommands[i] = commands[i];
        }

        _registeredCommands[registeredAmount] = this;
    }

    public override bool CanExecute(NetworkCommunicator executor) => true;

    public override bool Execute(NetworkCommunicator executor, string args)
    {
        if (args.Length == 0)
            return SendCommandList(executor);
        
        return SendCommandDetails(executor, args);
    }

    private bool SendCommandList(NetworkCommunicator executor)
    {
        Helper.SendMessageToPeer(executor, "Available commands:");

        foreach (var command in _registeredCommands)
        {
            if (command.CanExecute(executor))
                Helper.SendMessageToPeer(executor, command.CommandText);
        }
        
        Helper.SendMessageToPeer(executor, $"Type {DoFConfigOptions.Instance.CommandPrefix}{CommandText} [COMMANDNAME] to learn more about any command!");

        return true;
    }

    private bool SendCommandDetails(NetworkCommunicator executor, string commandName)
    {
        var command = _registeredCommands.FirstOrDefault(command => command.CommandText == commandName);

        if (command == null || !command.CanExecute(executor))
        {
            Helper.SendMessageToPeer(executor, "That command does not exist or you do not have access to it.");
            return false;
        }
        
        Helper.SendMessageToPeer(executor, $"Command: {command.CommandText}");
        Helper.SendMessageToPeer(executor, $"Usage: {command.UsageDescription}");
        Helper.SendMessageToPeer(executor, $"Description: {command.Description}");
        
        return true;
    }
}