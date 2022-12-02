using PseudoSummon.Audio;
using System.Collections.Generic;
using UnityEngine;

namespace PseudoSummon
{
    public class auto_Lariat : MonoBehaviour
    {
        [SerializeField] private float rotationSpeed = 200;
        [SerializeField] private SoundFile hitSfx;
        [SerializeField] private List<TestBullet> childProjectiles;

        private Health _health;
        private AudioProvider _audio;

        private float yRotation = 0;

        private void Awake()
        {
            _audio = GetComponent<AudioProvider>();
            _health = GetComponent<Health>();
        }

        private void OnEnable()
        {
            _health.CurrentHealthModified += OnCurrentHealthModified;
        }

        private void OnDisable()
        {
            _health.CurrentHealthModified -= OnCurrentHealthModified;
        }

        private void Start()
        {
            foreach (Transform child in transform)
            {
                TestBullet projectile = child.GetComponent<TestBullet>();

                if (projectile != null)
                {
                    childProjectiles.Add(projectile);
                }
            }
        }

        private void FixedUpdate()
        {
            float diff = Time.fixedDeltaTime * rotationSpeed;
            yRotation = (yRotation + diff) % 360;

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, yRotation, 0), 1f);
        }

        private void CastOutward()
        {
            transform.DetachChildren();

            for (int i = childProjectiles.Count - 1; i >= 0; --i)
            {
                if (childProjectiles[i] != null)
                {
                    Vector3 outwardVector = (childProjectiles[i].transform.position - transform.position).normalized;
                    childProjectiles[i].SetDirection(outwardVector);
                }
            }

            Destroy(gameObject);
        }

        private void OnCurrentHealthModified(int difference)
        {
            if (difference >= 0)
            {
                return;
            }

            _audio.PlaySound(hitSfx);

            if (_health.CurrentHealth <= 0)
            {
                CastOutward();
            }
        }
    }
}