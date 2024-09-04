using UnityEngine;
using UnityEngine.UI;

namespace Irisu.Utilities.Scripts
{
    public sealed class ScrollRectFixedSize : ScrollRect
    {
        [SerializeField, Range(0, 1)] private float _handleSize;

        protected override void LateUpdate() {
            base.LateUpdate();
            if (verticalScrollbar) {
                verticalScrollbar.size=_handleSize;
            }
        }
   
        public override void Rebuild(CanvasUpdate executing) {
            base.Rebuild(executing);
            if (verticalScrollbar) {
                verticalScrollbar.size=_handleSize;
            }
        }
    }
}