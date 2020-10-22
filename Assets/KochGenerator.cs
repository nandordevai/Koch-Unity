using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KochGenerator : MonoBehaviour
{
    protected enum _initiator
    {
        Triangle = 3,
        Square = 4,
        Pentagon = 5,
        Hexagon = 6,
        Heptagon = 7,
        Octagon = 8
    };

    protected enum _axis
    {
        XAxis,
        YAxis,
        ZAxis
    };

    [SerializeField]
    protected _axis axis = new _axis();

    [SerializeField]
    protected _initiator initiator = new _initiator();
    protected int _initiatorPointAmount;
    private Vector3[] _initiatorPoint;
    private Vector3 _rotateVector;
    private Vector3 _rotateAxis;
    private float _initialRotation;
    [SerializeField]
    protected float _initiatorSize;

    void InitializeAxes()
    {
        switch (axis)
        {
            case _axis.XAxis:
                _rotateVector = new Vector3(1, 0, 0);
                _rotateAxis = new Vector3(0, 0, 1);
                break;
            case _axis.YAxis:
                _rotateVector = new Vector3(0, 1, 0);
                _rotateAxis = new Vector3(1, 0, 0);
                break;
            case _axis.ZAxis:
                _rotateVector = new Vector3(0, 0, 1);
                _rotateAxis = new Vector3(0, 1, 0);
                break;
        };
    }

    void Awake()
    {
        //
    }

    void OnDrawGizmos()
    {
        _initiatorPointAmount = (int)initiator;
        _initialRotation = _initiatorPointAmount == 3 ? 0 : 360 / (_initiatorPointAmount * 2);
        InitializeAxes();
        _initiatorPoint = new Vector3[_initiatorPointAmount];
        _rotateVector = Quaternion.AngleAxis(_initialRotation, _rotateAxis) * _rotateVector;
        for (int i = 0; i < _initiatorPointAmount; i++)
        {
            _initiatorPoint[i] = _rotateVector * _initiatorSize;
            _rotateVector = Quaternion.AngleAxis(360 / _initiatorPointAmount, _rotateAxis) * _rotateVector;
        }

        for (int i = 0; i < _initiatorPointAmount - 1; i++)
        {
            Gizmos.color = Color.white;
            Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            Gizmos.matrix = rotationMatrix;
            Gizmos.DrawLine(_initiatorPoint[i], _initiatorPoint[i + 1]);
        }
        Gizmos.DrawLine(_initiatorPoint[_initiatorPointAmount - 1], _initiatorPoint[0]);
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }
}
