using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace ConfigStore
{
  [DebuggerDisplay("")]
  public abstract class JsonConfig
  {
    private readonly object _syncRoot = new object();

    private static readonly JsonSerializer _serializer = JsonSerializer.CreateDefault(
      new JsonSerializerSettings
      {
        NullValueHandling = NullValueHandling.Ignore,
        ObjectCreationHandling = ObjectCreationHandling.Auto,
        Formatting = Formatting.Indented
      });

    public static T Read<T>(string path) where T : class, new()
    {
      T obj;
      var data = File.ReadAllText(path);

      if (string.IsNullOrEmpty(data))
        throw new ArgumentNullException();

      using (var reader = new JsonTextReader(new StringReader(data)))
        obj = _serializer.Deserialize<T>(reader);

      return obj;
    }

    public void Write(string path)
    {
      lock (_syncRoot)
        File.WriteAllText(path, ToString());
    }

    public override string ToString()
    {
      var sb = new StringBuilder();

      using (var writer = new JsonTextWriter(new StringWriter(sb)))
        lock (_syncRoot)
          _serializer.Serialize(writer, this);

      return sb.ToString();
    }
  }
}