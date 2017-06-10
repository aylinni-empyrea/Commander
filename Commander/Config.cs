using System.Collections.Generic;
using System.Linq;
using ConfigStore;
using TShockAPI;

namespace Commander
{
  public class Config : JsonConfig
  {
    public CommandCollection[] Commands { get; set; } =
    {
      new CommandCollection
      {
        Aliases = new[] {"sheal", "superheal"},
        HelpSummary = "Heals a bit too good.",
        HelpText = new[] {"Heals a bit too good.", "Maybe too much?"},
        UsagePermission = "superheal",
        AllowServer = false,
        Cooldown = 5,
        Commands = new[]
        {
          new CommandDefinition("heal ${player}", true, false, false, true),
          new CommandDefinition("bc ${player} got healed by some holy spirit!", true, false, false, true)
        }
      }
    };

    public IEnumerable<Command> GetCommands() => Commands.Select(collection => collection.ToCommand());
  }
}