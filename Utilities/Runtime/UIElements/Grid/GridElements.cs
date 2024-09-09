using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Irisu.Utilities.UIElements
{
    public abstract class GridElements : VisualElement
    {
        internal HashSet<string> Groups { get; set; }

        protected GridElements()
        {
            Groups = new HashSet<string>();
        }

        protected virtual bool CanAdd() => true;
        protected abstract void OnAdd(VisualElement element);

        public new void Add(VisualElement element)
        {
            if (!CanAdd())
                return;

            base.Add(element);
            OnAdd(element);
        }

        protected void AddWithoutCallback(VisualElement element)
        {
            base.Add(element);
        }
    }
}