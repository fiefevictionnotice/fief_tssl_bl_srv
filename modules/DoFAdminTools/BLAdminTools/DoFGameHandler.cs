using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DoFAdminTools.ChatCommands;
using DoFAdminTools.Helpers;
using DoFAdminTools.Repositories;
using NetworkMessages.FromClient;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using PlayerMessageAll = NetworkMessages.FromClient.PlayerMessageAll;
using PlayerMessageTeam = NetworkMessages.FromClient.PlayerMessageTeam;

namespace DoFAdminTools;

public class DoFGameHandler : GameHandler
{
    private DoFConfigOptions _configOptions = DoFConfigOptions.Instance;

    public override void OnBeforeSave()
    {
    }

    public override void OnAfterSave()
    {
    }

    protected override void OnPlayerConnect(VirtualPlayer peer)
    {
        string peerId = peer.Id.ToString();

        if (AdminRepository.Instance.IsAdmin(peerId) && peer.Communicator is NetworkCommunicator networkPeer)
        {
            networkPeer.UpdateForJoiningCustomGame(true); // set as admin
            SyncAdminOptionsToPeer(networkPeer);

            Helper.Print($"{peer.UserName} joined as admin (ID = {peerId})");
            if (_configOptions.ShowJoinLeaveMessages)
                Helper.SendMessageToAllPeers($"{peer.UserName} joined as admin.");
        }
        else if (_configOptions.ShowJoinLeaveMessages)
        {
            Helper.SendMessageToAllPeers($"{peer.UserName} joined the server.");
        }
    }

    protected override void OnPlayerDisconnect(VirtualPlayer peer)
    {
        base.OnPlayerDisconnect(peer);
        if (_configOptions.ShowJoinLeaveMessages)
        {
            Helper.SendMessageToAllPeers($"{peer.UserName} left the server.");
        }
    }

