using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

public class InteriorItem : MonoBehaviour
{
    public Transform returnPoint;
    public Animator animator;
    public Vector3 targetSize = Vector3.one;
    public InteriorItemType type;
    private float returnSpeed = 10f;
    PlayerControl player;
    bool haveBeenThrown;

    // Start is called before the first frame update
    void Start()
    {
        player = PlayerControl.instance;
        haveBeenThrown = false;
        returnPoint = transform.parent;
    }

    private void OnMouseEnter()
    {
        if (!PlayerControl.IsHoldingSomething() && !haveBeenThrown)
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
        if (!PlayerControl.IsHoldingSomething() && !haveBeenThrown)
        {
            player.interiorSelectedObject = this;
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
        yield return null;
    }

    public void OnEndDragFly()
    {
        CloneSelfBack();

        //place in object holder
        haveBeenThrown = true;

        foreach (Collider col in GetComponentsInChildren<Collider>())
        {
            if (col.name != this.name)
            {
                col.enabled = true;
            }
            else
            {
                Destroy(col);
            }
        }

        transform.parent = null;
        ObjectFlyDetach[] group = transform.GetComponentsInChildren<ObjectFlyDetach>();
        foreach (ObjectFlyDetach item in group)
        {
            item.ObjectFly();
        }

        SetGameLayerRecursive(gameObject, LayerMask.NameToLayer("Default"));

        //stop highlight
        animator.SetBool("Grab", false);
        //transform.Find("Highlight").GetComponent<Renderer>().enabled = false;
    }

    void CloneSelfBack()
    {
        GameObject clone = GameObject.Instantiate(gameObject, returnPoint);
        clone.transform.localPosition = Vector3.zero;
        clone.name = gameObject.name;

        clone.GetComponent<Collider>().enabled = true;

        animator.SetBool("Hover", false);
        animator.SetBool("Grab", false);
    }

    //https://forum.unity.com/threads/help-with-layer-change-in-all-children.779147/
    private void SetGameLayerRecursive(GameObject _go, int _layer)
    {
        _go.layer = _layer;
        foreach (Transform child in _go.transform)
        {
            child.gameObject.layer = _layer;

            Transform _HasChildren = child.GetComponentInChildren<Transform>();
            if (_HasChildren != null)
                SetGameLayerRecursive(child.gameObject, _layer);

        }
    }
}

