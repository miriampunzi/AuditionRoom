using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapdoorCover : MonoBehaviour
{
	private Animation anim;
	private AnimationClip animationClip;

	void Start()
	{
		anim = GetComponent<Animation>();

		AnimationCurve constantX = AnimationCurve.Linear(0, transform.position.x, 2, transform.position.x);
		AnimationCurve translateY = AnimationCurve.Linear(0, -3, 2, 0.2f);
		AnimationCurve constantZ = AnimationCurve.Linear(0, transform.position.z, 2, transform.position.z);

		animationClip = new AnimationClip();
		animationClip.SetCurve("", typeof(Transform), "localPosition.x", constantX);
		animationClip.SetCurve("", typeof(Transform), "localPosition.y", translateY);
		animationClip.SetCurve("", typeof(Transform), "localPosition.z", constantZ);

		animationClip.legacy = true;
		anim.AddClip(animationClip, "GoUpSlow");
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
        transform.position = new Vector3(transform.position.x, transform.position.y - 3, transform.position.z);
    }
}
