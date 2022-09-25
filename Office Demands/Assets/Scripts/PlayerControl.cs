using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public static PlayerControl instance;
    public LayerMask targetMask;
    private Camera cam;
    public float mousePosZ = 1.1f;
    public float doorPosZ = 3f;
    public float doorBorderLeft = -0.5f;
    public float doorBorderRight = 0.45f;
    public LineRenderer arrow;
    Vector3 draggingTargetPos;
    float draggingFollowSpeed = 20f;
    public float throwForce = 20f;
    public float throwScaleSpeed = 2f;
    public bool interiorObjectWillFly;
    public GameObject exteriorSelectedObject;
    public GameObject interiorSelectedObject;
    public ObjectHolder objectHolder;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        interiorObjectWillFly = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (exteriorSelectedObject)
        {
            //object selected highlight
            exteriorSelectedObject.transform.Find("Highlight").GetComponent<Renderer>().enabled = true;

            //move object
            draggingTargetPos = MousePos();
            exteriorSelectedObject.transform.position = Vector3.Slerp(exteriorSelectedObject.transform.position, draggingTargetPos, Time.deltaTime * draggingFollowSpeed);
        }

        if (interiorSelectedObject)
        {
            //object selected highlight
            interiorSelectedObject.transform.Find("Highlight").GetComponent<Renderer>().enabled = true;

            //move object
            draggingTargetPos = MousePos();
            Vector3 target = draggingTargetPos;
            //if (cam.ScreenToViewportPoint(Input.mousePosition).x > 0.33f)
            if (target.x > doorBorderLeft + Camera.main.transform.position.x)
            {
                Renderer r = arrow.GetComponent<Renderer>();
                if (target.x <= doorBorderRight + Camera.main.transform.position.x)
                {
                    r.material.color = Color.green;
                    interiorObjectWillFly = true;
                }
                else
                {
                    interiorObjectWillFly = false;
                    r.material.color = Color.red;
                }
                target.x = doorBorderLeft + Camera.main.transform.position.x;
                arrow.gameObject.SetActive(true);
                Vector3 arrowTarget = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, doorPosZ));
                arrow.transform.position = Vector3.Slerp(arrow.transform.position, target, Time.deltaTime * draggingFollowSpeed);

                arrow.SetPosition(1, arrowTarget - target);
            }
            else
            {
                arrow.gameObject.SetActive(false);
                interiorObjectWillFly = false;
            }
            interiorSelectedObject.transform.position = Vector3.Slerp(interiorSelectedObject.transform.position, target, Time.deltaTime * draggingFollowSpeed);
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (exteriorSelectedObject != null)
            {
                if (objectHolder != null)
                {
                    ExteriorItem exItem = exteriorSelectedObject.GetComponent<ExteriorItem>();
                    if (objectHolder.CanHold(exItem))
                    {
                        objectHolder.TakeItem(exItem);
                    }
                }
                exteriorSelectedObject.SendMessage(objectHolder == null ? "OnEndDrag" : "OnEndDragFoundPlace", SendMessageOptions.DontRequireReceiver);
                exteriorSelectedObject = null;
            }

            if (interiorSelectedObject != null)
            {
                arrow.gameObject.SetActive(false);
                if (interiorObjectWillFly) StartCoroutine(ThrowItem(interiorSelectedObject, arrow.transform.position + arrow.GetPosition(1)));
                interiorSelectedObject.SendMessage(interiorObjectWillFly? "OnEndDragFly" : "OnEndDrag", SendMessageOptions.DontRequireReceiver);
                interiorSelectedObject = null;
                interiorObjectWillFly = false;
            }
        }
    }

    Vector3 MousePos()
    {
        return cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, mousePosZ));
    }

    public static bool IsHoldingSomething()
    {
        return instance.interiorSelectedObject != null || instance.exteriorSelectedObject != null;
    }

    IEnumerator ThrowItem(GameObject item, Vector3 target)
    {
        item.tag = "Untagged";
        Vector3 targetSize = item.GetComponent<InteriorItem>().targetSize;

        Collider col = item.GetComponent<Collider>();
        if (col)
        {
            col.enabled = true;
        }
        else Debug.LogWarning("Collider component not found in throw: " + item);
        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.isKinematic = false;
            rb.AddForce((target - item.transform.position).normalized * throwForce, ForceMode.VelocityChange);
        }
        else Debug.LogWarning("Rigidbody component not found in throw: " + item);

        while (item.transform.localScale.x < targetSize.x)
        {
            item.transform.localScale += targetSize * Time.deltaTime * throwScaleSpeed;
            yield return null;
        }
        item.transform.localScale = targetSize;
        yield return null;
    }
}

