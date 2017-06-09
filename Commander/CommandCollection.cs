using System;
using System.Collections.Generic;
using TShockAPI;
using TShockAPI.DB;

namespace Commander
{
  public class CommandCollection
  {
    private readonly Dictionary<User, DateTime> _lastUsed = new Dictionary<User, DateTime>();

    public string[] Aliases { get; set; }
    public bool Silent { get; set; }

    public bool AllowServer { get; set; }
    public string HelpSummary { get; set; }
    public string[] HelpText { get; set; }

    public string UsagePermission { get; set; }

    public uint Cooldown { get; set; }

    public CommandDefinition[] Commands { get; set; }

    public Command ToCommand()
    {
      var dlg = (CommandDelegate) Delegate.CreateDelegate(typeof(CommandDelegate), this, "Execute");
      var cmd = new Command(dlg, Aliases) {AllowServer = AllowServer};

      if (!string.IsNullOrEmpty(HelpSummary))
        cmd.HelpText = HelpSummary;

      if (HelpText.Length != 0)
        cmd.HelpDesc = HelpText;

      return cmd;
    }

    public void Execute(CommandArgs args)
    {
      try
      {
        Execute(args.Player);
      }
      catch (CommandException ex)
      {
        args.Player.SendErrorMessage(ex.Message);
      }
    }

    public void Execute(TSPlayer executor)
    {
      if (Cooldown != 0 && _lastUsed.TryGetValue(executor.User, out DateTime lastUsed) &&
          (DateTime.Now - lastUsed).TotalSeconds < Cooldown)
        throw new CommandException(CommandError.Cooldown, Cooldown - (DateTime.Now - lastUsed).TotalSeconds);

      if (Commands.Length == 0)
        throw new InvalidOperationException("There are no commands defined in this collection.");

      if (!executor.ConnectionAlive) return;

      if (executor.User == null)
        throw new CommandException(CommandError.NoPermission);

      if (!executor.HasPermission(UsagePermission))
        throw new CommandException(CommandError.NoPermission);

      if (Cooldown != 0)
        _lastUsed[executor.User] = DateTime.Now;

      var target = new CommandExecutor(executor.Index);

      if (Silent)
        target.SuppressOutput = true;

      foreach (var cmd in Commands)
      {
        if (!cmd.TryExecute(target))
          throw new CommandException(CommandError.Unspecified);

        if (cmd.StopOnError && target.LastErrorMessage != null)
          throw new CommandException(target.LastErrorMessage);

        if (cmd.StopOnInfo && target.LastInfoMessage != null)
          throw new CommandException(target.LastInfoMessage);
      }
    }
  }
}