using System;
using DoFAdminTools.Helpers;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace DoFAdminTools.ChatCommands.AdminCommands.Teleport;

public class MoveCommand : AdminChatCommand
{
    public override string CommandText => "move";
    public override string UsageDescription => $"{base.UsageDescription} [X] [Z]";

    public override string Description =>
        "Teleports you to a new position by provided coordinates from current position.";

    public override bool Execute(NetworkCommunicator executor, string args)
    {
        string[] stringArgs = args.Trim().Split();

        if (stringArgs.Length != 2)
        {
            Helper.SendMessageToPeer(executor, "Incorrect amount of parameters. Requires 2, provided " + args.Length + "("+args+")");
            return false;
        }

        if (executor.ControlledAgent is null)
        {
            Helper.SendMessageToPeer(executor, "You need to be alive to be teleported.");
            return false;
        }

        if (int.TryParse(stringArgs[0], out int xAmount) && 
            int.TryParse(stringArgs[1], out int zAmount))
        {
            Vec3 curPos = executor.ControlledAgent.Position;

            // angle is -PI .. PI, off by pi/2
            float angle = ClampPi(executor.ControlledAgent.LookDirectionAsAngle);
                
            double newX = curPos.x + xAmount * Math.Cos(angle);
            double newY = curPos.y + xAmount * Math.Sin(angle);
                
            Vec3 newPos = new Vec3((float) newX, (float) newY, curPos.z + zAmount);
            executor.ControlledAgent.TeleportToPosition(newPos);
                
            Helper.SendMessageToAllPeers($"{executor.UserName} teleported.");
            return true;
        }
            
        Helper.SendMessageToPeer(executor, "Failed to parse one or more coordinates, please make sure to provide only numbers.");
        return false;
    }
        
    /**
     * Helper to clamp an angle given by the camp to +/- PI
     */
    private float ClampPi(float input)
    {
        float angle = input + (float) Math.PI / 2;

        if (angle > Math.PI)
            return (float) (angle - 2 * Math.PI);

        if (angle < -1 * Math.PI)
            return (float) (angle + 2 * Math.PI);

        return angle;
    }
}