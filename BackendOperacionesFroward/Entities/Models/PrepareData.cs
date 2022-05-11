using BackendOperacionesFroward.Controllers.Utilities;
using System.Text.RegularExpressions;

public class DataManagement
{

    internal static string PrepareOT(string oldValue)
    {
        return RemoveAllSpaces(oldValue).ToUpper();
    }

    internal static string PrepareRUT(string oldValue)
    {
        return RemoveAllSpaces(oldValue).ToUpper();
    }

    internal static string PreparePatent(string oldValue)
    {
        return RemoveAllSpaces(oldValue).ToUpper();
    }

    private static string RemoveAllSpaces(string value)
    {
        if (string.IsNullOrEmpty(value))
            return "";
        return Regex.Replace(value, @"\s", "");
    }

    internal static bool PrepareRequestState(string state) {
        if (state == Constants.Request_State_Valid || state == Constants.Request_State_Defeated)
            return true;
        return false;
    }

}