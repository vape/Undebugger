namespace Undebugger.UI.Layout
{
    internal interface IULayoutNode
    {
        bool IsActive
        { get; }

        void OnHierarchyRebuild();
        void OnLayoutRebuild();
    }
}
