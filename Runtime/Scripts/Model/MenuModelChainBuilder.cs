using Undebugger.Model;
using Undebugger.Model.Commands;

namespace Undebugger
{
    public static class MenuModelChainBuilder
    {
        public class ChainContext
        {
            public MenuModel Menu;
        }

        public class ChainContext<TObject> : ChainContext
        {
            public TObject Object;
        }

        public class ChainContext<TObject, TParent> : ChainContext<TObject>
        {
            public TParent Parent;
        }

        public static ChainContext<PageModel> WithPage(this MenuModel model, string name, int priority = 0)
        {
            PageModel page = null;

            for (int i = 0; i < model.Commands.Pages.Count; ++i)
            {
                if (model.Commands.Pages[i].Name == name)
                {
                    page = model.Commands.Pages[i];
                    break;
                }
            }

            if (page == null)
            {
                page = new PageModel()
                {
                    Name = name,
                    Priority = priority
                };

                model.Commands.Pages.Add(page);
            }

            return new ChainContext<PageModel>()
            {
                Menu = model,
                Object = page
            };
        }

        public static ChainContext<PageModel> WithPage(this ChainContext context, string name, int priority = 0)
        {
            return WithPage(context.Menu, name, priority);
        }

        public static ChainContext<SegmentModel, PageModel> WithMainSegment(this ChainContext<PageModel> context)
        {
            return context.WithSegment(PageModel.MainSegmentName, priority: PageModel.MainSegmentPriority);
        }

        public static ChainContext<SegmentModel, PageModel> WithSegment(this ChainContext<PageModel> context, string name, int priority = 0)
        {
            SegmentModel segment = null;

            for (int i = 0; i < context.Object.Segments.Count; ++i)
            {
                if (context.Object.Segments[i].Name == name)
                {
                    segment = context.Object.Segments[i];
                    break;
                }
            }

            if (segment == null)
            {
                segment = new SegmentModel()
                {
                    Name = name,
                    Priority = priority
                };

                context.Object.Segments.Add(segment);
            }

            return new ChainContext<SegmentModel, PageModel>()
            {
                Menu = context.Menu,
                Object = segment,
                Parent = context.Object
            };
        }

        public static ChainContext<SegmentModel, PageModel> WithSegment(this ChainContext<SegmentModel, PageModel> context, string name, int priority = 0)
        {
            SegmentModel segment = null;

            for (int i = 0; i < context.Parent.Segments.Count; ++i)
            {
                if (context.Parent.Segments[i].Name == name)
                {
                    segment = context.Parent.Segments[i];
                    break;
                }
            }

            if (segment == null)
            {
                segment = new SegmentModel()
                {
                    Name = name,
                    Priority = priority
                };

                context.Parent.Segments.Add(segment);
            }

            return new ChainContext<SegmentModel, PageModel>()
            {
                Menu = context.Menu,
                Object = segment,
                Parent = context.Parent
            };
        }

        public static ChainContext<SegmentModel, PageModel> WithMainSegment(this ChainContext<SegmentModel, PageModel> context)
        {
            return context.WithSegment(PageModel.MainSegmentName, priority: PageModel.MainSegmentPriority);
        }

        public static ChainContext<SegmentModel, PageModel> AddCommand(this ChainContext<SegmentModel, PageModel> context, CommandModel command)
        {
            context.Object.Commands.Add(command);
            return context;
        }
    }
}
