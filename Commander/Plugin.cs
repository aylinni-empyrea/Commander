using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace Commander
{
  [ApiVersion(2, 1)]
  public class Plugin : TerrariaPlugin
  {
    public override string Name => "Commander";
    public override string Author => "Newy";

    public override string Description =>
      "Meta-command system to create new commands by adding existing commands to each other.";

    public override Version Version => typeof(Plugin).Assembly.GetName().Version;

    private Dictionary<string, CommandCollection> _commands;

    public Plugin(Main game) : base(game)
    {
      JsonConvert.DefaultSettings = () => new JsonSerializerSettings
      {
        NullValueHandling = NullValueHandling.Ignore,
        DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate
      };
    }


    private void LoadConfig()
    {
      const string fileName = "Commander.json";
      const string filePath = "tshock/" + fileName;

      List<CommandCollection> data;

      if (File.Exists(filePath))
      {
        data = JsonConvert.DeserializeObject<List<CommandCollection>>(File.ReadAllText(filePath));
      }
      else
      {
        TShock.Log.ConsoleInfo(fileName + " not found, generating new one.");
        data = new List<CommandCollection>
        {
          new CommandCollection
          {
            Aliases = new[] {"sheal"},
            HelpSummary = "Heals a bit too good.",
            HelpText = new[] {"Heals a bit too good.", "Maybe too much?"},
            UsagePermission = "superheal",
            AllowServer = false,
            Commands = new[]
            {
              new CommandDefinition("heal ${player}", true, false, false, true, true),
              new CommandDefinition("bc ${player} got healed by some holy spirit!", true, false, false, true, true)
            }
          }
        };

        File.WriteAllText(filePath, JsonConvert.SerializeObject(data));
      }

      _commands = data.ToDictionary(k => k.Aliases[0]);

      Commands.ChatCommands.RemoveAll(cmd => _commands.ContainsKey(cmd.Name));
      Commands.ChatCommands.AddRange(_commands.Values.Select(c => c.ToCommand()));
    }

    public override void Initialize()
    {
      LoadConfig();
      GeneralHooks.ReloadEvent += OnReload;
    }

    private void OnReload(ReloadEventArgs e) => LoadConfig();

    protected override void Dispose(bool disposing)
    {
      GeneralHooks.ReloadEvent -= OnReload;

      Commands.ChatCommands.RemoveAll(cmd => _commands.ContainsKey(cmd.Name));

      _commands = null;

      base.Dispose(disposing);
    }
  }
}