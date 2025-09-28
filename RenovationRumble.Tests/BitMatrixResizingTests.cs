namespace RenovationRumble.Tests
{
    using Logic.Rules.BitMatrix;
    using Logic.Primitives;
    using Xunit;
    using static BitMatrixTestHelpers;

    public sealed class BitMatrixResizingTests
    {
        [Fact]
        public void RemoveColumnLeft_DropsLeftmostBits()
        {
            var m = Build("1101", "1011"); // w=4
            var shr = m.Shrink(Edge.Left);

            Assert.Equal((byte)3, shr.w);
            Assert.Equal((byte)2, shr.h);
            Assert.Equal(["101", "011"], ExtractRows(shr));
        }

        [Fact]
        public void RemoveColumnRight_DropsRightmostBits()
        {
            var m = Build("1101", "1011");
            var shr = m.Shrink(Edge.Right);

            Assert.Equal((byte)3, shr.w);
            Assert.Equal((byte)2, shr.h);
            Assert.Equal(["110", "101"], ExtractRows(shr));
        }

        [Fact]
        public void RemoveRowTop_DropsFirstRow()
        {
            var m = Build("1110", "0011", "0101");
            var shr = m.Shrink(Edge.Top);

            Assert.Equal((byte)4, shr.w);
            Assert.Equal((byte)2, shr.h);
            Assert.Equal(["0011", "0101"], ExtractRows(shr));
        }

        [Fact]
        public void RemoveRowBottom_DropsLastRow()
        {
            var m = Build("1110", "0011", "0101");
            var shr = m.Shrink(Edge.Bottom);

            Assert.Equal((byte)4, shr.w);
            Assert.Equal((byte)2, shr.h);
            Assert.Equal(["1110", "0011"], ExtractRows(shr));
        }

        [Fact]
        public void AddColumnLeft_CopiesLeftEdgeBits()
        {
            var m = Build("010", "001");
            var gr = m.Grow(Edge.Left);

            Assert.Equal((byte)4, gr.w);
            Assert.Equal((byte)2, gr.h);
            Assert.Equal(["0010", "0001"], ExtractRows(gr));
        }

        [Fact]
        public void AddColumnRight_CopiesRightEdgeBits()
        {
            var m = Build("010", "001");
            var gr = m.Grow(Edge.Right);

            Assert.Equal((byte)4, gr.w);
            Assert.Equal((byte)2, gr.h);
            Assert.Equal(["0100", "0011"], ExtractRows(gr));
        }

        [Fact]
        public void AddRowTop_CopiesTopRow()
        {
            var m = Build("010", "001");
            var gr = m.Grow(Edge.Top);

            Assert.Equal((byte)3, gr.w);
            Assert.Equal((byte)3, gr.h);
            Assert.Equal(["010", "010", "001"], ExtractRows(gr));
        }

        [Fact]
        public void AddRowBottom_CopiesBottomRow()
        {
            var m = Build("010", "001");
            var gr = m.Grow(Edge.Bottom);

            Assert.Equal((byte)3, gr.w);
            Assert.Equal((byte)3, gr.h);
            Assert.Equal(["010", "001", "001"], ExtractRows(gr));
        }

        [Fact]
        public void Grow_NoOp_WhenExceedingMaxCells()
        {
            // 8x8 already at MaxCells
            var full = new BitMatrix(8, 8, ulong.MaxValue);

            var left = full.Grow(Edge.Left);
            var right = full.Grow(Edge.Right);
            var top = full.Grow(Edge.Top);
            var bottom = full.Grow(Edge.Bottom);

            Assert.Equal((byte)8, left.w);
            Assert.Equal((byte)8, left.h);
            Assert.Equal((byte)8, right.w);
            Assert.Equal((byte)8, right.h);
            Assert.Equal((byte)8, top.w);
            Assert.Equal((byte)8, top.h);
            Assert.Equal((byte)8, bottom.w);
            Assert.Equal((byte)8, bottom.h);
            Assert.Equal(ExtractRows(full), ExtractRows(left));
            Assert.Equal(ExtractRows(full), ExtractRows(right));
            Assert.Equal(ExtractRows(full), ExtractRows(top));
            Assert.Equal(ExtractRows(full), ExtractRows(bottom));
        }

        [Fact]
        public void Shrink_NoOp_WhenAtMinimumDimension()
        {
            var m = new BitMatrix(1, 1, 1UL);

            Assert.Equal(m, m.Shrink(Edge.Left));
            Assert.Equal(m, m.Shrink(Edge.Right));
            Assert.Equal(m, m.Shrink(Edge.Top));
            Assert.Equal(m, m.Shrink(Edge.Bottom));
        }
    }
}