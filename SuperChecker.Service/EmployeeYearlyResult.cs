using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperChecker.Service
{
    public class EmployeeYearlyResult
    {
        public int EmployeeCode { get; set; }

        public int Year { get; set; }

        public IEnumerable<QuarterlyResult> QuarterlyResults { get; set; }

        public string ToString()
        {

            var sb = new StringBuilder();

            sb.AppendLine($"Employee: {EmployeeCode}; Year: {Year}");

            if (QuarterlyResults.Any())
                foreach (var quarterlyResult in QuarterlyResults.OrderBy(qr=>qr.Quarter))
                {
                    sb.AppendLine(quarterlyResult.ToString());
                }

            return sb.ToString();
        }
    }
}
