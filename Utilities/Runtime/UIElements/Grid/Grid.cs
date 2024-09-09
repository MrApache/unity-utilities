using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;

namespace Irisu.Utilities.UIElements
{
    public class Grid : GridElements
    {
        [PublicAPI]
        public readonly GridStyle Style;
        private readonly CellArray _array;
        private readonly Dictionary<string, GroupRule> _rules;
        private readonly List<VisualElement> _fillers;
        private CellArray.Enumerator _arrayEnum;

        public Grid()
        {
            _fillers = new List<VisualElement>();
            _rules = new Dictionary<string, GroupRule>();
            _array = new CellArray();
            _arrayEnum = _array.GetEnumerator();
            style.flexWrap = Wrap.Wrap;
            style.flexShrink = 0;
            Style = new GridStyle(() => UpdateContent());
            RegisterCallback<GeometryChangedEvent>(_ => UpdateContent());
        }

        [PublicAPI]
        public void AddGroupRule(GroupRule rule)
        {
            _rules[rule.Group] = rule;
        }

        [PublicAPI]
        public void Rebuild() => UpdateContent(true);
        protected override bool CanAdd() => true;

        protected override void OnAdd(VisualElement element)
        {
            _arrayEnum.Current.Add(element);
            _arrayEnum.MoveNext();
        }

        private void Resize()
        {
            RecalculationResult result = new RecalculationResult(this);
            CellArray.ResizeResult resizeResult = _array.Resize(result.CellCount.x, result.CellCount.y);
            _arrayEnum.Reset();
            foreach (GridCell cell in resizeResult.Added)
                AddWithoutCallback(cell);
            foreach (GridCell cell in resizeResult.Removed)
                Remove(cell);
            UpdateCellRules();
        }

        private void ResetGroups()
        {
            foreach (GroupRule rule in _rules.Values)
                rule.DefaultElement?.Reset();
        }

        private void ResetCell(GridCell cell)
        {
            foreach (string group in cell.Groups)
            {
                IElementFactory? elementFactory = _rules[group].DefaultElement;
                if(elementFactory == null)
                    continue;
                VisualElement rootElement = cell.contentContainer.ElementAt(0);
                elementFactory.Unbind(rootElement);
            }
            cell.Clear();
            cell.Groups.Clear();
        }

        private void UpdateCellRules()
        {
            foreach (GridCell cell in _array)
            {
                ResetCell(cell);
                foreach (GroupRule rule in _rules.Values)
                {
                    if (rule.Predicate.Invoke(new PredicateContext(cell.Position)))
                        cell.Groups.Add(rule.Group);
                }
            }
            ResetGroups();
            ResetProperties();
        }

        private void ResetProperties()
        {
            foreach (GridCell cell in _array)
                SetProperties(cell, true);
        }

        private void TryResize()
        {
            RecalculationResult result = new RecalculationResult(this);
            if (!result.CanResize) return;
            Resize();
        }

        private void FillEmptySpace()
        {
            foreach (VisualElement filler in _fillers)
                Remove(filler);
            _fillers.Clear();
            int index = -1;
            for (int x = 0; x < _array.LengthX; x++)
            {
                float totalHeight = 0;
                for (int y = 0; y < _array.LengthY; y++)
                {
                    GridCell cell = _array[x, y];
                    totalHeight += cell.style.height.value.value;
                    totalHeight += Style.Margin.Height;
                }
                float fillerHeight = style.height.value.value - totalHeight;
                index += _array.LengthY + 1;
                InsertFiller(index, fillerHeight);
            }
        }

        private void InsertFiller(int index, float height)
        {
            VisualElement element = new VisualElement
            {
                style = { height = height },
                name = "filler"
            };
            Insert(index, element);
            _fillers.Add(element);
        }

        private void UpdateContent(bool forceRebuild = false)
        {
            if (forceRebuild) Resize();
            else TryResize();
            foreach (GridCell cell in _array)
                SetProperties(cell, false);
            FillEmptySpace();
        }

