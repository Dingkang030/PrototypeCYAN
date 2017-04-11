using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public abstract class IKController : MonoBehaviour {

    protected Animator animator;

    [SerializeField]
    protected Transform leftTarget;
    protected float leftWeight;

    [SerializeField]
    protected Transform rightTarget;
    protected float rightWeight;

    [SerializeField]
    protected Vector3 leftDirection;
    [SerializeField]
    protected Vector3 rightDirection;
    protected Transform leftTransform;
    protected Transform rightTransform;
    [SerializeField]
    protected LayerMask ikMask;
    [SerializeField]
    protected float rayHeight;
    [SerializeField]
    protected Vector3 leftOffset;
    [SerializeField]
    protected Vector3 rightOffset;

    protected virtual void Start () {
        animator = GetComponent<Animator>();
    }

    abstract protected void OnAnimatorIK();
}
