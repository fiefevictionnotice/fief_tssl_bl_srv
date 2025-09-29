using System.IO;
using DoFAdminTools.ChatCommands;
using DoFAdminTools.ChatCommands.AdminCommands;
using DoFAdminTools.ChatCommands.AdminCommands.Teleport;
using DoFAdminTools.ChatCommands.PublicCommands;
using DoFAdminTools.Helpers;
using DoFAdminTools.MissionBehaviors;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;


namespace DoFAdminTools;

public class DoFSubModule: MBSubModuleBase
{
    public static readonly string NativeModulePath =
        Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "../../Modules/Native/"));

    private AutoMessageHandler _autoMessageHandler;
        
    protected override void OnSubModuleLoad()
    {
        base.OnSubModuleLoad();
            
        Helper.Print("Loading...");
            
        Helper.Print("Adding Console Commands...");
        AddConsoleCommands();
            
        Helper.Print("Registering Chat Commands...");
        RegisterChatCommands();
            
        Helper.Print("Loaded.");
    }

    public override void OnMultiplayerGameStart(Game game, object starterObject)
    {
        base.OnMultiplayerGameStart(game, starterObject);
            
        Helper.Print("Adding GameHandler.");
        game.AddGameHandler<DoFGameHandler>();
    }

    public override void OnMissionBehaviorInitialize(Mission mission)
    {
        base.OnMissionBehaviorInitialize(mission);

        if (_autoMessageHandler == null || !_autoMessageHandler.Enabled)
            _autoMessageHandler = new AutoMessageHandler();

        var options = DoFConfigOptions.Instance;
        
        if (options.CorpseFadeoutTimer >= 0)
            Mission.Current.SetMissionCorpseFadeOutTimeInSeconds(options.CorpseFadeoutTimer);
        
        if (!options.PreventHpSyncToEnemies)
            return;
        
        // Disable HP sync between enemies for all Flag Domination Gamemodes (Skirmish/Battle/Captain).
        MissionMultiplayerGameModeBase gamemode = mission.GetMissionBehavior<MissionMultiplayerGameModeBase>();
        if (gamemode is MissionMultiplayerFlagDomination && mission.GetMissionBehavior<HpSyncAntiCheat>() == null)
        {
            mission.AddMissionBehavior(new HpSyncAntiCheat());
        }
    }

    private void AddConsoleCommands() => DedicatedServerConsoleCommandManager.AddType(typeof(ConsoleCommands));
        
    private void RegisterChatCommands()
    {
        ChatCommandHandler commandHandler = ChatCommandHandler.Instance;

        commandHandler.RegisterCommand(new MeCommand());
        commandHandler.RegisterCommand(new PlayerInfoCommand());
        commandHandler.RegisterCommand(new HealCommand());
        commandHandler.RegisterCommand(new RemoveHorsesCommand());
        commandHandler.RegisterCommand(new SlayCommand());
        commandHandler.RegisterCommand(new ListSpectatorsCommand());

        // Warmup Commands
        commandHandler.RegisterCommand(new ExtendWarmupCommand());
        commandHandler.RegisterCommand(new EndWarmupCommand());

        // Teleport Commands
        commandHandler.RegisterCommand(new MoveCommand());
        commandHandler.RegisterCommand(new TeleportMeToCommand());
        commandHandler.RegisterCommand(new TeleportToMeCommand());
        
        
        // the help command should always be registered last, as it only shows commands registered *before* it.
        commandHandler.RegisterCommand(new HelpCommand());
    }
}