using System.Collections;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndController : MonoBehaviour
{
    public Door EndDoor;
    public Transform EndCameraTransform;
    public Image EndBlackCover;
    public float FadeSpeed = 1f;
    public GameObject EndText;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<InputController>() != null && EndDoor.IsOpened())
        {
            FindObjectOfType<CinemachineVirtualCamera>()?.gameObject.SetActive(false);
            Camera.main.transform.parent = EndCameraTransform;
            Camera.main.transform.localPosition = Vector3.zero;
            Camera.main.transform.localRotation = Quaternion.identity;
            
            other.GetComponent<InputController>().SetEndMovement(EndCameraTransform);
            StartCoroutine(FadeToBlack());
        }
    }

    private IEnumerator FadeToBlack()
    {
        Color color = EndBlackCover.color;
        float fadeAmount;

        while (EndBlackCover.color.a < 1)
        {
            fadeAmount = color.a + (FadeSpeed * Time.deltaTime);

            color = new Color(color.r, color.g, color.b, fadeAmount);
            EndBlackCover.color = color;
            yield return null;
        }
        
        EndText.SetActive(true);
        yield return new WaitForSeconds(3);
        EndText.GetComponent<TMP_Text>().text = "Thank you for playing our game.";
        yield return new WaitForSeconds(3);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
