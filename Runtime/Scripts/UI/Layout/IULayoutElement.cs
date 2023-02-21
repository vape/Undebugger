namespace Undebugger.UI.Layout
{
    internal interface IULayoutElement
    {
        bool Ignore
        { get; }
        float MinHeight
        { get; }
        float MinWidth
        { get; }
    }
}
