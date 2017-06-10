using System.Collections.Generic;
using System.Linq;
using ConfigStore;
using TShockAPI;

namespace Commander
{
  public class Config : JsonConfig
  {
    public Dictionary<string, CommandCollection> Definitions { get; set; } = new Dictionary<string, CommandCollection>
    {
      {
        "superheal",
        new CommandCollection
        {
          Aliases = new[] {"sheal"},
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
      }
    };

    public IEnumerable<Command> GetCommands() => Definitions.Select(collection => collection.Value.ToCommand(collection.Key));
  }
}