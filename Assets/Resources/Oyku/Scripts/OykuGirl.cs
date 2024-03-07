using UnityEngine;
public class OykuGirl : BasicAICreature
{
    // [SerializeField] 
    // private TextMeshPro m_Text;

    //protected Coroutine _TextRoutine;
    
    protected bool _conjured = false;

    public override void Start()
    {
        base.Start();

        tagsWeChase = TileTags.Player;
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
            
            m_Text.DisplayText(":)", 2);
            //_TextRoutine = StartCoroutine(TextRefresh(":)", 2));

            otherTile.GetComponent<Animator>().enabled = false;
            otherTile.GetComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;
        }
    }

    protected TileText m_Text => GetComponent<TileText>();

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
