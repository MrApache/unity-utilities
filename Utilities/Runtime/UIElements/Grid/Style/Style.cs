namespace Irisu.Utilities.UIElements
{
    public struct Style
    {
        public float? Top;
        public float? Bottom;
        public float? Right;
        public float? Left;

        public float Height => GetSaveHeight();
        public float Width => GetSafeWidth();

        private float GetSafeWidth()
        {
            float right = Right ?? 0;
            float left = Left ?? 0;
            return right + left;
        }

        private float GetSaveHeight()
        {
            float top = Top ?? 0;
            float bottom = Bottom ?? 0;
            return top + bottom;
        }

        public Style(float top, float bottom, float right, float left)
        {
            Top = top;
            Bottom = bottom;
            Right = right;
            Left = left;
        }

        public override string ToString()
        {
            return $"Style (T:{Top}, B:{Bottom}, R:{Right}, L:{Left})";
        }

        public static implicit operator Style(float value)
        {
            return new Style
            {
                Top = value,
                Bottom = value,
                Right = value,
                Left = value
            };
        }
    }
}