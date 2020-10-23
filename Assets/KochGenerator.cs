using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KochGenerator : MonoBehaviour
{
    protected enum Initiator
    {
        Triangle = 3,
        Square = 4,
        Pentagon = 5,
        Hexagon = 6,
        Heptagon = 7,
        Octagon = 8
    };

    protected enum Axis
    {
        XAxis,
        YAxis,
        ZAxis
    };

    public struct LineSegment
    {
        public Vector3 StartPosition { get; set; }
        public Vector3 EndPosition { get; set; }
        public Vector3 Direction { get; set; }
        public float Length { get; set; }
    };

    protected int initiatorPointAmount;
    private Vector3[] initiatorPoint;
    private Vector3 rotateVector;
    private Vector3 rotateAxis;
    private float initialRotation;
    protected Vector3[] position;
    protected Keyframe[] keys;
    protected int generationCount;
    protected Vector3[] targetPosition;
    private List<LineSegment> lineSegment;
    protected Vector3[] bezierPosition;

    [SerializeField]
    [Range(8, 24)]
    protected int bezierVertexCount;
    [SerializeField]
    protected bool useBezierCurves;
    [SerializeField]
    protected Axis axis = new Axis();
    [SerializeField]
    protected Initiator initiator = new Initiator();
    [SerializeField]
    protected float initiatorSize;
    [SerializeField]
    protected AnimationCurve generator;

    protected Vector3[] BezierCurve(Vector3[] points, int vertexCount)
    {
        var pointList = new List<Vector3>();
        for (int i = 0; i < points.Length - 2; i+=2)
        {
            for (float ratio = 0f; ratio <= 1f; ratio += 1.0f / vertexCount)
            {
                var tangentLineVertex1 = Vector3.Lerp(points[i], points[i + 1], ratio);
                var tangentLineVertex2 = Vector3.Lerp(points[i + 1], points[i + 2], ratio);
                var bezierPoint = Vector3.Lerp(tangentLineVertex1, tangentLineVertex2, ratio);
                pointList.Add(bezierPoint);
            }
        }
        return pointList.ToArray();
    }

    void InitializeAxes()
    {
        switch (axis)
        {
            case Axis.XAxis:
                rotateVector = new Vector3(1, 0, 0);
                rotateAxis = new Vector3(0, 0, 1);
                break;
            case Axis.YAxis:
                rotateVector = new Vector3(0, 1, 0);
                rotateAxis = new Vector3(1, 0, 0);
                break;
            case Axis.ZAxis:
            default:
                rotateVector = new Vector3(0, 0, 1);
                rotateAxis = new Vector3(0, 1, 0);
                break;
        };
    }

    void Awake()
    {
        initiatorPointAmount = (int)initiator;
        initialRotation = initiatorPointAmount == 3 ? 0 : 360 / (initiatorPointAmount * 2);
        InitializeAxes();
        position = new Vector3[initiatorPointAmount + 1];
        targetPosition = new Vector3[initiatorPointAmount + 1];
        lineSegment = new List<LineSegment>();
        keys = generator.keys;
        rotateVector = Quaternion.AngleAxis(initialRotation, rotateAxis) * rotateVector;
        for (int i = 0; i < initiatorPointAmount; i++)
        {
            position[i] = rotateVector * initiatorSize;
            rotateVector = Quaternion.AngleAxis(360 / initiatorPointAmount, rotateAxis) * rotateVector;
        }
        position[initiatorPointAmount] = position[0];
        targetPosition = position;
    }

    protected void KochGenerate(Vector3[] positions, bool outwards, float generatorMultiplier)
    {
        lineSegment.Clear();
        for (int i = 0; i < positions.Length - 1; i++)
        {
            LineSegment line = new LineSegment();
            line.StartPosition = positions[i];
            line.EndPosition = i == positions.Length - 1 ? positions[0] : positions[i + 1];
            line.Direction = (line.EndPosition - line.StartPosition).normalized;
            line.Length = Vector3.Distance(line.EndPosition, line.StartPosition);
            lineSegment.Add(line);
        }
        List<Vector3> newPos = new List<Vector3>();
        List<Vector3> targetPos = new List<Vector3>();
        for (int i = 0; i < lineSegment.Count; i++)
        {
            newPos.Add(lineSegment[i].StartPosition);
            targetPos.Add(lineSegment[i].StartPosition);
            for (int j = 1; j < keys.Length - 1; j++)
            {
                float moveAmount = lineSegment[i].Length * keys[j].time;
                float heightAmount = lineSegment[i].Length * keys[j].value * generatorMultiplier;
                Vector3 movePos = lineSegment[i].StartPosition + (lineSegment[i].Direction * moveAmount);
                int phi = outwards ? -90 : 90;
                Vector3 direction = Quaternion.AngleAxis(phi, rotateAxis) * lineSegment[i].Direction;
                newPos.Add(movePos);
                targetPos.Add(movePos + (direction * heightAmount));
            }
        }
        newPos.Add(lineSegment[0].StartPosition);
        targetPos.Add(lineSegment[0].StartPosition);
        position = new Vector3[newPos.Count];
        targetPosition = new Vector3[targetPos.Count];
        position = newPos.ToArray();
        targetPosition = targetPos.ToArray();
        bezierPosition = BezierCurve(targetPosition, bezierVertexCount);
        generationCount++;
    }

    void OnDrawGizmos()
    {
        initiatorPointAmount = (int)initiator;
        initialRotation = initiatorPointAmount == 3 ? 0 : 360 / (initiatorPointAmount * 2);
        InitializeAxes();
        initiatorPoint = new Vector3[initiatorPointAmount];
        rotateVector = Quaternion.AngleAxis(initialRotation, rotateAxis) * rotateVector;
        for (int i = 0; i < initiatorPointAmount; i++)
        {
            initiatorPoint[i] = rotateVector * initiatorSize;
            rotateVector = Quaternion.AngleAxis(360 / initiatorPointAmount, rotateAxis) * rotateVector;
        }

        for (int i = 0; i < initiatorPointAmount - 1; i++)
        {
            Gizmos.color = Color.white;
            Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            Gizmos.matrix = rotationMatrix;
            Gizmos.DrawLine(initiatorPoint[i], initiatorPoint[i + 1]);
        }
        Gizmos.DrawLine(initiatorPoint[initiatorPointAmount - 1], initiatorPoint[0]);
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
