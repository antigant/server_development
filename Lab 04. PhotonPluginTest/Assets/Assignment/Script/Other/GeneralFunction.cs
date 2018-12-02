// Using this script for functions that can be used for other classes
public class GeneralFunction
{
    public static string GetStringDataFromMessage(string message, string returnData)
    {
        string temp = returnData + "=";
        int pFrom = message.IndexOf(temp) + temp.Length;
        int pTo = message.LastIndexOf(",");

        // there's no comma at the end
        if (pTo - pFrom < 0)
        {
            pFrom = message.LastIndexOf(temp) + temp.Length;
            return message.Substring(pFrom);
        }

        return message.Substring(pFrom, pTo - pFrom);
    }
}
