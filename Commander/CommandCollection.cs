using System;
using System.Collections.Generic;
using System.Linq;
using TShockAPI;
using TShockAPI.DB;

namespace Commander
{
  public class CommandCollection
  {
    private Dictionary<User, DateTime> _lastUsed;

    public string[] Aliases { get; set; }

    public bool AllowServer { get; set; }
    public string HelpSummary { get; set; }
    public string[] HelpText { get; set; }

    public string UsagePermission { get; set; }
    public int ExpectedParameterCount { get; set; }

    public uint Cooldown { get; set; }

    public CommandDefinition[] Commands { get; set; }

    public Command ToCommand() => ToCommand(Aliases[0]);

    public Command ToCommand(string name)
    {
      var aliases = new[] {name}.Concat(Aliases).ToArray();
      var dlg = (CommandDelegate) Delegate.CreateDelegate(typeof(CommandDelegate), this, "Execute");

      var cmd = string.IsNullOrEmpty(UsagePermission)
        ? new Command(dlg, aliases)
        : new Command(UsagePermission, dlg, aliases);

      if (!string.IsNullOrEmpty(HelpSummary))
        cmd.HelpText = HelpSummary;

      if (HelpText != null && HelpText.Length > 0)
        cmd.HelpDesc = HelpText;

      cmd.AllowServer = AllowServer;

      return cmd;
    }

    public void Execute(CommandArgs args)
    {
      try
      {
        Execute(args.Player, args.Parameters.ToArray());
      }
      catch (CommandException ex)
      {
        args.Player.SendErrorMessage(ex.Message);
      }
    }

    public void Execute(TSPlayer executor, params string[] args)
    {
      if (!executor.HasPermission(UsagePermission))
        throw new CommandException(CommandError.NoPermission);

      if (ExpectedParameterCount > args.Length)
        throw new CommandException(CommandError.NotEnoughParameters, ExpectedParameterCount);

      if (Cooldown != 0 && _lastUsed == null)
        _lastUsed = new Dictionary<User, DateTime>();

      if (executor.User != null && Cooldown != 0 &&
          _lastUsed.TryGetValue(executor.User, out DateTime lastUsed) &&
          (DateTime.Now - lastUsed).TotalSeconds < Cooldown)
        throw new CommandException(CommandError.Cooldown,
          Math.Round(Cooldown - (DateTime.Now - lastUsed).TotalSeconds));

      if (Commands.Length == 0)
        throw new InvalidOperationException("There are no commands defined in this collection.");

      if (!AllowServer && !executor.ConnectionAlive) return;

      if (executor.User != null && Cooldown != 0)
        _lastUsed[executor.User] = DateTime.Now;

      var target = executor.RealPlayer ? new CommandExecutor(executor.Index) : new CommandExecutorServer();

      foreach (var cmd in Commands)
      {
        if (!cmd.TryExecute(target, args))
          throw new CommandException(CommandError.Unspecified);

        if (cmd.StopOnError && target.LastErrorMessage != null)
          throw new CommandException(target.LastErrorMessage);

        if (cmd.StopOnInfo && target.LastInfoMessage != null)
          throw new CommandException(target.LastInfoMessage);
      }
    }
  }
}