using NUnit.Framework;
using System.Linq;
using Undebugger.Utility;

namespace Undebugger.Tests
{
    public class CircularBufferTests
    {
        [Test]
        public void AllAroundTest()
        {
            var buffer = new CircularBuffer<int>(capacity: 4);
            buffer.PushFront(1);
            buffer.PushFront(2);
            buffer.PushBack(3);

            AssertRaw(buffer, new int[] { 1, 2, 0, 3 });
            AssertIndexed(buffer, new int[] { 3, 1, 2 });

            buffer.PushFront(4);
            buffer.PushFront(5);
            buffer.PushFront(6);

            AssertRaw(buffer, new int[] { 6, 2, 4, 5 });
            AssertIndexed(buffer, new int[] { 2, 4, 5, 6 });

            buffer.PushFront(7);

            AssertRaw(buffer, new int[] { 6, 7, 4, 5 });
            AssertIndexed(buffer, new int[] { 4, 5, 6, 7 });

            buffer.PushBack(8);

            AssertRaw(buffer, new int[] { 6, 8, 4, 5 });
            AssertIndexed(buffer, new int[] { 8, 4, 5, 6 });

            buffer.PushBack(9);

            AssertRaw(buffer, new int[] { 9, 8, 4, 5 });
            AssertIndexed(buffer, new int[] { 9, 8, 4, 5 });

            buffer.PopBack(out var popValue);

            Assert.AreEqual(popValue, 9);
            AssertRaw(buffer, new int[] { 9, 8, 4, 5 });
            AssertIndexed(buffer, new int[] { 8, 4, 5 });

            buffer.PopFront(out popValue);

            Assert.AreEqual(popValue, 5);
            AssertRaw(buffer, new int[] { 9, 8, 4, 5 });
            AssertIndexed(buffer, new int[] { 8, 4 });

            buffer.PushFront(1);
            buffer.PushFront(2);

            AssertRaw(buffer, new int[] { 2, 8, 4, 1 });
            AssertIndexed(buffer, new int[] { 8, 4, 1, 2 });

            buffer.PushFront(3);
            buffer.PushFront(4);

            AssertRaw(buffer, new int[] { 2, 3, 4, 1 });
            AssertIndexed(buffer, new int[] { 1, 2, 3, 4 });

            buffer.PopBack(out popValue);
            Assert.AreEqual(popValue, 1);
            buffer.PopBack(out popValue);
            Assert.AreEqual(popValue, 2);

            AssertRaw(buffer, new int[] { 2, 3, 4, 1 });
            AssertIndexed(buffer, new int[] { 3, 4 } );

            buffer.PopFront(out popValue);
            Assert.AreEqual(popValue, 4);
            buffer.PopFront(out popValue);
            Assert.AreEqual(popValue, 3);

            AssertRaw(buffer, new int[] { 2, 3, 4, 1 });
            AssertIndexed(buffer, new int[0]);

            buffer.PopFront();

            AssertRaw(buffer, new int[] { 2, 3, 4, 1 });
            AssertIndexed(buffer, new int[0]);

            buffer.PopBack();

            AssertRaw(buffer, new int[] { 2, 3, 4, 1 });
            AssertIndexed(buffer, new int[0]);
        }

        private void AssertRaw(CircularBuffer<int> buffer, int[] expected)
        {
            var actual = buffer.GetRawArray();

            Assert.IsTrue(actual.SequenceEqual(expected), message: $"Raw expected {Format(expected)}, got {Format(actual)}");
        }

        private void AssertIndexed(CircularBuffer<int> buffer, int[] expected)
        {
            var actual = new int[buffer.Count];

            for (int i = 0; i < actual.Length; ++i)
            {
                actual[i] = buffer.Get(i);
            }

            Assert.IsTrue(actual.SequenceEqual(expected), message: $"Indexed expected {Format(expected)}, got {Format(actual)}");
        }

        private string Format(int[] array)
        {
            return $"[{string.Join(", ", array)}]";
        }
    }
}