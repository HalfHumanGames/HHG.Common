using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace HHG.Common.Runtime
{
    public class GhostSprites : MonoBehaviour
    {
        private static Lazy<Transform> _container = new Lazy<Transform>();
        private static Transform container => _container.FromFindOrCreate("Ghost Sprites");
        private static SpriteRenderer template;

        [SerializeField] private int ghostCount = 10;
        [SerializeField] private float spacing = .1f;
        [SerializeField] private float fadeDuration = .1f;
        [SerializeField] private Color color;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Material material;

        private GameObjectPool<SpriteRenderer> pool;
        private Queue<SpriteRenderer> ghosts;
        private SpriteRenderer ghostsTail;
        private SortingGroup sortingGroup;
        private Color startColor;
        private float fadeSpeed;

        private void Start()
        {
            if (template == null)
            {
                CreateTemplate();
            }

            pool = GameObjectPool.GetPool(nameof(GhostSprites), template, container, Debug.isDebugBuild, ghostCount, 10000, true);
            ghosts = new Queue<SpriteRenderer>(ghostCount);

            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponentInChildren<SpriteRenderer>(true);
            }

            sortingGroup = GetComponentInChildren<SortingGroup>(true);

            ReinitializeVariables();
        }

        private void CreateTemplate()
        {
            template = new GameObject("Ghost Template").AddComponent<SpriteRenderer>();
            SetupGhost(template, default);
            template.gameObject.SetActive(false);
        }

        private void ReinitializeVariables()
        {
            spacing = Mathf.Max(spacing, .01f);
            fadeDuration = Mathf.Max(fadeDuration, .01f);
            startColor = spriteRenderer.color.WithA(color.a * spriteRenderer.color.a);
            fadeSpeed = 1f / fadeDuration * startColor.a;

            // For if change in Editor during Play Mode via the Inspector
            if (ghosts.Count > ghostCount)
            {
                ClearTrail();
                ghosts = new Queue<SpriteRenderer>(ghostCount);
            }
        }

        public void ClearTrail()
        {
            if (ghosts == null)
            {
                return;
            }

            while (ghosts.Count > 0)
            {
                ReleaseGhost();
            }
        }

        private void OnDisable()
        {
            ClearTrail();
        }

        private void OnDestroy()
        {
            ClearTrail();
        }

        private void Update()
        {

#if UNITY_EDITOR
            ReinitializeVariables();
#endif

            Vector3 position = transform.position;
            Vector3 ghostPosition = position;

            if (ghosts.Count > 0)
            {
                Vector3 previousPosition = ghostsTail.transform.position;
                float distance = Vector3.Distance(previousPosition, position);

                while (distance > spacing)
                {
                    ghostPosition = previousPosition + (position - previousPosition).normalized * spacing;
                    if (ghosts.Count >= ghostCount)
                    {
                        ReleaseGhost();
                    }
                    RetrieveGhost(ghostPosition);
                    previousPosition = ghostPosition;
                    distance -= spacing;
                }
            }
            else
            {
                RetrieveGhost(ghostPosition);
            }

            int release = 0;
            foreach (SpriteRenderer ghost in ghosts)
            {
                Color color = ghost.color;
                color.a -= fadeSpeed * Time.deltaTime;
                ghost.color = color;

                if (color.a <= 0f)
                {
                    release++;
                }
            }

            for (int i = 0; i < release; i++)
            {
                ReleaseGhost();
            }
        }

        private void RetrieveGhost(Vector3 ghostPosition)
        {
            SpriteRenderer ghost = pool.Get();
            SetupGhost(ghost, ghostPosition);
            ghost.gameObject.SetActive(true);
            ghosts.Enqueue(ghost);
            ghostsTail = ghost;
        }

        private void ReleaseGhost()
        {
            SpriteRenderer ghost = ghosts.Dequeue();

            // Can be null when application is quitting
            if (ghost != null)
            {
                ghost.gameObject.SetActive(false);
                pool.Release(ghost);
            }
        }

        private void SetupGhost(SpriteRenderer ghost, Vector3 position)
        {
            ghost.transform.SetParent(container);
            ghost.transform.SetGlobalScale(transform.lossyScale);
            ghost.transform.position = position;
            ghost.transform.rotation = transform.rotation;
            ghost.color = startColor;
            ghost.sprite = spriteRenderer.sprite;
            ghost.sortingLayerID = sortingGroup == null ? spriteRenderer.sortingLayerID : sortingGroup.sortingLayerID;
            ghost.sortingOrder = sortingGroup == null ? spriteRenderer.sortingOrder - 1 : sortingGroup.sortingOrder - 1;
            ghost.flipX = spriteRenderer.flipX;
            ghost.flipY = spriteRenderer.flipY;

            if (material != null)
            {
                ghost.material = material;
            }
        }

        private void OnValidate()
        {
            if (spacing <= 0)
            {
                spacing = .01f;
            }
            if (fadeDuration <= 0)
            {
                fadeDuration = .01f;
            }
        }
    }
}