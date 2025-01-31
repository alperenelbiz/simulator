using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSelector : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private LayerMask _targetLayer;

    private Missile _missile;

    private void Start()
    {
        _missile = FindObjectOfType<Missile>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _targetLayer))
            {
                if (hit.collider.TryGetComponent<Target>(out Target target))
                {
                    _missile.SetTarget(target);
                }
            }
        }
    }
}
