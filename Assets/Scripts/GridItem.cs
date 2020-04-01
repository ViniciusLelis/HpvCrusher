using System;
using System.Collections;
using UnityEngine;

public class GridItem : MonoBehaviour, IEquatable<GridItem> {

    public int XPosition { get; private set; }
    public int YPosition { get; private set; }
    public int ItemType { get; set; }

    public static event OnMouseOverItem OnMouseOverItemEventHandler;
    public delegate void OnMouseOverItem(GridItem item);

    public GridItem(int XPosition, int YPosition)
    {
        this.XPosition = XPosition;
        this.YPosition = YPosition;
    }

    public void OnItemPositionChanged (int XPosition, int YPosition)
    {
        this.XPosition = XPosition;
        this.YPosition = YPosition;

        gameObject.name = String.Format("Item [{0},{1}]", this.XPosition, this.YPosition);
    }

    void OnMouseDown()
    {
        if (OnMouseOverItemEventHandler != null)
            OnMouseOverItemEventHandler(this);
    }

    public override String ToString()
    {
        return String.Format("Item [{0},{1}]", this.XPosition, this.YPosition);
    }

    public bool Equals(GridItem other)
    {
        if (other != null)
            return XPosition == other.XPosition && YPosition == other.YPosition && ItemType == other.ItemType;
        else
            return false;
    }
}
