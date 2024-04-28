using System;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public enum Type {
        Paw,
        Scratch,
        Bite
    };

    public enum ActivableType
    {
        None,
        SetActive,
        Increment
    }

    public Type InteractionType;

    public string PuzzleName;

    public GameObject Activable;
    public ActivableType InteractableActivableType;

    public int Index;

    public bool MoveObject;
    
    public void InteractAction()
    {
        switch (InteractableActivableType)
        {
            case ActivableType.SetActive:
                Activable.SetActive(true);
                break;
            case ActivableType.Increment:
                Activable.GetComponent<Puzzle>().IncrementCode(Index);
                break;
        }
        
        if (MoveObject)
        {
            GetComponent<Rigidbody>().AddExplosionForce(4, FindObjectOfType<InputController>().transform.position,3,0.5f, ForceMode.Impulse);
        }
        
        if(GetComponent<AudioSource>() != null)
            GetComponent<AudioSource>().Play();
    }
}
