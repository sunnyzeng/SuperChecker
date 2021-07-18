using System;
using SuperChecker.Core.Enum;

namespace SuperChecker.Core
{
    public static class ExtensionMethod
    {

        public static Quarter GetQuarter(this DateTime dateTime)
        {
            if (dateTime.Month >= 1 && dateTime.Month <= 3)
                return Quarter.JanToMarch;
            else if (dateTime.Month >= 4 && dateTime.Month <= 6)
                return Quarter.AprilToJune;
            else if (dateTime.Month >= 7 && dateTime.Month <= 9)
                return Quarter.JulyToSept;
            else
                return Quarter.OctToDec;
        }
        public static Tuple<DateTime, DateTime> GetQuarterStartEndDateTuple(this DateTime dateTime)
        {
            var quarterStartDate = new DateTime(dateTime.Year, ((int)GetQuarter(dateTime) - 1)*3 + 1, 1);
            var quarterEndDate = quarterStartDate.AddMonths(3).AddDays(-1);

            return new Tuple<DateTime, DateTime>(quarterStartDate, quarterEndDate);
        }

        public static Tuple<DateTime, DateTime> GetDisbursementDateRangeTuple(this DateTime dateTime, int cutoffDays)
        {
            var quarterStartEndDate = dateTime.GetQuarterStartEndDateTuple();

            return new Tuple<DateTime, DateTime>(quarterStartEndDate.Item1.AddDays(cutoffDays), quarterStartEndDate.Item2.AddDays(cutoffDays));
        }
        
    }
}
