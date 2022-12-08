using PseudoSummon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireFieldProjectile : MonoBehaviour
{
    [SerializeField] private int bulletDamage = 1;
    [SerializeField] protected LayerMask targetLayer;
    [SerializeField] private ParticleSystem particles;

    private void Start()
    {
        transform.localScale = new Vector3(0f, 0.05f, 0f);
    }

    private void FixedUpdate()
    {
        if (transform.localScale.x < 1f)
        {
            transform.localScale += new Vector3(0.02f, 0f, 0.02f);
        }
    }

    private void OnTriggerStay(Collider entity)
    {
        if (targetLayer.Contains(entity.gameObject.layer))
        {
            Health entityHealth = entity.gameObject.GetComponent<Health>();
            if (entityHealth != null)
            {
                entityHealth.ModifyCurrentHealth(-bulletDamage);
            }
        }
    }

    public void SetParticleEmission(bool shouldEmit)
    {
        var em = particles.emission;
        em.enabled = shouldEmit;
    }
}
