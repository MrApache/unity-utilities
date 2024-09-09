using UnityEngine.UIElements;

namespace Irisu.Utilities.UIElements
{
    public sealed class SplitView : TwoPaneSplitView
    {
        public new class UxmlFactory : UxmlFactory<SplitView, UxmlTraits> { }

        public SplitView() {}

        public SplitView(float fixedPaneInitialDim, VisualElement firstView, VisualElement secondView)
        {
            orientation = TwoPaneSplitViewOrientation.Horizontal;
            style.minHeight = 400f;
            style.minWidth = 400f;

            fixedPaneInitialDimension = fixedPaneInitialDim;
            Add(firstView);
            Add(secondView);
        }
    }
}