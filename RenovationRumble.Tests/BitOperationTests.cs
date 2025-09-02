namespace RenovationRumble.Tests
{
    using Logic.Primitives;
    using Xunit;

    public class BitOperationsTests
    {
        [Fact]
        public void TrailingZeroCount_Zero_Returns64()
        {
            Assert.Equal(64, BitOperations.TrailingZeroCount(0UL));
        }

        [Theory]
        [InlineData(0UL, 64)]
        [InlineData(1UL << 0, 0)]
        [InlineData(1UL << 1, 1)]
        [InlineData(1UL << 2, 2)]
        [InlineData(1UL << 7, 7)]
        [InlineData(1UL << 15, 15)]
        [InlineData(1UL << 31, 31)]
        [InlineData(1UL << 32, 32)]
        [InlineData(1UL << 48, 48)]
        [InlineData(1UL << 63, 63)]
        public void TrailingZeroCount_SingleBit_ReturnsIndex(ulong v, int expected)
        {
            Assert.Equal(expected, BitOperations.TrailingZeroCount(v));
        }

        [Fact]
        public void TrailingZeroCount_AllPositions_AreCorrect()
        {
            for (int i = 0; i < 64; i++)
            {
                var v = 1UL << i;
                var count = BitOperations.TrailingZeroCount(v);
                Assert.Equal(i, count);
            }
        }

        [Theory]
        [InlineData(0b_1010UL, 1)] // lowest set bit at index 1
        [InlineData(0b_1000UL, 3)]
        [InlineData(0xF0F0F0F0F0F0F0F0UL, 4)]
        [InlineData(0x8000_0000_0000_0001UL, 0)]
        [InlineData(0x8000_0000_0000_0000UL, 63)]
        public void TrailingZeroCount_MixedBits_MatchesNaive(ulong v, int expected)
        {
            Assert.Equal(expected, BitOperations.TrailingZeroCount(v));
        }

        [Fact]
        public void PopCount_Zero_Returns0()
        {
            Assert.Equal(0, BitOperations.PopCount(0UL));
        }

        [Fact]
        public void PopCount_AllOnes_Returns64()
        {
            Assert.Equal(64, BitOperations.PopCount(ulong.MaxValue));
        }

        [Theory]
        [InlineData(1UL << 0, 1)]
        [InlineData(1UL << 7, 1)]
        [InlineData(1UL << 31, 1)]
        [InlineData(1UL << 63, 1)]
        [InlineData(0b_1111UL, 4)]
        [InlineData(0b_1010UL, 2)]
        [InlineData(0xF0F0F0F0F0F0F0F0UL, 32)]
        [InlineData(0x0123_4567_89AB_CDEFUL, 32)]
        public void PopCount_KnownValues_AreCorrect(ulong v, int expected)
        {
            Assert.Equal(expected, BitOperations.PopCount(v));
        }

        [Fact]
        public void PopCount_EqualsNaive_ClearLowestSetBit()
        {
            // Deterministically pick a spread of values
            ulong[] samples =
            [
                0UL,
                1UL,
                2UL,
                3UL,
                0xFFFFUL,
                0xFFFF_FFFFUL,
                0x0123_4567_89AB_CDEFUL,
                0xF0F0_F0F0_F0F0_F0F0UL,
                0x8000_0000_0000_0000UL,
                ulong.MaxValue
            ];

            foreach (var v in samples)
            {
                Assert.Equal(NaivePopCount(v), BitOperations.PopCount(v));
            }
        }

        [Fact]
        public void PopCount_PowersOfTwo_AreOne()
        {
            for (int i = 0; i < 64; i++)
            {
                var v = 1UL << i;
                Assert.Equal(1, BitOperations.PopCount(v));
            }
        }

        [Fact]
        public void TrailingZeroCount_MatchesIndexOfIsolatedBit()
        {
            ulong[] samples =
            {
                0x01UL, 0x10UL, 0x80UL, 0x1000UL,
                0x0123_4567_89AB_CDEFUL,
                0x8000_0000_0000_0001UL,
                0x00F0_0000_0000_00F0UL
            };

            foreach (var v in samples)
            {
                if (v == 0)
                    continue;
                var isolated = v & (ulong)-(long)v;
                Assert.Equal(BitOperations.TrailingZeroCount(v), BitOperations.TrailingZeroCount(isolated));
            }
        }

        private static int NaivePopCount(ulong v)
        {
            // Kernighan's algorithm
            var count = 0;
            while (v != 0)
            {
                v &= v - 1;
                count++;
            }

            return count;
        }
    }
}