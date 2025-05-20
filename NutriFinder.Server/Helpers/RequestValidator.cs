using System.Text.RegularExpressions;

namespace NutriFinder.Server.Helpers;

public class RequestValidator
{
    public string Validate(string request)
    {
        if (string.IsNullOrWhiteSpace(request))
            return "Request can not be empty";

        if (request.Any(char.IsDigit))
            return "Request can not contain numbers";
        
        if (request.Any(char.IsSymbol))
            return "Request can not contain special characters";
        
        if (!Regex.IsMatch(request, "^[a-åA-Å ]+$"))
            return "Only English letters is accepted";
       
        return "ok";
    }
}