using Microsoft.Xna.Framework;
using TShockAPI;
using TShockAPI.DB;

namespace Commander
{
  public class CommandExecutorServer : CommandExecutor
  {
    public CommandExecutorServer() : base("Server")
    {
      Group = new SuperAdminGroup();
      User = Server.User;
    }

    public override void SendMessage(string msg, Color color)
      => Server.SendMessage(msg, color);

    public override void SendMessage(string msg, byte red, byte green, byte blue)
      => Server.SendMessage(msg, red, green, blue);

    public override void SendSuccessMessage(string msg) => Server.SendSuccessMessage(msg);

    public override void SendInfoMessage(string msg) => Server.SendInfoMessage(msg);

    public override void SendWarningMessage(string msg) => Server.SendWarningMessage(msg);

    public override void SendErrorMessage(string msg) => Server.SendErrorMessage(msg);
  }

  public class CommandExecutor : TSPlayer
  {
    public string LastInfoMessage { get; private set; }
    public string LastErrorMessage { get; private set; }

    public bool SuppressOutput { get; set; }

    public CommandExecutor(int index) : base(index)
    {
      User = TShock.Players[index]?.User;
    }

    protected CommandExecutor(string playerName) : base(playerName)
    {
    }

    public override void SendMessage(string msg, Color color)
    {
      if (!SuppressOutput)
        base.SendMessage(msg, color);
    }

    public override void SendMessage(string msg, byte red, byte green, byte blue)
    {
      if (!SuppressOutput)
        base.SendMessage(msg, red, green, blue);
    }

    public override void SendSuccessMessage(string msg)
    {
      if (!SuppressOutput)
        base.SendSuccessMessage(msg);
    }

    public override void SendInfoMessage(string msg)
    {
      LastInfoMessage = msg;

      if (!SuppressOutput)
        base.SendInfoMessage(msg);
    }

    public override void SendWarningMessage(string msg)
    {
      LastInfoMessage = msg;

      if (!SuppressOutput)
        base.SendWarningMessage(msg);
    }

    public override void SendErrorMessage(string msg)
    {
      LastErrorMessage = msg;

      if (!SuppressOutput)
        base.SendErrorMessage(msg);
    }
  }
}