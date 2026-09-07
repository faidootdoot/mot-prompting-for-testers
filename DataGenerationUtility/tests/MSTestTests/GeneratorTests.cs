using DataGenerationUtility.src.TestDataGenerator;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;
using System.Text.RegularExpressions;
using TestDataGenerator;

namespace MSTestTests;

[TestClass]
public class TestDataGeneratorUtilityTests
{
    [TestMethod]
    public void Generate_Should_Return_Requested_Number_Of_Rows()
    {
        var result = TestDataGeneratorUtility.Generate(
            100,
            TestDataType.Positive);

        result.Should().HaveCount(100);
    }

    [TestMethod]
    public void Generate_With_Zero_Rows_Should_Throw()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            TestDataGeneratorUtility.Generate(
                0,
                TestDataType.Positive));
    }

    [TestMethod]
    public void Generate_With_Negative_Rows_Should_Throw()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            TestDataGeneratorUtility.Generate(
                -1,
                TestDataType.Positive));
    }

    [TestMethod]
    public void PositiveData_Should_Generate_Valid_Date_Format()
    {
        var result = TestDataGeneratorUtility.Generate(
            500,
            TestDataType.Positive);

        result.Should().OnlyContain(x =>
            IsValidDateOfBirthFormat(x.DateOfBirth));
    }

    private static bool IsValidDateOfBirthFormat(string dateOfBirth)
    {
        return DateTime.TryParseExact(
            dateOfBirth,
            "yyyy-MM-dd",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out _);
    }

    [TestMethod]
    public void PositiveData_Should_Generate_Valid_Nino_Format()
    {
        var regex =
            new Regex(
                @"^[ABCEGHJKLMNPRSTWXYZ]{2}[0-9]{6}[ABCD]$");

        var result = TestDataGeneratorUtility.Generate(
            500,
            TestDataType.Positive);

        result.Should().OnlyContain(x =>
            regex.IsMatch(x.NationalInsuranceNumber!));
    }

    [TestMethod]
    public void PositiveData_Should_Generate_Valid_Country_Codes()
    {
        var result = TestDataGeneratorUtility.Generate(
            500,
            TestDataType.Positive);

        result.Should().OnlyContain(x =>
            !string.IsNullOrWhiteSpace(x.Country) &&
            x.Country!.Length == 2);
    }

    [TestMethod]
    public void PositiveData_Should_Have_Valid_Name_Lengths()
    {
        var result = TestDataGeneratorUtility.Generate(
            500,
            TestDataType.Positive);

        result.Should().OnlyContain(x =>
            x.FirstName.Length >= 1 &&
            x.FirstName.Length <= 50 &&
            x.LastName.Length >= 1 &&
            x.LastName.Length <= 50);
    }

    [TestMethod]
    public void BoundaryData_Should_Generate_Minimum_Length()
    {
        var result = TestDataGeneratorUtility.Generate(
            1000,
            TestDataType.Boundary);

        result.Should().Contain(x =>
            x.FirstName.Length == 1);
    }

    [TestMethod]
    public void BoundaryData_Should_Generate_Maximum_Length()
    {
        var result = TestDataGeneratorUtility.Generate(
            1000,
            TestDataType.Boundary);

        result.Should().Contain(x =>
            x.FirstName.Length == 50);
    }

    [TestMethod]
    public void BoundaryData_Should_Generate_Over_Maximum_Length()
    {
        var result = TestDataGeneratorUtility.Generate(
            1000,
            TestDataType.Boundary);

        result.Should().Contain(x =>
            x.FirstName.Length > 50);
    }

    [TestMethod]
    public void NegativeData_Should_Contain_Invalid_FirstName_Scenario()
    {
        var result = TestDataGeneratorUtility.Generate(
            5000,
            TestDataType.Negative);

        result.Should().Contain(x =>
            x.InvalidField == InvalidField.FirstName);
    }

    [TestMethod]
    public void NegativeData_Should_Contain_Invalid_LastName_Scenario()
    {
        var result = TestDataGeneratorUtility.Generate(
            5000,
            TestDataType.Negative);

        result.Should().Contain(x =>
            x.InvalidField == InvalidField.LastName);
    }

    [TestMethod]
    public void NegativeData_Should_Contain_Invalid_DateOfBirth_Scenario()
    {
        var result = TestDataGeneratorUtility.Generate(
            5000,
            TestDataType.Negative);

        result.Should().Contain(x =>
            x.InvalidField == InvalidField.DateOfBirth);
    }

    [TestMethod]
    public void NegativeData_Should_Contain_Invalid_Country_Scenario()
    {
        var result = TestDataGeneratorUtility.Generate(
            5000,
            TestDataType.Negative);

        result.Should().Contain(x =>
            x.InvalidField == InvalidField.Country);
    }

    [TestMethod]
    public void NegativeData_Should_Contain_Invalid_Nino_Scenario()
    {
        var result = TestDataGeneratorUtility.Generate(
            5000,
            TestDataType.Negative);

        result.Should().Contain(x =>
            x.InvalidField == InvalidField.NationalInsuranceNumber);
    }

    [TestMethod]
    public void PenetrationData_Should_Generate_Xss_Payloads()
    {
        var result = TestDataGeneratorUtility.Generate(
            1000,
            TestDataType.Penetration);

        result.Should().Contain(x =>
            x.FirstName.Contains("<script>") ||
            x.LastName.Contains("<script>"));
    }

    [TestMethod]
    public void PenetrationData_Should_Generate_SqlInjection_Payloads()
    {
        var result = TestDataGeneratorUtility.Generate(
            1000,
            TestDataType.Penetration);

        result.Should().Contain(x =>
            x.FirstName.Contains("DROP TABLE") ||
            x.LastName.Contains("DROP TABLE") ||
            x.FirstName.Contains("OR 1=1") ||
            x.LastName.Contains("OR 1=1"));
    }

    [TestMethod]
    public void PenetrationData_Should_Generate_PathTraversal_Payloads()
    {
        var result = TestDataGeneratorUtility.Generate(
            1000,
            TestDataType.Penetration);

        result.Should().Contain(x =>
            x.FirstName.Contains("../") ||
            x.LastName.Contains("../"));
    }

    [TestMethod]
    public void MixedData_Should_Contain_Multiple_Test_Types()
    {
        var result = TestDataGeneratorUtility.Generate(
            1000,
            TestDataType.Mixed);

        result
            .Select(x => x.TestDataType)
            .Distinct()
            .Count()
            .Should()
            .BeGreaterThan(1);
    }

    [TestMethod]
    public void Generate_10000_Rows_Should_Complete()
    {
        var result = TestDataGeneratorUtility.Generate(
            10000,
            TestDataType.Mixed);

        result.Should().HaveCount(10000);
    }

    [TestMethod]
    public void Generated_Person_Should_Have_Id()
    {
        var result = TestDataGeneratorUtility.Generate(
            1,
            TestDataType.Positive);

        result.First().Id.Should().NotBe(Guid.Empty);
    }

    [TestMethod]
    public void PositiveData_Should_Be_Flagged_As_Valid()
    {
        var result = TestDataGeneratorUtility.Generate(
            100,
            TestDataType.Positive);

        result.Should().OnlyContain(x =>
            x.ExpectedToBeValid);
    }

    [TestMethod]
    public void NegativeData_Should_Be_Flagged_As_Invalid()
    {
        var result = TestDataGeneratorUtility.Generate(
            100,
            TestDataType.Negative);

        result.Should().OnlyContain(x =>
            !x.ExpectedToBeValid);
    }

    [TestMethod]
    public void Every_Record_Should_Have_TestDataType()
    {
        var result = TestDataGeneratorUtility.Generate(
            100,
            TestDataType.Mixed);

        result.Should().OnlyContain(x =>
            Enum.IsDefined(x.TestDataType));
    }

    [TestMethod]
    public void Every_Record_Should_Have_Heuristic()
    {
        var result = TestDataGeneratorUtility.Generate(
            500,
            TestDataType.Mixed);

        result.Should().OnlyContain(x =>
            !string.IsNullOrWhiteSpace(x.Heuristic));
    }

    [TestMethod]
    public void Every_Record_Should_Have_Scenario_Description()
    {
        var result = TestDataGeneratorUtility.Generate(
            500,
            TestDataType.Mixed);

        result.Should().OnlyContain(x =>
            !string.IsNullOrWhiteSpace(x.ScenarioDescription));
    }

    [TestMethod]
    public void Every_Record_Should_Have_Generated_Date()
    {
        var result = TestDataGeneratorUtility.Generate(
            500,
            TestDataType.Mixed);

        result.Should().OnlyContain(x =>
            x.GeneratedUtc != default);
    }
}