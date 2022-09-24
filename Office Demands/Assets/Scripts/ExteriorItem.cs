using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ExteriorItem : MonoBehaviour
{
    public Vector3 returnPos;
    public Vector3 targetSize = Vector3.one;
    public ExteriorItemType type;
    public Transform hangingHorizontalPos;
    public Transform hangingVerticalPos;
    public Transform groundPos;
    private float returnSpeed = 10f;
    PlayerControl player;

    // Start is called before the first frame update
    void Start()
    {
        player = PlayerControl.instance;
        returnPos = transform.position;
    }

    private void OnMouseEnter()
    {
        if (!PlayerControl.IsHoldingSomething())
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
        if (!PlayerControl.IsHoldingSomething())
        {
            player.exteriorSelectedObject = gameObject;
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

    public void OnEndDragFoundPlace()
    {
        //place in object holder

        //stop highlight
        transform.Find("Highlight").GetComponent<Renderer>().enabled = false;
    }

    public Vector3 GetItemLocalPivotPosition()
    {
        Vector3 v = Vector3.zero;
        if (hangingHorizontalPos)
            v = hangingHorizontalPos.localPosition;
        else if (hangingVerticalPos)
            v = hangingVerticalPos.localPosition;
        else if (groundPos)
            v = groundPos.localPosition;
        return v;
    }
}
