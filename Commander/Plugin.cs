using System;
using System.IO;
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
    private Config Config;

    public Plugin(Main game) : base(game)
    {
      try
      {
        Config = JsonConfig.Read<Config>(Path.Combine(TShock.SavePath, "Commander.json"));
      }
      catch (FileNotFoundException)
      {
        TShock.Log.ConsoleInfo("Commander.json not found, generating new one.");
        Config = new Config();

        Config.Write("Commander.json");
      }
      catch (JsonException ex)
      {
        TShock.Log.ConsoleError("Exception occurred while reading Commander.json, check logs for more details.");
        TShock.Log.ConsoleError(ex.Message);

        var oldPath = Path.Combine(TShock.SavePath, "Commander.json");
        var newPath = $"{oldPath}.{DateTime.Now:s}.bak";

        File.Move(oldPath, newPath);

        Config = new Config();
        Config.Write("Commander.json");

        TShock.Log.ConsoleInfo("Commander.json has been backed up and a fresh one is generated.");
        TShock.Log.Error(ex.ToString());
      }
    }

    public override void Initialize()
    {
      Commands.ChatCommands.AddRange(Config.GetCommands());
    }

    protected override void Dispose(bool disposing)
    {
      Config = null;
      base.Dispose(disposing);
    }
  }
}