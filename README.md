# Sun Haven Mod Collection

This repository contains mods for the game Sunhaven.

## Containing Mods
* **CommandExtension**: This mod adds a variety of new console commands.
* **SmartMuseum**: This mod that adds a Quality of Life feature to the game by automatically filling the museum upon entry.
* **ControllerBypass**: This mod disables direct controller inputs to be able to use any remapper for controller.
* **YearCorrection [OBSOLETE]**: This mod corrects the year calculation, no permanent changes.
* **Testing [DEV]**: This mod is just a testing environment.

## Command Extension
This mod is designed to enhance your gaming experience by allowing you to enter commands directly into the game’s chat box. It operates alongside the game’s existing command system, but with its own unique command prefix.  

**Here’s how to use it:**

* **Open the game’s chat box:**
This is typically done by pressing a specific key or button in the game. The chat box is where you’ll be entering your commands.
* **Enter your command:** Commands for the game itself start with a `/` prefix, while commands for the mod use a `!` prefix. For example, if you want to use a command from the mod, you would type something like `!pause`.
* **Execute the command:** After typing your command into the chat box, simply press enter. If the text you entered is a recognized command, it will be executed. If it’s not a recognized command, it will be sent as a regular chat message.

### COMMANDS:
* General Commands
  * `!help`: Get a list of all commands and their state.
  * `!state`: Shows every command that's activated.
  * `!feedback`: Toggle command feedback (chat prints). For example, "Rx4Byte got 2 Tickets!" gets not printed while off.
  * `!name [playername]`: Change the command target player. Use `!name Rx4Byte` to target Rx4Byte, or `!name` to set to default (back to default).

* Time and Weather Commands
  * `!pause`: Toggle to pause/resume time.
  * `!timespeed [value]`: Adjust the speed of time. Use `!timespeed 60` for one hour per in-game day, `!timespeed reset` to reset to default, or `!timespeed` to toggle On/Off.
  * `!time [hour/day] [value]`: Set the time. Use `!time day 14` to set to day 14, or `!time h 12` to set the hour to 12.
  * `!weather [rain/heat/clear]`: Control the weather. Use `!weather r` to toggle raining or `h` for heatwave, or `!weather c` to toggle both off.
  * `!season [seasonType]`: Change the season. Use `!season summer` to set the season to summer. Options include summer, winter, fall, spring.
  * `!years [Amount]`: Add or subtract years. Use `!years 2023` to add 2023 years, add a '-' to subtract.

* NPC Commands
  * `!relationship [NPCName] [value]`
  * `!divorce [NPCName]`
  * `!marry [NPCName]`

* Item Commands
  * ~~`!give [ID/NAME] [Amount]`: Give items. Use `!give tickets 2` or `!give 18011 2`. It's also possible to give DLC pets/mounts like `!give wickedmountschest`.~~
  * ~~`!items [any word]`: Prints all items containing the given word to the chat. For example, `!items chest`.~~
  * ~~`!getitemids [xp/money/all/furniture/bonus/quest]`: Prints items to chat like this "FarmingEXP : 60004". Printing ALL items creates an item list as a text file inside the Sunhaven folder.~~
  * ~~`!showid`: Show the item ID in tooltip.~~

* Currency Commands
  * `!money [value]`: Adjust money. Use `!money 100` to add, or `!money -100` or `!money 100-` to subtract.
  * `!coins [value]`: Adjust coins. Use `!coins 100` to add, or `!coins -100` or `!coins 100-` to subtract.
  * `!orbs [value]`: Adjust orbs. Use `!orbs 100` to add, or `!orbs -100` or `!orbs 100-` to subtract.
  * `!tickets [value]`: Adjust tickets. Use `!tickets 100` to add, or `!tickets -100` or `!tickets 100-` to subtract.

* Mine Commands
  * `!minereset`: Reset minerals.
  * `!mineoverfill`: Fully fill the mine.
  * `!mineclear`: Fully clear the mine.

* Player Commands
  * `!jumper`: Toggle the ability to jump over objects.
  * `!dasher`: Dash without pause.
  * `!noclip`: Walk through walls and objects.
  * `!manafill`: Refill mana.
  * `!manainf`: Infinite mana.
  * `!healthfill`: Refill health.
  * `!nohit`: Godmode, infinite health.
  * `!sleep`: Sleep to the next day where you are (no end day screen).
  * `!tp [location]`: Teleport to a location. For example, `!tp home`.
  * `!tps`: Prints all teleport locations.

* Misc Commands
  * `!devkit`: Get all developer items.
  * `!cheats`: Toggle cheat-keybinds.
  * `!ui`: Toggle UI, hide or show the UI back.
  * ~~`!autofillmuseum`: Toggle, automatically fill museum upon entry (need items in inventory).~~
  * ~~`!cheatfillmuseum`: Toggle, automatically fill museum upon entry (no items needed, CHEATING).~~
  * ~~`!exp [ExpType] [Amount]`: Add experience. Use `!exp farming 100`.~~

* Pet Commands
  * ~~`!despawnpet`~~
  * ~~`!pet [petname]`~~
  * ~~`!pets`~~

## SmartMuseum: A Standalone Museum-AutoFill Mod
This standalone mod adds a Quality of Life feature to the game by automatically filling the museum upon entry. The QoL addition eliminates the need for manual placement of items in the museum.  
Simply **walk into the museum with the necessary items in your inventory**, and watch as it fills up!

## ControllerBypass
This mod disables direct controller inputs, allowing you to use any remapper for your controller. It provides you with the freedom to customize your controller settings as per your preferences. With this, you're no longer limited by the game's default controller settings.
