using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopUp : MonoBehaviour
{

    public int fadeSpeed;

    public GameObject popupObject;

    [SerializeField]
    GameObject popupText;

    TMP_Text tmp_text;

    // Update is called once per frame
    void Start()
    {
        popupText = popupObject.transform.GetChild(0).gameObject;
        TMP_Text tmp_text = popupText.GetComponent<TMP_Text>();
    }

    public void PopUpText(string s)
    {

        popupText.SetActive(true);
        popupObject.SetActive(true);
        tmp_text.text = s;
        tmp_text.faceColor = new Color32(255, 255, 255, 255);
        popupObject.GetComponent<Image>().color = new Color32(0, 0, 0, 127);

        for(int i=0; i<fadeSpeed; ++i)
        {
            
        }
    }
}
