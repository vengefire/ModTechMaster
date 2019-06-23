namespace ModTechMaster.Data.Models
{
    using System.Collections.Generic;
    using System.Linq;

    using ModTechMaster.Core.Enums;
    using ModTechMaster.Core.Interfaces.Models;

    public class ValidationResult : IValidationResult
    {
        public ValidationResultEnum Result { get; set; }

        public List<IValidationResultReason> ValidationResultReasons { get; } = new List<IValidationResultReason>();

        public static ValidationResult AggregateResults(IEnumerable<IValidationResult> results)
        {
            var result = new ValidationResult();
            results.ToList().ForEach(validationResult => result.ValidationResultReasons.AddRange(validationResult.ValidationResultReasons));
            result.Result = result.ValidationResultReasons.Any()
                                ? ValidationResultEnum.Failure
                                : ValidationResultEnum.Success;
            return result;
        }

        public static ValidationResult SuccessValidationResult()
        {
            return new ValidationResult()
                       {
                           Result = ValidationResultEnum.Success
                       };
        }
    }
}