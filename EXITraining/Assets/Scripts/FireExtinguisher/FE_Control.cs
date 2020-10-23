using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FE_Control : MonoBehaviour, IDragHandler, IPointerDownHandler, IEndDragHandler, IPointerUpHandler
{

    Vector3 LastTouchPoint;
    Vector3 NowTouchPoint;
    public Camera cam;
    public Transform pin;
    public Transform body;
    public Transform handle;
    public Transform FE_parent;
    public ParticleSystem Extinguisher_Particle;

    RaycastHit firstHit;

    [SerializeField]
    public bool handle_is_touched = false;
    bool somethingHit = false;
    bool pinOut = false;
    
    /* ------------------------------------- */

    public void OnPointerDown(PointerEventData eventData)
    {
        OnPointerDown(eventData.position);
        HandleUp();
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (somethingHit) OnDrag(eventData.position);

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        OnEndDrag();
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        HandleDown();
    }
    /* ------------------------------------- */


    private void OnPointerDown(Vector2 a_FirstPoint)
    {
        Ray touchray = cam.ScreenPointToRay(a_FirstPoint);
        Physics.Raycast(touchray, out firstHit);

        if (firstHit.collider != null)
        {
            somethingHit = true;
        }
        else somethingHit = false;
        NowTouchPoint = a_FirstPoint;
        LastTouchPoint = a_FirstPoint;

    }

    private void OnDrag(Vector2 a_SecondPoint)
    {
        NowTouchPoint = a_SecondPoint;
        float deltaX = (LastTouchPoint.x - NowTouchPoint.x) / Screen.width;

        if (somethingHit)
        {
            // 소화기 회전
            if (firstHit.transform.tag == "Body")
            {
                FE_parent.Rotate(Vector3.up * deltaX * 70f, Space.Self);
            }
            // 핀 뽑기
            else if (!pinOut && firstHit.transform.tag == "Pin")
            {
                // if (pin.transform.position.x >= 0 && deltaX < 0) deltaX = 0; // 위치제한

                if (FE_parent.rotation.y < 90 && FE_parent.rotation.y > -90)
                {
                    if (pin.transform.position.x - deltaX > 0) pin.transform.Translate(Vector3.left * pin.transform.position.x * 0.4f);
                    else pin.transform.Translate(Vector3.left * deltaX, Space.Self);
                }
                else
                {
                    if (pin.transform.position.x + deltaX > 0) pin.transform.Translate(Vector3.right * pin.transform.position.x / 1.2f);
                    else pin.transform.Translate(Vector3.left * deltaX, Space.Self);
                }
            }
        }
        LastTouchPoint = a_SecondPoint;
    }

    private void OnEndDrag()
    {
        if (!pinOut)
        {
            if (pin.transform.localPosition.x <= -0.3f) // 핀이 좀 나왔으면 핀 제거
            {
                pinOut = true;
                handle.GetComponent<GlowEffect>().active = true;
                StartCoroutine(PinOut());
            }
        }
    }

    private void HandleUp() // 핸들 누르기
    {
        // 핸들 누르는 동작
        if (pinOut && somethingHit && firstHit.transform.tag == "Handle")
        {
            handle_is_touched = true;
            StartCoroutine(HandleRotate_Upward());
            Extinguisher_Particle.Play();
        }
    }

    private void HandleDown() // 핸들 떼기
    {
        if (pinOut && somethingHit && firstHit.transform.tag == "Handle")
        {
            handle_is_touched = false;
            StartCoroutine(HandleRotate_Downward());
            Extinguisher_Particle.Stop();
        }
        somethingHit = false;
    }

    IEnumerator PinOut()
    {
        Material mat = pin.GetComponent<MeshRenderer>().material;
        Vector3 pin_firstPosition = pin.transform.position;
        while (mat.color.a >= 0.0001f)
        {
            mat.color = (new Color(0, 0, 0, mat.color.a - Time.deltaTime * 2));
            pin.position = Vector3.Lerp(pin.transform.position, pin_firstPosition - Vector3.right * 0.2f, 0.1f);
            yield return null;
        }
        if(mat.color.a <= 0.0001f)
        Destroy(pin.gameObject);
    }

    IEnumerator HandleRotate_Upward()
    {
        Transform handlePivot = handle.transform.parent.transform;
        while (handle_is_touched && handlePivot.localRotation.x < 0.15f)
        {
            handlePivot.Rotate(Time.deltaTime * 200, 0, 0);
            yield return null;
        }
    }

    IEnumerator HandleRotate_Downward()
    {
        Transform handlePivot = handle.transform.parent.transform;
        while (!handle_is_touched && handlePivot.localRotation.x > 0)
        {
            handlePivot.Rotate(-Time.deltaTime * 200, 0, 0);
            yield return null;
        }
    }
}