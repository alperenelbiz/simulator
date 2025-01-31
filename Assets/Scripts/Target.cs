using UnityEngine;

public class Target : MonoBehaviour, IExplode
{
    [Header("References")]
    [SerializeField] private Rigidbody _rb;

    [Header("Movement")]
    [SerializeField] private bool _isMovable = true;
    [SerializeField] private float _minMoveSpeed = 2f;
    [SerializeField] private float _maxMoveSpeed = 6f;
    [SerializeField] private float _turnSpeed = 60f;
    [SerializeField] private float _acceleration = 2f;
    private float _currentSpeed = 0f;

    [Header("AI Control")]
    [SerializeField] private float _minDecisionTime = 1f;
    [SerializeField] private float _maxDecisionTime = 4f;
    private float _decisionTimer;
    private float _targetTurnAngle;

    [Header("Movement Bounds")]
    [SerializeField] private Vector2 _areaSize = new Vector2(20f, 20f);
    private Vector3 _startPosition;

    public Rigidbody Rb => _rb;
    public bool IsMovable => _isMovable;

    void Start()
    {
        _startPosition = transform.position;
        _decisionTimer = Random.Range(_minDecisionTime, _maxDecisionTime);
        _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void Update()
    {
        if (!_isMovable) return;

        _decisionTimer -= Time.deltaTime;
        if (_decisionTimer <= 0)
        {
            DecideNextMove();
            _decisionTimer = Random.Range(_minDecisionTime, _maxDecisionTime);
        }
    }

    private void FixedUpdate()
    {
        if (!_isMovable) return;

        _currentSpeed = Mathf.Lerp(_currentSpeed, _maxMoveSpeed, _acceleration * Time.deltaTime);
        _rb.velocity = transform.forward * _currentSpeed;

        Quaternion targetRotation = Quaternion.Euler(0, _targetTurnAngle, 0);
        _rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, targetRotation, _turnSpeed * Time.deltaTime));

        KeepWithinBounds();
    }

    private void DecideNextMove()
    {
        _targetTurnAngle = transform.eulerAngles.y + Random.Range(-90f, 90f);
        _maxMoveSpeed = Random.Range(_minMoveSpeed, _maxMoveSpeed);
    }

    private void KeepWithinBounds()
    {
        Vector3 localPos = transform.position - _startPosition;
        if (Mathf.Abs(localPos.x) > _areaSize.x / 2 || Mathf.Abs(localPos.z) > _areaSize.y / 2)
        {
            _targetTurnAngle = transform.eulerAngles.y + 180f;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(_startPosition, new Vector3(_areaSize.x, 1, _areaSize.y));
    }

    public void Explode()
    {
        Destroy(gameObject);
    }
}
