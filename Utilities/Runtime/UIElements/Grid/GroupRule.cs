using System;
using JetBrains.Annotations;

namespace Irisu.Utilities.UIElements
{
    [PublicAPI]
    public class GroupRule
    {
        public readonly string Group;
        public readonly GridStyle GridStyle;
        public readonly Predicate<PredicateContext> Predicate;
        public IElementFactory? DefaultElement;

        public GroupRule(string group, Predicate<PredicateContext> predicate)
            : this(group, predicate, new GridStyle(), null)
        { }

        private GroupRule(string group, Predicate<PredicateContext> predicate, GridStyle style,
            IElementFactory? defaultElement)
        {
            Group = group;
            Predicate = predicate;
            GridStyle = style;
            DefaultElement = defaultElement;
        }

        public override string ToString()
        {
            return $"Group {Group}";
        }

        public GroupRule Clone()
        {
            return new GroupRule(Group, Predicate, GridStyle, DefaultElement);
        }
    }
}