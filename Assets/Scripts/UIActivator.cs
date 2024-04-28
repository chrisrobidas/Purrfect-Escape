using UnityEngine;

public class UIActivator : MonoBehaviour
{
    [SerializeField]
    private GameObject _interactionButton;
    private void OnCollisionEnter(Collision collision)
    {
        print("stuff");
        if(collision.gameObject.GetComponent<Interactable>() != null)
            _interactionButton.SetActive(true);
        else
        {
            _interactionButton.SetActive(false);
        }
    }
}
