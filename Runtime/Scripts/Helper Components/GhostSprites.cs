using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace HHG.Common.Runtime
{
    public class GhostSprites : MonoBehaviour
    {

        private static Lazy<Transform> _container = new Lazy<Transform>();
        private static Transform container => _container.FromGameObjectCreate("Ghost Sprites");

        [SerializeField] private int ghostCount = 10;
        [SerializeField] private float spacing = .1f;
        [SerializeField] private float fadeDuration = .1f;
        [SerializeField] private Color color;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Material material;

        private List<GameObject> ghostList = new List<GameObject>();
        private Color startColor;
        private float fadeSpeed;

        private void Start()
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>(true);
            }

            for (int i = 0; i < ghostCount; i++)
            {
                AddNewGhost(transform.position);
            }

            UpdateVariables();
        }

        private void UpdateVariables()
        {
            spacing = Mathf.Max(spacing, .01f);
            fadeDuration = Mathf.Max(fadeDuration, .01f);
            startColor = spriteRenderer.color.WithA(color.a * spriteRenderer.color.a);
            fadeSpeed = 1f / fadeDuration * startColor.a;
        }

        public void ClearTrail()
        {
            if (ghostList == null || ghostList.Count == 0)
            {
                return;
            }

            foreach (GameObject ghost in ghostList)
            {
                Destroy(ghost);
            }

            ghostList.Clear();
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
            UpdateVariables();
#endif

            if (ghostCount < ghostList.Count)
            {
                for (int i = ghostCount; i < ghostList.Count - 1; i++)
                {
                    GameObject toDestroy = ghostList[i];
                    Destroy(toDestroy);
                    ghostList.RemoveAt(i);
                }
            }

            Vector3 ghostPosition = transform.position;

            if (ghostList.Count > 0)
            {
                Vector3 previousPosition = ghostList[ghostList.Count - 1].transform.position;
                float distance = Vector3.Distance(previousPosition, transform.position);

                while (distance > spacing)
                {
                    ghostPosition = previousPosition + (transform.position - previousPosition).normalized * spacing;
                    AddGhost(ghostPosition);
                    previousPosition = ghostPosition;
                    distance -= spacing;
                }
            }
            else
            {
                AddGhost(ghostPosition);
            }

            for (int i = 0; i < ghostList.Count; i++)
            {
                SpriteRenderer ghostSprite = ghostList[i].GetComponent<SpriteRenderer>();
                Color color = ghostSprite.color;
                color.a -= fadeSpeed * Time.deltaTime;
                ghostSprite.color = color;
            }
        }

        private void AddGhost(Vector3 position)
        {
            if (ghostList.Count < ghostCount)
            {
                AddNewGhost(position);
            }
            else
            {
                AddExistingGhost(position);
            }
        }

        private void AddNewGhost(Vector3 position)
        {
            GameObject ghost = new GameObject(gameObject.name + " - GhostSprite");
            ghost.AddComponent<SpriteRenderer>();
            SetupGhost(ghost, position);
        }

        private void AddExistingGhost(Vector3 position)
        {
            GameObject recycle = ghostList[0];
            ghostList.RemoveAt(0);
            SetupGhost(recycle, position);
        }

        private void SetupGhost(GameObject ghost, Vector3 position)
        {
            ghost.transform.SetParent(container);
            ghost.transform.position = position;
            ghost.transform.SetGlobalScale(transform.lossyScale);
            ghost.transform.rotation = transform.rotation;

            SortingGroup sortingGroup = this.spriteRenderer.GetComponent<SortingGroup>();
            SpriteRenderer spriteRenderer = ghost.GetComponent<SpriteRenderer>();
            spriteRenderer.color = startColor;
            spriteRenderer.sprite = this.spriteRenderer.sprite;
            spriteRenderer.sortingLayerID = sortingGroup == null ? this.spriteRenderer.sortingLayerID : sortingGroup.sortingLayerID;
            spriteRenderer.sortingOrder = sortingGroup == null ? this.spriteRenderer.sortingOrder - 1 : sortingGroup.sortingOrder - 1;
            spriteRenderer.flipX = this.spriteRenderer.flipX;
            spriteRenderer.flipY = this.spriteRenderer.flipY;

            if (material != null)
            {
                spriteRenderer.material = material;
            }

            ghostList.Add(ghost);
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