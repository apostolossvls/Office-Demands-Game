using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectHolder : MonoBehaviour
{
    public Transform itemParent;
    public ExteriorItem item;
    public Material itemPreviewMaterial;
    public GameObject itemPreview;
    public GameObject indicator;
    public Animator animator;
    public ObjectHolderType type;
    public float setItemSpeed = 10f;
    public bool activated = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerControl.instance.exteriorSelectedObject != null)
            //if (item == null && PlayerControl.instance.exteriorSelectedObject != null)
        {
            indicator.gameObject.SetActive(true);
            if (CanHold(PlayerControl.instance.exteriorSelectedObject))
            {
                animator.SetBool("Compatable", true);
            }
            else
            {
                animator.SetBool("Compatable", false);
            }
        }
        else indicator.gameObject.SetActive(false);
    }

    public void TakeItem(ExteriorItem i)
    {
        if (item != null) RemoveHoldingObject();
        item = i;
        item.transform.SetParent(itemParent);
        //item.gameObject.layer = LayerMask.NameToLayer("Default");
        SetGameLayerRecursive(item.gameObject, LayerMask.NameToLayer("3DUI"), LayerMask.NameToLayer("Default"));
        if (itemPreview) Destroy(itemPreview);
        StartCoroutine(SetItemCo());
        indicator.gameObject.SetActive(false);
        //removeButton.transform.position = item.removeButtonPos.position;
        animator.SetBool("Compatable", false);
        animator.SetBool("Hover", false);
    }

    private void OnMouseEnter()
    {
        if (PlayerControl.instance.exteriorSelectedObject != null && activated)
            //if (item == null && PlayerControl.instance.exteriorSelectedObject != null && activated)
        {
            animator.SetBool("Hover", true);
            if (CanHold(PlayerControl.instance.exteriorSelectedObject))
            {
                PlayerControl.instance.objectHolder = this;
                itemPreview = Instantiate(PlayerControl.instance.exteriorSelectedObject.gameObject, itemParent);
                ExteriorItem exPreItem = itemPreview.GetComponent<ExteriorItem>();
                exPreItem.animator.SetBool("Grab", false);
                exPreItem.animator.SetBool("Hover", false);
                //itemPreview.transform.Find("HoverHighlight").GetComponent<Renderer>().enabled = false;
                //itemPreview.transform.Find("Highlight").GetComponent<Renderer>().enabled = false;
                itemPreview.transform.localScale = exPreItem.targetSize / itemParent.lossyScale.x;
                itemPreview.transform.rotation = itemParent.rotation;
                itemPreview.transform.localPosition = GetItemPivotOffset(exPreItem) / itemParent.lossyScale.x;
                foreach (Component c in itemPreview.GetComponentsInChildren<Component>())
                {
                    if (!(c is Transform || c is MeshRenderer || c is MeshFilter || c is Animator || c is CanvasRenderer))
                        Destroy(c);
                    else if (c is MeshRenderer)
                        (c as MeshRenderer).material = itemPreviewMaterial;
                }
            }
        }
    }

    private void OnMouseExit()
    {
        if (PlayerControl.instance.objectHolder == this) PlayerControl.instance.objectHolder = null;
        if (itemPreview) Destroy(itemPreview);
        if (activated)
        {
            animator.SetBool("Hover", false);
        }
    }

    public bool CanHold(ExteriorItem item)
    {
        if (type == ObjectHolderType.HangingHorizontal)
        {
            if (item.hangingHorizontalPos != null)
                return true;
        }
        else if (type == ObjectHolderType.HangingVertical)
        {
            if (item.hangingVerticalPos != null)
                return true;
        }
        else if (type == ObjectHolderType.Ground)
        {
            if (item.groundPos != null)
                return true;
        }
        return false;
    }

    public Vector3 GetItemPivotOffset(ExteriorItem exItem)
    {
        Vector3 v = Vector3.zero;
        if (exItem.hangingHorizontalPos)
            v = exItem.transform.position - exItem.hangingHorizontalPos.position;
        else if (exItem.hangingVerticalPos)
            v = exItem.transform.position - exItem.hangingVerticalPos.position;
        else if (exItem.groundPos)
            v = exItem.transform.position - exItem.groundPos.position;
        if (exItem.mirrorPos)
        {
            v.x = -v.x;
        }
        return v;
    }

    IEnumerator SetItemCo()
    {
        if (item != null)
        {
            item.tag = "Untagged";
            //Vector3  startPos = item.transform.position;
            Vector3 targetPos = GetItemPivotOffset(item);

            float timer = 0;
            float timeDuration = 1 / setItemSpeed;
            while (timer < timeDuration)
            {
                if (item == null) break;
                item.transform.localScale = Vector3.Slerp(item.transform.localScale, item.targetSize / itemParent.lossyScale.x, timer / timeDuration);
                targetPos = GetItemPivotOffset(item);
                item.transform.localPosition = Vector3.Lerp(item.transform.localPosition, targetPos / itemParent.lossyScale.x, timer / timeDuration);
                item.transform.localRotation = Quaternion.Slerp(item.transform.localRotation, Quaternion.Euler(Vector3.zero), timer / timeDuration);
                timer = Time.deltaTime;
                yield return null;
            }
            if (item != null)
            {
                item.transform.localScale = item.targetSize / itemParent.lossyScale.x;
                targetPos = GetItemPivotOffset(item);
                item.transform.localPosition = targetPos / itemParent.lossyScale.x;
            }
        }
        yield return null;
    }

    public void RemoveHoldingObject()
    {
        if (item != null)
        {
            Destroy(item.gameObject);
            item = null;
            StopCoroutine("SetItemCo");
        }
    }

    //https://forum.unity.com/threads/help-with-layer-change-in-all-children.779147/
    private void SetGameLayerRecursive(GameObject _go, int _layer, int _newLayer)
    {
        if (_go.layer == _layer) _go.layer = _newLayer;
        foreach (Transform child in _go.transform)
        {
            if (child.gameObject.layer == _layer) child.gameObject.layer = _newLayer;

            Transform _HasChildren = child.GetComponentInChildren<Transform>();
            if (_HasChildren != null)
                SetGameLayerRecursive(child.gameObject, _layer, _newLayer);

        }
    }
}

public enum ObjectHolderType
{
    HangingHorizontal = 0,
    HangingVertical = 1,
    Ground = 2
}
