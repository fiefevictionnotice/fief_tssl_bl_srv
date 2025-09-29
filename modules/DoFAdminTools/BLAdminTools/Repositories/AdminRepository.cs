using System.Collections.Generic;
using System.Linq;
using DoFAdminTools.Helpers;

namespace DoFAdminTools.Repositories;

public class AdminRepository
{
    private AdminRepository()
    {
        AdminIds = new List<string>();
    }

    private static AdminRepository _instance;
    public static AdminRepository Instance
    {
        get
        {
            if (_instance == null)
                _instance = new AdminRepository();

            return _instance;
        }
    }
        
    public List<string> AdminIds { get; private set; }

    public bool IsAdmin(string playerId)
    {
        return AdminIds.Any(adminId => adminId == playerId);
    }

    public void AddAdmin(string newAdminId)
    {
        AdminIds.Add(newAdminId);
        Helper.Print($"Added {newAdminId} as admin.");
    }
}