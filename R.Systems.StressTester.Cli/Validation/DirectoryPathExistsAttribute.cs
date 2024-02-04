using System.ComponentModel.DataAnnotations;

namespace R.Systems.StressTester.Cli.Validation;

[AttributeUsageAttribute(AttributeTargets.Parameter)]
internal class DirectoryPathExistsAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is string path && Directory.Exists(path))
        {
            return ValidationResult.Success;
        }

        return new ValidationResult($"Directory '{value}' doesn't exist");
    }
}
