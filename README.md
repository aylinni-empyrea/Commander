# Commander [![Github Releases](https://img.shields.io/github/downloads/deadsurgeon42/Commander/total.svg)](https://github.com/deadsurgeon42/Commander)
🔗 *Meta-command system to create new commands by adding existing commands to each other + a few useful tweaks*

Designed as a standalone alternative to [CmdAlias](https://github.com/tylerjwatson/SEconomy/tree/master/CmdAliasPlugin).
## Installation
Drop [the plugin](https://github.com/deadsurgeon42/Commander/releases) to your `ServerPlugins/` folder.
After first run with the plugin installed, the configuration file
`Commander.json` will be created.

## Configuration

ℹ️ **Reloading configuration file:** `/reload`
```js
[
  {
    "Aliases": [
      "sheal" // first alias is the command name
    ],
    "AllowServer": false,
    "HelpSummary": "Heals a bit too good.",
    "HelpText": [
      "Heals a bit too good.",
      "Maybe too much?"
    ],
    "UsagePermission": "superheal",
    "ExpectedParameterCount": 0,
    "Cooldown": 5,
    "Commands": [
      "sudo silent /heal ${player}",
      "sudo silent /bc ${player} got healed by some holy spirit!"
    ]
  }
]
```

## Modifiers

⭐ *Command templates may include modifiers before the slash*

+ `sudo` = Ignore user's permissions when running the command
+ `silent` = Suppress command output *to the user*
+ `stoponerror` = Stop command chain when an __error__ message is received
+ `stoponinfo` = Stop command chain when an __info__ message is received

## Variables

✏️ *Command templates may include special tokens in the form* `${token}`.

+ `${n}` = `n`th argument (`${@}` for all arguments)
+ `${player}` = Executor's name 
+ `${user}` = Executor's user account name
+ `${group}` = Executor's user group name
+ `${x}, ${y}` = Coordinates of executor
+ `${wx}, ${wy}` = World coordinates of executor (coordinates × 16)
+ `${life}, ${lifeMax}, ${mana}, ${manaMax}` = Stats of executor

**Need more variables?** [Make an issue!](https://github.com/deadsurgeon42/Commander/issues)
