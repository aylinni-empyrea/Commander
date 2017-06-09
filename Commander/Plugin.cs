using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ConfigStore;
using Newtonsoft.Json;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

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
      const string path = "Commander.json";
      try
      {
        Config = JsonConfig.Read<Config>(Path.Combine(TShock.SavePath, path));
      }
      catch (FileNotFoundException)
      {
        TShock.Log.ConsoleInfo(path + " not found, generating new one.");
        Config = new Config();

        Config.Write(path);
      }
      catch (JsonException ex)
      {
        TShock.Log.ConsoleError("Exception occurred while reading " + path + ", check logs for more details.");
        TShock.Log.ConsoleError(ex.Message);

        var oldPath = Path.Combine(TShock.SavePath, path);
        var newPath = $"{oldPath}.{DateTime.Now:s}.bak";

        File.Move(oldPath, newPath);

        Config = new Config();
        Config.Write(path);

        TShock.Log.ConsoleInfo(path + " has been backed up and a fresh one is generated.");
        TShock.Log.Error(ex.ToString());
      }
    }

    private static Command[] _commands;

    public override void Initialize()
    {
      _commands = Config.GetCommands().ToArray();

      Commands.ChatCommands.AddRange(_commands);
    }

    protected override void Dispose(bool disposing)
    {
      Commands.ChatCommands.RemoveAll(_commands.Contains);

      _commands = null;
      Config = null;

      base.Dispose(disposing);
    }
  }
}