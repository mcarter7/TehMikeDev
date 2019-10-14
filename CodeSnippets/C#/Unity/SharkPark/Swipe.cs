using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swipe : MonoBehaviour {

    private bool tap, swipeLeft, swipeRight, swipeUp, swipeDown, 
        swipeDg_UpRight, swipeDg_UpLeft, swipeDg_DownRight, swipeDg_DownLeft;
    private bool isDraging = false;
    private Vector2 startTouch, swipeDelta;

    private void Update()
    {
        if (Player.playerDamaged == true)
            return;


        tap = swipeLeft = swipeRight = swipeUp = swipeDown = swipeDg_UpRight 
            = swipeDg_UpLeft = swipeDg_DownRight = swipeDg_DownLeft = false;

        #region Standalone Inputs
        if (Input.GetMouseButtonDown(0))
        {
            tap = true;
            isDraging = true;
            startTouch = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDraging = false;
            Reset();
        }

        #endregion

        #region Mobile Inputs
        if (Input.touches.Length > 0)
        {
            if(Input.touches[0].phase == TouchPhase.Began)
            {
                isDraging = true;
                tap = true;
                startTouch = Input.touches[0].position;
            }
            else if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)
            {
                isDraging = false;
                Reset();
            }
        }

        #endregion

        // Calculate the distance
        swipeDelta = Vector2.zero;
        if(isDraging)
        {
            if (Input.touches.Length > 0)
                swipeDelta = Input.touches[0].position - startTouch;
            else if (Input.GetMouseButton(0))
                swipeDelta = (Vector2)Input.mousePosition - startTouch;
        }

        //Did we cross the deadzon?
        if(swipeDelta.magnitude > 125)
        {
            //Which direction?
            float x = swipeDelta.x;
            float y = swipeDelta.y;

            if(Mathf.Abs(x) > Mathf.Abs(y))
            {
                //left or right
                if (x < 0)
                {
                    if (y < -25)
                        swipeDg_DownLeft = true;
                    else if (y > 25)
                        swipeDg_UpLeft = true;
                    else if (y > -25 && y < 25)
                        swipeLeft = true;
                }
                else
                {
                    if (y < -25)
                        swipeDg_DownRight = true;
                    else if (y > 25)
                        swipeDg_UpRight = true;
                    else if (y > -25 && y < 25)
                        swipeRight = true;
                }
            }
            else
            {
                //down or up
                if (y < 0)
                {
                    if (x < -25)
                        swipeDg_DownLeft = true;
                    else if (x > 25)
                        swipeDg_DownRight = true;
                    else if (x > -25 && x < 25)
                        swipeDown = true;
                }
                else
                {
                    if (x < -25)
                        swipeDg_UpLeft = true;
                    else if (x > 25)
                        swipeDg_UpRight = true;
                    else if (x > -25 && x < 25)
                        swipeUp = true;
                }
            }

            //Reset();
        }
    }

    private void Reset()
    {
        startTouch = swipeDelta = Vector2.zero;
        isDraging = false;
    }

    public bool Tap { get { return tap; } }
    public Vector2 SwipeDelta { get { return swipeDelta; } }
    public bool SwipeLeft { get { return swipeLeft; } }
    public bool SwipeRight { get { return swipeRight; } }
    public bool SwipeUp { get { return swipeUp; } }
    public bool SwipeDgUpRight { get { return swipeDg_UpRight; } }
    public bool SwipeDgUpLeft { get { return swipeDg_UpLeft; } }
    public bool SwipeDown { get { return swipeDown; } }
    public bool SwipeDgDownRight { get { return swipeDg_DownRight; } }
    public bool SwipeDgDownLeft { get { return swipeDg_DownLeft; } }

}
