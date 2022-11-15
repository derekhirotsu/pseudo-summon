using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LariatBullet : PooledPlayerBullet
{
    [SerializeField] private float _rotationSpeed = 360f;

    protected override void FixedUpdate()
    {
        if (_timeAlive >= _timeToLive)
        {
            gameObject.SetActive(false);
            return;
        }

        _timeAlive += Time.fixedDeltaTime;


        transform.position += (_moveSpeed * Time.fixedDeltaTime * transform.forward);
        transform.Rotate(0, 0, _rotationSpeed * Time.fixedDeltaTime);
    }
}
