using UnityEngine;

public class Bottle : MonoBehaviour
{
    public bool ContainsKey;
    public Mesh KeyMesh;
    public Material KeyMaterial;
    public AudioClip Pop;
    public AudioClip Crack;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.name.StartsWith("Floor"))
        {
            if (ContainsKey)
            {
                GetComponent<MeshFilter>().mesh = KeyMesh;
                GetComponent<MeshRenderer>().material = KeyMaterial;
                GetComponent<Interactable>().MoveObject = false;
                GetComponent<Interactable>().InteractionType = Interactable.Type.Bite;
                GetComponent<Interactable>().PuzzleName = "key";
                GetComponent<AudioSource>().clip = Pop;
            }
            else
            {
                transform.position = new Vector3(-9999f, -9999f,-9999f);
            }
            
            GetComponent<AudioSource>().PlayOneShot(Crack);
        }
    }
}
