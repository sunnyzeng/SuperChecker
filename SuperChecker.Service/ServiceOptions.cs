namespace SuperChecker.Service
{
    public class ServiceOptions
    {
        public decimal SuperannuationRate { get; set; } = 0.095M;
        public int DisbursementCutoffDays { get; set; } = 28;
    }
}
