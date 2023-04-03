using System.Collections;
using System.Collections.Generic;
using MLAgents;
using UnityEngine;

public class BattleRoyaleNoDecision : Decision
{

	public override float[] Decide(List<float> vectorObs, List<Texture2D> visualObs, float reward, bool done, List<float> memory)
	{
		float[] decision = {0f};

		return decision;
	}

	public override List<float> MakeMemory(List<float> vectorObs, List<Texture2D> visualObs, float reward, bool done, List<float> memory)
	{
		return memory;
	}
}
