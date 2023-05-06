using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JoystickV2 : MonoBehaviour
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
                touchID = i;
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
                    touchID--;
                }
            }
        }

        if (touchID != -1)
        {
            touchPosition = Input.GetTouch(touchID).position;
        }
    }

    private bool IsInsideImage(Vector2 position)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, position, canvas.GetComponent<Canvas>().worldCamera, out position);
        Vector2 imagePos = new Vector2(image.localPosition.x, image.localPosition.y);
        Vector2 imageSize = new Vector2(image.rect.width, image.rect.height);
        Vector2 topRight = imageSize / 2f + imagePos;
        Vector2 bottomLeft = topRight - imageSize;

        return position.x > bottomLeft.x && position.y > bottomLeft.y && position.x < topRight.x && position.y < topRight.y;
    }

    private void JoystickEnter()
    {
        joystickBG.SetActive(true);
        center = touchPosition;
        joystickBG.transform.position = center;
        joystickTouch = true;
        maxDistance = joystickBG.GetComponent<RectTransform>().rect.height / 2f * Screen.width / canvas.GetComponent<CanvasScaler>().referenceResolution.x;
    }

    private void JoystickExit()
    {
        joystickBG.SetActive(false);
        joystickTouch = false;
        direction = Vector2.zero;
        touchID = -1;
    }

    private void HandleJoystick()
    {
        if (!joystickTouch) return;

        float distance = Vector2.Distance(center, touchPosition);
        direction = (touchPosition - center).normalized;

        if (distance > maxDistance)
            touchPosition = center + direction * maxDistance;
        else
            direction *= distance / maxDistance;

        joystickBG.transform.GetChild(0).position = touchPosition;

        //CC.Move(new Vector3(direction.x, 
    }
}
