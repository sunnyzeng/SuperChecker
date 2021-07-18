using System;
using System.Collections.Generic;
using ExcelDataReader;
using SuperChecker.Core.Model;
using System.Data;
using System.IO;

namespace SuperChecker.Data
{
    public class SuperDataReader : ISuperDataReader
    {
        public SuperDataReader()
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        }

        public SuperCheckerRequest GetSuperCheckerRequest(string filePath)
        {
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                // Auto-detect format, supports:
                //  - Binary Excel files (2.0-2003 format; *.xls)
                //  - OpenXml Excel files (2007 format; *.xlsx, *.xlsb)
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    // 2. Use the AsDataSet extension method
                    var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = true
                        }
                    });

                    DataTable disbursementTable = result.Tables["Disbursements"];
                    DataTable payslipTable = result.Tables["Payslips"];
                    DataTable payCodeTable = result.Tables["PayCodes"];

                    return new SuperCheckerRequest()
                    {
                        Disbursements = MapDisbursements(disbursementTable),
                        PayCodes = MapPayCodes(payCodeTable),
                        Payslips = MapPayslips(payslipTable),
                    };
                }
            }
        }

        private IEnumerable<Disbursement> MapDisbursements(DataTable dataTable)
        {
            var disbursements = new List<Disbursement>();

            var disbursementDataRows = dataTable.AsEnumerable();
            foreach (var dataRow in disbursementDataRows)
            {
                disbursements.Add(new Disbursement()
                {
                    Amount = (decimal) dataRow.Field<double>("sgc_amount"),
                    EmployeeCode = (int)dataRow.Field<double>("employee_code"),
                    PaymentMadeDateTime = DateTime.Parse(dataRow.Field<string>("payment_made")),
                    PayPeriodFrom = DateTime.Parse(dataRow.Field<string>("pay_period_from")),
                    PayPeriodTo = DateTime.Parse(dataRow.Field<string>("pay_period_to")),
                });
            }

            return disbursements;
        }

        private IEnumerable<Payslip> MapPayslips(DataTable dataTable)
        {
            var payslips = new List<Payslip>();

            var payslipDataRows = dataTable.AsEnumerable();
            foreach (var dataRow in payslipDataRows)
            {
                payslips.Add(new Payslip()
                {
                    PayslipId = new Guid(dataRow.Field<string>("payslip_id")),
                    EndDateTime = dataRow.Field<DateTime>("end"),
                    EmployeeCode = (int)dataRow.Field<double>("employee_code"),
                    Code = dataRow.Field<string>("code"),
                    Amount = (decimal)dataRow.Field<double>("amount"),
                });
            }

            return payslips;
        }

        private IEnumerable<PayCode> MapPayCodes(DataTable dataTable)
        {
            var payCodes = new List<PayCode>();

            var payslipDataRows = dataTable.AsEnumerable();
            foreach (var dataRow in payslipDataRows)
            {
                payCodes.Add(new PayCode()
                {
                    Code = dataRow.Field<string>("pay_code"),
                    OteTreament = dataRow.Field<string>("ote_treament"),
                });
            }
            return payCodes;

        }

    }
}
