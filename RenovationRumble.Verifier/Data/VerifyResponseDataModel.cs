namespace RenovationRumble.Verifier.Data
{
    public sealed class VerifyResponseDataModel
    {
        public VerifyStatus Status { get; set; }
        public string Message { get; set; }
        public uint ComputedScore { get; set; }
        public string EndReason { get; set; }
    }
}