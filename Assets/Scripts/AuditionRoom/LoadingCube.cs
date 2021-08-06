using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingCube : MonoBehaviour
{
	private Animation anim;
	public int id;

	void Start()
	{
		anim = GetComponent<Animation>();

		// GoUpSlow
		AnimationCurve xTransition = AnimationCurve.Linear(0, transform.localPosition.x, 10, -0.23f);
		AnimationCurve yTransition = AnimationCurve.Linear(0, transform.localPosition.y, 10, transform.localPosition.y);
		AnimationCurve zTransition = AnimationCurve.Linear(0, transform.localPosition.z, 10, transform.localPosition.z);

		AnimationClip goUpSlowClip = new AnimationClip();
		goUpSlowClip.SetCurve("", typeof(Transform), "localPosition.x", xTransition);
		goUpSlowClip.SetCurve("", typeof(Transform), "localPosition.y", yTransition);
		goUpSlowClip.SetCurve("", typeof(Transform), "localPosition.z", zTransition);

		goUpSlowClip.legacy = true;
		anim.AddClip(goUpSlowClip, "Move");
	}

	public void Move()
	{
		anim.Play("Move");
	}

	public bool IsMoving()
	{
		return anim.IsPlaying("Move");
	}
}
