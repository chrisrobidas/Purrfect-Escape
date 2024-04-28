using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Vector3 openedRotation;
    public Vector3 openedPosition;
    private bool _isOpened;
    public void Open()
    {
        if(!_isOpened)
            StartCoroutine(nameof(OpenDoor));
    }

    IEnumerator OpenDoor()
    {
        print("opening door");
        GetComponent<AudioSource>().Play();
        _isOpened = true;
        while (transform.localPosition != openedPosition || transform.rotation.eulerAngles != openedRotation)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, openedPosition, 0.02f);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(openedRotation), 0.02f);
            yield return new WaitForFixedUpdate();
        }
    }

    public bool IsOpened()
    {
        return _isOpened;
    }
}
