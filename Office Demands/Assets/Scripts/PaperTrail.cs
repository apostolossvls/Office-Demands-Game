using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperTrail : ObjectFlyDetach
{
    public ParticleSystem ps;

    public override void ObjectFly()
    {
        ps.Play();
    }
}

