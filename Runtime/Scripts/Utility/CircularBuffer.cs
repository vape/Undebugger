namespace Undebugger.Utility
{
    internal class CircularBuffer<T>
    {
        public int Count
        { get { return count; } }

        public readonly int Capacity;

        private int head;
        private int count;
        private T[] data;

        public CircularBuffer(int capacity)
        {
            Capacity = capacity;

            data = new T[capacity];
        }

        public void Clear()
        {
            head = 0;
            count = 0;
        }

        public void PushBack(T value)
        {
            head = (head - 1) % Capacity;

            if (head < 0)
            {
                // handle '%' being remainder operator instead of modulo
                head += Capacity;
            }

            data[head] = value;

            if (count < Capacity)
            {
                count++;
            }
        }

        public void PopBack(out T value)
        {
            value = data[head];
            PopBack();
        }

        public void PopBack()
        {
            if (count == 0)
            {
                return;
            }

            head = (head + 1) % Capacity;
            count--;
        }

        public void PushFront(T value)
        {
            data[(head + count) % Capacity] = value;

            if (count == Capacity)
            {
                head = (head + 1) % Capacity;
            }
            else
            {
                count++;
            }
        }

        public void PopFront(out T value)
        {
            if (count > 0)
            {
                value = data[(head + count - 1) % Capacity];
                count--;
            }
            else
            {
                value = default;
            }
        }

        public void PopFront()
        {
            if (count > 0)
            {
                count--;
            }
        }

        public ref T Get(int index)
        {
            return ref data[(head + index) % Capacity];
        }

        public T[] GetRawArray()
        {
            return data;
        }
    }
}
