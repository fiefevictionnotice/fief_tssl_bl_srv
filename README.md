# DoF Admin Tools (DAT) for Bannerlord Servers

While TaleWorlds has greatly improved the ingame admin tools with update 1.2.8, some options are still lacking. 

DAT aims to build on what TaleWorlds have given us by adding some new config options as well as ingame chat commands for actions not covered by TaleWorlds own administration panel.

## Installation
1. Download the latest version ("MODULE RELEASE") from the [Releases](https://gitlab.com/Krex/dofadmintools/-/releases)-page.
2. Unzip into `YOURBLSERVER/Modules/`.
3. Add to startup arguments for your server: `_MODULES_*Native*Multiplayer*DoFAdminTools*_MODULES_`.
    - Make sure to add after `Native` and `Multiplayer`. Load order with other modules should not matter.

## Features

Below is a list of all features currently implemented in DAT. 

Note that anything written in ALLCAPS is a parameter. If a parameter is contained in `<ANGLEDBRACKETS>`, it is optional and does not *need* to be provided. A pipe symbol `|` denotes that the parameter may be either what is to the right or what is to the left of it.

- Chat Commands
  - **Admin Commands** - These commands can only be used by admins.
    - `!playerinfo PLAYERNAME`
      - Shows the PlayerId of any player whose name contains the given `PLAYERNAME`. Only available to admins.
      - Note that `PLAYERNAME` does not require an exact match. For example, by typing `!playerinfo [DoF]` you can show the PlayerId of any player with the DoF clan tag.
    - `!heal <PLAYERNAME>`
      - Heal any player whose name contains the given `PLAYERNAME`. If no name is given, all players are healed instead.
      - Healing in this case means restoring the HP of the player and their mount, restoring ammunition (arrows and bolts, not throwing weapons) and restoring the HP of the players shields.
    - `!move X Z`
      - Teleport yourself, moving in the direction you're looking at by `X` meters and up by `Z` meters. `X` and `Z` may be positive, negative or zero but must be whole numbers.
    - `!tptome <PLAYERNAME>`
      - Teleports any player whose name contains the given `PLAYERNAME` to your current position. If no `PLAYERNAME` is given, all players are teleported.
    - `!tpmeto PLAYERNAME`
      - Teleport yourself to the position of a player whose name contains the given `PLAYERNAME`. If multiple players names contain the given `PLAYERNAME`, the first one found is used, so be precise.
    - `!removehorses`
      - Remove all horses from the scene that do not currently have a rider.
      - Provided by Doseq - thank you!
    - `!slay <PLAYERNAME>`
      - Slays all alive players whose names contain the given `PLAYERNAME`, or all players if no `PLAYERNAME` is given.
    - `!extendwarmup`
      - Resets the warmup timer to its configured maximum, if it is currently active.
      - Provided by Gotha - thank you!
    - `!endwarmup`
      - Reduces the warmup timer to 30 seconds, if it is currently active.
        - This is the same functionality as found in the admin panel; it is re-added as a chat command here for quick use for those that prefer to use it this way.
      - Provided by Gotha - thank you!
  - **Public Commands** - These commands can be used by every player.
    - `!me`
      - Shows the using player their PlayerId.
    - `!help <COMMANDNAME>`
      - If `<COMMANDNAME>` is not set, shows a list of all currently available commands. Includes admin commands if the player is an admin.
      - If `<COMMANDNAME>` is set, shows information for the given command, assuming there is one matching the name.
    - `specs`
      - Prints a list of all players currently in spectator mode.
- New Configuration Options / Console Commands
  - `dat_add_admin ADMINID` - Add a player id to the list of admins. When a player joins the server and their id is on the list, they can use the ingame admin panel and admin chat commands.
    - The player id can be obtained by running `!me` (by the player themselves) or `!playerinfo PLAYERNAME` (by an admin) ingame.
    - This not only saves your admins the hassle of typing in the admin password, it also makes it unnecessary for them to even have it if they do not absolutely need to use the web panel.
  - `dat_include FILENAME` - Load the file with the given name and parse all of its lines as console commands.
    - Note that only files stored either directly in or in a sub-directory of `YOURBLSERVER/Modules/Native/` may be included this way.
    - **EXAMPLE:** If you host multiple servers using the same files, this allows you to store shared configuration in a shared file. For example, if you have multiple servers with the same set of admins, you could store all of the respective `dat_add_admin X` commands in a file called `SharedAdmins.txt`, then load it in your server configs by using `dat_include SharedAdmins.txt`. This way, if you have to add or remove an admin, you only have to do it in one place, reducing the chance of missing something somewhere.
  - `dat_set_command_prefix PREFIX` - Set the prefix for chat commands to the given character or character sequence. 
    - Default is `!`.
    - Note that `/` is reserved for chat channels by TaleWorlds; it can not be used here.
  - `dat_set_show_joinleave_messages TRUE|FALSE`
    - Set whether to show a message in chat when a player joins or leaves the server. Options are `TRUE` or `FALSE`.
  - `dat_set_show_adminpanel_usage TRUE|FALSE`
    - Set whether actions taken by admins using TaleWorlds admin panel should also cause a chat message to be sent to all players, as admin actions using chat commands do.
    - Enabled by default.
  - `dat_set_and_load_banlist FILENAME`
    - Set the path to a ban list file (within your `YOURBLSERVER/Modules/Native/` folder, as with `dat_include`), then load all bans stored within the given file.
    - By default, banning someone only lasts until the server is restarted. This command allows you to persist bans across server restarts.
    - **If this command is not run, bans will not be loaded**. However, any new ban will still be written to `YOURBLSERVER/Modules/Native/banlist.txt`, should you want to use them later.
      - Similarly, if you run multiple servers using the same files, bans from one server will only be transferred to the others when they execute this command, re-reading the banlist file.
    - Anything in a line after a `#` is ignored. You can use this to store extra information on the PlayerId before it (by default, the name of the player, of the banning admin and the date of the ban are stored) or to (temporarily) exclude bans from being loaded. 
      - You can permanently remove a ban by deleting the relevant line from the banlist file. Note that a server restart is required for the unban to take effect.
  - `dat_set_prevent_unnecessary_hp_sync TRUE|FALSE` **FOR SKIRMISH/CAPTAIN/BATTLE ONLY**
    - In the game modes mentioned above, by default, players current hit points are synchronized to *all* players on the server. For competitive environments especially, this information could be used by client-side cheating software. Enabling this option will replace the default behaviour for synchronizing player hitpoints with a custom one, synchronizing the hitpoints only to teeammates and spectators (to allow for streamers to still have access to the information).
    - This option is enabled by default. 
      - For servers not running any of the game modes mentioned above, nothing will happen, even if it is enabled.
    - Provided by Gotha - thank you!
  - `dat_add_automessage MESSAGE`
    - Adds a message to the list of messages to be automatically sent to all players by the server.
    - If no messages are configured by the user, no messages will be sent.
    - You may add as many messages as you like, though each message is currently limited to 256 characters.
  - `dat_set_automessage_interval INTERVAL`
    - How often the server should send the messages configured using `dat_add_automessage` to the players, in seconds.
    - By default, messages are sent every 60 seconds.
    - You may disable the system entirely by setting a value of zero or below (or by not adding any messages).
  - `dat_set_automessage_type CHAT|ADMINCHAT|BROADCAST`
    - In what way the server should send the AutoMessages to the players.
      - `CHAT` is a white chat message
      - `ADMINCHAT` is a purple chat message
      - `BROADCAST` is a purple chat message with an extra sound notification as well as a popup in the center-top of the players screens.
    - By default, messages are sent as `CHAT` messages.
  - `dat_no_more_spam`
    - Disables the default DebugManager, preventing many messages usually spammed to the console from being printed, most notably the notifications for heartbeat messages to TaleWorlds main server.
    - Note that some mods may use the same mechanism to print their information messages, and those will be lost, too, if this option is set in the config.
    - Can not be re-enabled at runtime; if you do notice you need the debug messages, you will have to restart your server without this option in its config.
  - `dat_map_alias ALIAS MAP_NAME GAME,TYPES`
    - Adds an alias for a map to be shown to admins in the admin panel. [Example Video](https://www.youtube.com/watch?v=98IB7pVq_E0)
    - Only works for admins added via the `dat_add_admin` console command; others will see the original map names instead.
    - This command is intended to replace your usages of `add_map_to_usable_maps` - its functionality is automatically used in the background.
    - Parameters:
      - `ALIAS` can be any name you want, as long as you make sure it contains no spaces. In place of spaces, put `_` - these will be replaced by spaces.
        - All aliases are automatically prefixed with a `*` character to distinguish them from non-alias'd maps.
      - `MAPNAME` should be the name of the folder the map is in (which is also what you would usually set in `add_map_to_usable_maps` or see in the admin panel).
      - `GAMETYPES` should be a comma-seperated list (no spaces!) of all game types you want the map to be available in. 
    - Example usage: `dat_map_alias Town_Outskirts mp_skirmish_map_002f Skirmish,Battle` (replacing `add_map_to_usable_maps mp_skirmish_map_002f Skirmish,Battle`).
  - `dat_corpse_fadeout_timer NUMBER`
    - Sets after how many seconds after their death corpses should despawn. Set a negative value (or none) to use TaleWorlds default, or zero or above for a custom one.
    - If ran while a game is active, the command will take effect the next time the map is changed.
  - `dat_timestamp_servername`
    - Appends the (current) timestamp to the servername when ran.
    - This one is mostly a feature for modders, mappers and the like that may often restart their servers and then find two with the same name in the server list and are then getting stuck for a minute+ when guessing the wrong one.
## Planned Features

Below is a list of features currently planned to be added to the module. If you have any other ideas, feel free to reach out and suggest them - or open a [Merge Request](https://gitlab.com/Krex/dofadmintools/-/merge_requests)!

- [X] Timed messages - Add options to add one or more messages to be sent to players by the server on a configurable interval. 
- [ ] Scene Scripts - Not a whole lot is possible here, but teleport doors will come.
- [ ] Further Chat Commands
  - [X] `!help`
  - [X] `!heal`
  - [X] `!extendwarmup`
  - [ ] ...
  - [X] Multiple teleport variations (to player, player to me, ...)
- [ ] Logging
- [X] A fix for TaleWorlds ban system, keeping permanent bans across server restarts
- [ ] Configuration for messages shown in chat, to allow for customization & internationalization
- [ ] ...


## For Developers
The following information is mainly intended for those interested in building the tools from source themselves, contributing to their further development or building upon them.

### Building from source
1. Download or Clone this repository
2. Set the `BLSERVER` environment variable to the path of your local server files installation, e.g. `D:\SteamLibrary\steamapps\common\Mount & Blade II Dedicated Server`.
   - You may need to restart your PC for msbuild to pick up on the newly set environment variable.
3. Open in your favorite IDE (personally using Rider, Visual Studio should work as well)
4. Build.

Please note that currently, the build does not assemble a full, ready-to-use-module. Copying together the `SubModule.xml` as well as the `DoFAdminTools.dll` file into the correct folders is currently still a manual process. This will be fixed soonTM.

### Basic guide
TODO: Add a basic guide for adding new chat/console commands and anything else relevant.

### License
All code in this repository is licensed under the MIT License. See the [LICENSE](https://gitlab.com/Krex/dofadmintools/-/blob/master/LICENSE) file for the full license text.

### Contributing
As per the license, you are free to build on the code provided here pretty much as you see fit. That said, if you do add something cool, please consider opening a Merge Request for it [here](https://gitlab.com/Krex/dofadmintools/-/merge_requests)!

If you do open a merge request, please keep in mind:
- Please give your merge request a proper title and a (short) description
- No use of Harmony unless 100% necessary - preferably never. 
  - Reflection is fine, though please try and keep it to a minimum
- If possible, make things configurable via console commands :)