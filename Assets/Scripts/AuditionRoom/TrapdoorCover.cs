using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapdoorCover : MonoBehaviour, IComparer<TrapdoorCover>
{
	private Animation anim;
	public int id;

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

		// GoDownSlow
		AnimationCurve xGoDownSlow = AnimationCurve.Linear(0, transform.position.x, 2, transform.position.x);
		AnimationCurve yGoDownSlow = AnimationCurve.Linear(0, 0.2f, 2, -3);
		AnimationCurve zGoDownSlow = AnimationCurve.Linear(0, transform.position.z, 2, transform.position.z);

		AnimationClip goDownSlowClip = new AnimationClip();
		goDownSlowClip.SetCurve("", typeof(Transform), "localPosition.x", xGoDownSlow);
		goDownSlowClip.SetCurve("", typeof(Transform), "localPosition.y", yGoDownSlow);
		goDownSlowClip.SetCurve("", typeof(Transform), "localPosition.z", zGoDownSlow);

		goDownSlowClip.legacy = true;
		anim.AddClip(goDownSlowClip, "GoDownSlow");

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
		anim.Play("GoDownSlow");
    }

    public void GoDownFast()
    {
		anim.Play("GoDownFast");
    }

	public bool IsGoingDownSlow()
	{
		return anim.IsPlaying("GoDownSlow");
	}

	public bool IsGoingDownFast()
	{
		return anim.IsPlaying("GoDownFast");
	}

	public bool IsGoingUpSlow()
	{
		return anim.IsPlaying("GoUpSlow");
	}

	public int Compare(TrapdoorCover x, TrapdoorCover y)
	{
		return x.id.CompareTo(y.id);
	}
}

class TrapdoorComparer : IComparer<TrapdoorCover>
{
	public int Compare(TrapdoorCover x, TrapdoorCover y)
	{
		return x.id.CompareTo(y.id);
	}
}
