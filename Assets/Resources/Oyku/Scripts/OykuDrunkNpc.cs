using System.Collections;
using TMPro;
using UnityEngine;

public class OykuDrunkNpc : BasicAICreature
{
    [SerializeField] 
    private TextMeshPro m_Text;

    public AudioClip drinkSound, hasNeedsSound;
    
    protected Tile _tileWereChasing = null;

    protected bool _hasItsNeed = false;
    
    protected int _consumedBeer = 0;

    protected Coroutine _TextRoutine;

    public float maxDistanceToContinueChase = 4f;
 
    protected float _timeSinceLastStep = 0f;

    public override void Start()
    {
        base.Start();
        moveSpeed = Random.Range(moveSpeed -3, moveSpeed-1);
    }

    public virtual void Update()
    {
        if (_hasItsNeed)
        {
            return;   
        }
        
        _timeSinceLastStep += Time.deltaTime;
        Vector2 targetGlobalPos = Tile.toWorldCoord(_targetGridPos.x, _targetGridPos.y);
        float distanceToTarget = Vector2.Distance(transform.position, targetGlobalPos);
        if (distanceToTarget <= GRID_SNAP_THRESHOLD || _timeSinceLastStep >= 2f)
        {
            takeStep();
        }

        updateSpriteSorting();
    }

    protected override void takeStep()
    {
        _takingCorrectingStep = false;
        _timeSinceLastStep = 0f;

        if (_tileWereChasing == null)
        {
            _targetGridPos = toGridCoord(globalX, globalY);
            return;
        }

        // First, figure out if the target is too far away
        float distanceToTile = Vector2.Distance(transform.position, _tileWereChasing.transform.position);
        if (distanceToTile > maxDistanceToContinueChase)
        {
            _tileWereChasing = null;
            return;
        }

        // We do this to re-calculate exactly where we are right now. 
        _targetGridPos = Tile.toGridCoord(globalX, globalY);

        _neighborPositions.Clear();

        // Otherwise, we're going to look at all potential neighbors and then figure out the best one to go to.
        Vector2 upGridNeighbor = new Vector2(_targetGridPos.x, _targetGridPos.y + 1);
        if (pathIsClear(toWorldCoord(upGridNeighbor), CanOverlapIgnoreTargetTile))
        {
            _neighborPositions.Add(upGridNeighbor);
        }

        Vector2 upRightGridNeighbor = new Vector2(_targetGridPos.x + 1, _targetGridPos.y + 1);
        if (pathIsClear(toWorldCoord(upRightGridNeighbor), CanOverlapIgnoreTargetTile))
        {
            _neighborPositions.Add(upRightGridNeighbor);
        }

        Vector2 rightGridNeighbor = new Vector2(_targetGridPos.x + 1, _targetGridPos.y);
        if (pathIsClear(toWorldCoord(rightGridNeighbor), CanOverlapIgnoreTargetTile))
        {
            _neighborPositions.Add(rightGridNeighbor);
        }

        Vector2 downRightGridNeighbor = new Vector2(_targetGridPos.x + 1, _targetGridPos.y - 1);
        if (pathIsClear(toWorldCoord(downRightGridNeighbor), CanOverlapIgnoreTargetTile))
        {
            _neighborPositions.Add(downRightGridNeighbor);
        }

        Vector2 downGridNeighbor = new Vector2(_targetGridPos.x, _targetGridPos.y - 1);
        if (pathIsClear(toWorldCoord(downGridNeighbor), CanOverlapIgnoreTargetTile))
        {
            _neighborPositions.Add(downGridNeighbor);
        }

        Vector2 downLeftGridNeighbor = new Vector2(_targetGridPos.x - 1, _targetGridPos.y - 1);
        if (pathIsClear(toWorldCoord(downLeftGridNeighbor), CanOverlapIgnoreTargetTile))
        {
            _neighborPositions.Add(downLeftGridNeighbor);
        }

        Vector2 leftGridNeighbor = new Vector2(_targetGridPos.x - 1, _targetGridPos.y);
        if (pathIsClear(toWorldCoord(leftGridNeighbor), CanOverlapIgnoreTargetTile))
        {
            _neighborPositions.Add(leftGridNeighbor);
        }

        Vector2 upLeftGridNeighbor = new Vector2(_targetGridPos.x - 1, _targetGridPos.y + 1);
        if (pathIsClear(toWorldCoord(upLeftGridNeighbor), CanOverlapIgnoreTargetTile))
        {
            _neighborPositions.Add(upLeftGridNeighbor);
        }

        // Now, of the neighbor positions, pick the one that's closest. 
        float minDistance = distanceToTile;
        Vector2 minNeighbor = _targetGridPos;
        GlobalFuncs.shuffle(_neighborPositions);
        foreach (Vector2 neighborPos in _neighborPositions)
        {
            float distanceFromTarget = Vector2.Distance(Tile.toWorldCoord(neighborPos.x, neighborPos.y),
                _tileWereChasing.transform.position);
            if (distanceFromTarget < minDistance)
            {
                minNeighbor = neighborPos;
                minDistance = distanceFromTarget;
            }
        }

        if (minNeighbor == _targetGridPos)
        {
            // Couldn't get any closer, stop the chase!
            _tileWereChasing = null;
        }

        _targetGridPos = minNeighbor;
    }

