using System;
using UnityEngine;

public class PooledPlayerBullet : MonoBehaviour
{
    [SerializeField] protected float _moveSpeed;
    [SerializeField] protected float _timeToLive;

    protected LayerMask _mask;
    protected float _timeAlive = 0f;

    public EventHandler<BulletCollisionEvent> BulletCollided;

    protected void OnDisable()
    {
        _timeAlive = 0f;
    }

    protected virtual void FixedUpdate()
    {
        if (_timeAlive >= _timeToLive)
        {
            gameObject.SetActive(false);
            return;
        }

        _timeAlive += Time.fixedDeltaTime;


        transform.position += (_moveSpeed * Time.fixedDeltaTime * transform.forward);
    }

    protected void OnTriggerEnter(Collider other)
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