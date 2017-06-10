using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ConfigStore;
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

    private Config Config;

    public Plugin(Main game) : base(game)
    {
    }

    private void LoadConfig()
    {
      const string fileName = "Commander.json";
      var path = Path.Combine(TShock.SavePath, fileName);

      try
      {
        Config = JsonConfig.Read<Config>(path);
      }
      catch (FileNotFoundException)
      {
        TShock.Log.ConsoleInfo(fileName + " not found, generating new one.");
        Config = new Config();

        Config.Write(path);
      }
      catch (JsonException ex)
      {
        TShock.Log.ConsoleError("Exception occurred while reading " + fileName + ", check logs for more details.");
        TShock.Log.ConsoleError(ex.Message);

        var oldPath = path;
        var newPath = $"{oldPath}.{DateTime.Now:s}.bak";

        File.Move(oldPath, newPath);

        Config = new Config();
        Config.Write(path);

        TShock.Log.ConsoleInfo(fileName + " has been backed up and a fresh one is generated.");
        TShock.Log.Error(ex.ToString());
      }

      _commands = Config.Commands.ToDictionary(k => k.Aliases[0], v => v.ToCommand());

      Commands.ChatCommands.RemoveAll(cmd => _commands.ContainsKey(cmd.Name));
      Commands.ChatCommands.AddRange(_commands.Values);
    }

    private static Dictionary<string, Command> _commands;

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
      Config = null;

      base.Dispose(disposing);
    }
  }
}