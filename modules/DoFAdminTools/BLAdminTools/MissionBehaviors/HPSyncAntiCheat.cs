using TaleWorlds.MountAndBlade;
using DoFAdminTools.Helpers;
using TaleWorlds.Core;
using NetworkMessages.FromServer;

namespace DoFAdminTools.MissionBehaviors;

public class HpSyncAntiCheat : MissionBehavior
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HpSyncAntiCheat"/> class.
    /// </summary>
    public HpSyncAntiCheat()
    {
        Helper.Print("Initialized HPSyncAntiCheat MissionBehavior");
    }

    public override MissionBehaviorType BehaviorType => MissionBehaviorType.Other;

    public override void AfterStart()
    {
        base.AfterStart();
        if (GameNetwork.IsDedicatedServer)
        {
            MissionPeer.OnTeamChanged += OnMissionPeerTeamChanged;
            NetworkCommunicator.OnPeerSynchronized += OnPeerSynchronized;
        }
    }

    public override void OnAgentBuild(Agent agent, Banner banner)
    {
        base.OnAgentBuild(agent, banner);
        agent.UpdateSyncHealthToAllClients(false);
        agent.OnAgentHealthChanged += OnAgentHealthChangedDelegate;
    }

    public override void OnAgentDeleted(Agent affectedAgent)
    {
        affectedAgent.OnAgentHealthChanged -= OnAgentHealthChangedDelegate;
        base.OnAgentDeleted(affectedAgent);
    }

    // Sync horse HP to new rider
    public override void OnAgentMount(Agent agent)
    {
        base.OnAgentMount(agent);

        if (agent?.MountAgent == null || !GameNetwork.IsServerOrRecorder)
        {
            return;
        }

        SyncHealthToRelevantClients(agent.MountAgent);
    }

    protected override void OnEndMission()
    {
        if (GameNetwork.IsDedicatedServer)
        {
            MissionPeer.OnTeamChanged -= OnMissionPeerTeamChanged;
            NetworkCommunicator.OnPeerSynchronized -= OnPeerSynchronized;
        }

        base.OnEndMission();
    }

    // Synchronize HP of all agents to a player on reconnect (Not initial connect since it's synced when selecting a team)
    private void OnPeerSynchronized(NetworkCommunicator networkPeer)
    {
        if (networkPeer == null || !networkPeer.IsSynchronized)
        {
            return;
        }

        MissionPeer missionPeer = networkPeer.GetComponent<MissionPeer>();
        if (missionPeer?.Team == null)
        {
            return;
        }

        SyncCurrentHealthOfAllRelevantAgentsToClient(networkPeer, missionPeer.Team);
    }

    private void OnAgentHealthChangedDelegate(Agent agent, float oldHealth, float newHealth)
    {
        SyncHealthToRelevantClients(agent);
    }

    // Sync already damaged agents to new teammate / spectator
    private void OnMissionPeerTeamChanged(NetworkCommunicator networkPeer, Team previousTeam, Team newTeam)
    {
        if (networkPeer == null)
        {
            return;
        }

        SyncCurrentHealthOfAllRelevantAgentsToClient(networkPeer, newTeam);
    }

    private void SyncCurrentHealthOfAllRelevantAgentsToClient(NetworkCommunicator networkPeer, Team team)
    {
        foreach (Agent agent in Mission.Agents)
        {
            int healthToSend;
            if (team == Mission.SpectatorTeam || team == agent.Team || (agent.RiderAgent != null && agent.RiderAgent.Team == team))
            {
                // Sync correct HP values for teammates and spectators
                healthToSend = (int) agent.Health;
            }
            else
            {
                // Fake full hp for enemies
                healthToSend = (int) agent.HealthLimit;
            }
            
            GameNetwork.BeginModuleEventAsServer(networkPeer);
            GameNetwork.WriteMessage(new SetAgentHealth(agent.Index, healthToSend));
            GameNetwork.EndModuleEventAsServer();
        }
    }

    private void SyncHealthToRelevantClients(Agent agent)
    {
        if (agent.IsMount && agent.RiderAgent == null)
            return;
        
        Team targetAgentTeam = !agent.IsMount ? agent.Team : agent.RiderAgent.Team;
        foreach (NetworkCommunicator networkPeer in GameNetwork.NetworkPeers)
        {
            if (!networkPeer.IsSynchronized)
                continue;
            
            MissionPeer missionPeer = networkPeer.GetComponent<MissionPeer>();
            
            if (missionPeer.Team != targetAgentTeam && missionPeer.Team != Mission.SpectatorTeam)
                continue;

            GameNetwork.BeginModuleEventAsServer(networkPeer);
            GameNetwork.WriteMessage(new SetAgentHealth(agent.Index, (int)agent.Health));
            GameNetwork.EndModuleEventAsServer();
        }
    }
}
