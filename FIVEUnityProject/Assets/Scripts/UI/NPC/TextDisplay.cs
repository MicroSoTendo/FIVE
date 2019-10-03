using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextDisplay : TextFormat
{
    private string str;
    void Start()
    {
        GenerateDescription();
        string text = Description[0];
        gameObject.GetComponent<Text>().text = Description[Random.Range(0, 12)];
       // StartCoroutine(AnimateText(text));
    }


    IEnumerator AnimateText(string strComplete)
    {
        int i = 0;
        str = "";
        while (i < strComplete.Length)
        {
            gameObject.GetComponent<Text>().text = str;
            str += strComplete[i++];
            yield return new WaitForSeconds(0.05f);
        }
    }
    private void Update()
    {
        
    }
}
