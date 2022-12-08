using UnityEngine;

namespace PseudoSummon
{
    public class LariatProjectile : Projectile
    {
        [SerializeField] private float _rotationSpeed;
        private Health _health;

        private void Awake()
        {
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

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            transform.Rotate(0, _rotationSpeed * Time.fixedDeltaTime, 0);
        }

        protected override void OnTriggerEnter(Collider entity)
        {
            if (collisionMask.Contains(entity.gameObject.layer))
            {
                Health health = entity.gameObject.GetComponent<Health>();

                if (health)
                {
                    health.ModifyCurrentHealth(-damage);
                }

                Destroy(gameObject);
            }
        }

        private void CastOutward()
        {
            SubLariatProjectile[] _subLariats = GetComponentsInChildren<SubLariatProjectile>();

            foreach (SubLariatProjectile projectile in _subLariats)
            {
                Vector3 outwardVector = (projectile.transform.position - transform.position).normalized;
                Debug.DrawRay(transform.position, outwardVector * 20);
                projectile.SetDirection(outwardVector);
                projectile.CollisionMask = collisionMask;
                projectile.Launched = true;
            }

            transform.DetachChildren();
        }

        private void OnCurrentHealthModified(int difference)
        {
            if (difference >= 0)
            {
                return;
            }

            if (_health.CurrentHealth <= 0)
            {
                CastOutward();
                Destroy(gameObject);
            }
        }
    }
}
