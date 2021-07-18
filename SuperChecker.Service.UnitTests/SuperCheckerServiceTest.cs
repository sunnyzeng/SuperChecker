using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SuperChecker.Core;
using SuperChecker.Core.Enum;
using SuperChecker.Core.Model;

namespace SuperChecker.Service.UnitTests
{
    [TestClass]
    public class SuperCheckerServiceTest
    {
        private SuperCheckerService _service;
        private DateTime _fixedTestDateTime = new DateTime(2021, 7, 1);

        [TestInitialize]
        public void SuperCheckerServiceTestInitialize()
        {
            _service = new SuperCheckerService();
        }

        [TestMethod]
        public void SuperCheckerServiceTest_CheckSinglePayslipAndDisbursementOnSameDay()
        {
            // arrange
            var payslips = new List<Payslip>() { GetPayslip(_fixedTestDateTime, 1, 1000, "Salary")};
            var paycodes = new List<PayCode>() { GetPayCode("Salary", "OTE")};
            var disbursement = new List<Disbursement>() {GetDisbursement(_fixedTestDateTime, 1, 95)};
            SetServiceCollections(_service, disbursement, payslips, paycodes);

            // act
            var results = _service.GetCheckResults();

            // assert
            Assert.AreEqual(1, results.Count());
            var checkResult = results.FirstOrDefault().QuarterlyResults.FirstOrDefault();
            Assert.AreEqual(1000, checkResult.TotalOTE);
            Assert.AreEqual(Quarter.JulyToSept, checkResult.Quarter);
            Assert.AreEqual(95, checkResult.TotalSuperPayable);
            Assert.AreEqual(0, checkResult.TotalDisbursed); // disbursement need to be paid 28 days after the 1st of the quarter
            Assert.AreEqual(95, checkResult.Variance);
        }

        [TestMethod]
        public void SuperCheckerServiceTest_CheckSinglePayslipWithNoDisbursement()
        {
            // arrange
            var payslips = new List<Payslip>() { GetPayslip(_fixedTestDateTime, 1, 1000, "Salary") };
            var paycodes = new List<PayCode>() { GetPayCode("Salary", "OTE") };
            var disbursement = new List<Disbursement>() {};
            SetServiceCollections(_service, disbursement, payslips, paycodes);

            // act
            var results = _service.GetCheckResults();

            // assert
            Assert.AreEqual(1, results.Count());
            var checkResult = results.FirstOrDefault().QuarterlyResults.FirstOrDefault();
            Assert.AreEqual(1000, checkResult.TotalOTE);
            Assert.AreEqual(Quarter.JulyToSept, checkResult.Quarter);
            Assert.AreEqual(95, checkResult.TotalSuperPayable);
            Assert.AreEqual(0, checkResult.TotalDisbursed);
            Assert.AreEqual(95, checkResult.Variance);
        }

        [TestMethod]
        public void SuperCheckerServiceTest_CheckSinglePayslipWithDelayedDisbursement()
        {
            // arrange
            var payslips = new List<Payslip>() { GetPayslip(_fixedTestDateTime, 1, 1000, "Salary") };
            var paycodes = new List<PayCode>() { GetPayCode("Salary", "OTE") };
            var disbursement = new List<Disbursement>() { GetDisbursement(_fixedTestDateTime.AddMonths(3).AddDays(30), 1, 95) };
            SetServiceCollections(_service, disbursement, payslips, paycodes);

            // act
            var results = _service.GetCheckResults();

            // assert
            Assert.AreEqual(1, results.Count());
            var checkResult = results.FirstOrDefault().QuarterlyResults.FirstOrDefault();
            Assert.AreEqual(1000, checkResult.TotalOTE);
            Assert.AreEqual(Quarter.JulyToSept, checkResult.Quarter);
            Assert.AreEqual(95, checkResult.TotalSuperPayable);
            Assert.AreEqual(0, checkResult.TotalDisbursed);
            Assert.AreEqual(95, checkResult.Variance);
        }

