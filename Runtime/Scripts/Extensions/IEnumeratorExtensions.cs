using System.Collections;

namespace HHG.Common.Runtime
{
    public static class IEnumeratorExtensions
    {
        public static IEnumerator Then(this IEnumerator enumerator, IEnumerator other, System.Action done = null)
        {
            yield return enumerator;
            yield return other;
            done?.Invoke();
        }

        public static IEnumerator Then(this IEnumerator enumerator, System.Action done)
        {
            yield return enumerator;
            done?.Invoke();
        }
    }
}