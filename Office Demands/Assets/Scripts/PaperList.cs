using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PaperList : MonoBehaviour
{
    public RectTransform textsInteriorParent;
    public RectTransform textsExteriorParent;
    public RectTransform paperBackground;
    public PaperListItem itemTemplate;
    Vector2 paperStartPos;
    Vector2 paperStartDeltaSize;
    Vector3 paperStartScale;
    Vector2 paperTargetPos;
    public float paperShowSpeed = 10;
    RectTransform thisRectTransform;
    public OfficeListCollection[] officeListCollections;
    public OfficeListCollection currentCollection;
    public List<PaperListItem> interiorListItems = new List<PaperListItem>();
    public List<PaperListItem> exteriorListItems = new List<PaperListItem>();
    public List<InteriorItem> interiorItems = new List<InteriorItem>();
    public List<ExteriorItem> exteriorItems = new List<ExteriorItem>();

    // Start is called before the first frame update
    void Start()
    {
        FillPaper(PickOfficeListCollection());
        thisRectTransform = GetComponent<RectTransform>();
        paperStartPos = thisRectTransform.localPosition;
        paperStartDeltaSize = thisRectTransform.sizeDelta;
        paperStartScale = thisRectTransform.localScale;
        paperTargetPos = paperStartPos;
    }

    // Update is called once per frame
    void Update()
    {
        thisRectTransform.localPosition = Vector2.Lerp(thisRectTransform.localPosition, paperTargetPos, Time.deltaTime * paperShowSpeed);
        //paperBackground.sizeDelta = new Vector2(paperBackground.sizeDelta.x, textsInteriorParent.sizeDelta.y + textsExteriorParent.sizeDelta.y);
        //thisRectTransform.localPosition = Vector2.Lerp(thisRectTransform.localPosition, paperTargetPos, Time.deltaTime * paperShowSpeed);
        //thisRectTransform.localScale = paperStartScale / (0.9f + (0.2f*((paperBackground.sizeDelta.y + 1) / 500f)));
        //thisRectTransform.sizeDelta = new Vector2(paperStartDeltaSize.x, paperBackground.sizeDelta.y);
        
        UpdateList();
    }

    public void FillPaper(OfficeListCollection collection)
    {
        StartCoroutine(FillPaperCo(collection));
    }

    IEnumerator FillPaperCo(OfficeListCollection collection)
    {
        //remove previous texts
        bool first = false;
        foreach (Transform child in textsInteriorParent)
        {
            if (!first) first = true;
            else Destroy(child.gameObject);
        }
        first = false;
        foreach (Transform child in textsExteriorParent)
        {
            if (!first) first = true;
            else Destroy(child.gameObject);
        }

        exteriorListItems = new List<PaperListItem>();
        interiorListItems = new List<PaperListItem>();
        foreach (ExteriorItemType type in collection.exteriorItemTypes)
        {
            PaperListItem item = ItemListContains(exteriorListItems, type.ToString());
            if (item == null)
            {
                string message = GetDemandFromExteriorType(type);
                item = Instantiate(itemTemplate, textsExteriorParent);
                item.gameObject.SetActive(true);
                item.Setup(type.ToString(), 1);
                item.message.text = message;
                item.UpdateProgress();
                exteriorListItems.Add(item);
            }
            else
            {
                item.amountNeeded++;
                item.UpdateProgress();
            }
        }
        foreach (InteriorItemType type in collection.interiorItemTypes)
        {
            PaperListItem item = ItemListContains(interiorListItems, type.ToString());
            if (item == null)
            {
                string message = GetDemandFromInteriorType(type);
                item = Instantiate(itemTemplate, textsInteriorParent);
                item.gameObject.SetActive(true);
                item.Setup(type.ToString(), 1);
                item.message.text = message;
                item.UpdateProgress();
                interiorListItems.Add(item);
            }
            else
            {
                item.amountNeeded++;
                item.UpdateProgress();
            }
        }

        var l1 = textsInteriorParent.GetComponent<VerticalLayoutGroup>();
        l1.CalculateLayoutInputHorizontal();
        l1.CalculateLayoutInputVertical();
        l1.SetLayoutHorizontal();
        l1.SetLayoutVertical();
        var l2 = textsExteriorParent.GetComponent<VerticalLayoutGroup>();
        l2.CalculateLayoutInputHorizontal();
        l2.CalculateLayoutInputVertical();
        l2.SetLayoutHorizontal();
        l2.SetLayoutVertical();

        yield return new WaitForEndOfFrame();

        paperBackground.sizeDelta = new Vector2(paperBackground.sizeDelta.x, textsInteriorParent.sizeDelta.y + textsExteriorParent.sizeDelta.y);
        thisRectTransform.localScale = paperStartScale / (0.9f + (0.2f * ((paperBackground.sizeDelta.y + 1) / 500f)));
        Vector2 oldSizeDelta = thisRectTransform.sizeDelta;
        thisRectTransform.sizeDelta = new Vector2(paperStartDeltaSize.x, paperBackground.sizeDelta.y);
        paperStartPos.y = (paperStartPos.y + oldSizeDelta.y/2) - (thisRectTransform.sizeDelta.y * thisRectTransform.localScale.y / 2);
        //paperStartPos.y *= thisRectTransform.sizeDelta.y;
        paperTargetPos = paperStartPos;
    }

    public void PaperShow()
    {
        //paperTargetPos = paperStartPos + new Vector2(0, paperBackground.sizeDelta.y - 100f);
        paperTargetPos = Vector2.zero;
    }

    public void PaperHide()
    {
        paperTargetPos = paperStartPos;
    }

    public void UpdateList()
    {
        Dictionary<string, int> pairs = new Dictionary<string, int>();

        for (int i = 0; i < interiorItems.Count; i++)
        {
            string typeName = interiorItems[i].type.ToString();
            if (pairs.ContainsKey(typeName))
            {
                pairs[typeName] += 1;
            }
            else
            {
                pairs.Add(typeName, 1);
            }
        }

        foreach (var pair in pairs)
        {
            for (int i = 0; i < interiorListItems.Count; i++)
            {
                if (pair.Key == interiorListItems[i].itemName)
                {
                    interiorListItems[i].checkedOnUpdate = true;
                    interiorListItems[i].amount = pair.Value;
                    break;
                }
            }
        }
        for (int i = 0; i < interiorListItems.Count; i++)
        {
            if (!interiorListItems[i].checkedOnUpdate)
            {
                interiorListItems[i].amount = 0;
            }
            interiorListItems[i].UpdateProgress();        
        }

        pairs = new Dictionary<string, int>();

        for (int i = 0; i < exteriorItems.Count; i++)
        {
            string typeName = exteriorItems[i].type.ToString();
            if (pairs.ContainsKey(typeName))
            {
                pairs[typeName] += 1;
            }
            else
            {
                pairs.Add(typeName, 1);
            }
        }

        foreach (var pair in pairs)
        {
            for (int i = 0; i < exteriorListItems.Count; i++)
            {
                if (pair.Key == exteriorListItems[i].itemName)
                {
                    exteriorListItems[i].checkedOnUpdate = true;
                    exteriorListItems[i].amount = pair.Value;
                    break;
                }
            }
        }
        for (int i = 0; i < exteriorListItems.Count; i++)
        {
            if (!exteriorListItems[i].checkedOnUpdate)
            {
                exteriorListItems[i].amount = 0;
            }
            exteriorListItems[i].UpdateProgress();
        }
    }

    public string GetDemandFromInteriorType(InteriorItemType type)
    {
        string name = type.ToString();
        return name;
    }

    public string GetDemandFromExteriorType(ExteriorItemType type)
    {
        string name = type.ToString();
        return name;
    }

    public OfficeListCollection PickOfficeListCollection()
    {
        if (officeListCollections.Length > 0)
        {
            int index = Random.Range(0, officeListCollections.Length);
            return officeListCollections[index];
        }
        else
        {
            Debug.LogWarning("officeListCollections List is empty");
            return null;
        }
    }

    public PaperListItem ItemListContains(List<PaperListItem> list, string name)
    {
        foreach (PaperListItem item in list)
        {
            if (item.itemName == name) return item;
        }
        return null;
    }
}
