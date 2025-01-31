using UnityEngine;

public class ChangeCamera : MonoBehaviour
{
    [SerializeField] private Camera _missileCamera;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private Missile _missile;
    private bool _isFollowing = false;

    private void Start()
    {
        if (_missileCamera != null)
        {
            _missileCamera.enabled = false;
        }
    }

    private void Update()
    {
        if (_missile == null) return;

        if (!_isFollowing && _missile.IsLaunched)
        {
            SwitchToMissileCamera();
        }

        if (_isFollowing && _missile == null)
        {
            SwitchToMainCamera();
        }
    }

    private void LateUpdate()
    {
        if (_isFollowing && _missile != null)
        {
            _missileCamera.transform.position = _missile.transform.position - _missile.transform.forward * 5 + Vector3.up * 2;
            _missileCamera.transform.LookAt(_missile.transform.position + _missile.transform.forward * 10);
        }
    }

    private void SwitchToMissileCamera()
    {
        _mainCamera.enabled = false;
        _missileCamera.enabled = true;
        _isFollowing = true;
    }

    private void SwitchToMainCamera()
    {
        _mainCamera.enabled = true;
        _missileCamera.enabled = false;
        _isFollowing = false;
    }
}
