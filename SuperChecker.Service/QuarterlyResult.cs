using System.Text;
using SuperChecker.Core.Enum;

namespace SuperChecker.Service
{
    public class QuarterlyResult
    {
        public Quarter Quarter { get; set; }

        public decimal TotalOTE { get; set; }

        public decimal TotalSuperPayable { get; set; }

        public decimal TotalDisbursed { get; set; }

        public decimal Variance => TotalSuperPayable - TotalDisbursed;

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Quarter: {Quarter}");
            sb.AppendLine($"1) Total OTE = ${TotalOTE}");
            sb.AppendLine($"2) Total Super Payable = ${TotalSuperPayable}");
            sb.AppendLine($"3) Total Disbursed = ${TotalDisbursed}");
            sb.AppendLine($"4) Variance = ${Variance}");

            return sb.ToString();
        }
    }
}
