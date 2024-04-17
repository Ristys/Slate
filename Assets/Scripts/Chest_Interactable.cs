using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class Chest_Interactable : MonoBehaviour, IInteractable
{
    [Header("Interaction Tweaks")]
    [SerializeField] private float openingSpeed = 2.0f;

    private GameObject lid, lidOpenPos, lidClosedPos;
    private bool isOpen;

    // Start is called before the first frame update
    void Start()
    {
        lid = GameObject.Find("Lid");
        lidOpenPos = GameObject.Find("LidOpenPosition");
        lidClosedPos = GameObject.Find("LidClosedPosition");
        isOpen = false;

        if (lid != null)
        {
            Debug.Log("Lid found.");
        } else { Debug.LogWarning("Lid was not found."); }

        if (lidOpenPos != null)
        {
            Debug.Log("LidOpenPosition found.");
        }
        else { Debug.LogWarning("LidOpenPosition was not found."); }

        if (lidClosedPos != null)
        {
            Debug.Log("LidClosedPosition found.");
        }
        else { Debug.LogWarning("LidClosedPosition was not found."); }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Interact()
    {
        Debug.Log("Chest was interacted with.");
        ToggleChest();
    }

    private void ToggleChest()
    {
        if (!isOpen)
        {
            StartCoroutine(MoveObject(lid, lidOpenPos, openingSpeed));
            isOpen = true;
            Debug.Log("Chest opened");
        }
        else
        {
            StartCoroutine(MoveObject(lid, lidClosedPos, openingSpeed));
            isOpen = false;
            Debug.Log("Chest closed");
        }
    }

     /**
     * MoveObject Coroutine
     * ---------------------
     * Moves the chest lid to the target position and rotation.
     */
    IEnumerator MoveObject(GameObject obj, GameObject target, float duration)
    {
        float time = 0;
        Vector3 startPos = obj.transform.position;
        Vector3 targetPos = target.transform.position;
        Quaternion startRot = obj.transform.rotation;
        Quaternion targetRot = target.transform.rotation;

        while (time < duration)
        {
            obj.transform.position = Vector3.Lerp(startPos, targetPos, time / duration);
            obj.transform.rotation = Quaternion.Lerp(startRot, targetRot, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        obj.transform.position = targetPos;
        obj.transform.rotation = targetRot;
    }
}
