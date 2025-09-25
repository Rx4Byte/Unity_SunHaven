<div align="center">
	<!--
	<a href="https://github.com/Rx4Byte/Athena">
    	<img src="resources/assets/logo.png" alt="Logo" width="128" height="128">
  	</a>
	-->
   	<h3 align="center">Sun Haven Mod Collection</h3>
	<p align="center">
		This repository contains mods for the game Sunhaven.
    	<br/>
    	<!--<a href="https://github.com/Rx4Byte/Athena"><strong>Explore the docs »</strong></a>
    	<br />
    	<br />-->
    	<!-- TODO: add demo screenshots
    	<a href="https://github.com/">View Demo</a>
    	·-->
    	<a href="https://github.com/Rx4Byte/Unity_SunHaven/issues">Report Bug</a>
    	·									
    	<a href="https://github.com/Rx4Byte/Unity_SunHaven/issues">Request Feature</a>
		<!--								
    	·									
    	<a href="https://github.com/Rx4Byte/Unity_SunHaven/issues">Star Athena</a>
		-->
  	</p>
</div>

<!-- Table of Contents -->
<ol>
	<li><a href="#mod-collection">Mod Collection</a></li>
	<li><a href="#development-setup">Development Setup</a></li>
	<!--
	<li><a href="#contributing">Contributing</a>
		<ul>
			<li><a href="#opening-an-issue">Opening an Issue</a>
				<ul>
					<li><a href="#-bug-report">🐛 Bug Report</a></li>
					<li><a href="#-feature-request">💡 Feature Request</a></li>
					<li><a href="#-suggestion">💬 Suggestion</a></li>
					<li><a href="#-question">❓ Question</a></li>
				</ul>
			</li>
			<li><a href="#pull-request">Pull Request</a></li>
		</ul>
	</li>
	-->
	<!--<li><a href="#chat-discussion">Chat & Discussion</a></li>-->
</ol>

