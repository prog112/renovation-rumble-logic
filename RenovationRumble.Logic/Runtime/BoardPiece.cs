namespace RenovationRumble.Logic.Runtime
{
    using Data;

    public readonly struct BoardPiece
    {
        /// <summary>
        /// Reference to the data model.
        /// </summary>
        public readonly PieceDataModel dataModel;
        
        /// <summary>
        /// Where on the board the piece is.
        /// (0, 0) being the top-left corner.
        /// </summary>
        public readonly Coords coords;
        
        /// <summary>
        /// The rotation of the piece on the board.
        /// </summary>
        public readonly Orientation orientation;
        
        /// <summary>
        /// Rotated to match the piece orientation.
        /// </summary>
        public readonly BitMatrix contents;

        public BoardPiece(PieceDataModel dataModel, Coords coords, Orientation orientation, BitMatrix contents)
        {
            this.dataModel = dataModel;
            this.coords = coords;
            this.orientation = orientation;
            this.contents = contents;
        }
    }
}