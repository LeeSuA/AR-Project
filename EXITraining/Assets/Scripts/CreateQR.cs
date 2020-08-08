using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZXing;
using TMPro;
using UnityEngine.UI;
using ZXing.QrCode;

public class CreateQR : MonoBehaviour
{ 
    public RawImage codeimage;
    public GameObject inputQR;
    private string code;
    private TouchScreenKeyboard keyboard;

    private void Update()
    {
        
    }
  //  public void openKeyboard()
    //{
      //  keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false, false);
    //}
    private void Start()
    {

    }
    public void OnGUI()
    {
        
        if (Input.GetKeyDown(KeyCode.Return)||(TouchScreenKeyboard.visible==false))
            {
        
            code = inputQR.GetComponent<TMP_Text>().text;
            Debug.Log(code);
            Texture2D myQR = generateQR(code);
            codeimage.GetComponent<RawImage>().texture = generateQR(code);
        }
    }
    public Texture2D generateQR(string text)
    {
        var encoded = new Texture2D(256, 256);
        var color32 = Encode(text, encoded.width, encoded.height);
        encoded.SetPixels32(color32);
        encoded.Apply();
        return encoded;
    }
    private static Color32[] Encode(string textForEncoding, int width, int height)
    {
        var writer = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                Height = height,
                Width = width
            }
        };
        return writer.Write(textForEncoding);
    }
}