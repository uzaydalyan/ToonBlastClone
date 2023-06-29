using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
    public class AnimationManager : MonoBehaviour
    {
        public static AnimationManager Instance;
        
        [SerializeField] private GameObject particlePrefab;
        [SerializeField] private Sprite[] particleSprites;

        [SerializeField] private Transform _particleParent;

        [SerializeField] private Sprite[] _itemSprites;
        [SerializeField] private Transform goalAnimationParent;
        [SerializeField] private Transform goalsUIParent;
        
        private void Awake()
        {
            Instance = this;

            Sprite x = _itemSprites[0];
            Debug.Log(x.name);
        }
        
        public IEnumerator CubeSpreadParticles(Vector3 position, ItemType type)
        {

            for (int i = 0; i < 15; i++)
            {
                Sprite sprite = particleSprites[Random.Range(0, particleSprites.Length)];
                SpriteRenderer particleSpriteRenderer = particlePrefab.GetComponent<SpriteRenderer>();
                particleSpriteRenderer.sprite = sprite;
                particleSpriteRenderer.color = CubeColors.GetCubeColor(type); 
                GameObject particle = Instantiate(particlePrefab, position, Quaternion.identity, transform);
                Rigidbody2D rigidbody = particle.GetComponent<Rigidbody2D>();
                rigidbody.AddForce(particle.transform.position, ForceMode2D.Force);
            }

            yield return new WaitForSeconds(0.4f);
            foreach (Transform particle in transform)
            {
                Destroy(particle.gameObject);
            }
        }

        public void GoalAnimations(Vector3 position, ItemType type, Vector3 uiGoalPosition)
        {
            switch(type) 
            {
                case ItemType.g: case ItemType.b: case ItemType.p: case ItemType.r: case ItemType.y:
                    StartCoroutine(CubeGoalAnimations(position, type, uiGoalPosition));
                    break;
                default:
                    break;
            }
        }
        
        public IEnumerator CubeGoalAnimations(Vector3 position, ItemType type, Vector3 UiGoalPosition)
        {
            String itemName = type.ToString();
            Sprite sprite = Resources.Load<Sprite>("Sprites/" + itemName);
            
            var gameObject = new GameObject ();
            var spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sortingLayerName = "animation";
            spriteRenderer.sprite = sprite;
            gameObject.transform.position = position - new Vector3(-0.1f, 0.1f, 0);
            gameObject.transform.localScale = Vector3.zero;

            gameObject.GetComponent<Transform>().DOScale(Vector3.one, 0.3f);
            
            gameObject.GetComponent<Transform>().DOMove(UiGoalPosition, 0.8f)
                .SetDelay(0.2f).SetEase(Ease.InBack);
            
            yield return new WaitForSeconds(1.05f);
            
            Destroy(gameObject);
        }
        
    }
}