using System.Text.RegularExpressions;

namespace NutriFinderClient;

public class NutritionClient
{
    public string ValidateInput(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return "Input can not be empty";

        if (input.Any(char.IsDigit))
            return "Input can not contain numbers";
        
        // if (input.Any(char.IsSymbol))
        //     return "Input can not contain special characters";
        //
        // if (!input.Any(char.IsAsciiLetter)) 
        //     return "Input can only be A-Z with no tone indicators";
        
       if (!Regex.IsMatch(input, "^[a-zA-Z ]+$"))
           return "Only English letters is accepted";
       
       return "ok";
    }

 
}