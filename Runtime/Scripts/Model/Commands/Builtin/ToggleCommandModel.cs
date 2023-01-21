namespace Undebugger.Model.Commands.Builtin
{
    public class ToggleCommandModel : CommandModel
    {
        public NameTag Name
        { get; private set; }
        public ValueRef<bool> Reference
        { get; private set; }

        public ToggleCommandModel(NameTag name, ValueRef<bool> value)
        {
            Name = name;
            Reference = value;
        }
    }
}
