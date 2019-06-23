namespace ModTechMaster.Core.Interfaces.Models
{
    public interface IValidationResultReason
    {
        IObject FailingObject { get; }

        string FailureReason { get; }
    }
}