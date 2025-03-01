//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

// TODO: Not sure exactly how should function in regards to sync vs async and actionevents
//namespace HHG.Common.Runtime
//{
//    public class ActionDoParallelAsync : IActionAsync
//    {
//        [SerializeReference, ReferencePicker] private List<IActionAsync> actions = new List<IActionAsync>();

//        public IEnumerator InvokeAsync(MonoBehaviour invoker)
//        {
//            List<Coroutine> coroutines = new List<Coroutine>();

//            foreach (IActionAsync action in actions)
//            {
//                coroutines.Add(invoker.StartCoroutine(action?.InvokeAsync(invoker)));
//            }

//            foreach (Coroutine coroutine in coroutines)
//            {
//                yield return coroutine;
//            }
//        }
//    }
//}