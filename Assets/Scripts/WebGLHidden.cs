using UnityEngine;

[RequireComponent(typeof(GameObject))]
public class WebGLHidden : MonoBehaviour
{
    private void Awake()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            gameObject.SetActive(false);
        }
    }
}
