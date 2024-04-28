using System;
using System.Linq;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;
using TMPro;
public class Puzzle : MonoBehaviour
{
    public enum Type {
        Collect,
        Colors,
        Code
    };

    public Type PuzzleType;

    public int RequiredAmount;
    public int CollectedAmount = 0;
    public string PuzzleName;

    public Transform[] Slots;
    
    public Door door;
    private string _code;

    [SerializeField] private TMP_Text _codeText;
    public AudioClip Pop;
    
    private void Awake()
    {
        _code = Random.Range(1000, 10000).ToString("# # # #");
    }

    public void IncrementCode(int index)
    {
        int newNumber = Int32.Parse(_codeText.text[index].ToString()) + 1;
        StringBuilder sb = new StringBuilder(_codeText.text);
        sb[index] = newNumber > 9 ? '0' : newNumber.ToString().ToCharArray()[0];
        _codeText.text = sb.ToString();
        
        if(_code == _codeText.text)
            door.Open();
    }

    public string GetCode()
    {
        return _code;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (PuzzleType == Type.Collect)
        {
            if (other.GetComponent<Interactable>() &&
                other.GetComponent<Interactable>().PuzzleName == PuzzleName)
            {
                if (Pop != null)
                {
                    print("pop");
                    GetComponent<AudioSource>().PlayOneShot(Pop);
                }
                
                other.GetComponent<Interactable>().enabled = false;
                other.GetComponent<Collider>().enabled = false;

                if (Slots != null && Slots.Length > 0)
                {
                    other.transform.parent = Slots[other.GetComponent<Interactable>().Index];
                    other.transform.localPosition = Vector3.zero;
                    other.transform.localRotation = Quaternion.identity;
                }
                else
                {
                    other.transform.parent = null;
                    other.transform.position = new Vector3(-9999f, -9999f,-9999f);
                }

                CollectedAmount++;
                
                if (CollectedAmount >= RequiredAmount)
                {
                    door.Open();
                }
            }
        }
    }
}
