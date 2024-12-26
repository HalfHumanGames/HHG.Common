using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HHG.Common.Runtime
{
    public static class SelectableExtensions
    {
        public static bool IsSelected(this Selectable selectable)
        {
            return EventSystem.current.currentSelectedGameObject == selectable.gameObject;
        }

        public static void ClearNavigation(this Selectable selectable)
        {
            Navigation nav = selectable.navigation;
            nav.mode = Navigation.Mode.Explicit;
            nav.selectOnUp = null;
            nav.selectOnDown = null;
            nav.selectOnLeft = null;
            nav.selectOnRight = null;
            selectable.navigation = nav;
        }

        public static void SetNavigationUp(this Selectable selectable, Selectable destination)
        {
            Navigation nav = selectable.navigation;
            nav.mode = Navigation.Mode.Explicit;
            nav.selectOnUp = destination;
            selectable.navigation = nav;
        }

        public static void SetNavigationDown(this Selectable selectable, Selectable destination)
        {
            Navigation nav = selectable.navigation;
            nav.mode = Navigation.Mode.Explicit;
            nav.selectOnDown = destination;
            selectable.navigation = nav;
        }

        public static void SetNavigationLeft(this Selectable selectable, Selectable destination)
        {
            Navigation nav = selectable.navigation;
            nav.mode = Navigation.Mode.Explicit;
            nav.selectOnLeft = destination;
            selectable.navigation = nav;
        }

        public static void SetNavigationRight(this Selectable selectable, Selectable destination)
        {
            Navigation nav = selectable.navigation;
            nav.mode = Navigation.Mode.Explicit;
            nav.selectOnRight = destination;
            selectable.navigation = nav;
        }

        public static void ClearNavigation(this IEnumerable<Selectable> selectables)
        {
            foreach (Selectable selectable in selectables)
            {
                selectable.ClearNavigation();
            }
        }

        public static void SetNavigationUp(this IEnumerable<Selectable> selectables, Selectable destination)
        {
            foreach (Selectable selectable in selectables)
            {
                selectable.SetNavigationUp(destination);
            }
        }

        public static void SetNavigationDown(this IEnumerable<Selectable> selectables, Selectable destination)
        {
            foreach (Selectable selectable in selectables)
            {
                selectable.SetNavigationDown(destination);
            }
        }

        public static void SetNavigationLeft(this IEnumerable<Selectable> selectables, Selectable destination)
        {
            foreach (Selectable selectable in selectables)
            {
                selectable.SetNavigationLeft(destination);
            }
        }

        public static void SetNavigationRight(this IEnumerable<Selectable> selectables, Selectable destination)
        {
            foreach (Selectable selectable in selectables)
            {
                selectable.SetNavigationRight(destination);
            }
        }

        public static void SetNavigationHorizontal(this IEnumerable<Selectable> selectables)
        {
            SetNavigation(selectables, SelectableNavigation.Horizontal);
        }

        public static void SetNavigationVertical(this IEnumerable<Selectable> selectables)
        {
            SetNavigation(selectables, SelectableNavigation.Vertical);
        }

        public static void SetNavigationGrid(this IEnumerable<Selectable> selectables, int columns)
        {
            SetNavigation(selectables, SelectableNavigation.Grid, columns);
        }

        public static void SetNavigationAuto(this IEnumerable<Selectable> selectables)
        {
            SetNavigation(selectables, SelectableNavigation.Auto);
        }

        public static void SetNavigation(this IEnumerable<Selectable> selectables, SelectableNavigation direction, int columns = 0)
        {
            int length = selectables.Count();

            if (length < 2)
            {
                return;
            }

            for (int i = 0; i < length; i++)
            {
                Navigation nav = selectables.ElementAt(i).navigation;
                nav.mode = Navigation.Mode.Explicit;

                switch (direction)
                {
                    case SelectableNavigation.Horizontal:
                        if (i == 0)
                        {
                            nav.selectOnLeft = selectables.ElementAt(length - 1);
                            nav.selectOnRight = selectables.ElementAt(1);
                        }
                        else if (i == length - 1)
                        {
                            nav.selectOnLeft = selectables.ElementAt(i - 1);
                            nav.selectOnRight = selectables.ElementAt(0);
                        }
                        else
                        {
                            nav.selectOnLeft = selectables.ElementAt(i - 1);
                            nav.selectOnRight = selectables.ElementAt(i + 1);
                        }
                        break;

                    case SelectableNavigation.Vertical:
                        if (i == 0)
                        {
                            nav.selectOnUp = selectables.ElementAt(length - 1);
                            nav.selectOnDown = selectables.ElementAt(1);
                        }
                        else if (i == length - 1)
                        {
                            nav.selectOnUp = selectables.ElementAt(i - 1);
                            nav.selectOnDown = selectables.ElementAt(0);
                        }
                        else
                        {
                            nav.selectOnUp = selectables.ElementAt(i - 1);
                            nav.selectOnDown = selectables.ElementAt(i + 1);
                        }
                        break;

                    case SelectableNavigation.Grid:
                        int row = i / columns;
                        int col = i % columns;
                        nav.selectOnUp = (row > 0 && i - columns >= 0) ? selectables.ElementAt(i - columns) : (length - columns + col < length ? selectables.ElementAt(length - columns + col) : null);
                        nav.selectOnDown = (row < (length - 1) / columns && i + columns < length) ? selectables.ElementAt(i + columns) : (col < length ? selectables.ElementAt(col) : null);
                        nav.selectOnLeft = (col > 0 && i - 1 >= 0) ? selectables.ElementAt(i - 1) : (row * columns + (columns - 1) < length ? selectables.ElementAt(row * columns + (columns - 1)) : null);
                        nav.selectOnRight = (col < columns - 1 && i + 1 < length) ? selectables.ElementAt(i + 1) : (row * columns < length ? selectables.ElementAt(row * columns) : null);
                        break;

                    case SelectableNavigation.Auto:
                        nav.selectOnUp = FindNearestSelectable(selectables, i, Vector2.up);
                        nav.selectOnDown = FindNearestSelectable(selectables, i, Vector2.down);
                        nav.selectOnLeft = FindNearestSelectable(selectables, i, Vector2.left);
                        nav.selectOnRight = FindNearestSelectable(selectables, i, Vector2.right);
                        break;
                }

                selectables.ElementAt(i).navigation = nav;
            }
        }

        private static Selectable FindNearestSelectable(IEnumerable<Selectable> selectables, int currentIndex, Vector2 direction)
        {
            Selectable currentSelectable = selectables.ElementAt(currentIndex);
            Vector2 currentPosition = (currentSelectable.transform as RectTransform).anchoredPosition;

            Selectable nearest = null;
            float shortestDistance = float.MaxValue;

            foreach (Selectable selectable in selectables)
            {
                if (selectable == currentSelectable)
                {
                    continue;
                }

                Vector2 position = (selectable.transform as RectTransform).anchoredPosition;
                Vector2 offset = position - currentPosition;

                if (Vector2.Dot(offset.normalized, direction) > .5f)
                {
                    float distance = offset.sqrMagnitude;

                    if (distance < shortestDistance)
                    {
                        shortestDistance = distance;
                        nearest = selectable;
                    }
                }
            }

            return nearest;
        }
    }
}

