using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public List<GameObject> prefab;
        public int size;
    }

    #region Singleton
    public static ObjectPooler Instance;
    private void Awake()
    {
        Instance = this;
    }
    #endregion

    public Dictionary<string, Queue<GameObject>> poolDictionary;
    public List<Pool> pools;

    // Start is called before the first frame update
    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i= 0; i <pool.size; i++)
            {
                // Creates a random prefab from the list of GameObject's provided
                GameObject obj = Instantiate(pool.prefab[UnityEngine.Random.Range(0, pool.prefab.Count)]);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    internal int PoolActiveCountForTag(string poolTag)
    {
        int count = 0;
        if (!poolDictionary.ContainsKey(poolTag))
        {
            Debug.Log("Did not find a pool for object: " + poolTag);
            return 0;
        }

        Queue<GameObject> queue = poolDictionary[poolTag];
        foreach (GameObject go in queue)
        {
            if(go.activeSelf)
            {
                count++;
            }
        }
        //Debug.Log("Count of Active Object: " + count);

        return count;
    }

    public GameObject SpawnFromPool (string tag, Vector2 position)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.Log("Did not find a pool for object: " + tag);
            return null;
        }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;

        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }

    public int GetSizeOfActivePool(string tag)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.Log("Did not find a pool for object: " + tag);
            return 0;
        }

        return poolDictionary[tag].Count;
    }
}
