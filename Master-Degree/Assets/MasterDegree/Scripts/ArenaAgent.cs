using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using TMPro;

public class ArenaAgent : Agent
{
	[Header("Rewards")]
	public float RoundWonReward;
	public float TryingToShootReward;
	public float EnemyHitReward;
	public float AllyHitReward;
	public float MissedReward;
	public float MaxStepReward;
	public float DeathReward;
	public float EnemyInFrontReward;
	public float FacesWallReward;

	[Space]
	public TMP_Text _currentRewardText;
	public TMP_Text _LastRewardText;
	[SerializeField] private float _reloadTime;
	private float _attackReload;
	public float checkSpawnRadius;
	public Bounds AreaBounds;
	public GameObject ground;
	public bool UseVectorObs;
	[SerializeField]
	private RayPerception3D _rayPer;
	[SerializeField]private Rigidbody _agentRigidbody;
	public List<Agent> Enemies = new List<Agent>();
	public List<Agent> Allies = new List<Agent>();
	public float RotationSpeed = 200f;
	public float MovementSpeed = 5f;
	public float AttackOrbSecondsToLive = 2f;
	public float AttackOrbSpeed = 20f;
	public float Health
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

	private bool _isDead;
	private float _health;
	[SerializeField] private float _maxHealth;
	private float _rayDistance = 22f;
	private List<float> _observations;
	private List<AttackOrb> _attackOrbs = new List<AttackOrb>();
	float[] _rayAngles = { 30f, 60f, 90f, 120f, 150f, 180f, 210f, 240f, 270f, 300f, 330f, 360f };
	protected string[] _detectableObjects = { "wall", "Ally", "Enemy", "Projectile" };
	private bool _alreadyWon = false;

	public override void InitializeAgent()
	{
		AgentReset();
//		foreach (var enemy in Enemies)
//		{
//			enemy.gameObject.SetActive(true);
//			enemy.Done();
//		}
	}


	public override void CollectObservations()
	{
		if (UseVectorObs)
		{
			AddVectorObs(Health / _maxHealth);
			AddVectorObs(_attackReload);
			_observations = _rayPer.Perceive(_rayDistance, _rayAngles, _detectableObjects, 0f, 0f, this);
			AddVectorObs(_observations);
		}
	}

	public void Hit(bool hitAlly)
	{
		AddReward(hitAlly ? AllyHitReward : EnemyHitReward);
	}

	public void LoseHealth()
	{
		Health--;
	}

	public override void Done()
	{
		if (_LastRewardText != null)
		{
			_LastRewardText.text = "LAST " + _currentRewardText.text;
		}

//		foreach (Agent enemy in Enemies)
//		{
//			enemy.gameObject.SetActive(true);
//			enemy.Done();
//		}
//		foreach (Agent agent in Allies)
//		{
//			if (!agent.gameObject.activeSelf)
//			{
//				agent.gameObject.SetActive(true);
//				agent.Done();
//			}
//		}
		AgentReset();
		base.Done();
	}

	private void Death()
	{
		AddReward(DeathReward);
//		if (Allies.Exists(agent => agent.gameObject.activeSelf))
//		{
//			_isDead = true;
//			gameObject.SetActive(false);
//			//Debug.Log("Dead, Waiting For End of round");
//		}
//		else
//		{
//			Done();
//		}
		AgentReset();
	}

	private void Attack()
	{
		AddReward(TryingToShootReward);
		if (_attackReload <= 0)
		{
			_attackReload = _reloadTime;
			AttackOrb orb = SpawnManager.Instance.GetAttackOrb();
			orb.secondsToLive = AttackOrbSecondsToLive;
			orb._speed = AttackOrbSpeed;
			orb.transform.rotation = transform.rotation;
			orb.transform.position = transform.position + transform.forward * 0.75f;
			orb.Owner = this;
			SpawnManager.Instance.ShowAttackOrb(orb);
			_attackOrbs.Add(orb);
		}
	}

	public override void AddReward(float value)
	{
		if (_currentRewardText != null)
		{
			_currentRewardText.text = "REWARD: " + (m_CumulativeReward + value).ToString("F6");
		}
		base.AddReward(value);
	}

	public void TakeAction(float[] act)
	{
		Vector3 dirToGo = Vector3.zero;
		Vector3 rotateDir = Vector3.zero;

		int action0 = Mathf.FloorToInt(act[0]);
		int action1 = Mathf.FloorToInt(act[1]);
		int action2 = Mathf.FloorToInt(act[2]);

		switch (action0)
		{
			case 0:
				break;
			case 1:
				rotateDir = transform.up * 1f;
				break;
			case 2:
				rotateDir = transform.up * -1f;
				break;
		}
		switch (action1)
		{
			case 0:
				break;
			case 1:
				Attack();
				break;
		}
		switch (action2)
		{
			case 0:
				break;
			case 1:
				dirToGo = transform.forward * 1f;
				break;
		}
		transform.Rotate(rotateDir, Time.fixedDeltaTime * RotationSpeed);
		_agentRigidbody.velocity = dirToGo * MovementSpeed;
	}

	/// <summary>
	/// Called every step of the engine. Here the agent takes an action.
	/// </summary>
	public override void AgentAction(float[] vectorAction, string textAction)
	{
		_attackReload--;
//		if (Enemies.Count > 0)
//		{
//			if (!Enemies.Exists(agent => agent.gameObject.activeSelf) && !_alreadyWon)
//			{
//				_alreadyWon = true;
//				AddReward(RoundWonReward);
//				Done();
//			}
//		}
		CheckForWallSightAndEnemySight();
		TakeAction(vectorAction);
		if (agentParameters.maxStep != 0)
		{
			AddReward(MaxStepReward / agentParameters.maxStep);
		}
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
		//Debug.Log("Resetting");
		float randomRotation = Random.Range(0f, 359f);
		transform.localPosition = GetRandomSpawnPos();
		transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
		_agentRigidbody.velocity = Vector3.zero;
		_agentRigidbody.angularVelocity = Vector3.zero;
		transform.Rotate(Vector3.up, randomRotation);
		Health = _maxHealth;
		_alreadyWon = false;
		_isDead = false;
	}

	public void Missed(AttackOrb orb)
	{
		_attackOrbs.Remove(orb);
		AddReward(MissedReward);
		SpawnManager.Instance.HideAttackOrb(orb);
	}

	private void CheckForWallSightAndEnemySight()
	{
		RaycastHit hit;
		Debug.DrawRay(transform.position + new Vector3(0f, 0.4f, 0f) + transform.forward * .75f, transform.forward * _rayDistance, Color.red, 0.1f);

		if (Physics.Raycast(transform.position + new Vector3(0f, 0.4f, 0f) + transform.forward * 0.5f, transform.forward, out hit, _rayDistance))
		{
			if (hit.collider.CompareTag("Agent") && !hit.collider.gameObject != gameObject)
			{
				AddReward(EnemyInFrontReward);
			}

			if (hit.collider.CompareTag("wall"))
			{
				AddReward(FacesWallReward);
			}
		}
	}
}
