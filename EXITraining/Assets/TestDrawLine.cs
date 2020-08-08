using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDrawLine : MonoBehaviour
{
    LineRenderer lr;
    Vector3 cube1Pos, cube2Pos;

    private void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.startWidth = .05f;
        lr.endWidth = .05f;

        cube1Pos = gameObject.GetComponent<Transform>().position;
    }

    void Update()
    {
        lr.SetPosition(0, cube1Pos);
        lr.SetPosition(1, GameObject.Find("Cube2").GetComponent<Transform>().position);
    }
}