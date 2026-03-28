using System.Collections.Generic;
using UnityEngine;

namespace ObjectPool
{
    public class PoolManager : MonoBehaviour
    {
        public static PoolManager Instance;

        private Dictionary<GameObject, Queue<GameObject>> _pools = new();
        private Dictionary<GameObject, GameObject> _instanceToPrefab = new();

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public GameObject Spawn(GameObject prefab, Vector3 pos, Quaternion rot)
        {
            if (!_pools.TryGetValue(prefab, out var pool))
            {
                pool = new Queue<GameObject>();
                _pools[prefab] = pool;
            }

            GameObject obj;

            if (pool.Count > 0)
            {
                obj = pool.Dequeue();
                obj.SetActive(true);
            }
            else
            {
                obj = Instantiate(prefab, transform);
                _instanceToPrefab[obj] = prefab;
            }

            obj.transform.SetPositionAndRotation(pos, rot);
            obj.transform.SetParent(transform);

            return obj;
        }

        public void Despawn(GameObject obj)
        {
            if (!_instanceToPrefab.TryGetValue(obj, out var prefab))
            {
                Destroy(obj);
                return;
            }

            obj.SetActive(false);
            _pools[prefab].Enqueue(obj);
        }
    }
}