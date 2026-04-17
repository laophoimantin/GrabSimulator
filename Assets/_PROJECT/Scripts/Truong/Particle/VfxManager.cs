using System.Collections.Generic;
using UnityEngine;

public class VfxManager : Singleton<VfxManager>
{
    [System.Serializable]
    public class VfxPool
    {
        public VfxType Type;
        public ParticleSystem Prefab;
        public int InitialSize = 5; 
    }

    [Header("Effects")]
    [SerializeField] private List<VfxPool> _pools;

    private Dictionary<VfxType, Queue<ParticleSystem>> _poolDictionary;

    void Start()
    {
        InitializePools();
    }

    private void InitializePools()
    {
        _poolDictionary = new Dictionary<VfxType, Queue<ParticleSystem>>();

        foreach (var pool in _pools)
        {
            Queue<ParticleSystem> objectPool = new Queue<ParticleSystem>();

            for (int i = 0; i < pool.InitialSize; i++)
            {
                ParticleSystem obj = Instantiate(pool.Prefab, transform);
                obj.gameObject.SetActive(false);
                objectPool.Enqueue(obj);
            }

            _poolDictionary.Add(pool.Type, objectPool);
        }
    }

    public void PlayVFX(VfxType type, Vector3 position, Quaternion rotation = default)
    {
        if (!_poolDictionary.ContainsKey(type))
        {
            return;
        }

        Queue<ParticleSystem> pool = _poolDictionary[type];

        ParticleSystem vfx = pool.Dequeue();

        if (vfx.gameObject.activeInHierarchy)
        {
            pool.Enqueue(vfx);

            VfxPool poolConfig = _pools.Find(p => p.Type == type);
            vfx = Instantiate(poolConfig.Prefab, transform);
        }

        vfx.transform.position = position;
        if (rotation != default) vfx.transform.rotation = rotation;

        vfx.gameObject.SetActive(true);
        vfx.Play();

        pool.Enqueue(vfx);
    }
}

public enum VfxType
{
    DeliverSuccess, 
}