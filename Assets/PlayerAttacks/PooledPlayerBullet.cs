using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class PooledPlayerBullet : MonoBehaviour
{
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _timeToLive;

    private LayerMask _mask;
    private float _timeAlive = 0f;

    public EventHandler<BulletCollisionEvent> BulletCollided;

    private void OnDisable()
    {
        _timeAlive = 0f;
    }

    private void FixedUpdate()
    {
        if (_timeAlive >= _timeToLive)
        {
            gameObject.SetActive(false);
            return;
        }

        _timeAlive += Time.fixedDeltaTime;


        transform.position += (_moveSpeed * Time.fixedDeltaTime * transform.forward);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_mask.Contains(other.gameObject.layer))
        {
            BulletCollided?.Invoke(this, new BulletCollisionEvent(other));
        }
    }

    public void SetDirection(Vector3 direction)
    {
        transform.rotation = Quaternion.LookRotation(direction);
    }

    public void SetLayerMask(LayerMask mask)
    {
        _mask = mask;
    }
}

public class BulletCollisionEvent : EventArgs
{
    private Collider _trigger;
    public Collider Trigger
    {
        get { return _trigger; }
    }

    public BulletCollisionEvent(Collider collider)
    {
        _trigger = collider;
    }
}