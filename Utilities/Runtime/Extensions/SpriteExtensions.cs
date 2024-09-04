using UnityEngine;
using JetBrains.Annotations;

namespace Irisu.Utilities
{
    [PublicAPI]
    public static class SpriteExtensions
    {
        public static Vector2 GetColliderSize(this Sprite sprite, float correction = 0)
        {
            Vector4 border = sprite.border;
            Bounds bounds = sprite.bounds;
            float ppu = sprite.pixelsPerUnit;
            float x = bounds.size.x - (border.x + border.z) / ppu;
            float y = bounds.size.y - (border.w + border.y) / ppu;
            return new Vector2(x - correction, y - correction);
        }
    }
}