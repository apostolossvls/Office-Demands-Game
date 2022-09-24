using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfficeList : MonoBehaviour
{
    public PaperList paperlist;
    public OfficeListCollection[] officeListCollections;
    public OfficeListCollection currentCollection;

    // Start is called before the first frame update
    void Start()
    {
        currentCollection = PickOfficeListCollection();
        paperlist.FillPaper(currentCollection);
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
