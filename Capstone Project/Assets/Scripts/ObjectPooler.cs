using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{

    public GameObject pooledObject;

    public int pooledAmount;

    protected List<GameObject> pooledObjects;

	void Awake () {
		pooledObjects = new List<GameObject>();

	    for (int i = 0; i < pooledAmount; i++)
	    {
	        instantiateObject();
	    }
	}

    public virtual GameObject GetPooledObject()
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            // If pooled object is not activated in scene, return it's reference
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }
        return instantiateObject();
    }

    protected virtual GameObject instantiateObject()
    {
        GameObject obj = Instantiate(pooledObject) as GameObject;
        // Deactivating platform
        obj.SetActive(false);
        // Adding it's reference on pooledObjects list
        pooledObjects.Add(obj);
        // Returning it's reference
        return obj;
    }
}
