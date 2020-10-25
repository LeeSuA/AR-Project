using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneMove : MonoBehaviour
{
    public GameObject blackPanel;
    public Animator anim;
    private int index = 0;

    public void SceneMoveTo(int sceneIndex)
    {
        StartCoroutine(Fading());
        index = sceneIndex;
    }

    IEnumerator Fading()
    {
        anim.SetBool("Fade", true);
        yield return new WaitUntil(() => blackPanel.GetComponent<RawImage>().color.a >= 1);
        SceneManager.LoadScene(index);
    }
}