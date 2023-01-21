using System;

namespace Undebugger.Model.Status
{
    public interface IStatusSegmentDriver : IPrioritized
    {
        event Action Changed;

        string Title
        { get; }
        string Text
        { get; }
    }

    public abstract class StatusSegmentDriver : IStatusSegmentDriver
    {
        public int Priority
        { get; set; }

        private const string defaultTitle = "Untitled";

        public event Action Changed;

        public virtual string Title => defaultTitle;

        public abstract string Text
        { get; }

        protected void OnChanged()
        {
            Changed?.Invoke();
        }
    }

    public class StaticStatusSegmentDriver : StatusSegmentDriver
    {
        public override string Title => title;
        public override string Text => text;

        protected readonly string title;
        protected readonly string text;

        public StaticStatusSegmentDriver(string title, string text)
        {
            this.title = title;
            this.text = text;
        }
    }
}
