using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
    public class ParticleCreator : MonoBehaviour
    {
        public static ParticleCreator Instance;
        
        [SerializeField] private GameObject particlePrefab;
        [SerializeField] private Sprite[] particleSprites;

        [SerializeField] private Transform _particleParent;
        
        private void Awake()
        {
            Instance = this;
        }

        public void CubeDestroyAction(Vector3 position, ItemType type)
        {
            StartCoroutine(SpreadParticles(position, type));
        }
        
        public IEnumerator SpreadParticles(Vector3 position, ItemType type)
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
        
    }
}