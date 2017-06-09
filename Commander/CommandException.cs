using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commander
{
  internal class CommandException : Exception
  {
    private const string UnspecifiedError = "There has been an error during execution of your command.";
    private const string NoPermissionError = "You don't have permission to use this command!";
    private const string AbortedError = "Command execution was stopped midway due to an error.";
    private const string CooldownError = "You must wait {0} seconds before you can use this command again.";

    private static string GetErrorMessage(CommandError error)
    {
      switch (error)
      {
        case CommandError.NoPermission:
          return NoPermissionError;
        case CommandError.Aborted:
          return AbortedError;
        case CommandError.Cooldown:
          return CooldownError;

        case CommandError.Unspecified:
        default:
          return UnspecifiedError;
      }
    }

    internal CommandException(string message) : base(message)
    {
    }

    internal CommandException(CommandError error, params object[] args)
      : this(string.Format(GetErrorMessage(error), args))
    {
    }
  }

  internal enum CommandError
  {
    Unspecified,
    NoPermission,
    Aborted,
    Cooldown,
    ServerNotAllowed
  }
}