        [TestMethod]
        public void SuperCheckerServiceTest_CheckMixedPayslipWithDisbursement()
        {
            // arrange
            var payslips = new List<Payslip>()
            {
                GetPayslip(_fixedTestDateTime, 1, 1000, "Salary"),
                GetPayslip(_fixedTestDateTime, 1, 500, "Annual Leave"),
                GetPayslip(_fixedTestDateTime, 1, 235, "Random Payment"),
            };
            var paycodes = new List<PayCode>()
            {
                GetPayCode("Salary", "OTE"),
                GetPayCode("Annual Leave", "OTE"),
                GetPayCode("Random Payment", "NON OTE"),
            };
            var disbursement = new List<Disbursement>() { GetDisbursement(_fixedTestDateTime.AddDays(28), 1, 95) };
            SetServiceCollections(_service, disbursement, payslips, paycodes);

            // act
            var results = _service.GetCheckResults();

            // assert
            Assert.AreEqual(1, results.Count());
            var checkResult = results.FirstOrDefault().QuarterlyResults.FirstOrDefault();
            Assert.AreEqual(1500, checkResult.TotalOTE);
            Assert.AreEqual(Quarter.JulyToSept, checkResult.Quarter);
            Assert.AreEqual(142.5M, checkResult.TotalSuperPayable);
            Assert.AreEqual(47.5M, checkResult.Variance);
        }

        [TestMethod]
        public void SuperCheckerServiceTest_CheckExampleResult()
        {
            // arrange
            var payslips = new List<Payslip>()
            {
                GetPayslip(new DateTime(2021,1,1), 1, 5000, "Salary"),
                GetPayslip(new DateTime(2021,1,1), 1, 1500, "Overtime - Weekend"),
                GetPayslip(new DateTime(2021,1,1), 1, 475, "Super Withheld"),

                GetPayslip(new DateTime(2021,2,1), 1, 5000, "Salary"),
                GetPayslip(new DateTime(2021,2,1), 1, 475, "Super Withheld"),

                GetPayslip(new DateTime(2021,3,1), 1, 5000, "Salary"),
                GetPayslip(new DateTime(2021,3,1), 1, 475, "Super Withheld"),
            };
            var paycodes = new List<PayCode>()
            {
                GetPayCode("Salary", "OTE"),
                GetPayCode("Site Allowance", "OTE"),
                GetPayCode("Overtime - Weekend", "Not OTE"),
                GetPayCode("Super Withheld", "Not OTE"),
            };
            var disbursement = new List<Disbursement>()
            {
                GetDisbursement(new DateTime(2021,2,27), 1, 500, new DateTime(2021,1,1)),
                GetDisbursement(new DateTime(2021,3,30), 1, 500, new DateTime(2021,1,1)),
                GetDisbursement(new DateTime(2021,4,30), 1, 500, new DateTime(2021,1,1)),
            };
            SetServiceCollections(_service, disbursement, payslips, paycodes);

            // act
            var results = _service.GetCheckResults();

            // assert
            Assert.AreEqual(1, results.Count());
            var checkResult = results.FirstOrDefault().QuarterlyResults.FirstOrDefault();
            Assert.AreEqual(15000, checkResult.TotalOTE);
            Assert.AreEqual(Quarter.JanToMarch, checkResult.Quarter);
            Assert.AreEqual(1425, checkResult.TotalSuperPayable);
            Assert.AreEqual(1000M, checkResult.TotalDisbursed);
            Assert.AreEqual(425M, checkResult.Variance);
        }

