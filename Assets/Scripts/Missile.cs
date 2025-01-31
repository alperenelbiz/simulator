using System;
using UnityEngine;

public class Missile : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private GameObject _explosionPrefab;
    private Target _target;
    public bool _launched = false;

    [Header("MOVEMENT")]
    [SerializeField] private float _speed = 15;
    [SerializeField] private float _rotateSpeed = 95;

    [Header("PREDICTION")]
    [SerializeField] private float _maxDistancePredict = 100;
    [SerializeField] private float _minDistancePredict = 5;
    [SerializeField] private float _maxTimePrediction = 5;
    private Vector3 _targetPosition; //_standardPrediction, _deviatedPrediction;

    [Header("DEVIATION")]
    [SerializeField] private float _deviationAmount = 50;
    [SerializeField] private float _deviationSpeed = 2;

    public bool IsLaunched => _launched;

    private void Update()
    {
        if (!_launched && Input.GetKeyDown(KeyCode.X))
        {
            _launched = true;
        }
    }

    private void FixedUpdate()
    {
        if (!_launched || _target == null) return;

        _rb.velocity = transform.forward * _speed;

        //if (ShouldUseDirectAim())
        //{
        _targetPosition = _target.transform.position;
        //}
        //else
        //{
        //   var leadTimePercentage = Mathf.InverseLerp(_minDistancePredict, _maxDistancePredict, Vector3.Distance(transform.position, _target.transform.position));
        //  PredictMovement(leadTimePercentage);
        // AddDeviation(leadTimePercentage);
        //}

        RotateRocket();
    }

    //private bool ShouldUseDirectAim()
    //{
        //return _target != null && (!_target.Rb.gameObject.GetComponent<Target>().IsMovable);
    //}

    //private void PredictMovement(float leadTimePercentage)
    //{
        //var predictionTime = Mathf.Lerp(0, _maxTimePrediction, leadTimePercentage);
        //_standardPrediction = _target.Rb.position + _target.Rb.velocity.normalized * _target.Rb.velocity.magnitude * predictionTime;
        //_targetPosition = _standardPrediction;
    //}

    //private void AddDeviation(float leadTimePercentage)
    //{
        //var deviation = new Vector3(Mathf.Cos(Time.time * _deviationSpeed), 0, Mathf.Sin(Time.time * _deviationSpeed));
        //var predictionOffset = transform.TransformDirection(deviation) * _deviationAmount * leadTimePercentage;
        //_deviatedPrediction = _standardPrediction + predictionOffset;
        //_targetPosition = _deviatedPrediction;
    //}

    private void RotateRocket()
    {
        var heading = _targetPosition - transform.position;
        var rotation = Quaternion.LookRotation(heading);
        _rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, rotation, _rotateSpeed * Time.deltaTime));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_explosionPrefab) Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        if (collision.transform.TryGetComponent<IExplode>(out var ex)) ex.Explode();

        Destroy(gameObject);
    }

    public void SetTarget(Target newTarget)
    {
        if (_launched) return;
        _target = newTarget;
    }

    private void OnDrawGizmos()
    {
        if (_target == null) return;
        //if (_target.Rb.gameObject.GetComponent<Target>().IsMovable)
        //{
        //Gizmos.color = Color.red;
        //Gizmos.DrawLine(transform.position, _standardPrediction);
        //Gizmos.color = Color.green;
        //Gizmos.DrawLine(_target.transform.position, _standardPrediction);
        //}
        //else
        //{
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, _targetPosition);
        //}

    }
}
