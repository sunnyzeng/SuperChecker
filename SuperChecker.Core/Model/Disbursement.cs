using System;
using Newtonsoft.Json;

namespace SuperChecker.Core.Model
{
    public class Disbursement
    {
        [JsonProperty("sgc_amount")]
        public decimal Amount { get; set; }

        [JsonProperty("payment_made")]
        public DateTime PaymentMadeDateTime { get; set; }

        [JsonProperty("pay_period_from")]
        public DateTime PayPeriodFrom { get; set; }

        [JsonProperty("pay_period_to")]
        public DateTime PayPeriodTo { get; set; }

        [JsonProperty("employee_code")]
        public int EmployeeCode { get; set; }
    }
}
