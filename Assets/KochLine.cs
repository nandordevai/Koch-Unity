using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class KochLine : KochGenerator
{
    LineRenderer lineRenderer;
    [Range(0, 1)]
    public float lerpAmount;
    Vector3[] lerpPosition;
    public float generateMultiplier;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = true;
        lineRenderer.useWorldSpace = false;
        lineRenderer.loop = true;
        lineRenderer.positionCount = position.Length;
        lineRenderer.SetPositions(position);
    }

    void Generate(bool outward)
    {
        KochGenerate(targetPosition, outward, generateMultiplier);
        lerpPosition = new Vector3[position.Length];
        lineRenderer.positionCount = position.Length;
        lineRenderer.SetPositions(position);
        lerpAmount = 0;
    }

    void Update()
    {
        if (generationCount != 0)
        {
            for (int i = 0; i < position.Length; i++)
            {
                lerpPosition[i] = Vector3.Lerp(position[i], targetPosition[i], lerpAmount);
            }
            if (useBezierCurves)
            {
                bezierPosition = BezierCurve(lerpPosition, bezierVertexCount);
                lineRenderer.positionCount = bezierPosition.Length;
                lineRenderer.SetPositions(bezierPosition);
            }
            else
            {
                lineRenderer.positionCount = lerpPosition.Length;
                lineRenderer.SetPositions(lerpPosition);
            }
        }
        if (Input.GetKeyUp(KeyCode.O))
        {
            Generate(true);
        }
        if (Input.GetKeyUp(KeyCode.I))
        {
            Generate(false);
        }
    }
}
