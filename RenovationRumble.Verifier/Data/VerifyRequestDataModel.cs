namespace RenovationRumble.Verifier.Data
{
    using Logic.Data;
    using Logic.Data.Commands;

    public enum VerifyStatus : byte
    {
        Ok = 0,
        InvalidInput,
        SimulationError,
        DidNotEnd,
        ScoreMismatch
    }

    public sealed class VerifyRequestDataModel
    {
        public MatchDataModel Match { get; set; }
        public uint ClaimedScore { get; set; }
        public CommandDataModel[] Commands { get; set; }
    }
}
