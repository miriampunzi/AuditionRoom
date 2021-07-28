using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Classe associé a un objet permettant de detecter lorsqu'un autre objet (exptected Transform
/// Entre dedans.
/// 
/// Utilisé pour la calibration pour savoir quand un objet entre
/// PS : Si possible utiliser le collider generée par les Hand tracker mais pas trouvé comment le referencer et/ou le detecter
/// </summary>
public class HoverTrigger : MonoBehaviour
{

    public bool isIn = false;

    private MeshRenderer renderer;

    public Transform expectedTransform;

    public Material matIn;
    public Material matOut;

    public UnityEvent eventIn;
    public UnityEvent eventOut;

    //private CustomCalibration ccalib;

    private void Start()
    {
        renderer = GetComponent<MeshRenderer>();
        renderer.material = matOut;
        //ccalib = GameObject.FindObjectOfType<CustomCalibration>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject);
        if (other.transform == expectedTransform)
        {
            isIn = true;
            renderer.material = matIn;
            eventIn.Invoke();

            //ccalib.TryBeginCalibration();
            //triggerInEvent.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //if (other.gameObject.layer == LayerMask.NameToLayer("Hand"))
        if (other.transform == expectedTransform)
        {
            isIn = false;
            renderer.material = matOut;
            eventOut.Invoke();
            //triggerOutEvent.Invoke();
        }
    }


}
