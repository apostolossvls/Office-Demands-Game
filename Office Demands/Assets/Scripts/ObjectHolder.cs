using System.Collections;
using System.Collections.Generic;
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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (item == null && PlayerControl.instance.exteriorSelectedObject != null)
        {
            indicator.gameObject.SetActive(true);
            if (CanHold(PlayerControl.instance.exteriorSelectedObject.GetComponent<ExteriorItem>()))
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
        item = i;
        item.transform.SetParent(itemParent);
        if (itemPreview) Destroy(itemPreview);
        StartCoroutine(SetItemCo());
        indicator.gameObject.SetActive(false);
        animator.SetBool("Compatable", false);
        animator.SetBool("Hover", false);
    }

    private void OnMouseEnter()
    {
        if (item == null && PlayerControl.instance.exteriorSelectedObject != null)
        {
            animator.SetBool("Hover", true);
            ExteriorItem exItem = PlayerControl.instance.exteriorSelectedObject.GetComponent<ExteriorItem>();
            if (CanHold(exItem))
            {
                PlayerControl.instance.objectHolder = this;
                itemPreview = Instantiate(PlayerControl.instance.exteriorSelectedObject, itemParent);
                itemPreview.transform.Find("HoverHighlight").GetComponent<Renderer>().enabled = false;
                itemPreview.transform.Find("Highlight").GetComponent<Renderer>().enabled = false;
                ExteriorItem exPreItem = itemPreview.GetComponent<ExteriorItem>();
                itemPreview.transform.localScale = exPreItem.targetSize;
                itemPreview.transform.rotation = itemParent.rotation;
                itemPreview.transform.localPosition = GetItemPivotOffset(exPreItem);
                foreach (Component c in itemPreview.GetComponentsInChildren<Component>())
                {
                    if (!(c is Transform || c is MeshRenderer || c is MeshFilter))
                        Destroy(c);
                    else if (c is MeshRenderer)
                        (c as MeshRenderer).material = itemPreviewMaterial;
                }
            }
        }
    }

    private void OnMouseExit()
    {
        if (itemPreview) Destroy(itemPreview);
        if (PlayerControl.instance.objectHolder == this) PlayerControl.instance.objectHolder = null;
        animator.SetBool("Hover", false);
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
        return v;
    }

    IEnumerator SetItemCo()
    {
        item.tag = "Untagged";
        //Vector3  startPos = item.transform.position;
        Vector3 targetPos = GetItemPivotOffset(item);

        float timer = 0;
        float timeDuration = 1 / setItemSpeed;
        while (timer < timeDuration)
        {
            item.transform.localScale = Vector3.Slerp(item.transform.localScale, item.targetSize, timer / timeDuration);
            targetPos = GetItemPivotOffset(item);
            item.transform.localPosition = Vector3.Lerp(item.transform.localPosition, targetPos, timer / timeDuration);
            item.transform.localRotation = Quaternion.Slerp(item.transform.localRotation, Quaternion.Euler(Vector3.zero), timer / timeDuration);
            timer = Time.deltaTime;
            yield return null;
        }
        item.transform.localScale = item.targetSize;
        item.transform.localPosition = Vector3.zero;
        yield return null;
    }
}

public enum ObjectHolderType
{
    HangingHorizontal = 0,
    HangingVertical = 1,
    Ground = 2
}
