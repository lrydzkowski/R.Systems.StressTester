using System.ComponentModel.DataAnnotations;

namespace R.Systems.StressTester.Cli.Validation;

[AttributeUsageAttribute(AttributeTargets.Parameter)]
internal class FilePathExistsAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is string path && File.Exists(path))
        {
            return ValidationResult.Success;
        }

        return new ValidationResult($"File '{value}' doesn't exist");
    }
}
