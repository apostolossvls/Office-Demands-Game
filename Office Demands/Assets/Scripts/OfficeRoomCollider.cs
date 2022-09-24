using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfficeRoomCollider : MonoBehaviour
{
    public PaperList paperList;
    List<InteriorItem> interiorItems = new List<InteriorItem>();

    // Start is called before the first frame update
    void Start()
    {
        interiorItems = new List<InteriorItem>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        paperList.interiorItems = interiorItems;
        interiorItems = new List<InteriorItem>();
    }

    private void OnTriggerStay(Collider other)
    {
        InteriorItem interiorItem = other.GetComponentInParent<InteriorItem>();
        if (interiorItem)
        {
            interiorItems.Add(interiorItem);
        }
    }
}
