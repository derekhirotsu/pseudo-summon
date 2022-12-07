using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PseudoSummon
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] protected int damage;
        [SerializeField] protected float speed;
        [SerializeField] protected float timeToLive;
        [SerializeField] protected LayerMask collisionMask;

        public LayerMask CollisionMask 
        {
            get { return collisionMask; }
            set { collisionMask = value; }
        }

        public float Speed
        {
            get { return speed; }
            set { speed = value; }
        }

        protected Vector3 direction;
        protected float timeAlive = 0f;

        protected virtual void FixedUpdate()
        {
            if (timeAlive > timeToLive)
            {
                HandleTimeout();
            }

            timeAlive += Time.fixedDeltaTime;
            transform.position += direction * speed * Time.fixedDeltaTime;
        }

        protected virtual void OnTriggerEnter(Collider entity)
        {
            if (collisionMask.Contains(entity.gameObject.layer))
            {
                Destroy(gameObject);
            }
        }

        protected virtual void HandleTimeout()
        {
            Destroy(gameObject);
        }

        public virtual void SetDirection(Vector3 newDirection)
        {
            direction = newDirection.normalized;
            transform.rotation = Quaternion.LookRotation(direction);
        }

    }
}
