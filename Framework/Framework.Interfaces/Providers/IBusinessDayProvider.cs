namespace Framework.Interfaces.Providers
{
    using System;

    public interface IBusinessDayProvider
    {
        DateTime AddBusinessDays(DateTime dateToAddTo, int daysToAdd);
    }
}