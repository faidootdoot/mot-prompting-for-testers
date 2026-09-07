using DataGenerationUtility.src.TestDataGenerator;
using TestDataGenerator;
var persons=TestDataGeneratorUtility.Generate(5,TestDataType.Positive); 
foreach (var person in persons)
{
    Console.WriteLine(
        $"{person.FirstName} | " +
        $"{person.LastName} | " +
        $"{person.DateOfBirth} | " +
        $"{person.Country} | " +
        $"{person.NationalInsuranceNumber} | " +
        $"{person.TestDataType}");
}