using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHolderManager : MonoBehaviour
{
    public PaperList paperList;
    public ObjectHolder[] objectHolders;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        List<ExteriorItem> items = new List<ExteriorItem>(); 
        foreach (ObjectHolder holder in objectHolders)
        {
            if (holder.item != null)
            {
                items.Add(holder.item);
            }
        }
        paperList.exteriorItems = items;
    }
}
