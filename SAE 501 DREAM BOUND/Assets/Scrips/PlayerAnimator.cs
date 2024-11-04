using System.Collections;
using System.Collections.Generic;
using TomAg;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{


    // PARAMS

    [SerializeField]
    private string animMoveX = "MoveX";
    [SerializeField]
    private string animMoveZ = "MoveZ";

    // PRIVATE

    private Animator _animator;
    private PlayerController _controller;

    private int _animMoveX;
    private int _animMoveZ;

    private void Awake()
    {

        // Init Animator
        if (!TryGetComponent(out _animator))
            Debug.LogError(message: "Missing Animator Component", this);

        _animMoveX = Animator.StringToHash(animMoveX);
        _animMoveZ = Animator.StringToHash(animMoveZ);

        // Init PlayerController
        if (!TryGetComponent(out _controller))
            Debug.LogError(message: "Missing PlayerController Component", this);

        _controller.onMove += OnMove;
    }

    // HOOKS

    private void OnMove(Vector2 axis)
    {
        _animator.SetFloat(_animMoveX, axis.x);
        _animator.SetFloat(_animMoveZ, axis.y);
    }
}
