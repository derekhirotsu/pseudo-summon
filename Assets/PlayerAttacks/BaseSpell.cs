using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseSpell : MonoBehaviour
{
    [SerializeField] protected GameObject _spellInstance;
    [SerializeField] protected LayerMask _layerMask;

    // spell stats like damage, firerate, projectile amount per shot (?)

    protected ObjectPool _pool;

    protected virtual void Awake()
    {
        _pool = GetComponent<ObjectPool>();
    }

    protected virtual void Start()
    {
        _pool.InitializeObjectPool(_spellInstance);
        foreach (GameObject gameObject in _pool.Pool)
        {
            PooledPlayerBullet projectile = gameObject.GetComponent<PooledPlayerBullet>();

            if (projectile != null)
            {
                projectile.BulletCollided += OnBulletCollision;
                projectile.SetLayerMask(_layerMask);
            }
        }
    }

    protected abstract void Update();

    public abstract void OnAttackDown();

    public abstract void OnAttackUp();

    protected virtual bool SpawnInstance(Vector3 position, Quaternion rotation)
    {
        GameObject pooledObject = _pool.GetPooledObject();

        if (pooledObject == null)
        {
            Debug.LogWarning("No available objects found in pool.", this);
            return false;
        }

        pooledObject.transform.position = position;
        pooledObject.transform.rotation = rotation;
        pooledObject.SetActive(true);
        return true;
    }

    protected virtual void OnBulletCollision(object sender, BulletCollisionEvent e)
    {
        PooledPlayerBullet b = sender as PooledPlayerBullet;

        b.gameObject.SetActive(false);
    }
}
