using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Joystick : MonoBehaviour
{
    public GameObject character;

    private Vector2 center;
    public Vector2 direction;
    public float maxDistance;

    public GameObject joystickBG;
    private bool joystickTouch = false;
    public void JoystickEnter()
    {
        joystickBG.SetActive(true);
        center = Input.mousePosition;
        joystickBG.transform.position = center;
        joystickTouch = true;
    }
    public void JoystickExit()
    {
        joystickBG.SetActive(false);
        joystickTouch = false;
        direction = new Vector2(0, 0);
    }
    void Update()
    {
        if((Input.touchCount > 0 || Input.GetMouseButton(0)) && joystickTouch)
        {
            Vector2 touchPosition = Input.mousePosition;
            float distance = Vector2.Distance(center, touchPosition);
            direction = touchPosition - center;
            direction.Normalize();
            if(distance > maxDistance)
                touchPosition = center + direction * maxDistance;
            else
                direction *= distance / maxDistance; //for slow movement
            joystickBG.transform.GetChild(0).position = touchPosition;
        }
        character.GetComponent<CharacterController>().Move(new Vector3(direction.x, 0, direction.y) * Time.deltaTime * 4f);
    }
}