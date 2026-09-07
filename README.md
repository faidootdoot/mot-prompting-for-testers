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


