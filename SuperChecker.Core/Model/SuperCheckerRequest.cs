using System.Collections.Generic;

namespace SuperChecker.Core.Model
{
    public class SuperCheckerRequest
    {
        public IEnumerable<Disbursement> Disbursements { get; set; }
        public IEnumerable<Payslip> Payslips { get; set; }
        public IEnumerable<PayCode> PayCodes { get; set; }
    }
}
