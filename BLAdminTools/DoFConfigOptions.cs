using System.Collections.Generic;

namespace DoFAdminTools;

public class DoFConfigOptions
{
    public static DoFConfigOptions Instance { get; } = new();

    public string CommandPrefix { get; set; } = "!";
        
    public string BanListFileName { get; set; } = "banlist.txt";

    public bool ShowJoinLeaveMessages { get; set; } = true;

    public bool ShowAdminPanelUsageMessages { get; set; } = true;

    public bool PreventHpSyncToEnemies { get; set; } = true;

    public List<string> AutoMessages { get; } = new();

    public int AutoMessageInterval { get; set; } = 60;

    public AutoMessageHandler.MessageType AutoMessageType { get; set; } = AutoMessageHandler.MessageType.CHAT;

    public int CorpseFadeoutTimer { get; set; } = -1;
}