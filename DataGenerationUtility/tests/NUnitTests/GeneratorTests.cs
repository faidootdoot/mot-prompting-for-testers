using DataGenerationUtility.src.TestDataGenerator;
using NUnit.Framework;
using System.Globalization;
using System.Text.RegularExpressions;
using TestDataGenerator;
using Assert = NUnit.Framework.Assert;

namespace NUnitTests;

[TestFixture]
public class TestDataGeneratorUtilityTests
{
    [Test]
    public void Generate_Should_Return_Requested_Number_Of_Rows()
    {
        var result = TestDataGeneratorUtility.Generate(
            100,
            TestDataType.Positive);

        Assert.That(result.Count, Is.EqualTo(100));
    }

    [Test]
    public void Generate_With_Zero_Rows_Should_Throw()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            TestDataGeneratorUtility.Generate(
                0,
                TestDataType.Positive));
    }

    [Test]
    public void Generate_With_Negative_Rows_Should_Throw()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            TestDataGeneratorUtility.Generate(
                -1,
                TestDataType.Positive));
    }

    [Test]
    public void PositiveData_Should_Generate_Valid_Date_Format()
    {
        var result = TestDataGeneratorUtility.Generate(
            500,
            TestDataType.Positive);

        Assert.That(
            result.All(x =>
                DateTime.TryParseExact(
                    x.DateOfBirth,
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out _)),
            Is.True);
    }

    [Test]
    public void PositiveData_Should_Generate_Valid_Nino_Format()
    {
        var regex =
            new Regex(@"^[ABCEGHJKLMNPRSTWXYZ]{2}[0-9]{6}[ABCD]$");

        var result = TestDataGeneratorUtility.Generate(
            500,
            TestDataType.Positive);

        Assert.That(
            result.All(x =>
                regex.IsMatch(x.NationalInsuranceNumber!)),
            Is.True);
    }

    [Test]
    public void PositiveData_Should_Generate_Valid_Country_Codes()
    {
        var result = TestDataGeneratorUtility.Generate(
            500,
            TestDataType.Positive);

        Assert.That(
            result.All(x =>
                !string.IsNullOrWhiteSpace(x.Country)
                && x.Country!.Length == 2),
            Is.True);
    }

    [Test]
    public void PositiveData_Should_Have_Valid_Name_Lengths()
    {
        var result = TestDataGeneratorUtility.Generate(
            500,
            TestDataType.Positive);

        Assert.That(
            result.All(x =>
                x.FirstName.Length >= 1 &&
                x.FirstName.Length <= 50 &&
                x.LastName.Length >= 1 &&
                x.LastName.Length <= 50),
            Is.True);
    }

    [Test]
    public void BoundaryData_Should_Generate_Minimum_Length()
    {
        var result = TestDataGeneratorUtility.Generate(
            1000,
            TestDataType.Boundary);

        Assert.That(
            result.Any(x => x.FirstName.Length == 1),
            Is.True);
    }

    [Test]
    public void BoundaryData_Should_Generate_Maximum_Length()
    {
        var result = TestDataGeneratorUtility.Generate(
            1000,
            TestDataType.Boundary);

        Assert.That(
            result.Any(x => x.FirstName.Length == 50),
            Is.True);
    }

    [Test]
    public void BoundaryData_Should_Generate_Over_Maximum_Length()
    {
        var result = TestDataGeneratorUtility.Generate(
            1000,
            TestDataType.Boundary);

        Assert.That(
            result.Any(x => x.FirstName.Length > 50),
            Is.True);
    }

    [Test]
    public void NegativeData_Should_Contain_Invalid_FirstName()
    {
        var result = TestDataGeneratorUtility.Generate(
            1000,
            TestDataType.Negative);

        Assert.That(
            result.Any(x =>
                x.InvalidField == InvalidField.FirstName),
            Is.True);
    }

    [Test]
    public void NegativeData_Should_Contain_Invalid_LastName()
    {
        var result = TestDataGeneratorUtility.Generate(
            1000,
            TestDataType.Negative);

        Assert.That(
            result.Any(x =>
                x.InvalidField == InvalidField.LastName),
            Is.True);
    }

    [Test]
    public void NegativeData_Should_Contain_Invalid_Date()
    {
        var result = TestDataGeneratorUtility.Generate(
            1000,
            TestDataType.Negative);

        Assert.That(
            result.Any(x =>
                x.InvalidField == InvalidField.DateOfBirth),
            Is.True);
    }

    [Test]
    public void NegativeData_Should_Contain_Invalid_Country()
    {
        var result = TestDataGeneratorUtility.Generate(
            1000,
            TestDataType.Negative);
        Console.WriteLine(result.ElementAt(0).Country);
        Console.WriteLine(result.ElementAt(1).Country);

        Assert.That(
            result.Any(x =>
                x.InvalidField == InvalidField.Country),
            Is.True);
    }

    [Test]
    public void NegativeData_Should_Contain_Invalid_Nino()
    {
        var result = TestDataGeneratorUtility.Generate(
            1000,
            TestDataType.Negative);

        Assert.That(
            result.Any(x =>
                x.InvalidField == InvalidField.NationalInsuranceNumber),
            Is.True);
    }

    [Test]
    public void PenetrationData_Should_Generate_Xss_Payloads()
    {
        var result = TestDataGeneratorUtility.Generate(
            1000,
            TestDataType.Penetration);

        Assert.That(
            result.Any(x =>
                x.FirstName.Contains("<script>") ||
                x.LastName.Contains("<script>")),
            Is.True);
    }

    [Test]
    public void PenetrationData_Should_Generate_SqlInjection_Payloads()
    {
        var result = TestDataGeneratorUtility.Generate(
            1000,
            TestDataType.Penetration);

        Assert.That(
            result.Any(x =>
                x.FirstName.Contains("DROP TABLE") ||
                x.LastName.Contains("DROP TABLE") ||
                x.FirstName.Contains("OR 1=1") ||
                x.LastName.Contains("OR 1=1")),
            Is.True);
    }

    [Test]
    public void PenetrationData_Should_Generate_PathTraversal_Payloads()
    {
        var result = TestDataGeneratorUtility.Generate(
            1000,
            TestDataType.Penetration);

        Assert.That(
            result.Any(x =>
                x.FirstName.Contains("../") ||
                x.LastName.Contains("../")),
            Is.True);
    }

    [Test]
    public void MixedData_Should_Contain_Multiple_Test_Types()
    {
        var result = TestDataGeneratorUtility.Generate(
            500,
            TestDataType.Mixed);

        var distinctTypes = result
            .Select(x => x.TestDataType)
            .Distinct()
            .Count();

        Assert.That(distinctTypes, Is.GreaterThan(1));
    }

    [Test]
    public void Generate_10000_Rows_Should_Complete()
    {
        var result = TestDataGeneratorUtility.Generate(
            10000,
            TestDataType.Mixed);

        Assert.That(result.Count, Is.EqualTo(10000));
    }
}