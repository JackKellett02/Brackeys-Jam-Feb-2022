using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class to pool unity gameobjects.
/// </summary>
public class ObjectPool {
	#region Class Variables.

	private Queue<GameObject> m_gameObjectPool;
	private GameObject m_prefab = null;
	private Transform m_parent = null;
	#endregion

	#region Class Public Functions.

	public ObjectPool(Transform a_parent,GameObject a_prefab, int a_poolSize) {
		if (a_prefab != null)
		{
			m_prefab = a_prefab;
			m_gameObjectPool = new Queue<GameObject>();
			for (int i = 0; i < a_poolSize; i++)
			{
				//Instantiate the object and set it as a child of the parent.
				GameObject newPrefab = Object.Instantiate(a_prefab, a_parent);

				//Deactivate it.
				newPrefab.SetActive(false);

				//Add it to the pool.
				m_gameObjectPool.Enqueue(newPrefab);
			}
		}
	}

	public GameObject SpawnObject()
	{
		//Get the next object in the queue and make sure it is deactivated.
		GameObject newPrefab = m_gameObjectPool.Dequeue();
		newPrefab.SetActive(false);

		//Reactivate the object.
		newPrefab.SetActive(true);

		//Add it back to the queue.
		m_gameObjectPool.Enqueue(newPrefab);

		//Return it.
		return newPrefab;
	}
	#endregion

	#region Class Private Functions.

	#endregion
}
