using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PseudoSummon
{
    public class PlayerBusterAttack : MonoBehaviour
    {
        [SerializeField] int bulletDamage = 1;
        [SerializeField] float timeToLive;
        [SerializeField] LayerMask wallCollisionLayer;
        [SerializeField] protected LayerMask targetLayer;

        [SerializeField] protected GameObject optionalDeathParticle;
        protected bool directionSet = false;

        // The list of colliders currently inside the trigger
        [SerializeField] protected List<Collider> triggerList;

        Vector3 direction;

        //public Action CallbackTest;

        //public void SetCallback(Action callback)
        //{
        //    CallbackTest = callback;
        //}

        void Start()
        {
            triggerList = new List<Collider>();
            Destroy(gameObject, timeToLive);
        }

        void FixedUpdate()
        {
            transform.position += transform.up * 0.1f * Time.unscaledDeltaTime;
        }

        void OnDisable()
        {
            if (optionalDeathParticle != null)
            {
                GameObject particle = Instantiate(optionalDeathParticle, transform.position, transform.rotation);
                Destroy(particle, 0.8f);
            }

            //CallbackTest = null;
        }

        void OnTriggerEnter(Collider entity)
        {
            if (targetLayer.Contains(entity.gameObject.layer) && !triggerList.Contains(entity))
            {
                triggerList.Add(entity);
            }
        }

        void OnTriggerExit(Collider entity)
        {
            if (triggerList.Contains(entity))
            {
                triggerList.Remove(entity);
            }
        }

        void OnTriggerStay(Collider entity)
        {
            if (targetLayer.Contains(entity.gameObject.layer) && !triggerList.Contains(entity))
            {
                triggerList.Add(entity);
            }
        }

        public void DamageAllCollisions()
        {
            //CallbackTest?.Invoke();

            foreach (Collider target in triggerList)
            {
                if (target == null)
                {
                    continue;
                }
                Health entityHealth = target.gameObject.GetComponent<Health>();
                if (entityHealth != null)
                {
                    if (entityHealth.CurrentHealthPercent < 0.2f)
                    {
                        entityHealth.ModifyCurrentHealth(-bulletDamage * 2);
                    }
                    else
                    {
                        entityHealth.ModifyCurrentHealth(-bulletDamage);
                    }

                    GameManager.Instance.AddScore(8000, true);
                }
            }
        }

        public void SetDirection(Vector3 newDirection)
        {
            direction = newDirection;
            directionSet = true;
        }

        public void SetTargetLayer(LayerMask targets)
        {
            targetLayer = targets;
        }
    }
}