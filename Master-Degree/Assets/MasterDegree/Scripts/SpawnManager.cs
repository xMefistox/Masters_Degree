using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
	public static SpawnManager Instance;
	[SerializeField] private List<AttackOrb> _pooledObjects = new List<AttackOrb>();
	[SerializeField] private AttackOrb attackOrbPrefab;

	private void Start()
	{
		Instance = this;
	}

	public AttackOrb GetAttackOrb()
	{
		AttackOrb poolFoundScript = _pooledObjects.Find(poolObject => !poolObject.gameObject.activeSelf);
		if (poolFoundScript != null)
		{
			return poolFoundScript;
		}
		else
		{
			AttackOrb attackOrb = Instantiate(attackOrbPrefab,transform);
			attackOrb.gameObject.SetActive(false);
			_pooledObjects.Add(attackOrb);
			return attackOrb;
		}
	}

	public void HideAttackOrb(AttackOrb pooledObject)
	{
		//pooledObject.transform.localPosition = new Vector3(0f, 100f, 0f);
		pooledObject.GetComponent<TrailRenderer>().Clear();
		pooledObject.gameObject.SetActive(false);
	}

	public void ShowAttackOrb(AttackOrb pooledObject)
	{
		pooledObject.GetComponent<TrailRenderer>().Clear();
		pooledObject.gameObject.SetActive(true);
	}
}