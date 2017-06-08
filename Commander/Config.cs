using System.Collections.Generic;
using System.Linq;
using ConfigStore;
using TShockAPI;

namespace Commander
{
  public class Config : JsonConfig
  {
    public CommandCollection[] Commands { get; set; }

    public IEnumerable<Command> GetCommands() => Commands.Select(collection => collection.ToCommand());
  }
}