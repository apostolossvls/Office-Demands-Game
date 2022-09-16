using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHolder : MonoBehaviour
{
    public ExteriorItem item;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeItem(ExteriorItem i)
    {
        item = i;
        item.transform.position = transform.position;
        transform.Find("HoverHighlight").GetComponent<Renderer>().enabled = false;
    }

    private void OnMouseEnter()
    {
        if (item == null && PlayerControl.instance.exteriorSelectedObject != null)
        {
            PlayerControl.instance.objectHolder = this;
            transform.Find("HoverHighlight").GetComponent<Renderer>().enabled = true;
        }
    }

    private void OnMouseExit()
    {
        if (PlayerControl.instance.objectHolder == this) PlayerControl.instance.objectHolder = null;
        transform.Find("HoverHighlight").GetComponent<Renderer>().enabled = false;
    }
}
