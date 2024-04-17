using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class Door_Interactable : MonoBehaviour, IInteractable
{
    [Header("Interaction Tweaks")]
    [SerializeField] private float openingSpeed = 2.0f;

    private GameObject button;
    private GameObject leftDoor, rightDoor;
    private GameObject leftDoorOpenPos, rightDoorOpenPos;
    private GameObject leftDoorClosedPos, rightDoorClosedPos;
    private bool isOpen;

    // Start is called before the first frame update
    void Start()
    {
        button = GameObject.Find("DoorButton");
        leftDoor = GameObject.Find("LeftDoor");
        rightDoor = GameObject.Find("RightDoor");
        leftDoorOpenPos = GameObject.Find("LeftDoorOpenPosition");
        rightDoorOpenPos = GameObject.Find("RightDoorOpenPosition");
        leftDoorClosedPos = GameObject.Find("LeftDoorClosedPosition");
        rightDoorClosedPos = GameObject.Find("RightDoorClosedPosition");
        isOpen = false;

        if (button != null)
        {
            Debug.Log("Button found.");
        }
        else { Debug.LogWarning("Button was not found."); }

        if (leftDoor != null)
        {
            Debug.Log("LeftDoor found.");
        }
        else { Debug.LogWarning("LeftDoor was not found."); }

        if (rightDoor != null)
        {
            Debug.Log("RightDoor found.");
        }
        else { Debug.LogWarning("RightDoor was not found."); }

        if (leftDoorOpenPos != null)
        {
            Debug.Log("LeftDoorOpenPosition found.");
        }
        else { Debug.LogWarning("LeftDoorOpenPosition was not found."); }

        if (rightDoorOpenPos != null)
        {
            Debug.Log("RightDoorOpenPosition found.");
        }
        else { Debug.LogWarning("RightDoorOpenPosition was not found."); }

        if (leftDoorClosedPos != null)
        {
            Debug.Log("LeftDoorClosedPosition found.");
        }
        else { Debug.LogWarning("LeftDoorClosedPosition was not found."); }

        if (rightDoorClosedPos != null)
        {
            Debug.Log("RightDoorClosedPosition found.");
        }
        else { Debug.LogWarning("RightDoorClosedPosition was not found."); }

        isOpen = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Interact()
    {
        Debug.Log("Door was interacted with.");
        ToggleDoor();
    }

    void ToggleDoor()
    {
        if (!isOpen)
        {
            StartCoroutine(MoveObject(leftDoor, leftDoorOpenPos.transform.position, openingSpeed));
            StartCoroutine(MoveObject(rightDoor, rightDoorOpenPos.transform.position, openingSpeed));

            isOpen = true;

            Debug.Log("Door opened");
        }
        else
        {
            StartCoroutine(MoveObject(leftDoor, leftDoorClosedPos.transform.position, openingSpeed));
            StartCoroutine(MoveObject(rightDoor, rightDoorClosedPos.transform.position, openingSpeed));

            isOpen = false;

            Debug.Log("Door closed");
        }
    }

    /**
    * MoveObject Coroutine
    * ---------------------
    * Moves the object to the target position and rotation.
    */
    IEnumerator MoveObject(GameObject obj, Vector3 targetPos, float duration)
    {
        float time = 0;
        Vector3 startPos = obj.transform.position;

        while (time < duration)
        {
            obj.transform.position = Vector3.Lerp(startPos, targetPos, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        obj.transform.position = targetPos;
    }
}
