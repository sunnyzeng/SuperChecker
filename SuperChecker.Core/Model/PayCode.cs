using Newtonsoft.Json;

namespace SuperChecker.Core.Model
{
    public class PayCode
    {
        [JsonProperty("pay_code")]
        public string Code { get; set; }

        [JsonProperty("ote_treament")]
        public string OteTreament { get; set; }
    }
}
