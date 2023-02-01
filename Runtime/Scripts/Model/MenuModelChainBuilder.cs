using System;
using System.Linq;
using System.Reflection;
using Undebugger.Model;
using Undebugger.Model.Commands;
using Undebugger.Model.Commands.Builtin;

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

        public static ChainContext<SegmentModel, PageModel> CreateCommand(this ChainContext<SegmentModel, PageModel> context, CommandModel command)
        {
            context.Object.Commands.Add(command);
            return context;
        }

        public static ChainContext<SegmentModel, PageModel> CreateAction(this ChainContext<SegmentModel, PageModel> context, string name, Action action)
        {
            return context.CreateCommand(new ActionCommandModel(name, action));
        }

        public static ChainContext<SegmentModel, PageModel> CreateToggle(this ChainContext<SegmentModel, PageModel> context, string name, ValueReferenceGetter<bool> getter, ValueReferenceSetter<bool> setter)
        {
            return context.CreateCommand(new ToggleCommandModel(name, new ValueRef<bool>(getter, setter)));
        }

        public static ChainContext<SegmentModel, PageModel> CreateToggle(this ChainContext<SegmentModel, PageModel> context, string name, ValueRef<bool> valueRef)
        {
            return context.CreateCommand(new ToggleCommandModel(name, valueRef));
        }

        public static ChainContext<SegmentModel, PageModel> CreateToggle(this ChainContext<SegmentModel, PageModel> context, string name, object obj, string propertyName)
        {
            var type = obj.GetType();

            PropertyInfo property = null;
            FieldInfo field = null;

            property = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (property == null)
            {
                field = type.GetField(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            }

            ValueReferenceGetter<bool> getter = () =>
            {
                if (property != null)
                {
                    return (bool)property.GetValue(obj);
                }

                return (bool)field.GetValue(obj);
            };
            ValueReferenceSetter<bool> setter = (value) =>
            {
                if (property != null)
                {
                    property.SetValue(obj, value);
                }
                else
                {
                    field.SetValue(obj, value);
                }
            };

            return context.CreateCommand(new ToggleCommandModel(name, new ValueRef<bool>(getter, setter)));
        }

        public static ChainContext<SegmentModel, PageModel> CreateCarousel(this ChainContext<SegmentModel, PageModel> context, object[] values, ValueRef<int> indexRef)
        {
            return context.CreateCommand(new CarouselCommandModel(indexRef, values));
        }

        public static ChainContext<SegmentModel, PageModel> CreateCarousel(this ChainContext<SegmentModel, PageModel> context, object[] values, object current, Action<object> onChanged)
        {
            var index = Array.IndexOf(values, current);
            if (index == -1)
            {
                index = 0;
            }

            ValueReferenceGetter<int> getIndex = () => index;
            ValueReferenceSetter<int> setIndex = (i) =>
            {
                index = i;
                onChanged?.Invoke(values[index]);
            };

            return context.CreateCommand(new CarouselCommandModel(new ValueRef<int>(getIndex, setIndex), values));
        }

        public static ChainContext<SegmentModel, PageModel> CreateCarousel<T>(this ChainContext<SegmentModel, PageModel> context, T[] values, T current, Action<T> onChanged)
        {
            var objectValues = values.Cast<object>().ToArray();

            return context.CreateCarousel(objectValues, current, (value) =>
            {
                onChanged?.Invoke((T)value);
            });
        }
    }
}