    protected override void OnGameNetworkBegin()
    {
        base.OnGameNetworkBegin();
        Helper.Print("Registering Chat handlers...");
        AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Add);
    }

    protected override void OnGameNetworkEnd()
    {
        base.OnGameNetworkEnd();
        Helper.Print("Unregistering Chat handlers...");
        AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Remove);
    }

    private void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode mode)
    {
        if (!GameNetwork.IsServer)
            return;

        GameNetwork.NetworkMessageHandlerRegisterer handlerRegisterer =
            new GameNetwork.NetworkMessageHandlerRegisterer(mode);
        handlerRegisterer.Register<PlayerMessageAll>(HandleClientEventPlayerMessageAll);
        handlerRegisterer.Register<PlayerMessageTeam>(HandleClientEventPlayerMessageTeam);
        handlerRegisterer.Register<KickPlayer>(HandleClientEventKickPlayer);
        
        // admin panel options, for logging
        if (_configOptions.ShowAdminPanelUsageMessages ||
            mode == GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Remove) // in case the option is updated mid-mission, we still want to remove the handlers after
        {
            handlerRegisterer.Register<AdminMuteUnmutePlayer>(HandleAdminMuteUnmutePlayer);
            handlerRegisterer.Register<AdminRequestClassRestrictionChange>(HandleAdminRequestClassRestrictionChange);
            handlerRegisterer.Register<AdminRequestEndMission>(HandleAdminRequestEndMission);
            handlerRegisterer.Register<AdminRequestEndWarmup>(HandleAdminRequestEndWarmup);
            handlerRegisterer.Register<AdminUpdateMultiplayerOptions>(HandleAdminUpdateMultiplayerOptions);
        }
    }

    private bool HandleClientEventPlayerMessageAll(NetworkCommunicator peer, PlayerMessageAll message)
    {
        if (!HandleChatMessage(peer, message.Message))
        {
            message.GetPropertyInfo("ReceiverList")?
                .SetMethod?
                .Invoke(message, new object[] { new List<VirtualPlayer>() });
        }
        return true;
    }

    private bool HandleClientEventPlayerMessageTeam(NetworkCommunicator peer, PlayerMessageTeam message)
    {
        if (!HandleChatMessage(peer, message.Message))
        {
            message.GetPropertyInfo("ReceiverList")?
                .SetMethod?
                .Invoke(message, new object[] { new List<VirtualPlayer>() });
        }
        return true;
    }
        
    private bool HandleClientEventKickPlayer(NetworkCommunicator peer, KickPlayer message)
    {
        if (!peer.IsAdmin)
            return false;

        if (message.BanPlayer)
        {
            string banListPath = Path.Combine(DoFSubModule.NativeModulePath, _configOptions.BanListFileName);
            string playerId = message.PlayerPeer.VirtualPlayer.Id.ToString();
                
            File.AppendAllText(banListPath, $"{Environment.NewLine}{playerId} # Player {message.PlayerPeer.UserName}, banned by {peer.UserName} at {DateTime.Now}");
        }
        
        string returnMessage = $"{peer.UserName} {(message.BanPlayer ? "banned" : "kicked")} {message.PlayerPeer.UserName}!";
        Helper.Print(returnMessage);
        Helper.SendMessageToAllPeers(returnMessage);
            
        return false; // let TWs code handle the actual request
    }

    private bool HandleChatMessage(NetworkCommunicator sender, string message)
    {
        if (!message.StartsWith(_configOptions.CommandPrefix))
            return true; // don't hide, show in chat

        ChatCommandHandler.Instance.ExecuteCommand(sender, message);

        return false; // "hide" message from other MessageHandlers, making it not show up in chat for players
    }
    
    // TODO more sanity checks for all of these, i.e. dont show ended warmup if no warmup is running
    private bool HandleAdminUpdateMultiplayerOptions(NetworkCommunicator peer, AdminUpdateMultiplayerOptions message)
    {
        if (!peer.IsAdmin || message.Options == null)
            return false;

        var mapOption = message.Options.FirstOrDefault(x =>
            x.OptionType == MultiplayerOptions.OptionType.Map &&
            x.StringValue.StartsWith(MapAliasRepository.AliasPrefix));
        
        if (mapOption != null)
        {
            MultiplayerOptions.OptionType.Map.SetValue(
                MapAliasRepository.Instance.GetMapForAlias(mapOption.StringValue),
                MultiplayerOptions.MultiplayerOptionsAccessMode.NextMapOptions);
        }

        Helper.SendMessageToAllPeers($"{peer.UserName} has updated the server settings.");
        
        return true;
    }

    private bool HandleAdminRequestEndWarmup(NetworkCommunicator peer, AdminRequestEndWarmup message)
    {
        if (!peer.IsAdmin)
            return false;
        
        Helper.SendMessageToAllPeers($"{peer.UserName} has ended the warmup.");
        return true;
    }

    private bool HandleAdminRequestEndMission(NetworkCommunicator peer, AdminRequestEndMission message)
    {
        if (!peer.IsAdmin)
            return false;
        
        Helper.SendMessageToAllPeers($"{peer.UserName} has ended the mission.");
        return true;
    }

    private bool HandleAdminRequestClassRestrictionChange(NetworkCommunicator peer, AdminRequestClassRestrictionChange message)
    {
        if (!peer.IsAdmin)
            return false;
        
        Helper.SendMessageToAllPeers($"{peer.UserName} has {(message.NewValue ? "disabled" : "enabled")} {message.ClassToChangeRestriction.ToString()} classes.");
        return true;
    }

    private bool HandleAdminMuteUnmutePlayer(NetworkCommunicator peer, AdminMuteUnmutePlayer message)
    {
        if (!peer.IsAdmin)
            return false;
        
        Helper.SendMessageToAllPeers($"{peer.UserName} has {(message.Unmute ? "unmuted" : "muted")} {message.PlayerPeer.UserName}.");
        return true;
    }

    private void SyncAdminOptionsToPeer(NetworkCommunicator networkPeer)
    {
        GameNetwork.BeginModuleEventAsServer(networkPeer);
        GameNetwork.WriteMessage(new MultiplayerOptionsDefault());
        GameNetwork.EndModuleEventAsServer();

        var maps = MapAliasRepository.Instance.ApplyAliases(MultiplayerIntermissionVotingManager.Instance.UsableMaps);
        foreach (CustomGameUsableMap usableMap in maps)
        {
            GameNetwork.BeginModuleEventAsServer(networkPeer);
            GameNetwork.WriteMessage(new MultiplayerIntermissionUsableMapAdded(usableMap.map,
                usableMap.isCompatibleWithAllGameTypes,
                usableMap.isCompatibleWithAllGameTypes ? 0 : usableMap.compatibleGameTypes.Count,
                usableMap.compatibleGameTypes));
            GameNetwork.EndModuleEventAsServer();
        }

        GameNetwork.BeginModuleEventAsServer(networkPeer);
        GameNetwork.WriteMessage(new UpdateIntermissionVotingManagerValues());
        GameNetwork.EndModuleEventAsServer();
    }
}