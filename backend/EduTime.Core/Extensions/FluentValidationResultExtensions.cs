
using DigitalSkynet.DotnetCore.DataStructures.Exceptions.Api;
using FluentValidation.Results;

namespace EduTime.Core.Extensions
{
    public static class FluentValidationResultExtensions
    {
        public static DigitalSkynet.DotnetCore.DataStructures.Validation.ValidationResult ToDsValidationResult(this ValidationResult validationResult)
        {
            var result = new DigitalSkynet.DotnetCore.DataStructures.Validation.ValidationResult();

            foreach (var issue in validationResult.Errors)
            {
                result.AddError(issue.ErrorMessage);
            }

            return result;
        }

        public static void ThrowIfNotValid(this ValidationResult validationResult)
        {
            if (!validationResult.IsValid)
                throw new ApiValidationException(validationResult.ToDsValidationResult());
        }
    }
}
