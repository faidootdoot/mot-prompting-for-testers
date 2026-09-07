# mot-prompting-for-testers
For the course https://www.ministryoftesting.com/courses/prompting-for-testers/

# Initial prompt
Act as a SDET (Software Developer in Test) who works on creating test data to provide comprehensive test data coverage
I want to create a generate data utility that include, positive, negative, boundary-related and penetration testing related data to expose vulnerabilities
I want to generate a specified number of rows of test data for the following fields:
- first name - string - mandatory 1 to 50 characters 
- last name - string - mandatory 1 to 50 characters
- date of birth - date - mandatory, format is YYYY-MM-DD
- country - string - optional - ISO 3166
- national insurance number - optional - format as per NIM39110 - National Insurance Numbers (NINOs): Format and Security: What a NINO looks like - HMRC internal manual - GOV.UK 
- data


Generate test data utility to:
- take the number of rows of test data to be generated
- returns a list of person
- each person object to have a field to describe the data test type
- to have the option to identify the type of test data to be created, i.e., positive, negative etc. 


Create solution and project is C# using .NET 10
Create the generate test data utility.
Create a file that will call the generaate test data utility and output to console.
Create test files to test the generate test data utility using:
- NUnit
- xUnit
- MSTest

Provide this a downloadable file that can be extracted.

QA manually add the files to a C# project and solution, removed xUnit tests, added the require nuget packages, build and address compilation and build issues.

QA continued with prompt:
Use this resource for data creation Test Heuristics Cheat Sheet | MoTaverse 
Do not hard code the test data, make the test data random.
Create the TestDataGeneratorUtility.cs file.

QA continued with prompt:
Update test files to hage Additional Enterprise SDET Tests Recommended
Update the MSTest and NUnit test files.
=> Person class required to be updated as the data of birth data type changed to DateOnly and added complexity for test data generation with a negative and positive field for date of birth

QA continued with prompt:
Update the person class
=> Build errors were found and decided that blindly accepting DateOnly for date of birth was bad idea and reverted back to string

QA continued with prompt:
Update to use public string DateOfBirth { get; init; } = string.Empty;
Update all required files
Update the test files
=> Build warnings for MSTest Assert.

QA continue with prompt:
MSTest
Use Assert.Contains over Assert.IsTrue
Use Assert.ThrowsException over ExpectedException
Update MSTest file
Use FluentAssertions for onlycontain.
Update MSTest file
MSTEST assert should use  ThrowsExactly over ThrowsException
Update MSTest file

This is a good starting point but there are so many improvements that could be made, for example, randomising the data using faker (or similar) library.

I like that it identifies what it has created but what can be better, see below

Note: This is a foundational implementation. For a production-quality SDET-grade solution, I would normally extend it with:

Full .NET 10 solution/project files (.sln, .csproj)
Realistic ISO 3166 country generation
HMRC NINO format validation and generation
Configurable test-data strategies
Weighted randomization
Comprehensive boundary datasets
Security-focused payload libraries (XSS, SQL injection, Unicode, encoding, path traversal, etc.)
Fluent API
Bogus/Faker integration
Thousands of automated unit tests
CSV/JSON/XML export options
Deterministic seeding
Data annotations and validation rules
CI/CD pipeline configuration

QA tries to recreate prompt with all the necessary information, but finds the output is worse than iteratively prompting and improving.
The output is very basic, hardcoded, missing the richness of the tests that would reference the Ministry of Testing, Heuristic cheat sheet.


