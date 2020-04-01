using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameGrid : MonoBehaviour
{

    /* Game mechanics */
    public int MDimension;
    public int NDimension;
    private GameObject[] _items;
    private GridItem[,] _gridItems;
    private GridItem _selectedItem;
    private bool isUpdating;
    private bool isFinished;
    private bool isStoped;

    /* UI Elements */
    public TMP_Text scoreText;
    public Slider timeElapsedSlider;
    public Image firstStar;
    public Image secondStar;
    public Image thirdStar;
    public Sprite filledStar;
    public DialogLevelEnding levelCompleteDialog;
    public DialogLevelEnding levelFailDialog;
    public GameObject canvasItems;

    /* Board Level Information */
    private int currentScore;
    public int requiredScore;
    public float maximumTime;
    private float idleTime;
    private int stars = 0;

    public float minSwipeDistY = 1.1f;
    public float minSwipeDistX = 1.1f;
    private Vector2 startPos;

    void Start()
    {
        isFinished = false;
        isStoped = false;
        timeElapsedSlider.maxValue = maximumTime;
        timeElapsedSlider.minValue = 0;
        timeElapsedSlider.value = maximumTime;
        UpdateScore(0);
        GetItems();
        FillGrid();
        GridItem.OnMouseOverItemEventHandler += OnMouseOverItem;
        isUpdating = false;
        StartCoroutine(CountTimeElapsed());
        StartCoroutine(CountTimeForHint());
    }

    void Update()
    {
#if UNITY_ANDROID
        if (Input.touchCount > 0 && !isUpdating && !isStoped)
        {
            Touch touch = Input.touches[0];
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    startPos = touch.position;
                    break;
                case TouchPhase.Ended:
                    float swipeDistVertical = (new Vector3(0, touch.position.y, 0) - new Vector3(0, startPos.y, 0)).magnitude;
                    float swipeDistHorizontal = (new Vector3(touch.position.x, 0, 0) - new Vector3(startPos.x, 0, 0)).magnitude;

                    if (swipeDistVertical > minSwipeDistY && swipeDistVertical > swipeDistHorizontal)
                    {
                        float swipeValue = Mathf.Sign(touch.position.y - startPos.y);
                        if (swipeValue > 0 && _selectedItem != null) //up
                        {
                            print("Clicou em " + _selectedItem + "tentando mover para cima");
                            int xPosition = _selectedItem.XPosition;
                            int yPosition = _selectedItem.YPosition;
                            if (yPosition + 1 < NDimension)
                            {
                                StartCoroutine(TryMatch(_selectedItem, _gridItems[xPosition, yPosition + 1]));
                            }
                        }
                        else if (swipeValue < 0 && _selectedItem != null) //down
                        {
                            print("Clicou em " + _selectedItem + "tentando mover para baixo");
                            int xPosition = _selectedItem.XPosition;
                            int yPosition = _selectedItem.YPosition;
                            if (yPosition - 1 >= 0)
                            {
                                StartCoroutine(TryMatch(_selectedItem, _gridItems[xPosition, yPosition - 1]));
                            }
                        }
                    }
                    else if (swipeDistHorizontal > minSwipeDistX)
                    {
                        float swipeValue = Mathf.Sign(touch.position.x - startPos.x);
                        if (swipeValue > 0 && _selectedItem != null) //right
                        {
                            print("Clicou em " + _selectedItem + "tentando mover para direita");
                            int xPosition = _selectedItem.XPosition;
                            int yPosition = _selectedItem.YPosition;
                            if (xPosition + 1 < MDimension)
                            {
                                StartCoroutine(TryMatch(_selectedItem, _gridItems[xPosition + 1, yPosition]));
                            }
                        }
                        else if (swipeValue < 0 && _selectedItem != null) //left
                        {
                            print("Clicou em " + _selectedItem + "tentando mover para esquerda");
                            int xPosition = _selectedItem.XPosition;
                            int yPosition = _selectedItem.YPosition;
                            if (xPosition - 1 >= 0)
                            {
                                StartCoroutine(TryMatch(_selectedItem, _gridItems[xPosition - 1, yPosition]));
                            }
                        }
                    }
                    break;
            }
        }
#endif
    }

    void GetItems()
    {
        var _viruses = Resources.LoadAll<GameObject>("Prefabs/Viruses");
        var _powerUps = Resources.LoadAll<GameObject>("Prefabs/PowerUps");
        List<GameObject> itemsAux = new List<GameObject>();
        itemsAux.AddRange(_viruses);
        itemsAux.AddRange(_powerUps);
        _items = itemsAux.ToArray();
        int i = 0;
        foreach (GameObject item in _items)
        {
            item.GetComponent<GridItem>().ItemType = i;
            i++;
        }
    }

    void FillGrid()
    {
        _gridItems = new GridItem[MDimension, NDimension];
        for (int i = 0; i < this.MDimension; i++)
        {
            for (int j = 0; j < this.NDimension; j++)
            {
                do
                {
                    if (_gridItems[i, j] != null)
                        Destroy(_gridItems[i, j].gameObject);
                    _gridItems[i, j] = InstantiateItem(i, j);
                    if (_gridItems[i, j].ItemType > 3)
                        break;
                } while (MatchInformation(_gridItems[i, j]).IsValidMatch);
                SetRigidBodyStatus(_gridItems[i, j], false);
            }
        }
    }

    void ClearGrid()
    {
        for (int i = 0; i < MDimension; i++)
        {
            for (int j = 0; j < NDimension; j++)
            {
                Destroy(_gridItems[i, j].gameObject);
            }
        }
    }

    void UpdateScore(int points)
    {
        this.currentScore += points;
        this.scoreText.text = "pontos\n" + currentScore;
        if (this.currentScore > 0)
        {
            if (this.currentScore >= requiredScore && this.firstStar.sprite != filledStar)
            {
                this.firstStar.sprite = filledStar;
                stars = 1;
            }
            else if (this.currentScore >= requiredScore * 1.5 && this.secondStar.sprite != filledStar) // If the played achieved 66% above the required score
            {
                this.secondStar.sprite = filledStar;
                stars = 2;
            }
            else if (this.currentScore >= requiredScore * 2 && this.thirdStar.sprite != filledStar) // If the played achieved 100% above the required score
            {
                this.thirdStar.sprite = filledStar;
                stars = 3;
            }
        }
    }

    GridItem InstantiateItem(int i, int j)
    {
        float instantiatePowerUp = Random.Range(0f, 1f);
        GridItem newItem;

        if (instantiatePowerUp <= Constants.QuadrivalentVaccineSpawnRate) // Instantiate a Quadrivalent Vaccine Power Up
        {
            GameObject quadrivalentVaccineItem = _items[5];
            newItem = (Instantiate(quadrivalentVaccineItem, new Vector3(i * 1.1f, (j * 1.1f) + 5, 89), Quaternion.identity, this.transform) as GameObject).GetComponent<GridItem>();
            newItem.transform.parent = newItem.transform;
            newItem.OnItemPositionChanged(i, j); ;
            newItem.ItemType = quadrivalentVaccineItem.GetComponent<GridItem>().ItemType;
        }
        else if (instantiatePowerUp <= Constants.BivalentVaccineSpawnRate) // Instantiate a bivalent Vaccine Power Up
        {
            GameObject bivalentVaccineItem = _items[4];
            newItem = (Instantiate(bivalentVaccineItem, new Vector3(i * 1.1f, (j * 1.1f) + 5, 89), Quaternion.identity, this.transform) as GameObject).GetComponent<GridItem>();
            newItem.OnItemPositionChanged(i, j);
            newItem.ItemType = bivalentVaccineItem.GetComponent<GridItem>().ItemType;
        }
        else // Instantiate a virus
        {
            GameObject randomItem = _items[Random.Range(0, 4)]; // From 0 to 4 we find the Viruses
            newItem = (Instantiate(randomItem, new Vector3(i * 1.1f, (j * 1.1f) + 5, 89), Quaternion.identity, this.transform) as GameObject).GetComponent<GridItem>();
            newItem.OnItemPositionChanged(i, j);
            newItem.ItemType = randomItem.GetComponent<GridItem>().ItemType;
        }
        return newItem;
    }

    public void OnMouseOverItem(GridItem item)
    {
        if (!isUpdating && !isStoped)
        {
            _selectedItem = item;
        }
    }

    IEnumerator CountTimeElapsed()
    {
        float timeRemaining = maximumTime;
        while (timeRemaining > 0)
        {
            yield return new WaitForSeconds(1);
            if (!isStoped)
            {
                timeRemaining--;
                timeElapsedSlider.value = timeRemaining;
            }
        }
        timeElapsedSlider.fillRect.gameObject.SetActive(false);
        while (isUpdating)
            yield return null;
        // Show the player that his time is up
        FinishLevel();
    }

    public void AllowSecondChance()
    {
        isFinished = false;
        isStoped = false;
        timeElapsedSlider.fillRect.gameObject.SetActive(true);
        timeElapsedSlider.value = maximumTime;
        isUpdating = false;
        StartCoroutine(CountTimeElapsed());
        StartCoroutine(CountTimeForHint());
    }

    List<GridItem> FindPossibleMatches()
    {
        List<GridItem> possibleMatch = new List<GridItem>();
        for (int i = 0; i < this.MDimension; i++)
        {
            for (int j = 0; j < this.NDimension; j++)
            {
                int itemType = _gridItems[i, j].ItemType;
                if (j + 1 < NDimension) // Possible match above
                {
                    for (int lowerm = i - 1; lowerm >= 0 && _gridItems[lowerm, j + 1].ItemType == itemType; lowerm--)
                        possibleMatch.Add(_gridItems[lowerm, j + 1]);

                    for (int upperm = i + 1; upperm < MDimension && _gridItems[upperm, j + 1].ItemType == itemType; upperm++)
                        possibleMatch.Add(_gridItems[upperm, j + 1]);
                }
                if (possibleMatch.Count >= Constants.MinimumItemsForMatch - 1)
                {
                    possibleMatch.Add(_gridItems[i, j]);
                    return possibleMatch;
                }
                else
                    possibleMatch.Clear();

                if (j - 1 >= 0) // Possible match below
                {
                    for (int lowerm = i - 1; lowerm >= 0 && _gridItems[lowerm, j - 1].ItemType == itemType; lowerm--)
                        possibleMatch.Add(_gridItems[lowerm, j - 1]);

                    for (int upperm = i + 1; upperm < MDimension && _gridItems[upperm, j - 1].ItemType == itemType; upperm++)
                        possibleMatch.Add(_gridItems[upperm, j - 1]);
                }
                if (possibleMatch.Count >= Constants.MinimumItemsForMatch - 1)
                {
                    possibleMatch.Add(_gridItems[i, j]);
                    return possibleMatch;
                }
                else
                    possibleMatch.Clear();

                if (i - 1 >= 0) // Possible match left
                {
                    for (int lowern = j - 1; lowern >= 0 && _gridItems[i - 1, lowern].ItemType == itemType; lowern--)
                        possibleMatch.Add(_gridItems[i - 1, lowern]);

                    for (int uppern = j + 1; uppern < NDimension && _gridItems[i - 1, uppern].ItemType == itemType; uppern++)
                        possibleMatch.Add(_gridItems[i - 1, uppern]);
                }
                if (possibleMatch.Count >= Constants.MinimumItemsForMatch - 1)
                {
                    possibleMatch.Add(_gridItems[i, j]);
                    return possibleMatch;
                }
                else
                    possibleMatch.Clear();

                if (i + 1 < MDimension) // Possible match right
                {
                    for (int lowern = j - 1; lowern >= 0 && _gridItems[i + 1, lowern].ItemType == itemType; lowern--)
                        possibleMatch.Add(_gridItems[i + 1, lowern]);

                    for (int uppern = j + 1; uppern < NDimension && _gridItems[i + 1, uppern].ItemType == itemType; uppern++)
                        possibleMatch.Add(_gridItems[i + 1, uppern]);
                }
                if (possibleMatch.Count >= Constants.MinimumItemsForMatch - 1)
                {
                    possibleMatch.Add(_gridItems[i, j]);
                    return possibleMatch;
                }
                else
                    possibleMatch.Clear();
            }
        }
        return null;
    }

    IEnumerator CountTimeForHint()
    {
        while (!isFinished)
        {
            idleTime = 0;
            while (idleTime < Constants.HintTimeSeconds)
            {
                yield return new WaitForSeconds(1);
                if (!isStoped)
                    idleTime++;
            }
            var possibleMatches = FindPossibleMatches();
            print(possibleMatches.Count);

            foreach (GridItem item in possibleMatches)
            {
                StartCoroutine(item.transform.Blink(1.0f));
            }
        }
    }

    void FinishLevel()
    {
        if (currentScore >= requiredScore)
        {
            levelCompleteDialog.Stars = this.stars;
            levelCompleteDialog.Score = currentScore;
            levelCompleteDialog.UpdateInformation();
            levelCompleteDialog.gameObject.SetActive(true);
        }
        else
        {
            levelFailDialog.gameObject.SetActive(true);
        }
        isFinished = true;
        StopAllCoroutines();
        for(int i=0; i<MDimension; i++)
        {
            for(int j=0; j<NDimension; j++)
            {
                _gridItems[i, j].transform.SetAlpha(1f);
            }
        }
    }

    struct Coordinate : System.IEquatable<Coordinate>
    {
        public int IPos { get; set; }
        public int JPos { get; set; }

        public Coordinate(int i, int j) : this()
        {
            this.IPos = i;
            this.JPos = j;
        }

        public bool Equals(Coordinate other)
        {
            return IPos == other.IPos && JPos == other.JPos;
        }
    }

    IEnumerator DestroyAndUpdateGrid(MatchInfo match)
    {
        idleTime = 0;
        Dictionary<Coordinate, int> toTranslate = new Dictionary<Coordinate, int>(); // Key is the coordinate, Value is the amount of Y translating required
        Dictionary<int, int> toInstantiateByX = new Dictionary<int, int>(); // Key is the X coordinate, Value is the amount of Viruses to instantiate for each X
        for (int i = match.MatchStartingX; i <= match.MatchEndingX; i++)
        {
            for (int j = match.MatchStartingY; j <= match.MatchEndingY; j++)
            {
                if (match.MatchedItems.Contains(_gridItems[i, j]))
                {
                    if (toInstantiateByX.ContainsKey(i))
                        toInstantiateByX[i] = toInstantiateByX[i] + 1;
                    else
                        toInstantiateByX.Add(i, 1);
                    for (int k = j + 1; k < NDimension; k++)
                    {
                        var coords = new Coordinate(i, k);
                        if (toTranslate.ContainsKey(coords))
                            toTranslate[coords] = toTranslate[coords] + 1;
                        else
                            toTranslate.Add(coords, 1);
                    }
                }
            }
        }

        yield return StartCoroutine(DestroyItems(match.MatchedItems));

        foreach (Coordinate coord in toTranslate.Keys) // Translate items above the ones destroyed
        {
            int translateAmount = toTranslate[coord];
            if (_gridItems[coord.IPos, coord.JPos] != null)
            {
                _gridItems[coord.IPos, coord.JPos].OnItemPositionChanged(coord.IPos, coord.JPos - translateAmount);
                _gridItems[coord.IPos, coord.JPos - translateAmount] = _gridItems[coord.IPos, coord.JPos];
            }
        }

        for (int i = 0; i < MDimension; i++) // Instantiate new Items on the grid
        {
            if (toInstantiateByX.ContainsKey(i))
            {
                int instantiateCount = toInstantiateByX[i];
                for (int j = NDimension - instantiateCount; j < NDimension; j++)
                {
                    _gridItems[i, j] = InstantiateItem(i, j);
                    SetRigidBodyStatus(_gridItems[i, j], false);
                }
            }
        }

        for (int i = 0; i < this.MDimension; i++) //Continue looking for matches until there's none
        {
            for (int j = 0; j < this.NDimension; j++)
            {
                MatchInfo matchInfo = MatchInformation(_gridItems[i, j]);
                if (matchInfo.IsValidMatch)
                {
                    yield return new WaitForSeconds(Constants.DelayBetweenMatches);
                    yield return StartCoroutine(DestroyAndUpdateGrid(matchInfo));
                }
            }
        }
    }

    IEnumerator TryMatch(GridItem a, GridItem b)
    {
        isUpdating = true;
        yield return StartCoroutine(SwapItems(a, b));

        MatchInfo matchA = MatchInformation(a);
        MatchInfo matchB = MatchInformation(b);

        if (!matchA.IsValidMatch && !matchB.IsValidMatch)
        {
            yield return new WaitForSeconds(Constants.TimeForSwapBack);
            yield return StartCoroutine(SwapItems(a, b));
            isUpdating = false;
            yield break;
        }
        if (matchA.IsValidMatch)
        {
            yield return StartCoroutine(DestroyAndUpdateGrid(matchA));
        }
        else if (matchB.IsValidMatch)
        {
            yield return StartCoroutine(DestroyAndUpdateGrid(matchB));
        }
        isUpdating = false;
    }

    IEnumerator DestroyItems(List<GridItem> items)
    {
        UpdateScore(items.Count * 100);
        AudioManager._instance.Play("destroyVirus");

        foreach (GridItem item in items)
        {
            var animator = item.GetComponent<Animator>();
            if (animator != null && animator.isActiveAndEnabled)
                animator.SetBool("shouldDestroy", true);
        }

        yield return new WaitForSeconds(Constants.AnimationDuration);

        foreach (GridItem item in items)
        {
            Destroy(item.gameObject);
        }
    }

    IEnumerator SwapItems(GridItem a, GridItem b)
    {
        Vector3 auxAPosition = a.transform.position;
        StartCoroutine(a.transform.Move(b.transform.position, Constants.MovementDuration));
        StartCoroutine(b.transform.Move(auxAPosition, Constants.MovementDuration));
        yield return new WaitForSeconds(Constants.MovementDuration);

        var auxA = _gridItems[a.XPosition, a.YPosition];
        _gridItems[a.XPosition, a.YPosition] = b;
        _gridItems[b.XPosition, b.YPosition] = auxA;

        int auxXPosition = b.XPosition;
        int auxYPosition = b.YPosition;
        int auxItemType = a.ItemType;
        b.OnItemPositionChanged(a.XPosition, a.YPosition);
        a.OnItemPositionChanged(auxXPosition, auxYPosition);
    }

    List<GridItem> SearchHorizontally(GridItem item)
    {
        List<GridItem> horizontalItems = new List<GridItem> { item };
        int left = item.XPosition - 1;
        int right = item.XPosition + 1;
        while (left >= 0 && _gridItems[left, item.YPosition] != null && _gridItems[left, item.YPosition].ItemType == item.ItemType)
        {
            horizontalItems.Add(_gridItems[left, item.YPosition]);
            left--;
        }

        while (right < MDimension && _gridItems[right, item.YPosition] != null && _gridItems[right, item.YPosition].ItemType == item.ItemType)
        {
            horizontalItems.Add(_gridItems[right, item.YPosition]);
            right++;
        }

        return horizontalItems;
    }

    List<GridItem> SearchVertically(GridItem item)
    {
        List<GridItem> verticalItems = new List<GridItem> { item };
        int above = item.YPosition + 1;
        int below = item.YPosition - 1;
        while (above < NDimension && _gridItems[item.XPosition, above] != null && _gridItems[item.XPosition, above].ItemType == item.ItemType)
        {
            verticalItems.Add(_gridItems[item.XPosition, above]);
            above++;
        }

        while (below >= 0 && _gridItems[item.XPosition, below] != null && _gridItems[item.XPosition, below].ItemType == item.ItemType)
        {
            verticalItems.Add(_gridItems[item.XPosition, below]);
            below--;
        }

        return verticalItems;
    }

    MatchInfo MatchInformation(GridItem item)
    {
        MatchInfo match = new MatchInfo();
        if (item.ItemType >= 4)
        {
            BuildMatchAroundPowerUp(match, item);
        }
        else
        {
            List<GridItem> horizontalMatches = SearchHorizontally(item);
            List<GridItem> verticalMatches = SearchVertically(item);
            if (horizontalMatches.Count >= Constants.MinimumItemsForMatch && horizontalMatches.Count > verticalMatches.Count)
            {
                match.MatchedItems = horizontalMatches;
                match.MatchStartingX = MinimumXOnMatch(horizontalMatches);
                match.MatchEndingX = MaximumXOnMatch(horizontalMatches);
                match.MatchStartingY = match.MatchEndingY = horizontalMatches[0].YPosition;
            }
            else if (verticalMatches.Count >= Constants.MinimumItemsForMatch)
            {
                match.MatchedItems = verticalMatches;
                match.MatchStartingY = MinimumYOnMatch(verticalMatches);
                match.MatchEndingY = MaximumYOnMatch(verticalMatches);
                match.MatchStartingX = match.MatchEndingX = verticalMatches[0].XPosition;
            }
        }
        return match;
    }

    void BuildMatchAroundPowerUp(MatchInfo match, GridItem powerUp)
    {
        List<GridItem> matchedItems = new List<GridItem>();
        if (powerUp.ItemType == 4 || powerUp.ItemType == 5)
        {
            int inferiorXLimit = powerUp.XPosition - Constants.PowerUpRadius < 0 ? 0 : powerUp.XPosition - Constants.PowerUpRadius;
            int superiorXLimit = powerUp.XPosition + Constants.PowerUpRadius > MDimension - 1 ? MDimension - 1 : powerUp.XPosition + Constants.PowerUpRadius;

            int inferiorYLimit = powerUp.YPosition - Constants.PowerUpRadius < 0 ? 0 : powerUp.YPosition - Constants.PowerUpRadius;
            int superiorYLimit = powerUp.YPosition + Constants.PowerUpRadius > NDimension - 1 ? NDimension - 1 : powerUp.YPosition + Constants.PowerUpRadius;

            for (int i = inferiorXLimit; i <= superiorXLimit; i++)
            {
                for (int j = inferiorYLimit; j <= superiorYLimit; j++)
                {
                    if ((powerUp.ItemType == 5 && _gridItems[i, j].ItemType < 4) || (powerUp.ItemType == 4 && _gridItems[i, j].ItemType < 2))
                        matchedItems.Add(_gridItems[i, j]);
                }
            }
            matchedItems.Add(powerUp);

            match.MatchedItems = matchedItems;
            match.MatchStartingX = inferiorXLimit;
            match.MatchEndingX = superiorXLimit;
            match.MatchStartingY = inferiorYLimit;
            match.MatchEndingY = superiorYLimit;
            match.IsPowerUpMatch = true;
        }
        else
        {
            throw new System.Exception("Item is not a PowerUp");
        }
    }

    int MinimumXOnMatch(List<GridItem> items)
    {
        int[] indexes = new int[items.Count];
        for (int i = 0; i < indexes.Length; i++)
        {
            indexes[i] = items[i].XPosition;
        }
        return (int)Mathf.Min(indexes);
    }

    int MaximumXOnMatch(List<GridItem> items)
    {
        int[] indexes = new int[items.Count];
        for (int i = 0; i < indexes.Length; i++)
        {
            indexes[i] = items[i].XPosition;
        }
        return (int)Mathf.Max(indexes);
    }

    int MinimumYOnMatch(List<GridItem> items)
    {
        int[] indexes = new int[items.Count];
        for (int i = 0; i < indexes.Length; i++)
        {
            indexes[i] = items[i].YPosition;
        }
        return (int)Mathf.Min(indexes);
    }

    int MaximumYOnMatch(List<GridItem> items)
    {
        int[] indexes = new int[items.Count];
        for (int i = 0; i < indexes.Length; i++)
        {
            indexes[i] = items[i].YPosition;
        }
        return (int)Mathf.Max(indexes);
    }

    void SetRigidBodyStatus(GridItem item, bool status)
    {
        item.GetComponent<Rigidbody2D>().isKinematic = status;
    }

    public void PauseGame()
    {
        isStoped = true;
    }

    public void UnpauseGame()
    {
        idleTime = 0;
        isStoped = false;
    }

    public void UnsubscribeEvent()
    {
        GridItem.OnMouseOverItemEventHandler -= OnMouseOverItem;
    }
}
