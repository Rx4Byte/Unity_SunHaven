# Sun Haven Mod Collection
This repository contains mods for the game **Sunhaven**.

## Containing Mods
* **[Command Extension](#command-extension)**: This mod adds a variety of new console commands.
* **[AutoFill Museum](#autofill-museum)**: This mod that adds a Quality of Life feature to the game by automatically filling the museum upon entry.
* **[Controller Input Disabler](#controller-input-disabler)**: This mod disables direct controller inputs to be able to use any remapper for controller.

## Command Extension
This mod is designed to enhance your gaming experience by allowing you to enter commands directly into the game’s chat box.  
It operates alongside the game’s existing command system, but with its own unique command prefix.  

- **How to use Commands**:  
  Commands are entered using the Chat-Box.
  Open the Chat (Enter-key by default) and type e.g. `!help` for a list of Commands.

### Commands
```
ℹ️ Command Parameters with a star (*) are optional, e.g. [amount]*
```
- **Core Commands**
	- `CmdPrefix`: `!`  
	  The command prefix used for all commands.
	
	- `help`: `!help`  
	  Displays a list of all available commands.
	
	- `state`: `!state`  
	  Shows which commands are currently activated.
	
	- `feedback`: `!feedback`  
	  Toggles chat feedback for command execution.
	
	- `name`: `!name [playerName]*`  
	  Sets or resets the player name that commands will target.

- **Player Commands**
	- `jumper`: `!jumper`  
	  Toggles ability to jump through objects.
	
	- `dasher`: `!dasher`  
	  Toggles infinite dash charges.
	
	- `manafill`: `!manafill`  
	  Refills the player’s mana to maximum.
	
	- `manainf`: `!manainf`  
	  Toggles infinite mana.
	
	- `healthfill`: `!healthfill`  
	  Refills the player’s health to maximum.
	
	- `nohit`: `!nohit`  
	  Toggles invincibility (no damage taken).
	
	- `noclip`: `!noclip`  
	  Toggles noclip mode (walk through walls).
	
	- `sleep`: `!sleep`  
	  Sleeps through the night.

- **Time Commands**
	- `pause`: `!pause`  
	  Toggles the game’s time pause on or off.
	
	- `timespeed`: `!timespeed [multiplier|reset]`  
	  Sets or toggles a custom day-length multiplier.
	
	- `time`: `!time [h|d] [value]`  
	  Sets the current hour or day in the day/night cycle.
	
	- `weather`: `!weather [raining|heatwave|clear]`  
	  Changes weather to raining, heatwave, or clear.
	
	- `season`: `!season [Spring|Summer|Fall|Winter]`  
	  Changes the current season.
	
	- `years`: `!years (-)[years]`  
	  Advances or rewinds the in-game year by increments of four months.

- **Currency Commands**
	- `money`: `!money (-)[amount]`  
	  Alias for `!coins`.
	
	- `coins`: `!coins (-)[amount]`  
	  Adds or subtracts Coins from the player.
	
	- `orbs`: `!orbs (-)[amount]`  
	  Adds or subtracts Orbs from the player.
	
	- `tickets`: `!tickets (-)[amount]`  
	  Adds or subtracts Tickets from the player.
	
	- `devkit`: `!devkit`  
	  Grants the developer kit items.

- **Mine Commands**
	- `minereset`: `!minereset`  
	  Reset the current mine.
	
	- `mineclear`: `!mineclear`  
	  Removes all rocks and ores from the mine.
	
	- `mineoverfill`: `!mineoverfill`  
	  Fills the mine completely with rocks and ores.

- **Relationship Commands**
	- `relationship`: `!relationship [name|all] [value] [add]*`  
	  Sets or adds to NPC relationship values.
	
	- `marry`: `!marry [name|all]`  
	  Marries a single or all NPCs.
	
	- `divorce`: `!divorce [name|all]`  
	  Divorces a single or all NPCs.

- **Item Commands**
	- `give`: `!give [ID|name] [amount]*`  
	  Gives item(s) to the player by ID or name.
	
	- `list`: `!list [name]`  
	  Lists items matching the given name.
	
	- `items`: `!items [xp|currency|bonus|pet|decoration|armor|tool|food|crop|fish|normal] [get]* [amount]*`  
	  Show or give items filtered by category.
	
	- `showid`: `!showid`  
	  Toggles showing item IDs in tooltips.
	
	- `showidonhover`: `!showidonhover`  
	  Show the item ID in chat when hovering.

- **Teleport Commands**
	- `tp`: `!tp [location]`  
	  Teleports the player to a location.
	
	- `tps`: `!tps`  
	  Lists all available teleport destinations.

- **Misc Commands**
	- `ui`: `!ui [on|off]*`  
	  Toggles the game’s HUD and UI elements.
	
	- `autofillmuseum`: `!autofillmuseum`  
	  Toggles auto-fill of museum bundles on entry.
	
	- `cheatfillmuseum`: `!cheatfillmuseum`  
	  Toggles cheat-fill of museum bundles on entry.
	
	- `yearfix`: `!yearfix`  
	  Toggles corrected year calculation.
	
	- `cheats`: `!cheats`  
	  Toggles the game’s built-in cheats and hotkeys.

## AutoFill Museum
This standalone mod adds a Quality of Life feature to the game by automatically filling the museum upon entry.  
The QoL addition eliminates the need for manual placement of items in the museum.  
Simply **walk into the museum with the necessary items in your inventory**, and watch as it fills up!

## Controller Input Disabler
This mod disables direct controller inputs, allowing you to use any remapper for your controller.  
It provides you with the freedom to customize your controller settings as per your preferences.  
With this, you're no longer limited by the game's default controller settings.
