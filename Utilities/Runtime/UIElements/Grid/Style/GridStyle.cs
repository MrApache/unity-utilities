using System;
using UnityEngine;

namespace Irisu.Utilities.UIElements
{
    public sealed class GridStyle
    {
        private readonly Action? _styleChanged;
        private Style _spacing;
        private StyleSize _cellSize;
        private Style _margin;
        private Color? _cellBackgroundColor;

        public Color? CellBackgroundColor
        {
            get => _cellBackgroundColor ?? new Color();
            set
            {
                _cellBackgroundColor = value;
                _styleChanged?.Invoke();
            }
        }

        public StyleSize CellSize
        {
            get => _cellSize;
            set
            {
                _cellSize = value;
                _styleChanged?.Invoke();
            }
        }

        public Style Spacing
        {
            get => _spacing;
            set
            {
                _spacing = value;
                _styleChanged?.Invoke();
            }
        }

        public Style Margin
        {
            get => _margin;
            set
            {
                _margin = value;
                _styleChanged?.Invoke();
            }
        }

        public GridStyle() {}

        public GridStyle(Action styleChanged)
        {
            _styleChanged = styleChanged;
        }

        public GridStyle(GridStyle style)
        {
            _styleChanged = style._styleChanged;
            _spacing = style._spacing;
            _cellSize = style._cellSize;
            _margin = style._margin;
            _cellBackgroundColor = style._cellBackgroundColor;
        }

        public void OverrideValues(GridStyle style)
        {
            _spacing.Top = style.Spacing.Top ?? _spacing.Top;
            _spacing.Bottom = style.Spacing.Bottom ?? _spacing.Bottom;
            _spacing.Left = style.Spacing.Left ?? _spacing.Left;
            _spacing.Right = style.Spacing.Right ?? _spacing.Right;

            _cellSize.Width = style.CellSize.Width ?? _cellSize.Width;
            _cellSize.Height = style.CellSize.Height ?? _cellSize.Height;

            _margin.Top = style.Margin.Top ?? _margin.Top;
            _margin.Bottom = style.Margin.Bottom ?? _margin.Bottom;
            _margin.Left = style.Margin.Left ?? _margin.Left;
            _margin.Right = style.Margin.Right ?? _margin.Right;

            _cellBackgroundColor ??= style._cellBackgroundColor;
        }

        public override string ToString()
        {
            return "Grid style:" +
                   $"Spacing: {_spacing}" +
                   $"Cell size: {_cellSize}" +
                   $"Margin: {_margin}" +
                   $"Cell background color:{_cellBackgroundColor})";
        }
    }
}