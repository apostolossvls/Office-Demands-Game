using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InteriorItem : MonoBehaviour
{
    public Vector3 returnPos;
    private float returnSpeed = 10f;
    PlayerControl player;
    bool haveBeenThrown;

    // Start is called before the first frame update
    void Start()
    {
        player = PlayerControl.instance;
        haveBeenThrown = false;
    }

    private void OnMouseEnter()
    {
        if (!PlayerControl.IsHoldingSomething() && !haveBeenThrown)
        {
            transform.Find("HoverHighlight").GetComponent<Renderer>().enabled = true;
        }
    }

    private void OnMouseExit()
    {
        transform.Find("HoverHighlight").GetComponent<Renderer>().enabled = false;
    }

    private void OnMouseDown()
    {
        if (!PlayerControl.IsHoldingSomething() && !haveBeenThrown)
        {
            player.interiorSelectedObject = gameObject;
            returnPos = transform.position;
            GetComponent<Collider>().enabled = false;
            StopAllCoroutines();
        }
    }

    public void OnEndDrag()
    {
        GetComponent<Collider>().enabled = true;
        StartCoroutine("EndDragCo");
    }

    IEnumerator EndDragCo()
    {
        //stop highlight
        transform.Find("Highlight").GetComponent<Renderer>().enabled = false;

        while (Vector3.Distance(transform.position, returnPos) > 0.04f)
        {
            transform.position = Vector3.Slerp(transform.position, returnPos, Time.deltaTime * returnSpeed);
            yield return null;
        }
        transform.position = returnPos;
        yield return null;
    }

    public void OnEndDragFly()
    {
        //place in object holder
        haveBeenThrown = true;

        //stop highlight
        transform.Find("Highlight").GetComponent<Renderer>().enabled = false;
    }
}

