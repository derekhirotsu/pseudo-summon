using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledPlayerSpell : MonoBehaviour
{
    [SerializeField] private GameObject _bullet;
    [SerializeField] private LayerMask _layerMask;

    private ObjectPool _pool;

    private void Awake()
    {
        _pool = GetComponent<ObjectPool>();
    }

    private void Start()
    {
        _pool.InitializeObjectPool(_bullet);
        foreach (GameObject gameObject in _pool.Pool)
        {
            PooledPlayerBullet bullet = gameObject.GetComponent<PooledPlayerBullet>();

            if (bullet != null)
            {
                bullet.BulletCollided += OnBulletCollision;
                bullet.SetLayerMask(_layerMask);
            }
        }
    }

    private void OnBulletCollision(object sender, BulletCollisionEvent e)
    {
        PooledPlayerBullet b = sender as PooledPlayerBullet;

        b.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Fire(transform.position, transform.rotation);
        }
    }

    public void OnAttackDown()
    {

    }

    public void OnAttackUp()
    {

    }

    private void Fire(Vector3 position, Quaternion rotation)
    {
        GameObject bullet = _pool.GetPooledObject();

        if (bullet == null)
        {
            Debug.LogWarning("No available objects found in pool.", this);
            return;
        }

        bullet.transform.position = position;
        bullet.transform.rotation = rotation;
        bullet.SetActive(true);
    }
}
