using UnityEngine;

public class RandomRotation : MonoBehaviour
{
    void Start()
    {
        Vector3 randomRotation = new Vector3(transform.rotation.x, transform.rotation.y, transform.rotation.z);
        randomRotation.y = randomRotation.y + 90 * Random.Range(1,5);
        transform.rotation = Quaternion.Euler(randomRotation);
    }
}
