namespace RenovationRumble.Logic.Runtime.Catalog
{
    using System;
    using Board;
    using Data;
    using Utility.Collections;

    public sealed class GameData
    {
        public ReadOnlyArray<PieceDataModel> Pieces => catalog.Pieces.AsReadOnlyArray();
        public RotationCache RotationCache { get; }

        private readonly CatalogDataModel catalog;
        
        public GameData(CatalogDataModel catalog)
        {
            this.catalog = catalog;
            
            RotationCache = new RotationCache();
        }

        public PieceDataModel GetPiece(ushort pieceId)
        {
            foreach (var piece in catalog.Pieces)
            {
                if (piece.PieceId == pieceId)
                    return piece;
            }

            throw new ArgumentException($"Piece {pieceId} not found!");
        }
    }
}