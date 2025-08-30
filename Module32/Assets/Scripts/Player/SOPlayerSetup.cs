using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SOPlayerSetup : ScriptableObject
{
    public Animator player;

    [Header("Speed Setup")]
    public Vector3 friction = new Vector3(.1f, 0, 0);
    public float speed;
    public float speedRun;
    public float forceJump = 15;
    public float rotationSpeed = 1f;

    [Header("Animation Setup")]
    public float jumpScaleZ = .5f;
    public float jumpScaleY = 2f;
    public float jumpScaleX = .5f;
    public float animationDuration = 0.5f;
    public Ease ease = Ease.OutBack;
    public float landingScaleZ = 2f;
    public float landingScaleY = .5f;
    public float landingScaleX = 2f;

    [Header("Animation Player")]
    public string boolRun = "Run";
    public string boolJumpUp = "JumpUp";
    public string boolJumpDown = "JumpDown";
    public string boolJumpLanding = "JumpLanding";
}
