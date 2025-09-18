using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance;

    [SerializeField]
    private List<PoolableObjectSO> poolsToCreate;

    private Dictionary<PoolableObjectSO, Queue<GameObject>> poolDictionary;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        poolDictionary = new Dictionary<PoolableObjectSO, Queue<GameObject>>();

        foreach (PoolableObjectSO so in poolsToCreate)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < so.poolSize; i++)
            {
                GameObject obj = Instantiate(so.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(so, objectPool);
        }
    }

    public GameObject SpawnFromPool(PoolableObjectSO type, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(type))
        {
            Debug.LogWarning("Pool para el tipo " + type.name + " no existe.");
            return null;
        }

        
        if (poolDictionary[type].Count == 0)
        {
            
            GameObject obj = Instantiate(type.prefab);
            obj.SetActive(false);
            poolDictionary[type].Enqueue(obj);
            Debug.LogWarning("Pool para " + type.name + " estaba vacía. Se expandió.");
        }

        GameObject objectToSpawn = poolDictionary[type].Dequeue();

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        

        return objectToSpawn;
    }

    
    public void ReturnToPool(PoolableObjectSO type, GameObject objectToReturn)
    {
       

        objectToReturn.SetActive(false);
        poolDictionary[type].Enqueue(objectToReturn);
    }
}