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

        bool CanTake(int windowIndex);
        bool Peek(int windowIndex, out ushort id);
    }

    /// <summary>
    /// Rotating choice window over a queue of pieces. You can take one of the next <see cref="WindowSize"/> items.
    /// Maps to pieces Unique Id.
    /// </summary>
    public sealed class ChoiceWheel : IReadOnlyChoiceWheel
    {
        /// <summary>
        /// Current visible window. Nulls can appear if the visible portion has fewer than <see cref="WindowSize"/> elements.
        /// </summary>
        public ReadOnlyArray<ushort?> Choices => window.AsReadOnlyArray();

        public int TotalRemaining => queue.Count;
        public bool IsEmpty => queue.Count == 0;

        private readonly ushort?[] window = new ushort?[WindowSize];
        private readonly List<ushort> queue;

        private int headIndex;

        public const int WindowSize = 3;

        public ChoiceWheel(ushort[] startingPieces)
        {
            queue = new List<ushort>(startingPieces);
            RefreshWindow();
        }

        public bool CanTake(int windowIndex)
        {
            return windowIndex >= 0 && windowIndex < WindowSize && queue.Count > windowIndex;
        }

        public bool Peek(int windowIndex, out ushort id)
        {
            if (!CanTake(windowIndex))
            {
                id = default;
                return false;
            }

            id = queue[GetQueueIndex(windowIndex)];
            return true;
        }

        /// <summary>
        /// Takes a piece by index in the current window.
        /// Throws an exception if index is out of range or there is nothing to take.
        /// </summary>
        public ushort Take(int windowIndex)
        {
            if (windowIndex < 0 || windowIndex >= WindowSize)
                throw new ArgumentOutOfRangeException(nameof(windowIndex), $"Index '{windowIndex}' exceeds the window size.");

            if (queue.Count <= windowIndex)
                throw new InvalidOperationException($"Not enough items in the queue to take '{windowIndex}'.");
            
            var removeIndex = GetQueueIndex(windowIndex);

            var id = queue[removeIndex];
            queue.RemoveAt(removeIndex);

            headIndex = queue.Count == 0 ? 0 : removeIndex % queue.Count;
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

        private int GetQueueIndex(int windowIndex)
        {
            return (headIndex + windowIndex) % queue.Count;
        }

        private void RefreshWindow()
        {
            for (int i = 0; i < WindowSize; i++)
                window[i] = i < queue.Count ? queue[(i + headIndex) % queue.Count] : (ushort?)null;
        }
    }
}