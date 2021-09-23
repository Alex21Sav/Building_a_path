using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathBuilder : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private GameObject _wayPoint;

    [Header("Component")]
    [SerializeField] private LineRenderer _lineRenderer;

    [Header("Customizable Value")]
    [SerializeField] private float _timeForNextRay = 0.05f;

    private Rigidbody _rigidbody;
    private float _timer = 0;
    private int _currentWayPoint = 0;
    private int _wayIndex;
    private bool _move;
    private bool _touchStartedOnPlayer;

    public List<GameObject> WayPoints;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();

        _lineRenderer.enabled = false;
        _wayIndex = 1;        
        _move = false;
        _touchStartedOnPlayer = false;
    }
    private void OnMouseDown()
    {
        _lineRenderer.enabled = true;
        _touchStartedOnPlayer = true;

        _lineRenderer.positionCount = 1;
        _lineRenderer.SetPosition(0, transform.position);
        
    }
    private void Update()
    {
        if (Input.GetMouseButton(0) && _timer > _timeForNextRay && _touchStartedOnPlayer)
        {
            GetPoints();
        }        
        _timer += Time.deltaTime;

        if (Input.GetMouseButtonUp(0))
        {
            StartMovement();
        }

        if (_move)
        {
            Move();
        }
    }

    private void GetPoints()
    {
        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 100));
        Vector3 direction = worldMousePos - Camera.main.transform.position;
        RaycastHit hit;

        if (Physics.Raycast(Camera.main.transform.position, direction, out hit, 500f))
        {

            Debug.DrawLine(Camera.main.transform.position, hit.point, Color.green, 1f);

            GameObject newWaypoint = Instantiate(_wayPoint, new Vector3(hit.point.x, transform.position.y, hit.point.z), Quaternion.identity);

            WayPoints.Add(newWaypoint);

            _lineRenderer.positionCount = _wayIndex + 1;
            _lineRenderer.SetPosition(_wayIndex, newWaypoint.transform.position);

            _timer = 0;

            _wayIndex++;
        }
    }
    private void StartMovement()
    {
        _touchStartedOnPlayer = false;
        _move = true;
    }
    private void Move()
    {
        transform.LookAt(WayPoints[_currentWayPoint].transform);

        _rigidbody.MovePosition(WayPoints[_currentWayPoint].transform.position);

        if (transform.position == WayPoints[_currentWayPoint].transform.position) _currentWayPoint++;

        if (_currentWayPoint == WayPoints.Count)
        {
            foreach (var item in WayPoints) Destroy(item);

            WayPoints.Clear();

            _wayIndex = 1;
            _currentWayPoint = 0;
            _move = false;
        }
    }    
}
