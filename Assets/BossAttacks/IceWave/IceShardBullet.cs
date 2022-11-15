using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceShardBullet : PooledPlayerBullet
{
    [SerializeField] private Transform _projectileVisual;
    [SerializeField] private float _rotationSpeed = 360f;

    private void Start()
    {
        _projectileVisual.Rotate(0, 0, Random.Range(0, 360));
    }

    protected override void FixedUpdate()
    {
        if (_timeAlive >= _timeToLive)
        {
            gameObject.SetActive(false);
            return;
        }

        _timeAlive += Time.fixedDeltaTime;


        transform.position += (_moveSpeed * Time.fixedDeltaTime * transform.forward);
        _projectileVisual.Rotate(0, 0, _rotationSpeed * Time.fixedDeltaTime);
    }
}
