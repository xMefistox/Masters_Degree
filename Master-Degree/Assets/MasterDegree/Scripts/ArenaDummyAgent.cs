using System.Collections.Generic;
using UnityEngine;
using MLAgents;


public class ArenaDummyAgent : Agent
{
	public float checkSpawnRadius;
    public Bounds AreaBounds;
    public GameObject ground;
	[SerializeField]
	private Rigidbody _agentRigidbody;
	public int Health
	{
		get
		{
			return _health;
		}
		private set
		{
			_health = value;
			if (_health <= 0)
			{
				Death();
			}
		} 
	}
	private int _health;

	[SerializeField] private int _maxHealth;

	public void LoseHealth()
	{
		Health--;		
	}

	public override void InitializeAgent()
	{
		AgentReset();
	}

	private void Death()
	{
		//gameObject.SetActive(false);
		//Done();
		AgentReset();
	}

	public override void AgentAction(float[] vectorAction, string textAction)
	{
	}

	public override void CollectObservations()
	{
		AddVectorObs(Health/_maxHealth);
	}

	/// <summary>
	/// Use the ground's bounds to pick a random spawn position.
	/// </summary>
	public Vector3 GetRandomSpawnPos()
	{
		LayerMask layerMask = LayerMask.NameToLayer("wall");
		Vector3 randomSpawnPos = Vector3.zero;;
		bool foundNewSpawnLocation = false;
		while (foundNewSpawnLocation == false)
		{
			float randomPosX = Random.Range(-AreaBounds.extents.x, AreaBounds.extents.x);

			float randomPosZ = Random.Range(-AreaBounds.extents.z, AreaBounds.extents.z);
			randomSpawnPos = ground.transform.localPosition + new Vector3(randomPosX, 1.1f, randomPosZ);
			if (Physics.CheckSphere(randomSpawnPos, checkSpawnRadius) == false)
			{
				foundNewSpawnLocation = true;
			}
		}
		return randomSpawnPos;
	}

	public override void AgentReset()
	{
		base.AgentReset();

		float randomRotation = Random.Range(0f, 359f);
		transform.localPosition = GetRandomSpawnPos();
		transform.rotation = new Quaternion(0f,0f,0f,0f);
		_agentRigidbody.velocity = Vector3.zero;
		_agentRigidbody.angularVelocity = Vector3.zero;
		transform.Rotate(Vector3.up,randomRotation);
		Health = _maxHealth;
	}
}
