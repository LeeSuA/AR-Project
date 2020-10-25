using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;


public class blink : MonoBehaviour
{
    string flashingText;
    // Start is called before the first frame update
    void Start()
    {
        flashingText = GetComponent<TMP_Text>().text;
        StartCoroutine(BlinkText());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public IEnumerator BlinkText()
    {
        while (true)
        {
            flashingText = "";
            yield return new WaitForSeconds(.5f);
            flashingText = "Spacebar to Start";
            yield return new WaitForSeconds(.5f);
        }
    }
}