        [TestMethod]
        public void SuperCheckerServiceTest_CheckExampleResult_WithMultipleEmployees()
        {
            // arrange
            var payslips = new List<Payslip>()
            {
                // employee 1
                GetPayslip(new DateTime(2021,1,1), 1, 5000, "Salary"),
                GetPayslip(new DateTime(2021,1,1), 1, 1500, "Overtime - Weekend"),
                GetPayslip(new DateTime(2021,1,1), 1, 475, "Super Withheld"),

                GetPayslip(new DateTime(2021,2,1), 1, 5000, "Salary"),
                GetPayslip(new DateTime(2021,2,1), 1, 475, "Super Withheld"),

                GetPayslip(new DateTime(2021,3,1), 1, 5000, "Salary"),
                GetPayslip(new DateTime(2021,3,1), 1, 475, "Super Withheld"),

                // employee 2
                GetPayslip(new DateTime(2021,3,1), 2, 6000, "Salary"),
                GetPayslip(new DateTime(2021,3,1), 2, 475, "Super Withheld"),
            };
            var paycodes = new List<PayCode>()
            {
                GetPayCode("Salary", "OTE"),
                GetPayCode("Site Allowance", "OTE"),
                GetPayCode("Overtime - Weekend", "Not OTE"),
                GetPayCode("Super Withheld", "Not OTE"),
            };
            var disbursement = new List<Disbursement>()
            {
                GetDisbursement(new DateTime(2021,2,27), 1, 500, new DateTime(2021,1,1)),
                GetDisbursement(new DateTime(2021,3,30), 1, 500, new DateTime(2021,1,1)),
                GetDisbursement(new DateTime(2021,4,30), 1, 500, new DateTime(2021,1,1)),

                GetDisbursement(new DateTime(2021,4,25), 2, 500, new DateTime(2021,1,1)),

            };
            SetServiceCollections(_service, disbursement, payslips, paycodes);

            // act
            var results = _service.GetCheckResults();

            // assert
            Assert.AreEqual(2, results.Count());

            // assert employee 1
            var checkResult = results.FirstOrDefault(r=>r.EmployeeCode == 1).QuarterlyResults.FirstOrDefault();
            Assert.AreEqual(15000, checkResult.TotalOTE);
            Assert.AreEqual(Quarter.JanToMarch, checkResult.Quarter);
            Assert.AreEqual(1425, checkResult.TotalSuperPayable);
            Assert.AreEqual(1000M, checkResult.TotalDisbursed);
            Assert.AreEqual(425M, checkResult.Variance);

            // assert employee 2
            var employeeResult2 = results.FirstOrDefault(r => r.EmployeeCode == 2).QuarterlyResults.FirstOrDefault();
            Assert.AreEqual(6000, employeeResult2.TotalOTE);
            Assert.AreEqual(Quarter.JanToMarch, employeeResult2.Quarter);
            Assert.AreEqual(570, employeeResult2.TotalSuperPayable);
            Assert.AreEqual(500M, employeeResult2.TotalDisbursed);
            Assert.AreEqual(70M, employeeResult2.Variance);
        }


        [TestMethod]
        public void SuperCheckerServiceTest_DateTimeExtensionMethodTests()
        {
            var testDate = _fixedTestDateTime; // 2021-07-01

            var quarterStartEndTuple = testDate.GetQuarterStartEndDateTuple();

            Assert.AreEqual(7, quarterStartEndTuple.Item1.Month);
            Assert.AreEqual(1, quarterStartEndTuple.Item1.Day);
            
        }

        #region Private Methods

        private Payslip GetPayslip(DateTime? endDateTime, int employeeCode = 1, decimal amount = 1000,
            string code = "Salary")
        {
            return new Payslip()
            {
                PayslipId = Guid.NewGuid(),
                EmployeeCode = employeeCode,
                Amount = amount,
                Code = code,
                EndDateTime = endDateTime ?? _fixedTestDateTime,
            };
        }

        private PayCode GetPayCode(string code = "Salary", string ote_treatment = "OTE")
        {
            return new PayCode()
            {
                Code = code,
                OteTreament = ote_treatment,
            };
        }

        private Disbursement GetDisbursement(DateTime? disbursementDateTime, int employeeCode = 1, decimal amount = 95, DateTime? paymentFromToDateTime = null)
        {
            return new Disbursement()
            {
                EmployeeCode = employeeCode,
                Amount = amount,
                PaymentMadeDateTime = disbursementDateTime ?? _fixedTestDateTime,
                PayPeriodFrom = paymentFromToDateTime ?? _fixedTestDateTime,
                PayPeriodTo = paymentFromToDateTime ??_fixedTestDateTime,
            };
        }

        private void SetServiceCollections(SuperCheckerService service, IEnumerable<Disbursement> disbursements, IEnumerable<Payslip> payslips,
            IEnumerable<PayCode> payCodes)
        {
            service.Disbursements = disbursements;
            service.Payslips = payslips;
            service.PayCodes = payCodes;
        }

        #endregion
    }
}
