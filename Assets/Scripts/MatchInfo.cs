using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchInfo {

    public List<GridItem> MatchedItems { get; set; }
    public int MatchStartingX { get; set; }
    public int MatchEndingX { get; set; }
    public int MatchStartingY { get; set; }
    public int MatchEndingY { get; set; }
    public bool IsPowerUpMatch { get; set; }

    public MatchInfo()
    {
        IsPowerUpMatch = false;
        MatchedItems = null;
    }

    public bool IsValidMatch
    {
        get { return MatchedItems != null; }
    }
}
