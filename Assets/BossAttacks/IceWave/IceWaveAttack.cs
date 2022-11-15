using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

public class IceWaveAttack : BaseSpell
{
    [SerializeField] private AudioProvider _audio;
    [SerializeField] private SoundFile iceWaveFireSfx;

    [SerializeField] private int _projectilesPerShot;
    [Range(0f, 360f)][SerializeField] private float _arcDegrees;

    [SerializeField] private float _randomSpreadDegrees;
    [SerializeField] private float _fireInterval;
    private float _fireCooldown = 0f;

    private float halfArcDegrees
    {
        get { return _arcDegrees / 2; }
    }

    private float intervals
    {
        get { return _projectilesPerShot - 1; }
    }

    private float degreesPerInterval
    {
        get { return _arcDegrees / intervals; }
    }

    private bool projectSingleVector
    {
        get { return _projectilesPerShot == 1 || _arcDegrees <= 0f; }
    }

    public override void OnAttackDown()
    {
        _isFiring = true;
        //vfx_iceShardCastObject.SetActive(true);
    }

    public override void OnAttackUp()
    {
        _isFiring = false;
        //vfx_iceShardCastObject.SetActive(false);
    }

    protected override void Start()
    {
        base.Start();
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

        if (SpawnInstance(transform.position, transform.rotation)) {
            _fireCooldown = _fireInterval;
            _audio.PlaySound(iceWaveFireSfx);
            //Instantiate(vfx_iceShardCast, autoWindupOrb.transform);
        }
    }

    private bool IsReadyToFire()
    {
        return CanFire && _fireCooldown <= 0f && _isFiring;
    }

    protected override bool SpawnInstance(Vector3 position, Quaternion rotation)
    {
        List<GameObject> pooledObjects = _pool.GetPooledObjects();

        if (pooledObjects.Count < _projectilesPerShot)
        {
            Debug.LogWarning("Not enough available objects found in pool. Found: " + pooledObjects.Count, this);
            return false;
        }

        for (int i = 0; i < _projectilesPerShot; i++)
        {
            GameObject pooledObject = pooledObjects[i];

            float deviance = Random.Range(-_randomSpreadDegrees, _randomSpreadDegrees);
            float offset = i * degreesPerInterval + deviance - halfArcDegrees;
            Quaternion q = Quaternion.AngleAxis(offset, transform.up);

            pooledObject.transform.position = position;
            pooledObject.transform.rotation = q *= rotation;
            pooledObject.SetActive(true);
        }

        return true;
    }
}
