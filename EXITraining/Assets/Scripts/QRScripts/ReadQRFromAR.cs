using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using ZXing;


public class ReadQRFromAR : MonoBehaviour
{

    Texture2D m_Texture;
    public ARCameraManager cameraManager;
    XRCameraImage image;

    IBarcodeReader reader = new BarcodeReader();
    bool foundQR = false;

    private void Start()
    {
        if (foundQR)
        {
            cameraManager.frameReceived += OnFrameReceived;
        }
        else
        {
            cameraManager.frameReceived -= OnFrameReceived;
        }
        
    }

    private unsafe void OnFrameReceived(ARCameraFrameEventArgs eventArgs)
    {
        if (!cameraManager.TryGetLatestImage(out image))
            return;

        var conversionParams = new XRCameraImageConversionParams
        {
            // Get the entire image
            inputRect = new RectInt(0, 0, image.width, image.height),

            // Downsample by 2
            outputDimensions = new Vector2Int(image.width / 2, image.height / 2),

            // Choose RGBA format
            outputFormat = TextureFormat.RGBA32,

            // Flip across the vertical axis (mirror image)
            transformation = CameraImageTransformation.MirrorY
        };

        // See how many bytes we need to store the final image.
        int size = image.GetConvertedDataSize(conversionParams);

        // Allocate a buffer to store the image
        var buffer = new NativeArray<byte>(size, Allocator.Temp);

        // Extract the image data
        image.Convert(conversionParams, new IntPtr(buffer.GetUnsafePtr()), buffer.Length);

        // The image was converted to RGBA32 format and written into the provided buffer
        // so we can dispose of the CameraImage. We must do this or it will leak resources.
        image.Dispose();

        // At this point, we could process the image, pass it to a computer vision algorithm, etc.
        // In this example, we'll just apply it to a texture to visualize it.

        // We've got the data; let's put it into a texture so we can visualize it.
        m_Texture = new Texture2D(
            conversionParams.outputDimensions.x,
            conversionParams.outputDimensions.y,
            conversionParams.outputFormat,
            false);

        m_Texture.LoadRawTextureData(buffer);
        m_Texture.Apply();

        // Done with our temporary data
        buffer.Dispose();
    }
    
    public string GetCodeFromQR(Texture2D tex)
    {
        var barcodeBitmap = tex.GetPixels32();
        var result = reader.Decode(barcodeBitmap, tex.width, m_Texture.height);
        if (result != null)
        {
            Debug.Log(result.BarcodeFormat.ToString());
            Debug.Log(result.Text);

            return result.Text;
        }
        else
        {
            return "-NULL-";
        }
    }
}