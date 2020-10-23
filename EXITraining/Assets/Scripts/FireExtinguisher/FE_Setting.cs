using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FE_Setting : MonoBehaviour
{
    private bool settingExtinguisher = false;

    public void Set()
    {
        settingExtinguisher = true;
        this.gameObject.SetActive(true);
        StartCoroutine(SettingCoroutine());
    }

    public void Eliminate()
    {
        settingExtinguisher = false;
        this.gameObject.SetActive(true);
        StartCoroutine(EliminatingCoroutine());
    }

    IEnumerator SettingCoroutine()
    {
        this.gameObject.SetActive(true);
        while (settingExtinguisher && this.transform.localPosition.y <= -0.417f)
        {
            this.transform.Translate(Vector3.up * Time.deltaTime);
            yield return null;
        }
    }

    IEnumerator EliminatingCoroutine()
    {
        while (!settingExtinguisher && this.transform.localPosition.y >= -1f)
        {
            this.transform.Translate(Vector3.up * -Time.deltaTime);
            yield return null;
        }
        if (!settingExtinguisher) this.gameObject.SetActive(false);
    }
}
