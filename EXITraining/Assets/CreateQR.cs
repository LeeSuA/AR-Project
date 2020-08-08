using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZXing;
using TMPro;
using UnityEngine.UI;
using ZXing.QrCode;

public class CreateQR : MonoBehaviour
{
   // [SerializeField] private BarcodeFormat format = BarcodeFormat.QR_CODE;
    public RawImage codeimage;
    public GameObject inputQR;
    private string code;
  
    // Start is called before the first frame update
    private void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {
        
    }
   public void OnGUI()
    {
        code = inputQR.GetComponent<TMP_Text>().text;
        Texture2D myQR = generateQR(code);
        codeimage.GetComponent<RawImage>().texture = generateQR(code);

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


