namespace RenovationRumble.Logic.Data
{
    using System;
    using Primitives;

    [Serializable]
    public sealed class PieceDataModel
    {
        /// <summary>
        /// USO identifier.
        /// </summary>
        public ushort PieceId { get; set; }
        
        /// <summary>
        /// USO identifier.
        /// </summary>
        public ushort StyleId { get; set; }
        
        /// <summary>
        /// Holds width, height, and info whether a given cell is filled at the default orientation.
        /// </summary>
        public BitMatrix DefaultContents { get; set; }
    }
}