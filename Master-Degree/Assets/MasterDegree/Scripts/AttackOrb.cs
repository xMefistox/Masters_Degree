using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AttackOrb : MonoBehaviour
{
	public ArenaAgent Owner;
	public float secondsToLive;
	public float _speed;
	private float liveCounter;

	public void AgentReset()
	{
		SpawnManager.Instance.HideAttackOrb(this);
	}

	private void Update()
	{
		transform.Translate(Vector3.forward * _speed * Time.deltaTime);
		liveCounter += Time.deltaTime;
		if (liveCounter > secondsToLive)
		{
			OrbMissed();
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		liveCounter = 0f;
		if (other.CompareTag("wall"))
		{
			OrbMissed();
		}
		else if (other.gameObject != Owner.gameObject && !CompareTag(other.gameObject.tag))
		{
			Owner.Hit(Owner.CompareTag(other.gameObject.tag));

			ArenaAgent agent = other.GetComponent<ArenaAgent>();
			if (agent == null)
			{
				ArenaDummyAgent arenaDummyAgent = other.GetComponent<ArenaDummyAgent>();
				if (arenaDummyAgent)
				{
					other.GetComponent<ArenaDummyAgent>().LoseHealth();
				}
			}
			else
			{
				agent.LoseHealth();
			}
		}
		SpawnManager.Instance.HideAttackOrb(this);
	}

	private void OrbMissed()
	{
		liveCounter = 0f;
		Owner.Missed(this);
	}

}
