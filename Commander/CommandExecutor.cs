using TShockAPI;

namespace Commander
{
  public class CommandExecutor : TSPlayer
  {
    public string LastInfoMessage { get; private set; }
    public string LastErrorMessage { get; private set; }

    public bool SuppressOutput { get; set; }

    public CommandExecutor(int index) : base(index)
    {
    }

    public CommandExecutor(string playerName) : base(playerName)
    {
    }

    public override void SendInfoMessage(string msg)
    {
      LastInfoMessage = msg;

      if (!SuppressOutput)
        base.SendInfoMessage(msg);
    }

    public override void SendErrorMessage(string msg)
    {
      LastErrorMessage = msg;

      if (!SuppressOutput)
        base.SendErrorMessage(msg);
    }
  }
}