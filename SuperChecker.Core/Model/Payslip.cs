using System;
using Newtonsoft.Json;

namespace SuperChecker.Core.Model
{
    public class Payslip
    {
        [JsonProperty("payslip_id")]
        public Guid PayslipId { get; set; }

        [JsonProperty("end")]
        public DateTime EndDateTime { get; set; }

        [JsonProperty("employee_code")]
        public int EmployeeCode { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }
    }
}
