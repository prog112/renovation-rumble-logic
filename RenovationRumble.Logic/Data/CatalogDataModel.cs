namespace RenovationRumble.Logic.Data
{
    using System;

    /// <summary>
    /// Holds all the data of the given build.
    /// </summary>
    [Serializable]
    public sealed class CatalogDataModel
    {
        public int CatalogVersion { get; set; }
        
        /// <summary>
        /// Array of all available pieces.
        /// </summary>
        public PieceDataModel[] Pieces { get; set; }
    }
}