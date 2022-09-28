using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

public class ExteriorItem : MonoBehaviour
{
    public Transform returnPoint;
    public Animator animator;
    public Vector3 targetSize = Vector3.one;
    public ExteriorItemType type;
    public Transform hangingHorizontalPos;
    public Transform hangingVerticalPos;
    public Transform groundPos;
    public bool mirrorPos = false;
    private float returnSpeed = 10f;
    PlayerControl player;

    // Start is called before the first frame update
    void Start()
    {
        player = PlayerControl.instance;
        returnPoint = transform.parent;
    }

    private void OnMouseEnter()
    {
        if (!PlayerControl.IsHoldingSomething())
        {
            animator.SetBool("Hover", true);
            //transform.Find("HoverHighlight").GetComponent<Renderer>().enabled = true;
        }
    }

    private void OnMouseExit()
    {
        animator.SetBool("Hover", false);
        //transform.Find("HoverHighlight").GetComponent<Renderer>().enabled = false;
    }

    private void OnMouseDown()
    {
        if (!PlayerControl.IsHoldingSomething())
        {
            player.exteriorSelectedObject = this;
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
        animator.SetBool("Grab", false);
        //transform.Find("Highlight").GetComponent<Renderer>().enabled = false;

        while (Vector3.Distance(transform.position, returnPoint.position) > 0.04f)
        {
            transform.localPosition = Vector3.Slerp(transform.localPosition, Vector3.zero, Time.deltaTime * returnSpeed);
            yield return null;
        }
        transform.localPosition = Vector3.zero;

        mirrorPos = false;
        yield return null;
    }

    public void OnEndDragFoundPlace()
    {
        CloneSelfBack();

        //stop highlight
        animator.SetBool("Grab", false);

        foreach (ObjectHolder holder in GetComponentsInChildren<ObjectHolder>())
        {
            holder.activated = true;
            if (mirrorPos)
            {
                Vector3 pos = holder.transform.localPosition;
                pos.x = -pos.x;
                holder.transform.localPosition = pos;
            }
        }
        foreach (Collider col in GetComponentsInChildren<Collider>())
        {
            if (col.name != this.name)
            {
                col.enabled = true;
            }
        }
        //transform.Find("Highlight").GetComponent<Renderer>().enabled = false;
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

    void CloneSelfBack()
    {
        GameObject clone = GameObject.Instantiate(gameObject, returnPoint);
        clone.transform.localPosition = Vector3.zero;
        clone.name = gameObject.name;
        clone.GetComponent<ExteriorItem>().mirrorPos = false;

        clone.GetComponent<Collider>().enabled = true;

        animator.SetBool("Hover", false);
        animator.SetBool("Grab", false);
    }
}
