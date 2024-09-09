namespace Irisu.Utilities.UIElements
{
    public struct StyleSize
    {
        public float? Width;
        public float? Height;

        public StyleSize(float width, float height)
        {
            Width = width;
            Height = height;
        }

        public override string ToString()
        {
            return $"Style size (W: {Width}, H: {Height})";
        }

        public static implicit operator StyleSize(float value)
        {
            return new StyleSize
            {
                Width = value,
                Height = value
            };
        }

        public static StyleSize operator +(StyleSize a, StyleSize b)
        {
            return new StyleSize(a.Width!.Value + b.Width!.Value, a.Height!.Value + b.Height!.Value);
        }

        public static StyleSize operator -(StyleSize a, StyleSize b)
        {
            return new StyleSize(a.Width!.Value - b.Width!.Value, a.Height!.Value - b.Height!.Value);
        }

        public static StyleSize operator +(StyleSize a, Style b)
        {
            return new StyleSize(a.Width!.Value + b.Width, a.Height!.Value + b.Height);
        }

        public static StyleSize operator -(StyleSize a, Style b)
        {
            return new StyleSize(a.Width!.Value - b.Width, a.Height!.Value - b.Height);
        }
    }
}