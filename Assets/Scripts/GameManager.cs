using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Items;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    //Main grid
    private GameObject[,] _grid;
    
    // Object which has items and has border as background
    [SerializeField] public Transform gridParent;
    
    // Grid properties
    [SerializeField] private int _width;
    [SerializeField] private int _height;
    [SerializeField] private string[] _gridInput;
    private bool _endMoveActionsisRunning = false;
    
    // A variable for state management of grid
    private GridState _gridState = GridState.Fall;
     
    // A factory for creating items
    private ItemFactory _itemFactory;
    
    // Variables to process items on clicks
    private int _currentExplodingPowerCount;
    private Vector2[,] _itemLocations;
    private List<ElementLocation> matchedItems = new();
    private List<ElementLocation> visitedItems = new();
    private List<ElementLocation> destroyedObstacles = new();
    int[] missingElementCounts;
    
    //Game Over Popup
    [SerializeField] private GameObject _gameOverPopup;
    [SerializeField] private Text _gameOverText;
    [SerializeField] private Button _restartButton;

    public int Height => _height;
    public int Width => _width;
    public GameObject[,] Grid => _grid;
    public Vector2[,] ItemLocations => _itemLocations;

    private void Awake()
    {
        Instance = this;
        _grid = new GameObject[_height, _width];
        _itemLocations = new Vector2[_height, _width];
        missingElementCounts = new int[_width];
        _itemFactory = GetComponent<ItemFactory>();
        SetGridSize();
        _itemFactory.CreateItems(_width, _height, _gridInput);
        _gridState = GridState.Free;
        _restartButton.onClick.AddListener(delegate { RestartGame(); });
    }

    private void Update()
    {
        if (_gridState == GridState.Fall)
        {
            if (FallCompleted())
            {
                _gridState = GridState.EndOfMove;
            }
        } else if (_gridState == GridState.EndOfMove)
        {
            EndOfMoveActions();

        } else if (_gridState == GridState.FallOfEndOfMove)
        {
            if (CheckForPossibleDestroys())
            {
                if (_endMoveActionsisRunning!= true)
                {
                    _endMoveActionsisRunning = true;
                    StartCoroutine(CallDelayedEndOfMove());
                }
                
            }
            else
            {
                if (FallCompleted())
                {
                    _gridState = GridState.Free;
                    GameStatus status = ProgressManager.Instance.GetGameStatus();
                    if (status != GameStatus.Present)
                    {
                        _gridState = GridState.Finished;
                        GameOverAction(status);
                    }
                }
            }

        } else if (_gridState == GridState.Animation)
        {
            FreezeGrid();
        }
    }

    private IEnumerator CallDelayedEndOfMove()
    {
        yield return new WaitForSeconds(0.5f);
        _gridState = GridState.EndOfMove;
    }

    private bool CheckForPossibleDestroys()
    {
        for (int i = 0; i < _height; i++)
        {
            for (int j = 0; j < _width; j++)
            {
                if (_grid[i, j].CompareTag("nonclickable"))
                {
                    Obstacle script = _grid[i, j].GetComponent<Obstacle>();
                    if (script.EndOfMoveControl())
                    { 
                        return true;
                    }

                }
            }
        }

        return false;
    }

    private bool FallCompleted()
    {
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
                {
                    
                    double y1 = _grid[j, i].transform.position.y;
                    double y2 = i != 0 ? _grid[j, i - 1].transform.position.y : y1;
                    double y3 = i != _width - 1 ? _grid[j, i + 1].transform.position.y : y1;

                    double distance1 = Math.Abs(y1 - y2);
                    double distance2 = Math.Abs(y1 - y3);

                    if (distance1 > 0.5 || distance2 > 0.5)
                    {
                        return false;
                    }
                }
            }

        return true;
    }

    private void SetGridSize()
    {
        gridParent.GetComponent<SpriteRenderer>().size = new Vector2(_width + (float)0.12, _height + (float)0.42);
        gridParent.GetComponent<BoxCollider2D>().size = new Vector2(_width, (float) 0.01);
        gridParent.GetComponent<BoxCollider2D>().offset = new Vector2((float) 0, (float) - ((_height + (float)0.32) / 2));
    }

    public void onCubeClicked(int x, int y)
    {

        if (_gridState != GridState.Free)
        {
            return;
        }
        ResetArrays();
        string clickedColor = _grid[x, y].tag;
        
        GetMatches(clickedColor, x, y);

        // Rocket Creation Condition
        if (matchedItems.Count >= 5)
        {
            ProgressManager.Instance.DecreaseMoveCount();
            AudioManager.Instance.PlayFx("cubeExplode");
            missingElementCounts[y]--;
            RemoveMatchedItems();
            JoinDestroyedObstacles();
            CreateRocket(x, y);
            RelocateRemainingItems();
            _gridState = GridState.Fall;
            FillSpaces();
        }
        else if (matchedItems.Count >= 2)
        {
            ProgressManager.Instance.DecreaseMoveCount();
            AudioManager.Instance.PlayFx("cubeExplode");
            _gridState = GridState.Fall;
            RemoveMatchedItems();
            JoinDestroyedObstacles();
            RelocateRemainingItems();
            _gridState = GridState.Fall;
            FillSpaces();
        }
        
        
    }

    private void ResetArrays()
    {
        visitedItems.Clear();
        matchedItems.Clear();
        destroyedObstacles.Clear();
        for (int i = 0; i < Width; i++)
        {
            missingElementCounts[i] = 0;
        }
    }

    public void EndOfMoveActions()
    {
        if (_gridState != GridState.EndOfMove)
        {
            return;
        }
        
        _endMoveActionsisRunning = true;
        ResetArrays();
        _gridState = GridState.Animation;

        for(int i = 0; i < Height; i++) {
            for (int j = 0; j < Width; j++)
            {
                if (_grid[i, j].CompareTag("nonclickable"))
                {
                    Obstacle script = _grid[i, j].GetComponent<Obstacle>();
                    if (script.EndOfMoveControl())
                    {
                        script.EndOfMoveAction();
                    }
                }
            }
        }
        
        RelocateRemainingItems();
        _gridState = GridState.FallOfEndOfMove;
        _endMoveActionsisRunning = false;
        FillSpaces(); 
        ResetArrays();
    }

    private void JoinDestroyedObstacles()
    {
        matchedItems.AddRange(destroyedObstacles);
    }

    private void CreateRocket(int x, int y)
    {
        int i = Random.Range(0, 2);
        if (i == 0)
        {
            _itemFactory.CreateRocketHorizontal(x, y);
        }
        else
        {
            _itemFactory.CreateRocketVertical(x, y);
        }
        
    }

    private void FillSpaces()
    {
        UnFreezeGrid();
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < missingElementCounts[i]; j++)
            {
                createMissingBlock(i, j, missingElementCounts[i]);
            }
        }
    }

    private void RelocateRemainingItems()
    {
        for (int j = 0; j < Width; j++)
        {
            for (int i = 0; i < Height; i++)
            {
                if (_grid[i, j] == null)
                {
                    for (int k = i + 1; k < Height; k++)
                    {
                        if (_grid[k, j] != null)
                        {
                            _grid[i, j] = _grid[k, j];
                            Item script = _grid[i, j].GetComponent<Item>();
                            script.x = i;
                            script.y = j;
                            script.rigidBody.velocity = new Vector2(0, -0.1f);
                            _grid[i, j].GetComponent<SpriteRenderer>().sortingOrder = i+1;
                            _grid[k, j] = null;
                            break;
                        }
                    }
                }
            }
        }
    }

    private void RemoveMatchedItems()
    {
        foreach (var item in matchedItems)
        {
            _grid[item.x, item.y].GetComponent<Item>().DestroyActions();
            Destroy(_grid[item.x, item.y]);
            _grid[item.x, item.y] = null;
        }

        foreach (var item in destroyedObstacles)
        {
            _grid[item.x, item.y].GetComponent<Item>().DestroyActions();
            Destroy(_grid[item.x, item.y]);
            _grid[item.x, item.y] = null;
        }
    }

    private void GetMatches(string clickedColor,  int x, int y)
    {
        if (x < Height && x >= 0 && y < Width && y >= 0)
        {
            if (ElementAlreadyChecked(x, y))
            {
                return;
            }
            
            if (_grid[x, y].CompareTag("nonclickable"))
            {
                visitedItems.Add(new ElementLocation(x, y));
                Obstacle script = _grid[x, y].GetComponent<Obstacle>();
                if (script.SideExplosionControl())
                {
                    script.SideExplosionAction();
                }
                return;
            }
            
            if (!_grid[x, y].CompareTag(clickedColor))
            {
                visitedItems.Add(new ElementLocation(x, y));
                return;
            }

            visitedItems.Add(new ElementLocation(x, y));
            matchedItems.Add(new ElementLocation(x, y));
            missingElementCounts[y]++;
            
            GetMatches(clickedColor, x - 1, y);
            GetMatches(clickedColor, x, y + 1);
            GetMatches(clickedColor, x + 1, y);
            GetMatches(clickedColor, x, y - 1);
        }
        
    }

    public void createMissingBlock(int i, int j, int missingCount)
    {
        _itemFactory.CreateRandomCube( Height - missingCount + j, i, j + missingCount + 1);
    }

    private bool ElementAlreadyChecked(int x, int y)
    {
        return visitedItems.Contains(new ElementLocation(x, y));
    }

    public void OnHorizontalRocketClicked(int x, int y)
    {
        StartCoroutine(HorizontalRocketClickAction(x, y));
    }

    public IEnumerator HorizontalRocketClickAction(int x, int y)
    {
        if (_gridState != GridState.Free)
        {
            yield break;
        }

        _currentExplodingPowerCount = 1;
        ResetArrays();
        ProgressManager.Instance.DecreaseMoveCount();
        _gridState = GridState.Animation;

        Destroy(_grid[x, y]);
        _grid[x, y] = null;
        matchedItems.Add(new ElementLocation(x, y));
        missingElementCounts[y]++;
        int i = y - 1;
        int j = y + 1;
        while (i >= 0 || j < Width)
        {
            yield return new WaitForSeconds(0.05f);
            if (i >= 0)
            {
                DestroyWithRocket(x, i);
            }

            if (j < Width)
            {
                DestroyWithRocket(x, j);
            }

            i--;
            j++;
        }
        
        if (_currentExplodingPowerCount == 1)
        {
            JoinDestroyedObstacles();
            RelocateRemainingItems();
            _gridState = GridState.Fall;
            FillSpaces();
        }

        _currentExplodingPowerCount--;
    }

    private void DestroyWithRocket(int x, int i)
    {
        if (_grid[x, i] != null)
        {
            if (_grid[x, i].CompareTag("nonclickable"))
            {
                Obstacle script = _grid[x, i].GetComponent<Obstacle>();
                if (script.PowerEffectControl())
                {
                    script.PowerEffectAction();
                }
            } else if (_grid[x, i].CompareTag("power"))
            {
                ClickableItem script = _grid[x, i].GetComponent<ClickableItem>();
                script.OnExplode();
            } else
            {
                _grid[x, i].GetComponent<Cube>().DestroyActions();
                Destroy(_grid[x, i]);
                _grid[x, i] = null;
                matchedItems.Add(new ElementLocation(x, i));
                missingElementCounts[i]++;
            }
        }
    }

    public void DestroyObstacle(int x, int y)
    {
        Destroy(_grid[x, y]);
        _grid[x, y] = null;
        matchedItems.Add(new ElementLocation(x, y));
        missingElementCounts[y]++;
    }
    
    public void DestroyCube(int x, int y)
    {
        Destroy(_grid[x, y]);
        _grid[x, y] = null;
    }
    
    public void AddObstacleToMatchedList(int x, int y)
    {
        destroyedObstacles.Add(new ElementLocation(x, y));
        missingElementCounts[y]++;
    }

    public void OnRocketHorizontalExplode(int x, int y)
    {
        StartCoroutine(ExplodeHorizontalRocket(x, y));
    }

    public IEnumerator ExplodeHorizontalRocket(int x, int y)
    {
        _currentExplodingPowerCount++;

        Destroy(_grid[x, y]);
        _grid[x, y] = null;
        matchedItems.Add(new ElementLocation(x, y));
        missingElementCounts[y]++;

        int i = y - 1;
        int j = y + 1;
        while (i >= 0 || j < Width)
        {
            yield return new WaitForSeconds(0.05f);
            if (i >= 0)
            {
                DestroyWithRocket(x, i);
            }

            if (j < Width)
            {
                DestroyWithRocket(x, j);
            }

            i--;
            j++;
        }
        
        if (_currentExplodingPowerCount == 1)
        {
            JoinDestroyedObstacles();
            RelocateRemainingItems();
            _gridState = GridState.Fall;
            FillSpaces();
        }
        _currentExplodingPowerCount--;
    }

    public void OnVerticalRocketClicked(int x, int y)
    {
        StartCoroutine(VerticalRocketClickAction(x, y));
    }
    
    public IEnumerator VerticalRocketClickAction(int x, int y)
    {
        if (_gridState != GridState.Free)
        {
            yield break;
        }

        _currentExplodingPowerCount = 1;
        ResetArrays();
        ProgressManager.Instance.DecreaseMoveCount();
        _gridState = GridState.Animation;

        Destroy(_grid[x, y]);
        _grid[x, y] = null;
        matchedItems.Add(new ElementLocation(x, y));
        missingElementCounts[y]++;
        int i = x - 1;
        int j = x + 1;
        while (i >= 0 || j < Height)
        {
            yield return new WaitForSeconds(0.05f);
            if (i >= 0)
            {
                DestroyWithRocket(i, y);
            }

            if (j < Height)
            {
                DestroyWithRocket(j, y);
            }

            i--;
            j++;
        }
        
        if (_currentExplodingPowerCount == 1)
        {
            JoinDestroyedObstacles();
            RelocateRemainingItems();
            _gridState = GridState.Fall;
            FillSpaces();
        }

        _currentExplodingPowerCount--;
    }
    
    public void OnRocketVerticalExplode(int x, int y)
    {
        StartCoroutine(ExplodeVerticalRocket(x, y));
    }

    public IEnumerator ExplodeVerticalRocket(int x, int y)
    {
        _currentExplodingPowerCount++;

        Destroy(_grid[x, y]);
        _grid[x, y] = null;
        matchedItems.Add(new ElementLocation(x, y));
        missingElementCounts[y]++;

        int i = x - 1;
        int j = x + 1;
        while (i >= 0 || j < Height)
        {
            yield return new WaitForSeconds(0.05f);
            if (i >= 0)
            {
                DestroyWithRocket(i, y);
            }

            if (j < Height)
            {
                DestroyWithRocket(j, y);
            }

            i--;
            j++;
        }
        
        if (_currentExplodingPowerCount == 1)
        {
            JoinDestroyedObstacles();
            RelocateRemainingItems();
            _gridState = GridState.Fall;
            FillSpaces();
        }
        _currentExplodingPowerCount--;
    }

    public void FreezeGrid()
    {
        for (int i = 0; i < Height; i++)
        {
            for (int j = 0; j < Width; j++)
            {
                if (_grid[i, j] != null)
                {
                    Rigidbody2D body = _grid[i, j].GetComponent<Rigidbody2D>();
                    body.constraints = RigidbodyConstraints2D.FreezeAll;
                }
            }
        }
    }
    
    public void UnFreezeGrid()
    {
        for (int i = 0; i < Height; i++)
        {
            for (int j = 0; j < Width; j++)
            {
                if (_grid[i, j] != null)
                {
                    Rigidbody2D body = _grid[i, j].GetComponent<Rigidbody2D>();
                    body.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
                }
            }
        }
    }
    
    private void GameOverAction(GameStatus status)
    {
        _gameOverPopup.SetActive(true);
        _gameOverText.text = status == GameStatus.Won ? "You Win" : "You Lost";
    }
    
    private void RestartGame()
    {
        _gameOverPopup.SetActive(false);
        ResetArrays();
        for (int i = 0; i < _height; i++)
        {
            for (int j = 0; j < _width; j++)
            {
                Destroy(_grid[i, j]);
                _grid[i, j] = null;
            }
        }
        _gridState = GridState.Fall;
        _itemFactory.CreateItems(_width, _height, _gridInput);
        _gridState = GridState.Free;
        ProgressManager.Instance.ResetProgress();
    }
}
