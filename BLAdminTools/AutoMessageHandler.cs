
using System.Timers;
using DoFAdminTools.Helpers;
using TaleWorlds.MountAndBlade;

namespace DoFAdminTools;

public class AutoMessageHandler
{
    private DoFConfigOptions _configOptions = DoFConfigOptions.Instance;
    
    private int _nextMessageIndex;

    private Timer _timer;
    
    public bool Enabled { get; set; }

    public AutoMessageHandler()
    {
        if (_configOptions.AutoMessageInterval <= 0)
            return;
        
        _timer = new Timer();
        _timer.AutoReset = true;
        _timer.Interval = _configOptions.AutoMessageInterval * 1000;
        _timer.Elapsed += TimerElapsed;
        
        _timer.Start();
        Helper.Print("AutoMessages: Starting.");

        Enabled = true;
    }

    private void TimerElapsed(object sender, ElapsedEventArgs e)
    {
        if (Mission.Current == null)
        {
            Helper.PrintWarning("AutoMessages: Tried sending message, but no mission is active. Skipping.");
            return;
        }
        
        // update interval in case it was changed in the meantime
        if (_configOptions.AutoMessageInterval <= 0)
        {
            _timer.Stop();
            Enabled = false;
            Helper.PrintWarning($"AutoMessages: Stopped, as Interval is set to {_configOptions.AutoMessageInterval}");
            return;
        }
        _timer.Interval = _configOptions.AutoMessageInterval * 1000;

        if (_configOptions.AutoMessages.Count == 0)
        {
            _timer.Stop();
            Enabled = false;
            Helper.PrintError("AutoMessages: Tried sending a message but none were configured, stopping AutoMessages.");
            return;
        }

        SendMessage();

        _nextMessageIndex = (_nextMessageIndex + 1) % _configOptions.AutoMessages.Count;
    }

    private void SendMessage()
    {
        string message = _configOptions.AutoMessages[_nextMessageIndex];
        
        if (_configOptions.AutoMessageType == MessageType.CHAT)
            Helper.SendMessageToAllPeers(message);
        else Helper.SendAdminMessageToAllPeers(message, isBroadcast: _configOptions.AutoMessageType == MessageType.BROADCAST);
        
        Helper.Print($"AutoMessages: Sent message #{_nextMessageIndex + 1} (out of {_configOptions.AutoMessages.Count}) ('{message}').");
    }

    public enum MessageType
    {
        CHAT,
        ADMINCHAT,
        BROADCAST
    }
}