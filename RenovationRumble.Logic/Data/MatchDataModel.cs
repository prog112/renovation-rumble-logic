namespace RenovationRumble.Logic.Data
{
    using System;

    /// <summary>
    /// Holds all the data of the given gameplay session.
    /// </summary>
    [Serializable]
    public sealed class MatchDataModel
    {
        public byte BoardWidth { get; set; }
        public byte BoardHeight { get; set; }
    }
}