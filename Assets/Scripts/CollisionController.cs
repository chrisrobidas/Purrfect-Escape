using UnityEngine;

public class CollisionController : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private Animator _animator;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
    }

    private void OnAnimatorMove()
    {
        _rigidbody.velocity = new Vector3(0,_rigidbody.velocity.y,0);

        _rigidbody.position = _animator.rootPosition;
        _rigidbody.rotation = _animator.rootRotation;
    }

}
