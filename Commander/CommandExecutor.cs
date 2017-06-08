using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace Commander
{
  public class CommandExecutor : TSPlayer
  {
    public string LastInfoMessage { get; private set; }
    public string LastErrorMessage { get; private set; }

    public CommandExecutor(int index) : base(index)
    {
    }

    public CommandExecutor(string playerName) : base(playerName)
    {
    }

    public override void SendInfoMessage(string msg)
    {
      LastInfoMessage = msg;
      base.SendInfoMessage(msg);
    }

    public override void SendErrorMessage(string msg)
    {
      LastErrorMessage = msg;
      base.SendErrorMessage(msg);
    }
  }
}