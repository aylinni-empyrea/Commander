using System;
using System.Globalization;
using System.Text;
using TShockAPI;

namespace Commander
{
  public class CommandDefinition
  {
    public CommandDefinition(string commandTemplate, bool runAsSuperadmin, bool stopOnError, bool stopOnInfo)
    {
      CommandTemplate = commandTemplate;
      RunAsSuperadmin = runAsSuperadmin;
      StopOnError = stopOnError;
      StopOnInfo = stopOnInfo;
    }

    public string CommandTemplate { get; }
    public bool RunAsSuperadmin { get; }
    public bool StopOnError { get; }
    public bool StopOnInfo { get; }

    public bool TryExecute(CommandExecutor executor)
    {
      if (CommandTemplate == null)
        throw new InvalidOperationException(nameof(CommandTemplate));

      var fullcmd = new StringBuilder(CommandTemplate)
        .Replace("${player}", executor.Name)
        .Replace("${user}", executor.User?.Name ?? "")
        .Replace("${group}", executor.User?.Group ?? "Unregistered")
        .Replace("${x}", executor.TileX.ToString())
        .Replace("${y}", executor.TileY.ToString())
        .Replace("${wx}", executor.TPlayer.position.X.ToString(CultureInfo.InvariantCulture))
        .Replace("${wy}", executor.TPlayer.position.Y.ToString(CultureInfo.InvariantCulture))
        .Replace("${life}", executor.TPlayer.statLife.ToString())
        .Replace("${mana}", executor.TPlayer.statMana.ToString())
        .Replace("${lifeMax}", executor.TPlayer.statLife.ToString())
        .Replace("${manaMax}", executor.TPlayer.statMana.ToString())
        .ToString();

      if (RunAsSuperadmin)
        executor.Group = new SuperAdminGroup();

      return Commands.HandleCommand(executor, fullcmd);
    }
  }
}