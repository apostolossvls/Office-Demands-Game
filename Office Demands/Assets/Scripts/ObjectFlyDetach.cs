using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFlyDetach : MonoBehaviour
{
    public float delayTime = 1f;

    public virtual void ObjectFly()
    {
        Invoke("Detach", delayTime);
    }

    void Detach()
    {
        GetComponent<Rigidbody>().isKinematic = false;
    }
}
