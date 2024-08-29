using UnityEngine;
using UnityEngine.AI;

namespace HHG.Common.Runtime
{
    public class WaitUntilPathCalculated : CustomYieldInstruction
    {
        public override bool keepWaiting => agent.pathPending;

        private readonly NavMeshAgent agent;

        public WaitUntilPathCalculated(NavMeshAgent agent)
        {
            this.agent = agent;
        }
    }
}