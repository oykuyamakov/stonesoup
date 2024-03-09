using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TileText : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro m_Text;

    public float m_FontSize = 5;
    
    private Coroutine m_TextRoutine;
    
    private Queue<string> m_TextQueue = new();

    private void Awake()
    {
        if (m_Text == null)
        {
            GameObject textObject = new GameObject("TMP");
            textObject.transform.SetParent(transform);
            m_Text = textObject.AddComponent<TextMeshPro>();
        
            m_Text.rectTransform.position = new Vector3(0, 1.5f, 0);
            m_Text.rectTransform.sizeDelta = new Vector2(4, 1);
            m_Text.fontSize = m_FontSize;
            m_Text.alignment = TextAlignmentOptions.Center;
        }
    }

    public void DisplayText(string txt, float dur = 5, bool append = false)
    {
        if (append)
        {
            m_TextQueue.Enqueue(txt);
            return;
        }
        
        if (m_TextRoutine != null)
        {
            StopCoroutine(m_TextRoutine);
        }
        
        m_TextRoutine = StartCoroutine(TextRefresh(txt, dur));
    }
    
    private IEnumerator TextRefresh(string text, float dur = 1)
    {
        m_Text.text = text;
        yield return new WaitForSeconds(dur);
        m_Text.text = "";
        if (m_TextQueue.Count > 0)
        {
            StartCoroutine(TextRefresh(m_TextQueue.Dequeue(), dur));
        }
    }

}
