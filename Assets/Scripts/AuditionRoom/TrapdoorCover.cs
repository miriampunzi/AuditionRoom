using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapdoorCover : MonoBehaviour
{
	private Animation anim;

	void Start()
	{
		anim = GetComponent<Animation>();

		// GoUpSlow
		AnimationCurve xGoUpSlow = AnimationCurve.Linear(0, transform.position.x, 2, transform.position.x);
		AnimationCurve yGoUpSlow = AnimationCurve.Linear(0, -3, 2, 0.2f);
		AnimationCurve zGoUpSlow = AnimationCurve.Linear(0, transform.position.z, 2, transform.position.z);

		AnimationClip goUpSlowClip = new AnimationClip();
		goUpSlowClip.SetCurve("", typeof(Transform), "localPosition.x", xGoUpSlow);
		goUpSlowClip.SetCurve("", typeof(Transform), "localPosition.y", yGoUpSlow);
		goUpSlowClip.SetCurve("", typeof(Transform), "localPosition.z", zGoUpSlow);

		goUpSlowClip.legacy = true;
		anim.AddClip(goUpSlowClip, "GoUpSlow");

		// GoDownFast
		AnimationCurve xGoDownFast = AnimationCurve.Linear(0, transform.position.x, 3, transform.position.x);
		AnimationCurve yGoDownFast = AnimationCurve.Linear(0, -3, 3, -3);
		AnimationCurve zGoDownFast = AnimationCurve.Linear(0, transform.position.z, 2, transform.position.z);

		AnimationClip goDownFastClip = new AnimationClip();
		goDownFastClip.SetCurve("", typeof(Transform), "localPosition.x", xGoDownFast);
		goDownFastClip.SetCurve("", typeof(Transform), "localPosition.y", yGoDownFast);
		goDownFastClip.SetCurve("", typeof(Transform), "localPosition.z", zGoDownFast);

		goDownFastClip.legacy = true;
		anim.AddClip(goDownFastClip, "GoDownFast");
	}

	public void GoUpSlow()
    {
		anim.Play("GoUpSlow");
	}

    public void GoDownSlow()
    {

    }

    public void GoDownFast()
    {
		anim.Play("GoDownFast");
    }

	public bool IsPlayingAnimation()
	{
		return anim.IsPlaying("GoDownFast");
	}
}
