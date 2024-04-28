using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;
using UnityEngine.UI;
using Cursor = UnityEngine.Cursor;

public class InputController : MonoBehaviour
{
    private bool _catCanMove;

    private Animator _animator;
    private CinemachineVirtualCamera _virtualCamera;
    private CinemachineTransposer _transposer;
    private Rigidbody _rigidbody;

    [SerializeField]
    private GameObject _interactButton;
    
    [SerializeField]
    private SphereCollider _sphereCollider;

    [SerializeField] private float _ySensitivity = 0.01f;
    [SerializeField] private float _yMin;
    [SerializeField] private float _yMax;
    [SerializeField] private bool _YInverse;

    [SerializeField] private float _damping;
    [SerializeField] private float _jumpForce;

    [SerializeField] private GameObject _indicatorPrefab;
    //KEY: Interactable, VALUE: Indicator
    private Dictionary<GameObject, GameObject> _indicatorByInteractables;


    [SerializeField] private GameObject _canvas;
    [SerializeField] private Transform _mouthHolder;
    [SerializeField] private AudioClip _waterSplash;

    private bool _isPlaying;
    private bool _isGameEnded;

    private AudioSource _audioSource;
    
    void Start()
    {
        _catCanMove = false;
        _isGameEnded = false;
    }

    public void WakeUpCat()
    {
        _indicatorByInteractables = new Dictionary<GameObject, GameObject>();
            
        _animator = GetComponent<Animator>();
        _virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        _transposer = _virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        _rigidbody = GetComponent<Rigidbody>();
        _audioSource = GetComponent<AudioSource>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (_YInverse)
            _ySensitivity *= -1;

        InitializeIndicators(FindObjectsOfType<Interactable>().Select(x => x.gameObject).ToList());
        
        _isPlaying = true;
    }

    public void ResumeCat()
    {
        _catCanMove = true;
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        Time.timeScale = 1;
    }

    public void StopCat()
    {
        _catCanMove = false;
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0;
    }

    private void InitializeIndicators(List<GameObject> interactables)
    {
        foreach (var obj in interactables)
        {
            GameObject interactableDot = Instantiate(_indicatorPrefab, _canvas.transform);
            _indicatorByInteractables.Add(obj, interactableDot);
        }
    }

