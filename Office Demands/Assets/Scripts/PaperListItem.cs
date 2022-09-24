using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum ItemListState
{
    Pending = 0,
    Success = 1,
    Fail = 2,
    Extra = 3
}

public class PaperListItem : MonoBehaviour
{
    public string itemName;
    public int amountNeeded;
    public int amount;
    public bool initialized = false;
    public TextMeshProUGUI message;
    public TextMeshProUGUI progress;
    public ItemListState state;
    public Color[] colors;
    public bool checkedOnUpdate = false;

    public void Setup(string itemName, int amountNeeded, int amount = 0)
    {
        initialized = true;
        this.itemName = itemName;
        this.amountNeeded = amountNeeded;
        this.amount = amount;
    }

    public void SetAmount(int value)
    {
        amount = value;
    }

    public void UpdateProgress()
    {
        if (amountNeeded == 0)
        {
            progress.text = "";
            if (amount > 0) state = ItemListState.Extra;
            else state = ItemListState.Pending;
        }
        else if (amountNeeded == 1)
        {
            progress.text = "";
            if (amount < amountNeeded)
            {
                state = ItemListState.Pending;
            }
            else if (amount == amountNeeded)
            {
                state = ItemListState.Success;
            }
            else if (amount > amountNeeded)
            {
                progress.text = amount.ToString() + "/" + amountNeeded.ToString();
                state = ItemListState.Extra;
            }
        }
        else if (amountNeeded > 1)
        {
            progress.text = amount.ToString() + "/" + amountNeeded.ToString();
            if (amount < amountNeeded)
            {
                state = ItemListState.Pending;
            }
            else if (amount == amountNeeded)
            {
                state = ItemListState.Success;
            }
            else if (amount > amountNeeded)
            {
                state = ItemListState.Extra;
            }
        }
        message.color = colors[(int)state];
        progress.color = colors[(int)state];
    }
}