        private void SetProperties(GridCell cell, bool createDefaultElements)
        {
            GridStyle finalStyle = new GridStyle(Style);
            IElementFactory? factory = null;
            foreach (string group in cell.Groups)
            {
                GroupRule rule = _rules[group];
                factory ??= rule.DefaultElement;
                GridStyle localStyle = _rules[group].GridStyle;
                finalStyle.OverrideValues(localStyle);
            }

            cell.style.backgroundColor = finalStyle.CellBackgroundColor!.Value;
            cell.style.width = finalStyle.CellSize.Width!.Value;
            cell.style.height = finalStyle.CellSize.Height!.Value;
            cell.style.marginRight = finalStyle.Spacing.Right!.Value;
            cell.style.marginLeft = finalStyle.Spacing.Left!.Value;
            cell.style.marginBottom = finalStyle.Spacing.Bottom!.Value;
            cell.style.marginTop = finalStyle.Spacing.Top!.Value;
            cell.style.flexShrink = 0;

            if(createDefaultElements && factory != null)
                CreateDefaultElement(cell, factory);
        }

        private void CreateDefaultElement(GridCell cell, IElementFactory factory)
        {
            VisualElement element = factory.Instantiate();
            factory.Bind(element);
            cell.Add(element);
        }

        private void Init(float sizeX, float sizeY, float spacingX, float spacingY, float marginX, float marginY)
        {
            Style.CellSize = new StyleSize
            {
                Width = sizeX,
                Height = sizeY
            };

            Style.Spacing = new Style
            {
                Right = spacingX,
                Left = spacingX,
                Top = spacingY,
                Bottom = spacingY
            };

            Style.Margin = new Style
            {
                Right = marginX,
                Left = marginX,
                Top = marginY,
                Bottom = marginY
            };
        }

        public new class UxmlFactory : UxmlFactory<Grid, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private readonly UxmlFloatAttributeDescription _sizeX;
            private readonly UxmlFloatAttributeDescription _sizeY;
            private readonly UxmlFloatAttributeDescription _spacingX;
            private readonly UxmlFloatAttributeDescription _spacingY;
            private readonly UxmlFloatAttributeDescription _marginX;
            private readonly UxmlFloatAttributeDescription _marginY;

            public UxmlTraits()
            {
                _sizeX = new UxmlFloatAttributeDescription();
                _sizeX.name = "Cell-Size-X";

                _sizeY = new UxmlFloatAttributeDescription();
                _sizeY.name = "Cell-Size-Y";

                _spacingX = new UxmlFloatAttributeDescription();
                _spacingX.name = "Spacing-X";

                _spacingY = new UxmlFloatAttributeDescription();
                _spacingY.name = "Spacing-Y";

                _marginX = new UxmlFloatAttributeDescription();
                _marginX.name = "Margin-X";

                _marginY = new UxmlFloatAttributeDescription();
                _marginY.name = "Margin-Y";
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                float sizeX = _sizeX.GetValueFromBag(bag, cc);
                float sizeY = _sizeY.GetValueFromBag(bag, cc);
                float spacingX = _spacingX.GetValueFromBag(bag, cc);
                float spacingY = _spacingY.GetValueFromBag(bag, cc);
                float marginX = _marginX.GetValueFromBag(bag, cc);
                float marginY = _marginY.GetValueFromBag(bag, cc);
                ((Grid) ve).Init(sizeX, sizeY, spacingX, spacingY, marginX, marginY);
            }
        }

        private readonly ref struct RecalculationResult
        {
            private readonly Vector2 _maxCellSize;
            private readonly Vector2Int _arraySize;
            public readonly Vector2Int CellCount;

            public bool CanResize => _maxCellSize is { x: > 0, y: > 0 }
                                     && CellCount.x != _arraySize.x
                                     && CellCount.y != _arraySize.y;

            public RecalculationResult(Grid grid)
            {
                GridStyle style = grid.Style;
                Vector2 maxGridSize = new Vector2(grid.style.width.value.value,
                    grid.style.height.value.value);

                _arraySize = new Vector2Int(grid._array.LengthX, grid._array.LengthY);
                _maxCellSize = new Vector2(
                    style.CellSize.Width + style.Spacing.Left + style.Spacing.Right ?? 0,
                    style.CellSize.Height + style.Spacing.Top + style.Spacing.Bottom ?? 0);
                CellCount = new Vector2Int(
                    (int)(maxGridSize.x / _maxCellSize.x),
                    (int)(maxGridSize.y / _maxCellSize.y));

                CellCount.x = CellCount.x < 0 ? 0 : CellCount.x;
                CellCount.y = CellCount.y < 0 ? 0 : CellCount.y;
            }
        }
    }
}