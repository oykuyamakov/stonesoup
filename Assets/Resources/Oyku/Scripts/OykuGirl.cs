using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class OykuGirl : BasicAICreature
{

    protected TileText _tileText;
    
    protected bool _conjured = false;
    

    public override void Start()
    {
        base.Start();

        tagsWeChase = TileTags.Player | TileTags.Friend | TileTags.Enemy;
        
        _tileText = GetComponent<TileText>();
		
        _tileText.DisplayText("Hieoww");
		
        StartCoroutine(RandomDialogue());
    }
    
    private List<string> _randomDialogue = new List<string>()
    {
        "You look so ugly, I can fix you",
        "Come to me, I will make you beautiful",
        "Do you know mike?",
        "Beware of the copy cat, it just jealous of me",
        
    };
    
    protected IEnumerator RandomDialogue()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(5f,9f));
            _tileText.DisplayText(_randomDialogue[Random.Range(0, _randomDialogue.Count)]);
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        OnCollisionEnter2D(collision);
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if(_conjured)
            return;
        
        TryConjurePlayer(collision.gameObject.GetComponent<Tile>());
    }
    
    protected void TryConjurePlayer(Tile otherTile)
    {
        if (otherTile != null && otherTile.hasTag(tagsWeChase))
        {
            // if (_TextRoutine != null)
            // {
            //     StopCoroutine(_TextRoutine);
            // }
            
            _tileText.DisplayText("Hi sugar :)", 2);
            //_TextRoutine = StartCoroutine(TextRefresh(":)", 2));

            otherTile.GetComponent<Animator>().enabled = false;
            otherTile.GetComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;
        }
    }


    // protected IEnumerator TextRefresh(string text, float dur = 1)
    // {
    //     m_Text.text = text;
    //     yield return new WaitForSeconds(dur);
    //     m_Text.text = "";
    //     
    //     die();
    //
    // }
}
