using System.Collections.Generic;

namespace Undebugger.Model.Commands
{
    public class PageModel : IPrioritized
    {
        public const string MainSegmentName = "Main";
        public const int MainSegmentPriority = 1000;

        public int Priority
        { get; set; }

        public string Name;
        public List<SegmentModel> Segments = new List<SegmentModel>();

        public SegmentModel FindOrCreateSegment(string name, int priority)
        {
            for (int i = 0; i < Segments.Count; ++i)
            {
                if (Segments[i].Name == name)
                {
                    return Segments[i];
                }
            }

            var segment = new SegmentModel()
            {
                Name = name,
                Priority = priority
            };

            Segments.Add(segment);

            return segment;
        }
    }
}
