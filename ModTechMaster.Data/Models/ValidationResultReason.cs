namespace ModTechMaster.Data.Models
{
    using ModTechMaster.Core.Interfaces.Models;

    public class ValidationResultReason : IValidationResultReason
    {
        public IObject FailingObject { get; }

        public string FailureReason { get; }
    }
}