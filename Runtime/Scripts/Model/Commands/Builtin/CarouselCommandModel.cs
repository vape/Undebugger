namespace Deszz.Undebugger.Model.Commands.Builtin
{
    public class CarouselCommandModel : CommandModel
    {
        public object Value => values[Index];

        public object[] Values => values;
        public int Index => index.Value;
        
        private ValueRef<int> index;
        private object[] values;

        public CarouselCommandModel(ValueRef<int> index, object[] values)
        {
            this.index = index;
            this.values = values;
        }

        public void Set(int value)
        {
            index.Value = Wrap(value, values.Length);
        }

        private static int Wrap(int index, int count)
        {
            return ((index % count) + count) % count;
        }
    }
}
