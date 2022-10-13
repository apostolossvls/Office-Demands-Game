using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillOnTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        InteriorItem i = other.GetComponentInParent<InteriorItem>();
        if (i != null)
        {
            if (other.GetComponent<ObjectFlyDetach>())
            {
                Destroy(other.gameObject);
            }
            else
            {
                Destroy(i.gameObject);
            }
        }
    }
}
