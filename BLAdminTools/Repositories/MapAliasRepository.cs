using System.Collections.Generic;
using System.Linq;
using DoFAdminTools.Helpers;
using TaleWorlds.MountAndBlade;

namespace DoFAdminTools.Repositories;

public class MapAliasRepository
{
    public const char AliasPrefix = '*';
    private static MapAliasRepository _instance;

    /// <summary>
    /// Maps map aliases to the actual map (folder) names used by the game.
    /// </summary>
    private Dictionary<string, string> _aliases;

    public static MapAliasRepository Instance
    {
        get
        {
            if (_instance == null)
                _instance = new MapAliasRepository();

            return _instance;
        }
    }

    private MapAliasRepository()
    {
        _aliases = new();
    }

    public bool AddAlias(string alias, string mapName)
    {
        try
        {
            _aliases.Add(alias, mapName);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public string GetMapForAlias(string alias)
    {
        if (_aliases.TryGetValue(alias, out string mapName))
            return mapName;
        
        Helper.PrintError($"Tried getting map name for alias {alias}, but no such alias was configured!");
        return ""; // TODO check if we need to return a map name for some TW map instead
    }
    
    public List<CustomGameUsableMap> ApplyAliases(List<CustomGameUsableMap> list)
    {
        var newList = new List<CustomGameUsableMap>();
        
        foreach (var map in list)
        {
            var alias = _aliases.FirstOrDefault(x => x.Value == map.map).Key;
            
            if (alias == null)
                newList.Add(map);
            else newList.Add(new CustomGameUsableMap(alias, map.isCompatibleWithAllGameTypes, map.compatibleGameTypes));
        }

        return newList;
    }
}