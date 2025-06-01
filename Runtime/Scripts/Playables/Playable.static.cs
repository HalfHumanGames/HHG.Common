using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace HHG.Common.Runtime
{
    public partial class Playable
    {
        private static readonly Dictionary<Type, Actions> creators = new Dictionary<Type, Actions>();

        static Playable()
        {
            Register<Animation>(
                animation => animation.Play(), 
                animation => animation.Stop(),
                null,
                null,
                animation => animation.isPlaying);

            Register<AudioSource>(
                audioSource => audioSource.Play(), 
                audioSource => audioSource.Stop(),
                null,
                null,
                audioSource => audioSource.isPlaying);

            Register<ParticleSystem>(
                particleSystem => particleSystem.Play(false), 
                particleSystem => particleSystem.Stop(false),
                null,
                null,
                particleSystem => particleSystem.isPlaying);
        }

        public static void Register<T>(
            Action<T> play, 
            Action<T> stop = null, 
            Action<T> pause = null,
            Action<T> resume = null, 
            Func<T, bool> isPlaying = null) where T : Component
        {
            stop ??= _ => { };
            pause ??= _ => { };
            resume ??= _ => { };
            isPlaying ??= _ => false;

            creators[typeof(T)] = new Actions(
                component => play((T)component), 
                component => stop((T)component), 
                component => pause((T)component), 
                component => resume((T)component), 
                component => isPlaying((T)component));
        }

        public static void Unregister<T>() where T : Component
        {
            creators.Remove(typeof(T));
        }

        public static Playable Create(GameObject gameObject)
        {
            Playable root = new Playable();

            using (ListPool<Component>.Get(out List<Component> components))
            {
                gameObject.GetComponentsInChildren(true, components);

                foreach (Component component in components)
                {
                    Type type = component.GetType();

                    if (creators.TryGetValue(type, out Actions actions))
                    {
                        Playable child = new Playable(
                            () => actions.Play(component), 
                            () => actions.Stop(component),
                            () => actions.Pause(component),
                            () => actions.Resume(component),
                            () => actions.IsPlaying(component));

                        root.Add(child);
                    }
                }
            }

            return root;
        }

        public static Playable Create<T>(
            GameObject gameObject, 
            Action<T> play, 
            Action<T> stop = null, 
            Action<T> pause = null, 
            Action<T> resume = null, 
            Func<T, bool> isPlaying = null) where T : Component
        {
            Playable root = new Playable();

            stop ??= _ => { };
            pause ??= _ => { };
            resume ??= _ => { };
            isPlaying ??= _ => false;

            using (ListPool<T>.Get(out List<T> components))
            {
                gameObject.GetComponentsInChildren(components);

                foreach (T component in components)
                {
                    Playable child = new Playable(
                        () => play(component), 
                        () => stop(component),
                        () => pause(component),
                        () => resume(component),
                        () => isPlaying(component));

                    root.Add(child);
                }
            }

            return root;
        }


    }
}
