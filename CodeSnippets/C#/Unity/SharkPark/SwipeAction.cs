using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeAction : MonoBehaviour
{
    Vector2 startPos, endPos, direction;
    float touchTimeStart, touchTimeFinish, timeInterval;

    [Range(0.001f, 1f)]
    public float throwForce;

    private bool dragging;
    public Camera Camera;
    private Vector3 _target;
    LineRenderer line;

    private void Start()
    {
        dragging = false;
    }

    void Update()
    {
        if (FishingHookAction.sharkCaught || Player.playerDamaged || !Player.gamePlayerFreed)
            return;

        #region Mobile Inputs
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            touchTimeStart = Time.time;
            startPos = Input.GetTouch(0).position;
        }
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            touchTimeFinish = Time.time;
            timeInterval = touchTimeFinish - touchTimeStart;
            endPos = Input.GetTouch(0).position;
            direction = startPos - endPos;
            GetComponent<Rigidbody2D>().AddForce(-direction / timeInterval * throwForce);
        }
        #endregion

        #region Standalone Inputs
        if (Input.GetMouseButtonDown(0))
        {
            touchTimeStart = Time.time;
            startPos = Input.mousePosition;
            dragging = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            dragging = false;
            touchTimeFinish = Time.time;
            timeInterval = touchTimeFinish - touchTimeStart;
            endPos = Input.mousePosition;
            direction = (Vector2)Input.mousePosition - startPos;

            var force = direction / timeInterval * throwForce;

            if (timeInterval > 0.8)
            {
                timeInterval = 0.5f;
                force = direction / timeInterval * throwForce;
            }

            Rigidbody2D playerRb2d = GetComponent<Rigidbody2D>();

            playerRb2d.AddForce(force);
        }

        if (dragging == true && Force.inForce != true && ForceDown.inForce != true)
        {
            _target = Camera.ScreenToWorldPoint(Input.mousePosition);
            gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, _target, 2 * Time.deltaTime);
        }

        #endregion
    }
}