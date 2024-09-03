using UnityEngine;
using UnityEngine.AI;

namespace HHG.Common.Runtime
{
    public static class NavMeshAgentExtensions
    {
        private const float sampleDistance = 1000f;
        private const float agentDrift = .0001f;

        public static bool CalculatePathSafe(this NavMeshAgent agent, Vector2 target, NavMeshPath path)
        {
            return NavMesh.SamplePosition(target, out NavMeshHit hit, sampleDistance, agent.areaMask) && agent.CalculatePath(hit.position, path);
        }

        public static bool SetDestinationSample(this NavMeshAgent agent, Vector3 destination)
        {
            // Not sampling position first tends to cause setting the destination to fail for whatever reason
            if (NavMesh.SamplePosition(destination, out NavMeshHit hit, sampleDistance, agent.areaMask))
            {
                destination = hit.position;

                agent.SetDestinationFix(destination);

                return true; // SetDestination can return false sometimes, not sure why
            }

            return false;
        }

        public static bool SetDestinationFix(this NavMeshAgent agent, Vector3 destination)
        {
            // Fixes a known issue: https://github.com/h8man/NavMeshPlus/wiki/HOW-TO#known-issues
            if (Mathf.Abs(agent.transform.position.x - destination.x) < agentDrift)
            {
                agent.Warp(agent.transform.position + new Vector3(agentDrift, 0f, 0f));
            }

            return agent.SetDestination(destination);
        }

        public static bool HasReachedDestination(this NavMeshAgent agent)
        {
            return !agent.pathPending && (!agent.hasPath || agent.velocity.sqrMagnitude == 0f || agent.remainingDistance <= agent.stoppingDistance || Vector3.Distance(agent.transform.position, agent.destination) <= agent.stoppingDistance);
        }

        public static bool HasPathOrPendingPath(this NavMeshAgent agent)
        {
            return agent.hasPath || agent.pathPending;
        }

        public static Vector3 GetDirection(this NavMeshAgent agent)
        {
            return agent.velocity.normalized;
        }

        public static void WarpToNavMesh(this NavMeshAgent agent)
        {
            if (!agent.isOnNavMesh)
            {
                if (NavMesh.SamplePosition(agent.transform.position, out NavMeshHit hit, sampleDistance, NavMesh.AllAreas))
                {
                    agent.Warp(hit.position);
                }
            }
        }
    }
}