## Mod collection
* **[Command Extension](#command-extension)**: This mod adds a variety of new console commands.  
* **[AutoFill Museum](#autofill-museum)**: This mod adds a quality-of-life feature by automatically filling the museum upon entry.  
* **[Controller Input Disabler](#controller-input-disabler)**: This mod disables direct controller inputs so you can use any controller remapper.

### Command Extension
This mod is designed to enhance your gaming experience by allowing you to enter commands directly into the game’s chat box.  
It operates alongside the game’s existing command system but uses its own command prefix `!`.  
*Note: Messages starting with a single `!` will not be sent to other players.*

- **How to use commands**: Commands are entered using the chat box.  
  Open chat (Enter by default) and type the command prefix `!` followed by a command, e.g. `!help` for a list of commands.

#### List of Commands
```
ℹ️ Command Parameters with a star (*) are optional, e.g. [amount]*
```
- **Core Commands**
	- `help` ➜ `!help` `<command key>`  
	  Displays a list of all available commands.
	
	- `state` ➜ `!state`  
	  Shows which commands are currently activated.

- **Player Commands**
	- `jumper` ➜ `!jumper`  
	  Toggles ability to jump through objects.
	
	- `dasher` ➜ `!dasher`  
	  Toggles infinite dash charges.
	
	- `manafill` ➜ `!manafill`  
	  Refills the player’s mana to maximum.
	
	- `manainf` ➜ `!manainf`  
	  Toggles infinite mana.
	
	- `healthfill` ➜ `!healthfill`  
	  Refills the player’s health to maximum.
	
	- `nohit` ➜ `!nohit`  
	  Toggles invincibility (no damage taken).
	
	- `noclip` ➜ `!noclip`  
	  Toggles noclip mode (walk through walls).
	
	- `sleep` ➜ `!sleep`  
	  Sleeps through the night.

- **Time Commands**
	- `pause` ➜ `!pause`  
	  Toggles the game’s time pause on or off.
	
	- `timespeed` ➜ `!timespeed` `<multiplier> | reset | default`  
	  Sets or toggles a custom day-length multiplier.
	
	- `time` ➜ `!time` `[hour | day] <value>`  
	  Sets the current hour or day in the day/night cycle.
	
	- `weather` ➜ `!weather` `raining | heatwave | clear`  
	  Changes weather to raining, heatwave, or clear.
	
	- `season` ➜ `!season` `spring | summer | fall | winter`  
	  Changes the current season.
	
	- `years` ➜ `!years` `-* <amount>`  
	  Increase or Decrease the Year.

- **Currency Commands**
	- `money` ➜ `!money` `-* <amount>`  
	  Alias for `!coins`.
	
	- `coins` ➜ `!coins` `-* <amount>`  
	  Adds or subtracts Coins from the player.
	
	- `orbs` ➜ `!orbs` `-* <amount>`  
	  Adds or subtracts Orbs from the player.
	
	- `tickets` ➜ `!tickets` `-* <amount>`  
	  Adds or subtracts Tickets from the player.
	
	- `devkit` ➜ `!devkit`  
	  Grants the developer kit items.

- **Mine Commands**
	- `minereset` ➜ `!minereset`  
	  Reset the current mine.
	
	- `mineclear` ➜ `!mineclear`  
	  Removes all rocks and ores from the mine.
	
	- `mineoverfill` ➜ `!mineoverfill`  
	  Fills the mine completely with rocks and ores.

- **Relationship Commands**
	- `relationship` ➜ `!relationship` `[<name> | all] <value>`  
	  Sets or adds to NPC relationship values.
	
	- `marry` ➜ `!marry` `<name> | all`  
	  Marries a single or all NPCs.
	
	- `divorce` ➜ `!divorce` `<name> | all`  
	  Divorces a single or all NPCs.

- **Item Commands**
	- `give` ➜ `!give` `<id | name>` `amount*`  
	  Gives item(s) to the player by ID or name.
	
	- `items` ➜ `!items <name>`  
	  Lists items matching the given name.
	
	- `list` ➜ `!list` `xp | currency | bonus | pet | decoration | armor | tool | food | crop | fish | normal]` `[get <amount>]*`  
	  Show or give items filtered by category.
	
	- ~~`showid` ➜ `!showid`~~  
	  ~~Toggles showing item IDs in tooltips.~~
	
	- `showidonhover` ➜ `!showidonhover`  
	  Show the item ID in chat when hovering.

- **Teleport Commands**
	- `tp` ➜ `!tp` `<location>`  
	  Teleports the player to a location.
	
	- `tps` ➜ `!tps`  
	  Lists all available teleport destinations.

- **Misc Commands**
	- `ui` ➜ `!ui` `[on | off]*`  
	  Toggles the game’s HUD and UI elements.
	
	- `autofillmuseum` ➜ `!autofillmuseum`  
	  Toggles auto-fill of museum bundles on entry.
	
	- `cheatfillmuseum` ➜ `!cheatfillmuseum`  
	  Toggles cheat-fill of museum bundles on entry.
	
	- `cheats` ➜ `!cheats`  
	  Toggles the game’s built-in cheats and hotkeys.

### AutoFill Museum
This standalone mod adds a Quality of Life feature to the game by automatically filling the museum upon entry.  
The QoL addition eliminates the need for manual placement of items in the museum.  
Simply **walk into the museum with the necessary items in your inventory**, and watch as it fills up!

### Controller Input Disabler
This mod disables direct controller inputs, allowing you to use any remapper for your controller.  
It provides you with the freedom to customize your controller settings as per your preferences.  
With this, you're no longer limited by the game's default controller settings.

## Development Setup
### Dependencies
Create a folder at the repository root named `.dependencies` *(git-ignored)* and copy required DLLs there.  
**The files required vary by project**, you can either copy all files from the game's Managed folder (e.g. 'Sun Haven_Data\Managed') or only the DLLs referenced by the projects.

- **Game DLLs** (from 'Game\Game_Data\Managed')
	+ UnityEngine.dll
	+ UnityEngine.CoreModule.dll

- **BepInEx** (Mod Loader) [GitHub](https://github.com/BepInEx/BepInEx)
	+ BepInEx.dll (found under 'BepInEx\core')

- **Harmony** (Run-time Patching) [GitHub](https://github.com/pardeike/Harmony)
	+ 0Harmony.dll (also found under 'BepInEx\core')

### Setup Steps
1. Clone the repository.
2. Create the `.dependencies` folder at the solution root.
3. Copy [required DLLs](#required-files) from your Sun Haven game install and/or BepInEx into `.dependencies`.
