using System;

namespace Undebugger.Model.Status
{
    public interface IStatusSegmentDriver : IPrioritized
    {
        event Action Changed;

        string PersistentId
        { get; }
        string Title
        { get; }
        string Text
        { get; }
    }

    public abstract class StatusSegmentDriver : IStatusSegmentDriver
    {
        private const string defaultTitle = "Untitled";

        public abstract string PersistentId
        { get; }
        public virtual int Priority
        { get; }

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
        public override string PersistentId => id;
        public override string Title => title;
        public override string Text => text;

        protected readonly string id;
        protected readonly string title;
        protected readonly string text;

        public StaticStatusSegmentDriver(string persistentId, string title, string text)
        {
            this.id = persistentId;
            this.title = title;
            this.text = text;
        }
    }
}
