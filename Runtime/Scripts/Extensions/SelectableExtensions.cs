using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

namespace HHG.Common.Runtime
{
    public static class SelectableExtensions
    {
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

        public static void SetNavigation(this IEnumerable<Selectable> selectables, SelectableNavigation direction)
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
                }

                selectables.ElementAt(i).navigation = nav;
            }
        }
    }
}

