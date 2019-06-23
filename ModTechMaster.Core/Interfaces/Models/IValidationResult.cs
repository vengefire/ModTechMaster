namespace ModTechMaster.Core.Interfaces.Models
{
    using System.Collections.Generic;

    using ModTechMaster.Core.Enums;

    public interface IValidationResult
    {
        ValidationResultEnum Result { get; }

        List<IValidationResultReason> ValidationResultReasons { get; }
    }
}