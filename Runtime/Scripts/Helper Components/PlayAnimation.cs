using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace HHG.Common.Runtime
{
    [RequireComponent(typeof(Animator))]
    public class PlayAnimation : MonoBehaviour
    {
        [SerializeField] private AnimationClip animationClip;
        [SerializeField] private DirectorUpdateMode updateMode = DirectorUpdateMode.GameTime;

        private PlayableGraph playableGraph;

        private void Start()
        {
            playableGraph = PlayableGraph.Create();
            playableGraph.SetTimeUpdateMode(updateMode);

            AnimationPlayableOutput playableOutput = AnimationPlayableOutput.Create(playableGraph, "Animation", GetComponent<Animator>());

            // Wrap the clip in a playable.
            AnimationClipPlayable clipPlayable = AnimationClipPlayable.Create(playableGraph, animationClip);

            // Connect the Playable to an output.
            playableOutput.SetSourcePlayable(clipPlayable);

            // Plays the Graph.
            playableGraph.Play();
        }

        private void OnDestroy()
        {
            // Destroys all Playables and PlayableOutputs created by the graph.
            playableGraph.Destroy();
        }
    } 
}