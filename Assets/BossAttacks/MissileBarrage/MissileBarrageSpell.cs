using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileBarrageSpell : BaseSpell
{
    [SerializeField] private SoundFile missileBarrageFireSfx;
    [SerializeField] private AudioProvider _audio;
    [SerializeField] private float _randomSpreadDegrees;
    [SerializeField] private float _fireInterval;
    private float _fireCooldown = 0f;

    public override void OnAttackDown()
    {
        _isFiring = true;
        //vfx_missileCastObject.SetActive(true);
    }

    public override void OnAttackUp()
    {
        _isFiring = false;
        //vfx_missileCastObject.SetActive(false);
    }

    protected override void Update()
    {
        if (_fireCooldown > 0f)
        {
            _fireCooldown -= Time.deltaTime;
        }

        if (!IsReadyToFire())
        {
            return;
        }

        if (SpawnInstance(transform.position, transform.rotation))
        {
            _fireCooldown = _fireInterval;
            _audio.PlaySound(missileBarrageFireSfx);
            //Instantiate(vfx_iceShardCast, autoWindupOrb.transform);
        }
    }
    private bool IsReadyToFire()
    {
        return CanFire && _fireCooldown <= 0f && _isFiring;
    }

    protected override bool SpawnInstance(Vector3 position, Quaternion rotation)
    {
        GameObject pooledObject = _pool.GetPooledObject();

        if (pooledObject == null)
        {
            Debug.LogWarning("No available objects found in pool.", this);
            return false;
        }

        float deviance = Random.Range(-_randomSpreadDegrees, _randomSpreadDegrees);
        Quaternion q = Quaternion.AngleAxis(deviance, transform.up);

        pooledObject.transform.position = position;
        pooledObject.transform.rotation = q *= rotation;
        pooledObject.SetActive(true);
        return true;
    }
}
