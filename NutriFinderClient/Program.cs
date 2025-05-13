using NutriFinderClient;

Console.WriteLine("Yup");

var inputClient = new NutritionClient();

while (true)
{
    Console.WriteLine(" Enter food item in english");

    var input = Console.ReadLine();

    var result = inputClient.ValidateInput(input);
    
    var message = result == "ok" ? "good job" : "buuh";

    Console.WriteLine(message);
}