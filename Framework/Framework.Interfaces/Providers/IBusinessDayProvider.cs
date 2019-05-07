using System;

namespace Framework.Interfaces.Providers
{
    public interface IBusinessDayProvider
    {
        DateTime AddBusinessDays(DateTime dateToAddTo, int daysToAdd);
    }
}