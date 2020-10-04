using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowEffect : MonoBehaviour
{

    Material mat;
    float i = 0;
    public bool active = false;

    private void Start()
    {
        mat = this.GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            if (i < 1) { i += Time.deltaTime * 0.1f; }
            else { i = i - 1; }
            Color color = Color.HSVToRGB(i, 1f, 1f);
            mat.SetColor("_EmissionColor", new Vector4(color.r, color.g, color.b, 1f) * 1f);
        }
    }
}