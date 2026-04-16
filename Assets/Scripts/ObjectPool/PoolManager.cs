using System.Collections.Generic;
using Core;
using UnityEngine;

namespace ObjectPool
{
    public class PoolManager : MonoBehaviour
    {
        #region Singleton

        public static PoolManager Instance;

        #endregion

        #region Runtime

        private readonly Dictionary<GameObject, Queue<GameObject>> _pools = new();
        private readonly Dictionary<GameObject, GameObject> _instanceToPrefab = new();
        private readonly Dictionary<GameObject, Transform> _instanceToDefaultParent = new();
        private GameSession _session;

        #endregion

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public void Configure(GameSession session)
        {
            _session = session;
        }

        public void ResetRun()
        {
            _pools.Clear();
            _instanceToPrefab.Clear();
            _instanceToDefaultParent.Clear();
        }

        public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            if (prefab == null)
                return null;

            if (!_pools.TryGetValue(prefab, out var pool))
            {
                pool = new Queue<GameObject>();
                _pools[prefab] = pool;
            }

            GameObject instance;

            if (pool.Count > 0)
            {
                instance = pool.Dequeue();
                instance.SetActive(true);
            }
            else
            {
                instance = Instantiate(prefab);
                _instanceToPrefab[instance] = prefab;
            }

            var spawnParent = parent != null ? parent : transform;
            _instanceToDefaultParent[instance] = spawnParent;
            instance.transform.SetParent(spawnParent, false);
            instance.transform.SetPositionAndRotation(position, rotation);
            return instance;
        }

        public void Despawn(GameObject instance)
        {
            if (instance == null)
                return;

            if (!_instanceToPrefab.TryGetValue(instance, out var prefab))
            {
                Destroy(instance);
                return;
            }

            instance.SetActive(false);

            if (_instanceToDefaultParent.TryGetValue(instance, out var parent) && parent != null)
                instance.transform.SetParent(parent, false);
            else
                instance.transform.SetParent(transform, false);

            _pools[prefab].Enqueue(instance);
        }
    }
}
