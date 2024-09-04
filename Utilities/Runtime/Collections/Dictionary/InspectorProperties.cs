using System;

namespace Irisu.Collections
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class InspectorProperties : Attribute
    {
        public bool AllowEdit { get; set; }
        public bool AllowElementEdit { get; set; }

        public InspectorProperties()
        {
            AllowEdit = true;
            AllowElementEdit = true;
        }
    }
}