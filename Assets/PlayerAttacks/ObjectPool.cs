using System.Collections.Generic;
using UnityEngine;

public class ObjectPool: MonoBehaviour
{
    [SerializeField] private int _poolAmount;
    private List<GameObject> _pool;
    private GameObject _objectContainer;

    public List<GameObject> Pool 
    { 
        get { return _pool; }
    }

    public void InitializeObjectPool(GameObject pooledObject)
    {
        _objectContainer = new GameObject("Object Pool - " + gameObject.name);
        _pool = new List<GameObject>(_poolAmount);

        GameObject instance;
        for (int i = 0; i < _poolAmount; i++)
        {
            instance = Instantiate(pooledObject, _objectContainer.transform, true);
            instance.SetActive(false);
            _pool.Add(instance);
        }
    }

    public GameObject GetPooledObject()
    {
        return _pool.Find(obj => !obj.activeInHierarchy);
    }

    public List<GameObject> GetPooledObjects()
    {
        return _pool.FindAll(obj => !obj.activeInHierarchy);
    }

    private void OnDestroy()
    {
        Destroy(_objectContainer);
    }
}
