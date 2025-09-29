using DoFAdminTools.Helpers;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace DoFAdminTools.ChatCommands.AdminCommands;

public class HealCommand : AdminChatCommand
{
    public override string CommandText => "heal";
    public override string UsageDescription => $"{base.UsageDescription} (PLAYERNAME)";

    public override string Description =>
        "Heals the player(s) (and their mounts, shields & ammo counts) whose name match the given text, or all if no text is given.";

    public override bool Execute(NetworkCommunicator executor, string args)
    {
        bool checkPlayerName = args.Length > 0;

        int count = 0;
        foreach (NetworkCommunicator peer in GameNetwork.NetworkPeers)
        {
            if (peer.ControlledAgent is null)
                continue;

            // if a parameter was provided, it must be part of the players name.
            if (checkPlayerName && !(peer.UserName.Contains(args)))
                continue;

            peer.ControlledAgent.Health = peer.ControlledAgent.HealthLimit;

            if (peer.ControlledAgent.MountAgent != null)
            {
                peer.ControlledAgent.MountAgent.Health = peer.ControlledAgent.MountAgent.HealthLimit;
            }

            for (int i = 0; i < 4; i++)
            {
                MissionWeapon weapon = peer.ControlledAgent.Equipment[i];

                if (weapon.IsAnyAmmo())
                {
                    peer.ControlledAgent.SetWeaponAmountInSlot((EquipmentIndex) i, weapon.ModifiedMaxAmount, true);
                }
                else if (weapon.IsShield())
                {
                    peer.ControlledAgent.ChangeWeaponHitPoints((EquipmentIndex) i, weapon.ModifiedMaxHitPoints);
                }
            }

            count++;
        }

        if (count > 0)
        {
            Helper.SendMessageToAllPeers($"{executor.UserName} healed {count} player(s).");

            return true;
        }

        Helper.SendMessageToPeer(executor, "No player found with matching name.");
        return false;
    }
}