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
		AnimationCurve xTransition = AnimationCurve.Linear(0, 0.252f, 4, -0.23f);
		AnimationCurve yTransition = AnimationCurve.Linear(0, transform.localPosition.y, 4, transform.localPosition.y);
		AnimationCurve zTransition = AnimationCurve.Linear(0, transform.localPosition.z, 4, transform.localPosition.z);

		AnimationClip moveClip = new AnimationClip();
		moveClip.SetCurve("", typeof(Transform), "localPosition.x", xTransition);
		moveClip.SetCurve("", typeof(Transform), "localPosition.y", yTransition);
		moveClip.SetCurve("", typeof(Transform), "localPosition.z", zTransition);

		moveClip.legacy = true;
		anim.AddClip(moveClip, "Move");
	}

	public void Move()
	{
		anim.Play("Move");
	}

	public bool IsMoving()
	{
		return anim.IsPlaying("Move");
	}

	public void Show()
    {
		GetComponent<MeshRenderer>().enabled = true;
    }

	public void Hide()
    {
		GetComponent<MeshRenderer>().enabled = false;
	}
}
