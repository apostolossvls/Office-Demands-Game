using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMenu : MonoBehaviour
{
    public Camera cam;
    public Transform interiorParent;
    public Transform exteriorParent;
    public float diff = 0.06f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 v1 = cam.ScreenToWorldPoint(new Vector3(Screen.width * diff, Screen.height / 2f, -transform.position.z));
        interiorParent.position = new Vector3(v1.x, interiorParent.position.y, interiorParent.position.z);
        
        Vector3 v2 = cam.ScreenToWorldPoint(new Vector3(Screen.width * (1 - diff), Screen.height / 2f, -transform.position.z));
        exteriorParent.position = new Vector3(v2.x, exteriorParent.position.y, exteriorParent.position.z);
    }
}
