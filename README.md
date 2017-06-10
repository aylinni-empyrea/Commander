# Commander [![Github Releases](https://img.shields.io/github/downloads/deadsurgeon42/Commander/latest/total.svg)](https://github.com/deadsurgeon42/Commander)
🔗 *Meta-command system to create new commands by adding existing commands to each other + a few useful tweaks*

Designed as a standalone alternative to [CmdAlias](https://github.com/tylerjwatson/SEconomy/tree/master/CmdAliasPlugin).
## Installation
Drop [the plugin](https://github.com/deadsurgeon42/Commander/releases) to your `ServerPlugins/` folder.
After first run with the plugin installed, the configuration file
`Commander.json` will be created.

## Configuration

ℹ️ **Reloading configuration file:** `/reload`
```js
{
    "Definitions": {                                     // holds commands
        "superheal": {                                   // command's primary name
            "Aliases": [ 
                "sheal"                                  // won't show up on /help
            ],
            "AllowServer": false,                        // allow command to run from console?
            "HelpSummary": "Heals a bit too good.",      // help summary
            "HelpText": [ 
                "Heals a bit too good.",                 // detailed help for
                "Maybe too much?"                        // /help <command>
            ],
            "UsagePermission": "superheal",              // permission to use this command
            "ExpectedParameterCount": 0,                 // how many parameters should be passed?
            "Cooldown": 5,                               // how often can be the command used? (in seconds)
            "Commands": [                                // array of commands to run in order
                {
                    "CommandTemplate": "heal ${player}", // template of command (see below)
                    "RunAsSuperadmin": true,             // ignore tshock permission checks for above command
                    "StopOnError": false,                // stop command chain when an error occurs
                    "StopOnInfo": false,                 // stop command chain when an info message is sent
                    "Silent": true                       // completely silence command output (to the executing player)
                },
                {
                    "CommandTemplate": "bc ${player} got healed by some holy spirit!",
                    "RunAsSuperadmin": true,
                    "StopOnError": false,
                    "StopOnInfo": false,
                    "Silent": true
                }
            ]
        }
    }
}
```

## Variables

✏️ *Command templates may include special tokens in the form* `${token}`.

+ `${n}` = `n`th argument (`${@}` for all arguments)
+ `${player}` = Executor's name 
+ `${user}` = Executor's user account name
+ `${group}` = Executor's user group name
+ `${x}, ${y}` = Coordinates of executor
+ `${wx}, ${wy}` = World coordinates of executor (coordinates × 16)
+ `${life}, ${lifeMax}, ${mana}, ${manaMax}` = Stats of executor

**Need more variables?** [Make an issue!](issues/)
