namespace RenovationRumble.Logic.Runtime.Wheel
{
    using System;
    using System.Collections.Generic;
    using Utility.Collections;

    public interface IReadOnlyChoiceWheel
    {
        ReadOnlyArray<ushort?> Choices { get; }

        int TotalRemaining { get; }
        bool IsEmpty { get; }

        bool CanTake(int index);
        bool Peek(int index, out ushort id);
    }

    /// <summary>
    /// Rotating choice window over a queue of pieces. You can take one of the next N items.
    /// Maps to pieces Unique Id.
    /// </summary>
    public sealed class ChoiceWheel : IReadOnlyChoiceWheel
    {
        /// <summary>
        /// Current visible window. Nulls can appear if the visible portion has fewer than <see cref="WindowSize"/> elements.
        /// </summary>
        public ReadOnlyArray<ushort?> Choices => visible.AsReadOnlyArray();

        public int TotalRemaining => queue.Count;
        public bool IsEmpty => queue.Count == 0;

        private readonly ushort?[] visible = new ushort?[WindowSize];
        private readonly List<ushort> queue;

        public const int WindowSize = 3;

        public ChoiceWheel(ushort[] startingPieces)
        {
            queue = new List<ushort>(startingPieces);
            RefreshWindow();
        }

        public bool CanTake(int index)
        {
            return index >= 0 && index < WindowSize && queue.Count > index;
        }

        public bool Peek(int index, out ushort id)
        {
            if (!CanTake(index))
            {
                id = default;
                return false;
            }

            id = queue[index];
            return true;
        }

        /// <summary>
        /// Takes a piece by index in the current window.
        /// Throws an exception if index is out of range or there is nothing to take.
        /// </summary>
        public ushort Take(int index)
        {
            if ((uint)index >= WindowSize)
                throw new ArgumentOutOfRangeException(nameof(index), "Index exceeds the window size.");

            if (queue.Count <= index)
                throw new InvalidOperationException("No piece available at that index.");

            var id = queue[index];
            queue.RemoveAt(index);
           
            RefreshWindow();
            return id;
        }

        public void Enqueue(ushort piece)
        {
            queue.Add(piece);
            RefreshWindow();
        }

        public void Enqueue(ReadOnlySpan<ushort> pieces)
        {
            foreach (var piece in pieces)
                queue.Add(piece);

            RefreshWindow();
        }

        private void RefreshWindow()
        {
            for (int i = 0; i < WindowSize; i++)
                visible[i] = i < queue.Count ? queue[i] : (ushort?)null;
        }
    }
}