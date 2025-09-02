namespace RenovationRumble.Tests
{
    using System;
    using System.Collections.Generic;
    using RenovationRumble.Logic.Primitives;
    using Xunit;

    public sealed class BitMatrixTests
    {
        [Fact]
        public void Ctor_Valid_DoesNotThrow()
        {
            var m = new BitMatrix(3, 2, 0b_111_000);
            Assert.Equal((byte)3, m.w);
            Assert.Equal((byte)2, m.h);
        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(1, 0)]
        public void Ctor_Invalid_Dimension_Throws(byte w, byte h)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new BitMatrix(w, h, 0));
        }

        [Fact]
        public void Ctor_TooManyCells_Throws()
        {
            // 9x8 = 72 > 64
            Assert.Throws<ArgumentOutOfRangeException>(() => new BitMatrix(9, 8, 0));
        }

        [Fact]
        public void Indexer_BoundsChecks()
        {
            var m = new BitMatrix(2, 2, 0b_1111);
            Assert.True(m[0, 0]);
            Assert.True(m[1, 0]);
            Assert.True(m[0, 1]);
            Assert.True(m[1, 1]);

            Assert.Throws<ArgumentOutOfRangeException>(() => _ = m[-1, 0]);
            Assert.Throws<ArgumentOutOfRangeException>(() => _ = m[2, 0]);
            Assert.Throws<ArgumentOutOfRangeException>(() => _ = m[0, -1]);
            Assert.Throws<ArgumentOutOfRangeException>(() => _ = m[0, 2]);
        }

        [Fact]
        public void FilledCells_EnumeratesRowMajorOrder()
        {
            // 3x2:
            // y=0: 1 0 1
            // y=1: 0 1 0
            var m = BitMatrixTestHelpers.Build("101", "010");

            var list = new List<(byte x, byte y)>();
            foreach (var p in m.FilledCells()) 
                list.Add(p);

            Assert.Equal(3, list.Count);
            Assert.Equal(((byte)0, (byte)0), list[0]); // (x=0,y=0)
            Assert.Equal(((byte)2, (byte)0), list[1]); // (x=2,y=0)
            Assert.Equal(((byte)1, (byte)1), list[2]); // (x=1,y=1)
        }

        [Fact]
        public void Rotate90_180_270_LShape()
        {
            // L: 2x3
            var m = BitMatrixTestHelpers.Build("10", "10", "11");

            var m90 = m.Rotate90();
            var m180 = m.Rotate180();
            var m270 = m.Rotate270();

            Assert.Equal(["111", "100"], BitMatrixTestHelpers.ExtractRows(m90)); // 3x2
            Assert.Equal(["11", "01", "01"], BitMatrixTestHelpers.ExtractRows(m180)); // 2x3
            Assert.Equal(["001", "111"], BitMatrixTestHelpers.ExtractRows(m270)); // 3x2
        }

        [Fact]
        public void Rotate_FullSquare_IsStableFor180Twice()
        {
            var m = BitMatrixTestHelpers.Build("11", "11");
            var r = m.Rotate180().Rotate180();
            Assert.Equal(BitMatrixTestHelpers.ExtractRows(m), BitMatrixTestHelpers.ExtractRows(r));
        }

        [Fact]
        public void Rotate90_DimensionsSwap()
        {
            var m = BitMatrixTestHelpers.Build("1110", "0001"); // 4x2
            var r = m.Rotate90();
            Assert.Equal((byte)2, r.w);
            Assert.Equal((byte)4, r.h);
        }
    }
}