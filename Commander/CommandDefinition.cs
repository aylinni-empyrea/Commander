using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using TShockAPI;

namespace Commander
{
  [JsonConverter(typeof(CommandDefinitionConverter))]
  public class CommandDefinition
  {
    public CommandDefinition(string commandTemplate, bool sudo, bool stopOnError, bool stopOnInfo, bool silent)
    {
      CommandTemplate = commandTemplate;
      Sudo = sudo;
      StopOnError = stopOnError;
      StopOnInfo = stopOnInfo;
      Silent = silent;
    }

    public string CommandTemplate { get; }

    public bool Sudo { get; }
    public bool StopOnError { get; }
    public bool StopOnInfo { get; }
    public bool Silent { get; }

    private static readonly Regex _argRegex = new Regex(@"\$\{(\w+|@)\}", RegexOptions.Compiled);

    private static readonly Dictionary<string, Func<CommandExecutor, string>> _argFuncs =
      new Dictionary<string, Func<CommandExecutor, string>>
      {
        {"player", ex => "\"" + (ex.RealPlayer ? ex.Name : "Server") + "\""},
        {"user", ex => ex.User?.Name ?? ""},
        {"group", ex => ex.User?.Group ?? "Unregistered"},
        {"x", ex => ex.X.ToString(CultureInfo.InvariantCulture)},
        {"y", ex => ex.Y.ToString(CultureInfo.InvariantCulture)},
        {"wx", ex => ex.TileX.ToString()},
        {"wy", ex => ex.TileY.ToString()},
        {"life", ex => ex.RealPlayer ? ex.TPlayer.statLife.ToString() : "500"},
        {"mana", ex => ex.RealPlayer ? ex.TPlayer.statMana.ToString() : "200"},
        {"maxLife", ex => ex.RealPlayer ? ex.TPlayer.statLifeMax.ToString() : "500"},
        {"maxMana", ex => ex.RealPlayer ? ex.TPlayer.statManaMax.ToString() : "200"}
      };

    private static string InsertArgs(string template, CommandExecutor executor, params string[] args)
    {
      return _argRegex.Replace(template, match =>
      {
        var key = match.Groups[1].Captures[0].Value;

        if (key[0] == '@')
          return string.Join(" ", args);

        if (_argFuncs.TryGetValue(key, out Func<CommandExecutor, string> func))
          return func(executor);

        var index = int.Parse(key);

        if (index >= args.Length)
          return "";

        return args[index] ?? "";
      });
    }

    public bool TryExecute(CommandExecutor executor, params string[] args)
    {
      executor.SuppressOutput = Silent;

      if (CommandTemplate == null)
        throw new InvalidOperationException(nameof(CommandTemplate));

      var fullcmd = InsertArgs(CommandTemplate, executor, args)
        .Insert(0, Silent ? Commands.SilentSpecifier : Commands.Specifier);

      if (Sudo)
        executor.Group = new SuperAdminGroup();

      return Commands.HandleCommand(executor, fullcmd);
    }

    public static CommandDefinition FromString(string composite)
    {
      string[] data = composite.Split('/');
      string[] args = data[0].Split(' ');

      var cmd = string.Join("", data.Skip(1));

      if (string.IsNullOrEmpty(cmd))
        throw new ArgumentException("Command cannot be empty.", nameof(composite));

      return new CommandDefinition(cmd,
        args.Contains("sudo"), args.Contains("stoponerror"), args.Contains("stoponinfo"), args.Contains("silent"));
    }

    public override string ToString()
    {
      var sb = new StringBuilder();

      if (Sudo)
        sb.Append("sudo ");

      if (Silent)
        sb.Append("silent ");

      if (StopOnError)
        sb.Append("stoponerror ");

      if (StopOnInfo)
        sb.Append("stoponinfo ");

      sb.Append("/" + CommandTemplate);
      return sb.ToString();
    }

    private class CommandDefinitionConverter : JsonConverter
    {
      public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        => writer.WriteValue(((CommandDefinition) value).ToString());

      public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
        JsonSerializer serializer)
        => FromString(reader.Value.ToString());

      public override bool CanConvert(Type objectType) => typeof(CommandDefinition) == objectType;
    }
  }
}