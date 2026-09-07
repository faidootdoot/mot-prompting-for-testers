using DataGenerationUtility.src.TestDataGenerator;

namespace TestDataGenerator;

public sealed class Person
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public string FirstName { get; init; } = string.Empty;

    public string LastName { get; init; } = string.Empty;

    /// <summary>
    /// Test data value in YYYY-MM-DD format for valid data.
    /// May contain intentionally invalid values for negative testing.
    /// </summary>
    public string DateOfBirth { get; init; } = string.Empty;

    public string? Country { get; init; }

    public string? NationalInsuranceNumber { get; init; }

    public TestDataType TestDataType { get; init; }

    public string Heuristic { get; init; } = string.Empty;

    public bool ExpectedToBeValid { get; init; }

    public InvalidField InvalidField { get; init; }

    public string FailureReason { get; init; } = string.Empty;

    public string ScenarioDescription { get; init; } = string.Empty;

    public DateTimeOffset GeneratedUtc { get; init; }
        = DateTimeOffset.UtcNow;
}