using System.Collections;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Color = UnityEngine.Color;

public class Player : MonoBehaviour
{
    private Animator _animator;
    private CameraController _cameraController;
    private Outline _targetObj;

    [SerializeField] Slider _slider;
    [SerializeField] ParticleSystem _cancelVFX;
    [SerializeField] ParticleSystem _swapVFX;
    [SerializeField] Transform _cancelVFXPos;

    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _detectionRange = 6f;
    [SerializeField] private float _rotatitonSpeed = 500f;

    private float _dampTime = 0.2f;
    private float _objDistance = 20f;

    private Vector3 _selectedObjectPosition;
    private Vector3 _currentTransform;
    private RaycastHit _hit;
    private Quaternion _targetRotation;

    private bool _isConnected;
    private bool _isOutLine;
    private float _distanceBetweenObjects;
    private float _holdtimer;

    //Animation States
    const string PLAYER_CHOSE = "chose";
    const string PLAYER_TELEPORT = "teleport";
    const string PLAYER_CANCEL = "cancel";

    private void Awake()
    {
        _slider = FindObjectOfType<Slider>().GetComponent<Slider>();
        _animator = GetComponent<Animator>();
        _cameraController = Camera.main.GetComponent<CameraController>();
    }

    void Update()
    {
        PlayerMovement();
        SwapObject();
    }

    void PlayerMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        float moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
        Vector3 velocity = (new Vector3(horizontal, 0, vertical)).normalized;
        var moveDir = _cameraController.PlanarRotation * velocity;

        if (moveAmount > 0)
        {
            transform.position += moveDir * _speed * Time.deltaTime;
            _targetRotation = Quaternion.LookRotation(moveDir);
        }

        transform.rotation = Quaternion.RotateTowards(transform.rotation, _targetRotation, _rotatitonSpeed * Time.deltaTime);
        _animator.SetFloat("moveAmount", moveAmount, _dampTime, Time.deltaTime);
    }

    void ChangeAnimationState(string newState)
    {
        _animator.Play(newState);
    }

    void SwapObject()
    {
        if (Input.GetKeyUp(KeyCode.E))
        {
            Debug.Log("e tuþu release edildi");
            if (_isConnected == false)
            {
                Ray ray = new Ray(this.transform.position, this.transform.TransformDirection(Vector3.forward));
                if (Physics.Raycast(ray, out _hit, _detectionRange, LayerMask.GetMask("Swapable")) && _isConnected == false)
                {
                    ChangeAnimationState(PLAYER_CHOSE);
                    OutLineColor(_isOutLine=true);

                    Debug.Log("Çarpýþma tespit edildi! Nesne: " + _hit.collider.name);
                    _isConnected = true;
                    _selectedObjectPosition = _hit.transform.position;
                }
            }
            else if (_isConnected == true)
            {
                _currentTransform = this.transform.position;
                _distanceBetweenObjects = Vector3.Distance(_currentTransform, _selectedObjectPosition);

                if (_distanceBetweenObjects < _objDistance)
                {
                    ChangeAnimationState(PLAYER_TELEPORT);
                }
                else if (_distanceBetweenObjects > _objDistance)
                {
                    _isConnected = true;
                }
            }
        }
        if (Input.GetKey(KeyCode.E) && _isConnected == true)
        {
            _holdtimer += Time.deltaTime;
            _slider.value = _holdtimer;
            
            ChangeAnimationState(PLAYER_CANCEL);
            Debug.Log(_holdtimer);
            if (_holdtimer >= 1.2f)
            {
                _holdtimer = 0;
                _slider.value = _holdtimer;
                _isConnected = false;
                OutLineColor(_isOutLine = false);
            }
        }
    }

    void Swap()
    {
        this.transform.position = new Vector3(_selectedObjectPosition.x, _currentTransform.y, _selectedObjectPosition.z);
        _hit.transform.position = new Vector3(_currentTransform.x, _hit.transform.position.y, _currentTransform.z);
        _isConnected = false;

        _holdtimer = 0;
        _slider.value = 0;
        Vector3 vfxTransform = new Vector3(this.transform.position.x, this.transform.position.y + 2f, this.transform.position.z);
        Instantiate(_swapVFX, vfxTransform, Quaternion.identity);
        Instantiate(_swapVFX, _hit.transform.position, Quaternion.identity);
        OutLineColor(_isOutLine=false);
    }
    void CancelVFX()
    {
        Vector3 cancelPos = new Vector3(_cancelVFXPos.position.x, _cancelVFXPos.position.y, _cancelVFXPos.position.z);
        Instantiate(_cancelVFX, cancelPos, Quaternion.identity);
    }

    void OutLineColor(bool _isOutline)
    {
        if (_isOutLine==true)
        {
            _targetObj = _hit.collider.GetComponent<Outline>();
            _targetObj.OutlineColor = new Color(200, 190, 40, 180);
            _targetObj.OutlineWidth = 3f;
        }
        else if (_isOutLine==false)
        {
            _targetObj.OutlineColor = Color.white;
            _targetObj.OutlineWidth = 0.0f;
        }
    }
}

