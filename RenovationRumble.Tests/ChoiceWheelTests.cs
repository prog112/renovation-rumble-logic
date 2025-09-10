namespace RenovationRumble.Tests
{
    using Logic.Runtime.Wheel;

    public sealed class ChoiceWheelTests
    {
        [Fact]
        public void Ctor_SeedsQueue()
        {
            var wheel = new ChoiceWheel([10, 11, 12, 13]);

            Assert.Equal(4, wheel.TotalRemaining);
            Assert.False(wheel.IsEmpty);

            var choices = wheel.Choices;
            Assert.Equal(ChoiceWheel.WindowSize, choices.Length);

            AssertWindowEquals(wheel, 10, 11, 12);
        }

        [Fact]
        public void Ctor_FewerThanWindow_PadsWithNulls()
        {
            var wheel = new ChoiceWheel([7]);

            Assert.Equal(1, wheel.TotalRemaining);

            var choices = wheel.Choices;
            Assert.Equal((ushort?)7, choices[0]);
            Assert.Null(choices[1]);
            Assert.Null(choices[2]);
        }

        [Fact]
        public void Peek_ValidAndInvalid()
        {
            var wheel = new ChoiceWheel([1, 2]);

            Assert.True(wheel.Peek(0, out var p0));
            Assert.Equal((ushort)1, p0);

            Assert.True(wheel.Peek(1, out var p1));
            Assert.Equal((ushort)2, p1);

            Assert.False(wheel.Peek(2, out _)); // Beyond count (even though within WindowSize)
            Assert.False(wheel.Peek(-1, out _));
        }

        [Fact]
        public void CanTake_Bounds()
        {
            var wheel = new ChoiceWheel([1, 2]);

            Assert.True(wheel.CanTake(0));
            Assert.True(wheel.CanTake(1));
            Assert.False(wheel.CanTake(2)); // Index exists in window but not in queue
            Assert.False(wheel.CanTake(-1));
        }

        [Fact]
        public void Take_RemovesFromQueue_AndAdvancesHead()
        {
            var wheel = new ChoiceWheel([1, 2, 3, 4, 5]);

            var first = wheel.Take(0);
            Assert.Equal((ushort)1, first);
            Assert.Equal(4, wheel.TotalRemaining);
            AssertWindowEquals(wheel, 2, 3, 4);

            var second = wheel.Take(2);
            Assert.Equal((ushort)4, second);
            Assert.Equal(3, wheel.TotalRemaining);

            AssertWindowEquals(wheel, 5, 2, 3);
        }

        [Fact]
        public void Take_LastElement_WrapsHeadAndEmpties()
        {
            var wheel = new ChoiceWheel([42]);

            var id = wheel.Take(0);
            Assert.Equal((ushort)42, id);
            Assert.True(wheel.IsEmpty);
            Assert.Equal(0, wheel.TotalRemaining);

            var choices = wheel.Choices;
            Assert.Null(choices[0]);
            Assert.Null(choices[1]);
            Assert.Null(choices[2]);
        }

        [Fact]
        public void Take_OutOfRange_Throws()
        {
            var wheel = new ChoiceWheel([1, 2, 3]);

            Assert.Throws<ArgumentOutOfRangeException>(() => wheel.Take(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => wheel.Take(ChoiceWheel.WindowSize));
        }

        [Fact]
        public void Take_NoPieceAtIndex_Throws()
        {
            var wheel = new ChoiceWheel([1]); // Window becomes [1, null, null]

            Assert.Throws<InvalidOperationException>(() => wheel.Take(1));
            Assert.Throws<InvalidOperationException>(() => wheel.Take(2));
        }

        [Fact]
        public void Enqueue_UpdatesWindow()
        {
            var wheel = new ChoiceWheel([5]);

            AssertWindowEquals(wheel, 5, null, null);

            wheel.Enqueue(6);
            AssertWindowEquals(wheel, 5, 6, null);

            wheel.Enqueue([7, 8]);
            AssertWindowEquals(wheel, 5, 6, 7);

            _ = wheel.Take(0); // Remove 5
            AssertWindowEquals(wheel, 6, 7, 8);

            wheel.Enqueue(9);
            AssertWindowEquals(wheel, 6, 7, 8); // Still first three
            
            _ = wheel.Take(0); // Remove 6
            AssertWindowEquals(wheel, 7, 8, 9);
        }

        [Fact]
        public void Sequence_DeterministicAcrossEquivalentOperations()
        {
            var wheel = new ChoiceWheel([10, 20, 30, 40]);

            var a = wheel.Take(0);
            var b = wheel.Take(0);
            var c = wheel.Take(0);
            var d = wheel.Take(0);

            Assert.Equal((ushort)10, a);
            Assert.Equal((ushort)20, b);
            Assert.Equal((ushort)30, c);
            Assert.Equal((ushort)40, d);
            Assert.True(wheel.IsEmpty);
        }

        private static void AssertWindowEquals(IReadOnlyChoiceWheel window, ushort? a, ushort? b, ushort? c)
        {
            var choices = window.Choices;
            Assert.Equal(a, choices[0]);
            Assert.Equal(b, choices[1]);
            Assert.Equal(c, choices[2]);
        }
    }
}