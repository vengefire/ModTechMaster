namespace ModTechMaster.Data.Models
{
    using ModTechMaster.Core.Interfaces.Models;

    public class ValidationResultReason : IValidationResultReason
    {
        public ValidationResultReason(IObject failingObject, string failureReason)
        {
            this.FailingObject = failingObject;
            this.FailureReason = failureReason;
        }

        public IObject FailingObject { get; }

        public string FailureReason { get; }
    }
}