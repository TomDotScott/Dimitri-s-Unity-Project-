using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetherScript : MonoBehaviour
{
    [SerializeField] private Transform hookLaunchPoint;
    [SerializeField] private Transform grapplePoint;
    private LineRenderer lineRenderer;
    [SerializeField] private AnimationCurve hookCurve;
    [SerializeField] private AnimationCurve progressionCurve;
    [SerializeField] private float activeTime;
    [SerializeField] private float progressionSpeed;
    [SerializeField] private int pointCount;
    [SerializeField] private int waveHeightValue;
    private float currentWaveHeight;
    private bool isStraight = false;
    [SerializeField] private float straightenSpeed;
    private TetherState currentState;


    private enum TetherState
    {
        Firing,
        Straightening,
        Straight,
    }

    private void Awake()
    {
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        InitializeTether();
    }

    // Update is called once per frame
    void Update()
    {
        activeTime += Time.deltaTime;
        RenderTether();
    }

    public void ResetTether()
    {
        //InitializeTether();
        lineRenderer.positionCount = pointCount;
        currentWaveHeight = waveHeightValue;
        activeTime = 0;
        currentState = TetherState.Firing;
    }

    private void InitializeTether()
    {
        lineRenderer.positionCount = pointCount;
        currentWaveHeight = waveHeightValue;
        activeTime = 0;
        currentState = TetherState.Firing;

        for (int i = 0; i < pointCount; ++i)
        {
            lineRenderer.SetPosition(i, hookLaunchPoint.position);
            activeTime = 0; 
        }
    }

    private void RenderTether()
    {
        switch (currentState)
        {
            case TetherState.Firing:
                if (lineRenderer.GetPosition(pointCount - 1).x == grapplePoint.position.x)
                {
                    currentState = TetherState.Straightening;
                }

                RenderWaves();
                break;
            case TetherState.Straightening:
                if (currentWaveHeight > 0)
                {
                    currentWaveHeight -= Time.deltaTime * straightenSpeed;
                    RenderWaves();
                }
                else
                {
                    currentWaveHeight = 0;
                    lineRenderer.positionCount = 2;
                    currentState = TetherState.Straight;
                    RenderStraight();
                }
                break;
            case TetherState.Straight:
                RenderStraight();
                break;
            default:
                break;
        }    
    }

    private void RenderStraight()
    {
        lineRenderer.SetPosition(0, hookLaunchPoint.position);
        lineRenderer.SetPosition(1, grapplePoint.position);
    }

    private void RenderWaves()
    {
        for (int i = 0; i < pointCount; ++i)
        {
            float changeInPosition = (float) i / (pointCount -1f);
            Vector2 offset = Vector2.Perpendicular(grapplePoint.position).normalized * hookCurve.Evaluate(changeInPosition) * currentWaveHeight;
            Vector2 target = Vector2.Lerp(hookLaunchPoint.position, grapplePoint.position, changeInPosition) + offset;
            Vector2 current = Vector2.Lerp(hookLaunchPoint.position, target, progressionCurve.Evaluate(activeTime) * progressionSpeed);
            lineRenderer.SetPosition(i, current);
        }
    }
}