    protected void takeCorrectionStep()
    {
        // We do this when we need to correct where we think we are
        // i.e. if we and another creature think we're both on the same gridpos, one of us needs to switch to a neighboring gridPos.
        _timeSinceLastStep = 0f;
        _takingCorrectingStep = true;

        _neighborPositions.Clear();

        // Otherwise, we're going to look at all potential neighbors and then figure out the best one to go to.
        Vector2 upGridNeighbor = new Vector2(_targetGridPos.x, _targetGridPos.y + 1);
        if (pathIsClear(toWorldCoord(upGridNeighbor), dontOverlapWalls))
        {
            _neighborPositions.Add(upGridNeighbor);
        }

        Vector2 upRightGridNeighbor = new Vector2(_targetGridPos.x + 1, _targetGridPos.y + 1);
        if (pathIsClear(toWorldCoord(upRightGridNeighbor), dontOverlapWalls))
        {
            _neighborPositions.Add(upRightGridNeighbor);
        }

        Vector2 rightGridNeighbor = new Vector2(_targetGridPos.x + 1, _targetGridPos.y);
        if (pathIsClear(toWorldCoord(rightGridNeighbor), dontOverlapWalls))
        {
            _neighborPositions.Add(rightGridNeighbor);
        }

        Vector2 downRightGridNeighbor = new Vector2(_targetGridPos.x + 1, _targetGridPos.y - 1);
        if (pathIsClear(toWorldCoord(downRightGridNeighbor), dontOverlapWalls))
        {
            _neighborPositions.Add(downRightGridNeighbor);
        }

        Vector2 downGridNeighbor = new Vector2(_targetGridPos.x, _targetGridPos.y - 1);
        if (pathIsClear(toWorldCoord(downGridNeighbor), dontOverlapWalls))
        {
            _neighborPositions.Add(downGridNeighbor);
        }

        Vector2 downLeftGridNeighbor = new Vector2(_targetGridPos.x - 1, _targetGridPos.y - 1);
        if (pathIsClear(toWorldCoord(downLeftGridNeighbor), dontOverlapWalls))
        {
            _neighborPositions.Add(downLeftGridNeighbor);
        }

        Vector2 leftGridNeighbor = new Vector2(_targetGridPos.x - 1, _targetGridPos.y);
        if (pathIsClear(toWorldCoord(leftGridNeighbor), dontOverlapWalls))
        {
            _neighborPositions.Add(leftGridNeighbor);
        }

        Vector2 upLeftGridNeighbor = new Vector2(_targetGridPos.x - 1, _targetGridPos.y + 1);
        if (pathIsClear(toWorldCoord(upLeftGridNeighbor), dontOverlapWalls))
        {
            _neighborPositions.Add(upLeftGridNeighbor);
        }

        if (_neighborPositions.Count > 0)
        {
            _targetGridPos = GlobalFuncs.randElem(_neighborPositions);
        }
        else
        {
            _targetGridPos += Vector2.up;
        }
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if(_hasItsNeed)
            return;
        
        Tile otherTile = collision.gameObject.GetComponent<Tile>();

        // If we're chasing something, then take a step probably
        if (otherTile != _tileWereChasing
            && otherTile != null
            && otherTile.hasTag(TileTags.Creature))
        {
            BasicAICreature maybeOtherCreature = (otherTile as BasicAICreature);
            if (maybeOtherCreature != null
                && maybeOtherCreature.targetGridPos == _targetGridPos
                && (!_takingCorrectingStep || maybeOtherCreature.takingCorrectingStep))
            {
                takeCorrectionStep();
            }
        }
        
        if (otherTile != null && otherTile.hasTag(tagsWeChase))
        {
            if (_TextRoutine != null)
            {
                StopCoroutine(_TextRoutine);
            }
            _TextRoutine = StartCoroutine(TextRefresh("Lets Get Drinks Man!"));
        }
    }

    protected IEnumerator TextRefresh(string text, float dur = 1)
    {
        m_Text.text = text;
        yield return new WaitForSeconds(dur);
        m_Text.text = "";
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        OnCollisionEnter2D(collision);
    }
    
    void OnTriggerEnter2D(Collider2D other) {
        
        if(_hasItsNeed)
            return;
        
        var otherTile = other.transform.GetComponent<OykuBeer>();
        
        if (otherTile != null)
        {
            Debug.Log(otherTile.tag);
            Debug.Log(otherTile.tags);
            otherTile.useAsItem(this);
            _consumedBeer++;
            
            AudioManager.playAudio(drinkSound);
            
            if (_TextRoutine != null)
            {
                StopCoroutine(_TextRoutine);
            }
            _TextRoutine = StartCoroutine(TextRefresh("ohhh!", 2f));
            
            if(_consumedBeer == 2)
            {
                _hasItsNeed = true;
                
                AudioManager.playAudio(hasNeedsSound);
                
                if (_TextRoutine != null)
                {
                    StopCoroutine(_TextRoutine);
                }
                _TextRoutine = StartCoroutine(TextRefresh("I'm Drunk!", 3));
            }
        }
    }


    public override void tileDetected(Tile otherTile)
    {
        if (_tileWereChasing == null && otherTile.hasTag(tagsWeChase))
        {
            _tileWereChasing = otherTile;
            takeStep();
        }
    }

    // For the purposes of chasing an object, we make a special CanOverlapFunc that ignores the tile we're chasing
    protected bool CanOverlapIgnoreTargetTile(RaycastHit2D hitResult)
    {
        Tile maybeResultTile = hitResult.transform.GetComponent<Tile>();
        if (maybeResultTile == _tileWereChasing)
        {
            return true;
        }

        return DefaultCanOverlapFunc(hitResult);
    }

    protected bool dontOverlapWalls(RaycastHit2D hitResult)
    {
        Tile maybeResultTile = hitResult.transform.GetComponent<Tile>();
        if (maybeResultTile != null && maybeResultTile.hasTag(TileTags.Wall))
        {
            return false;
        }

        return true;
    }
}