    private void TransformIndicators()
    {
        foreach (var pair in _indicatorByInteractables)
        {
            if (pair.Key == null)
            {
                _indicatorByInteractables.Remove(pair.Key);
                return;
            }
            
            Vector3 pos = Camera.main.WorldToViewportPoint(pair.Key.transform.position) - Vector3.up * 0.02f;

            if (Vector3.Distance(transform.position, pair.Key.transform.position) < 2 && pos.z > 0 && pos.x > 0 && pos.x < 1 && pos.y > 0 && pos.y < 1)
            {
                pair.Value.GetComponent<RectTransform>().anchorMax = pos;
                pair.Value.GetComponent<RectTransform>().anchorMin = pos;
                pair.Value.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                pair.Value.SetActive(true);
            }
            else
            {
                pair.Value.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            if (!_animator.GetBool("IsInWater"))
            {
                _audioSource.Stop();
                _audioSource.PlayOneShot(_waterSplash, 0.7f);
            }
            _animator.SetBool("IsInWater", true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            _animator.SetBool("IsInWater", false);
        }
    }

    public void SetEndMovement(Transform endTransform)
    {
        _isGameEnded = true;

        _catCanMove = false;
        _isPlaying = false;

        transform.rotation = endTransform.rotation;
        transform.position = new Vector3(endTransform.position.x, 0, endTransform.position.z);

        _animator.SetFloat("SpeedZ", 0.5f);

    }

    void Update()
    {
        if(_isGameEnded)
            return;
        
        Vector3 inputDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        inputDirection.Normalize();
        
        if(_audioSource != null && inputDirection.magnitude == 0 && _audioSource.isPlaying)
            _audioSource.Stop();

        if ((!_catCanMove || !_isPlaying) && inputDirection.magnitude == 0)
            return;

        _catCanMove = true;
        _animator.SetBool("IsSleeping", false);
        
        _transposer.m_FollowOffset = new Vector3(_transposer.m_FollowOffset.x,Mathf.Clamp(_transposer.m_FollowOffset.y + Input.GetAxis("Mouse Y") * _ySensitivity, _yMin, _yMax), _transposer.m_FollowOffset.z);

        int divider = Input.GetKey(KeyCode.LeftShift) ? 1 : 2;

        _animator.SetFloat("SpeedX", inputDirection.x, _damping, Time.deltaTime);
        _animator.SetFloat("SpeedZ", inputDirection.z/divider, _damping, Time.deltaTime);

        if (divider == 1)
            _audioSource.pitch = 1.5f;
        else
            _audioSource.pitch = 1f;
        
        if(!_audioSource.isPlaying && _animator.GetCurrentAnimatorStateInfo(0).IsName("WalkBlendTree"))
            _audioSource.Play();
        
        _animator.SetBool("IsCrouching", Input.GetKey(KeyCode.LeftControl));
        if (_animator.GetBool("IsCrouching"))
            _audioSource.volume = 0.5f;
        else
            _audioSource.volume = 1f;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_animator.GetCurrentAnimatorStateInfo(0).IsName("WalkBlendTree"))
            {
                _animator.SetTrigger("Jump");
                Invoke(nameof(Jump), 0.2f);
            }
        }
        Collider[] hitColliders = Physics.OverlapSphere(_sphereCollider.transform.position, _sphereCollider.radius);
        Interactable interactable = hitColliders.FirstOrDefault(x => x.GetComponent<Interactable>())?.GetComponent<Interactable>();
        
        if (interactable != null && 
            _indicatorByInteractables.ContainsKey(interactable.gameObject) && 
            _indicatorByInteractables[interactable.gameObject].GetComponent<Image>().enabled)
        {
            _interactButton.SetActive(true);

            Vector2 pos = Camera.main.WorldToViewportPoint(interactable.transform.position) + Vector3.up * 0.1f;
            _interactButton.GetComponent<RectTransform>().anchorMax = pos;
            _interactButton.GetComponent<RectTransform>().anchorMin = pos;
        }
        else
        {
            _interactButton.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if(hitColliders.Count(x => x.gameObject.GetComponent<Interactable>() != null) > 0)
            {
                switch (hitColliders.FirstOrDefault(x => x.GetComponent<Interactable>())?.gameObject.GetComponent<Interactable>().InteractionType)
                {
                    case Interactable.Type.Paw:
                        InteractAnimation(0);
                        hitColliders.FirstOrDefault(x => x.GetComponent<Interactable>())?.GetComponent<Interactable>().InteractAction();
                        break;
                    case Interactable.Type.Scratch:
                        InteractAnimation(1);
                        hitColliders.FirstOrDefault(x => x.GetComponent<Interactable>())?.GetComponent<Interactable>().InteractAction();
                        break;
                    case Interactable.Type.Bite:
                        InteractAnimation(2);
                        hitColliders.FirstOrDefault(x => x.GetComponent<Interactable>())?.GetComponent<Interactable>().InteractAction();
                        Hold(hitColliders.FirstOrDefault(x => x.GetComponent<Interactable>())?.gameObject);
                        break;
                }
            }
        }
        
        //_animator.SetFloat("SpeedY", _rigidbody.velocity.y, _damping, Time.deltaTime);
    }

    private void InteractAnimation(int anim)
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("WalkBlendTree"))
        {
            _animator.SetInteger("Interaction", anim);
            _animator.SetTrigger("Interact");
        }
    }

    private void Hold(GameObject target)
    {
        if (_mouthHolder.transform.childCount == 0)
        {
            target.GetComponent<Rigidbody>().isKinematic = true;
            target.GetComponent<Collider>().isTrigger = true;
            target.transform.SetParent(_mouthHolder);
            target.transform.localPosition = Vector3.zero + Vector3.up * 0.00078f;
            _indicatorByInteractables[target].GetComponent<Image>().enabled = false;
        }
        else
        {
            target.transform.SetParent(null);
            target.GetComponent<Rigidbody>().isKinematic = false;
            target.GetComponent<Collider>().isTrigger = false;
            _indicatorByInteractables[target].GetComponent<Image>().enabled = true;
        }
    }

    private void LateUpdate()
    {
        if (!_catCanMove)
            return;
        
        TransformIndicators();
    }

    private void Jump()
    {
        _rigidbody.AddForce(new Vector3(0,1 * _jumpForce,0), ForceMode.Impulse);
    }
}
