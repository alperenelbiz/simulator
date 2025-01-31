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
    [SerializeField] private Transform _movementPlane;
    private Vector3 _planeMinBounds;
    private Vector3 _planeMaxBounds;

    public Rigidbody Rb => _rb;
    public bool IsMovable => _isMovable;

    void Start()
    {
        if (_movementPlane != null)
        {
            MeshRenderer planeRenderer = _movementPlane.GetComponent<MeshRenderer>();
            if (planeRenderer != null)
            {
                _planeMinBounds = planeRenderer.bounds.min;
                _planeMaxBounds = planeRenderer.bounds.max;
            }
        }
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
        int maxAttempts = 10;
        bool validMove = false;

        for (int i = 0; i < maxAttempts; i++)
        {
            _targetTurnAngle = transform.eulerAngles.y + Random.Range(-90f, 90f);
            _maxMoveSpeed = Random.Range(_minMoveSpeed, _maxMoveSpeed);

            Vector3 potentialMove = transform.position + Quaternion.Euler(0, _targetTurnAngle, 0) * Vector3.forward * _maxMoveSpeed;
            if (IsWithinBounds(potentialMove))
            {
                validMove = true;
                break;
            }
        }

        if (!validMove)
        {
            _targetTurnAngle = transform.eulerAngles.y + 180f;
        }

        _decisionTimer = Random.Range(_minDecisionTime, _maxDecisionTime);
    }

    private bool IsWithinBounds(Vector3 position)
    {
        return position.x >= _planeMinBounds.x && position.x <= _planeMaxBounds.x &&
               position.z >= _planeMinBounds.z && position.z <= _planeMaxBounds.z;
    }

    private void KeepWithinBounds()
    {
        Vector3 position = transform.position;
        bool outOfBounds = false;

        if (position.x < _planeMinBounds.x)
        {
            position.x = _planeMinBounds.x;
            outOfBounds = true;
        }
        else if (position.x > _planeMaxBounds.x)
        {
            position.x = _planeMaxBounds.x;
            outOfBounds = true;
        }

        if (position.z < _planeMinBounds.z)
        {
            position.z = _planeMinBounds.z;
            outOfBounds = true;
        }
        else if (position.z > _planeMaxBounds.z)
        {
            position.z = _planeMaxBounds.z;
            outOfBounds = true;
        }

        if (outOfBounds)
        {
            transform.position = position;
            DecideNextMove();
        }
    }

    private void OnDrawGizmos()
    {
        if (_movementPlane != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(_movementPlane.GetComponent<MeshRenderer>().bounds.center, _movementPlane.GetComponent<MeshRenderer>().bounds.size);
        }
    }

    public void Explode()
    {
        Destroy(gameObject);
    }
}