using SuperChecker.Core;
using SuperChecker.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SuperChecker.Service
{
    public class SuperCheckerService : ISuperCheckerService
    {
        private ServiceOptions _serviceOptions;

        public IEnumerable<Disbursement> Disbursements { get; set; }

        public IEnumerable<Payslip> Payslips { get; set; }

        public IEnumerable<PayCode> PayCodes { get; set; }

        private IEnumerable<string> OTEPayCodes
        {
            get
            {
                return PayCodes?.Where(pc => pc.OteTreament.Equals("OTE", StringComparison.InvariantCultureIgnoreCase)).Select(pc=>pc.Code);
            }
        }

        public SuperCheckerService(IEnumerable<Disbursement> disbursements, IEnumerable<Payslip> payslips,
            IEnumerable<PayCode> payCodes) : this()
        {
            Disbursements = disbursements;
            Payslips = payslips;
            PayCodes = payCodes;
        }

        public SuperCheckerService()
        {
            _serviceOptions = new ServiceOptions();
        }

        /// <summary>
        /// Main logic for getting the result sets
        /// </summary>
        public IEnumerable<EmployeeYearlyResult> GetCheckResults()
        {
            var results = new List<EmployeeYearlyResult>();

            var disbursementsGroupedByEmployee = Disbursements.GroupBy(disbursement => disbursement.EmployeeCode)
                .ToDictionary(d => d.Key, d => d.ToList());

            var payslipsGroupedByEmployee = Payslips.GroupBy(p => p.EmployeeCode)
                .ToDictionary(d => d.Key, d => d.ToList());

            if (payslipsGroupedByEmployee.Any())
            {
                foreach (var employeePayslips in payslipsGroupedByEmployee)
                {
                    if (disbursementsGroupedByEmployee.ContainsKey(employeePayslips.Key))
                    {
                        results.AddRange(GetEmployeeYearlyResults(employeePayslips.Key, disbursementsGroupedByEmployee[employeePayslips.Key], payslipsGroupedByEmployee[employeePayslips.Key]));
                    }
                    else
                        results.AddRange(GetEmployeeYearlyResults(employeePayslips.Key, null, payslipsGroupedByEmployee[employeePayslips.Key]));
                }
            }


            return results.OrderBy(r=>r.EmployeeCode).ThenBy(r=>r.Year);
        }

        public void SetServiceOptions(ServiceOptions options)
        {
            _serviceOptions = options;
        }

        public void SetSuperCheckerRequest(SuperCheckerRequest request)
        {
            Disbursements = request.Disbursements;
            Payslips = request.Payslips;
            PayCodes = request.PayCodes;
        }

        private IEnumerable<EmployeeYearlyResult> GetEmployeeYearlyResults(int employeeCode, IEnumerable<Disbursement> disbursements,
            IEnumerable<Payslip> payslips)
        {
            var employeeResults = new List<EmployeeYearlyResult>();

            var payslipsGroupedByYear = payslips.GroupBy(ps => ps.EndDateTime.Year);

            foreach (var yearlyPayslips in payslipsGroupedByYear)
            {
                var result = new EmployeeYearlyResult() { EmployeeCode = employeeCode, Year = yearlyPayslips.Key};

                result.QuarterlyResults = GetEmployeeQuarterlyResults(disbursements, yearlyPayslips.ToList());

                employeeResults.Add(result);
            }

            return employeeResults;
        }

        private IEnumerable<QuarterlyResult> GetEmployeeQuarterlyResults(IEnumerable<Disbursement> disbursements,
            IEnumerable<Payslip> payslips)
        {
            var quarterlyResults = new List<QuarterlyResult>();

            var payslipsByQuarter = payslips.GroupBy(ps => ps.EndDateTime.GetQuarter());

            foreach (var quarterlyPayslips in payslipsByQuarter)
            {
                var otePayslips = quarterlyPayslips.ToList().Where(p => OTEPayCodes.Contains(p.Code));

                var totalOTEAmount = otePayslips.Sum(p => p.Amount);
                var totalSuperPayable = totalOTEAmount * _serviceOptions.SuperannuationRate;
                var totalDisbursed = GetValidDisbursementsForPayslips(disbursements, otePayslips).Sum(s => s.Amount);

                quarterlyResults.Add(new QuarterlyResult()
                {
                    Quarter = quarterlyPayslips.Key,
                    TotalOTE = totalOTEAmount,
                    TotalSuperPayable = totalSuperPayable,
                    TotalDisbursed = totalDisbursed,
                });
            }

            return quarterlyResults;
        }

        /// <summary>
        /// Gets the disbursements for the current payslip quarter which is within the cutoff days
        /// </summary>
        private IEnumerable<Disbursement> GetValidDisbursementsForPayslips(IEnumerable<Disbursement> disbursements, IEnumerable<Payslip> payslips)
        {
            if (!payslips.Any() || disbursements == null || !disbursements.Any())
                return new List<Disbursement>();

            var payslip = payslips.FirstOrDefault();
            var validDisbursementDateTuple = payslip.EndDateTime.GetDisbursementDateRangeTuple(_serviceOptions.DisbursementCutoffDays);
            var quarterStartEndDateTuple = payslip.EndDateTime.GetQuarterStartEndDateTuple();

            return disbursements.Where(disbursement =>
                disbursement.EmployeeCode == payslip.EmployeeCode &&
                disbursement.PayPeriodFrom >= quarterStartEndDateTuple.Item1 &&
                disbursement.PayPeriodTo <= quarterStartEndDateTuple.Item2 &&
                disbursement.PaymentMadeDateTime <= validDisbursementDateTuple.Item2 &&
                disbursement.PaymentMadeDateTime >= validDisbursementDateTuple.Item1);

        }


    }
}
