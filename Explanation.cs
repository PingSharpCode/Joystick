using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Joystick : MonoBehaviour
{
    [SerializeField] private RectTransform image;
    [SerializeField] private RectTransform canvas;
    [SerializeField] private GameObject joystickBG;
    [SerializeField] private CharacterController CC;

    private Vector2 center;
    private Vector2 direction;
    private Vector2 touchPosition;
    private float maxDistance;
    private bool joystickTouch;
    private int touchID;

    private void Awake()
    {
        joystickTouch = false;
        touchID = -1;
    }

    private void Update()
    {
        if (SystemInfo.deviceType == DeviceType.Handheld)
            HandleTouchInput();
        else
            HandleMouseInput();

        HandleJoystick();
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0) && IsInsideImage(Input.mousePosition))
        {
            touchPosition = Input.mousePosition;
            JoystickEnter();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            JoystickExit();
        }

        if (Input.GetMouseButton(0) && joystickTouch)
        {
            touchPosition = Input.mousePosition;
        }
    }

    private void HandleTouchInput()
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            
            if (touch.phase == TouchPhase.Began && !joystickTouch && IsInsideImage(touch.position))
            {
                //touch.phase includes: Began, Stationary, Moved, Ended and Cancelled

                touchID = i; //touchID gets stored when "button" is pressed
                touchPosition = touch.position;
                JoystickEnter();
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                if (i == touchID)
                {
                    JoystickExit();
                }

                if (i < touchID)
                {
                    //touches are stored in chronological order
                    //when a touch is released, the next touch takes its place, and ID needs to be reassigned
                    touchID--;
                }
            }
        }

        if (touchID != -1) //if a touch that activated the "button" is still on screen
        {
            touchPosition = Input.GetTouch(touchID).position;
        }
    }

    private bool IsInsideImage(Vector2 position)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, position, canvas.GetComponent<Canvas>().worldCamera, out position);
        //"Transform a screen space point to a position in the local space of a RectTransform that is on the plane of its rectangle"
        //https://docs.unity3d.com/ScriptReference/RectTransformUtility.ScreenPointToLocalPointInRectangle.html
        Vector2 imagePos = new Vector2(image.localPosition.x, image.localPosition.y);
        Vector2 imageSize = new Vector2(image.rect.width, image.rect.height);
        Vector2 topRight = imageSize / 2f + imagePos;
        Vector2 bottomLeft = topRight - imageSize;


        //checks whether the initial touch is within the button's RectTransform
        return position.x > bottomLeft.x && position.y > bottomLeft.y && position.x < topRight.x && position.y < topRight.y;

    }

    private void JoystickEnter()
    {
        joystickBG.SetActive(true);
        center = touchPosition; //stores the position of the initial touch, where the joystick will be centered
        joystickBG.transform.position = center;
        joystickTouch = true;


        //this is a cap for how far away the dot image of the joystick will go. 1 radius here.
        maxDistance = joystickBG.GetComponent<RectTransform>().rect.height / 2f * Screen.width / canvas.GetComponent<CanvasScaler>().referenceResolution.x;
    }

    private void JoystickExit()
    {
        joystickBG.SetActive(false);
        joystickTouch = false;
        direction = Vector2.zero; //Vector2.zero is the same as "new Vector2(0, 0)". Vector2.one works the same way.
        touchID = -1;
    }

    private void HandleJoystick()
    {
        if (!joystickTouch) return; //do not continue if the button is not pressed / has been released

        float distance = Vector2.Distance(center, touchPosition); //distance between touch and the inital touch position

        direction = (touchPosition - center).normalized; //.normalized scales a Vector2 (Vector basically means a direction) to length 1. Angle is preserved
        if (distance > maxDistance)
            touchPosition = center + direction * maxDistance; //touchposition gets locked within the maxdistance of the center
        else
            direction *= distance / maxDistance; //this can be removed, used for slow movement if the touch is still within the max distance

        joystickBG.transform.GetChild(0).position = touchPosition; //sets position of the dot

        //CC.Move(new Vector3(direction.x, 0, direction.y) * Time.deltaTime * 4f);
    }
}
