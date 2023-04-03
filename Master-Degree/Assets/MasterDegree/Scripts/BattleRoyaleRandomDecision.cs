using System.Collections;
using System.Collections.Generic;
using MLAgents;
using UnityEngine;

public class BattleRoyaleRandomDecision : Decision
{

	public override float[] Decide(List<float> vectorObs, List<Texture2D> visualObs, float reward, bool done, List<float> memory)
	{
		float[] decision = {0f,0f,0f};
	
		decision[0] = Random.Range(0, 5);
		decision[0] = Random.Range(0, 3);
		decision[0] = Random.Range(0, 2);

		return decision;
	}

	public override List<float> MakeMemory(List<float> vectorObs, List<Texture2D> visualObs, float reward, bool done, List<float> memory)
	{
		return memory;
	}
}
