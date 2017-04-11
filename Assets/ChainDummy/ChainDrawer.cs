using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class NodeData
{
    public Vector3 position;
    public Vector3 normal;
    public Vector3 crossProductToHead;
    public Transform transform;
}

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(Rigidbody))]
public class ChainDrawer : MonoBehaviour
{

    LineRenderer lineRenderer;
    List<NodeData> nodes = new List<NodeData>();

    public float movingSpeed;
    
    [Header("-Connect Set")]
    [Header("--Head")]
    public Rigidbody headConnectedRigid;
    public Vector3 headConnectedOffset;
    public Transform headObject;
    [Header("--Tail")]
    public Rigidbody tailConnectedRigid;
    public Vector3 tailConnectedOffset;
    public Transform tailObject;

    

    [Header("-Renderer")]
    public Vector3 tailRendererOffset;
    public Vector3 headRendererOffset;
    [HideInInspector]
    public float rayDistance=1f;

    [Header("-ETC")]
    public bool isDrawable;
    public float displacementAmount=0.3f;
    public float traceDistance = 1.5f;

    public float maxDistance = 30.0f;


    float lastDistance = -1;

    new Rigidbody rigidbody;

    public delegate void OnReachedEvent();
    OnReachedEvent onReachedEvent;
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    public NodeData headNode { set { nodes[0] = value; } get { return nodes[0]; } }
    public NodeData tailNode { set { nodes[nodes.Count - 1] = value; } get { return nodes[nodes.Count - 1]; } }
    public Vector3 headRendererPos { set { headObject.position = value+headObject.TransformVector(headRendererOffset); } get { return headObject.TransformPoint(headRendererOffset); } }
    public Vector3 tailRendererPos { set { tailObject.position = value+tailObject.TransformVector(tailRendererOffset); } get { return tailObject.TransformPoint(tailRendererOffset); } }


    public Vector3 headConnectedRigidPos { set { headConnectedRigid.position = value; } get { return headObject.position; } }
    public Vector3 tailConnectedRigidPos { set { tailConnectedRigid.position = value; } get { return tailObject.position; } }

    public float totalDistance
    {
        get
        {
            float d = 0;
            if (nodes.Count == 0)
            {
                d += Vector3.Distance(tailRendererPos, headRendererPos);
            }
            else
            {
                for (int i = 0; i < nodes.Count - 2; i++)
                {
                    d += Vector3.Distance(nodes[i].position, nodes[i + 1].position);
                }
                d += Vector3.Distance(tailRendererPos, tailNode.position);
                d += Vector3.Distance(headRendererPos, headNode.position);
            }
            return d;
        }
    }

    void Update()
    {
        if (!isDrawable)
        {
            lineRenderer.numPositions = 0;
            return;
        }
        HitUpdate();
        //ColliderUpdate();
        CollisionCheck();
        CrossProductUpdateToHead();
        CrossProductUpdateToTail();
        //NodeUpdateToTail();
        //NodeUpdateToHead();
        LineRendererUpdate();
        //Debug.Log(totalDistance);
    }
    void FixedUpdate()
    {
    }
    void DistanceCheck()
    {
        if(totalDistance>maxDistance)
        {
            float diff =  totalDistance- maxDistance;
        }
    }
    void NodeUpdateToTail()
    {

        if (nodes.Count > 1)
        {
            RaycastHit hit;
            if (!Physics.SphereCast(headRendererPos, 0.5f,(nodes[1].position- headRendererPos).normalized, out hit,Vector3.Distance(headRendererPos, nodes[1].position), LayerMask.GetMask("ground")))
            {
                nodes.Remove(headNode);
            }
        }
        else if(nodes.Count==1)
        {
            RaycastHit hit;
            if (!Physics.SphereCast(headRendererPos, 0.5f, (tailRendererPos - headRendererPos).normalized, out hit, Vector3.Distance(headRendererPos, tailRendererPos), LayerMask.GetMask("ground")))
            {
                nodes.Remove(headNode);
            }
        }
    }
    void NodeUpdateToHead()
    {
        if (nodes.Count > 1)
        {
            RaycastHit hit;
            if (!Physics.SphereCast(tailRendererPos, 0.1f, -(tailRendererPos - nodes[nodes.Count - 2].position).normalized, out hit, Vector3.Distance(tailRendererPos, nodes[nodes.Count - 2].position), LayerMask.GetMask("ground")))
            {
                nodes.Remove(tailNode);
            }
        }
        else if(nodes.Count==1)
        {

            RaycastHit hit;
            if (!Physics.SphereCast(tailRendererPos, 0.1f, -(tailRendererPos - headRendererPos).normalized, out hit, Vector3.Distance(tailRendererPos, headRendererPos), LayerMask.GetMask("ground")))
            {
                nodes.Remove(tailNode);
            }
        }
    }
    Vector3 GetCross(Vector3 destPoint, Vector3 passPoint, Vector3 movingPoint)
    {
        Vector3 v1 = destPoint - passPoint;
        Vector3 v2 = passPoint - movingPoint;
        return Vector3.Cross(v1, v2);
    }

    void HitUpdate()
    {

        Vector3 centerPos=Vector3.zero;
        Vector3 lookDir=Vector3.zero;
        Vector3 halfExtents=Vector3.zero;
        float d = 0.2f;
        if(nodes.Count==0)
        {
            lookDir = headRendererPos - tailRendererPos;
            centerPos = tailRendererPos + lookDir / 2;
            halfExtents = new Vector3(0.0f, 0.1f, lookDir.magnitude / 2);
        }
        else if (nodes.Count > 0)
        {
            lookDir = tailNode.position - tailRendererPos;
            centerPos = tailRendererPos + lookDir / 2;
            halfExtents = new Vector3(0.0f, 0.1f, lookDir.magnitude / 2);
        }
        RaycastHit hit;
        if (Physics.BoxCast(centerPos, halfExtents, Quaternion.LookRotation(lookDir) * Vector3.left, out hit,Quaternion.LookRotation(lookDir), d, LayerMask.GetMask("ground")))
        {
            Debug.DrawLine(hit.point, hit.point + hit.normal, Color.green);
            AddNode(hit.point+ hit.normal*0.3f, hit.normal, hit.transform);
        }
        else if (Physics.BoxCast(centerPos, halfExtents, Quaternion.LookRotation(lookDir) * -Vector3.left, out hit, Quaternion.LookRotation(lookDir), d, LayerMask.GetMask("ground")))
        {
            Debug.DrawLine(hit.point, hit.point + hit.normal, Color.magenta);
            AddNode(hit.point + hit.normal * 0.3f, hit.normal, hit.transform);
        }
        else if (Physics.BoxCast(centerPos, halfExtents, Quaternion.LookRotation(lookDir) * -Vector3.left, out hit, Quaternion.LookRotation(lookDir), d, LayerMask.GetMask("ground")))
        {
            Debug.DrawLine(hit.point, hit.point + hit.normal, Color.magenta);
            AddNode(hit.point + hit.normal * 0.3f, hit.normal, hit.transform);
        }
    }

    void CrossProductUpdateToHead()
    {
        if (nodes.Count > 0)
        {
            Vector3 dest;
            if (nodes.Count == 1)
                dest = headRendererPos;
            else
                dest = nodes[nodes.Count - 2].position;
            Vector3 cross = GetCross(dest, tailNode.position, tailRendererPos);
            if (tailNode.crossProductToHead.y / cross.y < 0)
            {
                nodes.Remove(tailNode);
            }
        }
    }
    void CrossProductUpdateToTail()
    {
        if (nodes.Count > 0)
        {
            Vector3 dest;
            if (nodes.Count == 1)
                dest = tailRendererPos;
            else
                dest = nodes[1].position;
            Vector3 cross = GetCross(dest, headNode.position, headRendererPos);
            if (-headNode.crossProductToHead.y / cross.y < 0)
            {
                nodes.Remove(headNode);
            }
        }
    }
    void CollisionCheck()
    {
        for (int i = 1; i < nodes.Count; i++)
        {
            Collider[] col = Physics.OverlapSphere(nodes[i].position, 0.5f, LayerMask.GetMask("ground"));
            if (col.Length == 0)
            {
                nodes.RemoveAt(i);
            }
        }
    }
    void LineRendererUpdate()
    {
        lineRenderer.numPositions = nodes.Count + 2;
        lineRenderer.SetPosition(0, headRendererPos);
        for (int i = 0; i < nodes.Count; i++)
        {
            lineRenderer.SetPosition(i + 1, nodes[i].position-nodes[i].normal*displacementAmount);
        }
        lineRenderer.SetPosition(nodes.Count + 1, tailRendererPos);
    }

    public void AddNode(Vector3 point, Vector3 normal,Transform _transform = null)
    {
        NodeData n = new NodeData();
        n.position = point;
        if (nodes.Count > 0)
        {
            Vector3 cross = GetCross(tailNode.position, n.position, tailRendererPos);
            n.crossProductToHead = cross;
        }
        else
        {
            Vector3 cross = GetCross(headRendererPos, n.position, tailRendererPos);
            n.crossProductToHead = cross;
        }
        n.normal = normal;
        n.transform = _transform;
        nodes.Add(n);
    }
    public void SetOnReachedEvent(OnReachedEvent _event)
    {
        onReachedEvent = _event;
    }



    //void OnCdollisionEnter(Collision col)
    //{
    //    if (!isDrawable)
    //        return;
    //    if (col.gameObject.layer != LayerMask.NameToLayer("ground"))
    //        return;
    //    Debug.DrawLine(col.contacts[0].point, col.contacts[0].point + col.contacts[0].normal * 10, Color.cyan, 5);
    //    Vector3 point = col.contacts[0].point + col.contacts[0].normal * (0.4f);
    //    AddNode(point,col.contacts[0].normal, col.transform);

    //}
    //public void ColliderUpdate()
    //{
    //    transform.position = tailRendererPos;
    //    if (nodes.Count == 0)
    //    {
    //        if (headRendererPos == tailRendererPos)
    //            return;
    //        Vector3 dir = headRendererPos - tailRendererPos;
    //        rigidbody.transform.localScale = new Vector3(1, 1, Vector3.Distance(headRendererPos, tailRendererPos));
    //        rigidbody.MoveRotation(Quaternion.LookRotation(dir.normalized));
    //    }
    //    else
    //    {
    //        Vector3 dir = tailNode.position - tailRendererPos;
    //        rigidbody.transform.localScale = new Vector3(1, 1, Vector3.Distance(tailRendererPos, tailNode.position));
    //        rigidbody.MoveRotation(Quaternion.LookRotation(dir.normalized));
    //    }
    //}
    //public void ColliderInit()
    //{
    //    transform.position = tailRendererPos;
    //    rigidbody.transform.localScale = new Vector3(1, 1,0);
    //}
    public IEnumerator Trace(bool isTailToHead = true)
    {
        Rigidbody tracerRigid = (isTailToHead) ? tailConnectedRigid : /*(headConnectedRigid)?headConnectedRigid:*/headObject.GetComponent<Rigidbody>();
        Vector3 tracerVelocity = Vector3.zero;
        tracerRigid.velocity = Vector3.zero;

        bool reached = false;
        NodeData currentNode;
        Vector3 dest = Vector3.zero;
        Vector3 startPoint;


        lastDistance = -1;
        while (!reached)
        {
            currentNode = (nodes.Count == 0) ? null : (isTailToHead) ? tailNode : headNode;
            dest = (nodes.Count > 0) ? currentNode.position : (isTailToHead) ? headConnectedRigid.transform.TransformPoint(headConnectedOffset) : tailConnectedRigid.transform.TransformPoint(tailConnectedOffset);
            startPoint = (isTailToHead) ? tailConnectedRigid.transform.TransformPoint(tailConnectedOffset) : headConnectedRigid.transform.TransformPoint(headConnectedOffset);
            float distanceBetweenDest = Vector3.Distance(tracerRigid.position, dest);
            if (distanceBetweenDest < traceDistance)
            {
                if (nodes.Count > 0)
                {
                    nodes.Remove(currentNode);
                    lastDistance = -1;
                    distanceBetweenDest = Vector3.Distance(tracerRigid.position, dest);
                    currentNode = (nodes.Count == 0) ? null : (isTailToHead) ? tailNode : headNode;
                    dest = (nodes.Count > 0) ? currentNode.position : (isTailToHead) ? headConnectedRigid.transform.TransformPoint(headConnectedOffset) : tailConnectedRigid.transform.TransformPoint(tailConnectedOffset);
                }
                else
                {
                    reached = true;
                    break;
                    //transform.position = dest;
                }
            }
            if(!reached)
            {
                float d = Vector3.Distance(tracerRigid.position, dest);
                tracerVelocity = (dest - startPoint).normalized * movingSpeed;
                Vector3 newVelocity = Vector3.MoveTowards(tracerRigid.velocity, tracerVelocity, Time.deltaTime*((nodes.Count == 0&&d < 6)?90:(nodes.Count==0)?60:30));
                Vector3 dir = (isTailToHead) ? (dest - startPoint).normalized : (startPoint - dest).normalized;
                Vector3 newEuler = Quaternion.LookRotation(dir).eulerAngles;
                tracerRigid.transform.rotation=Quaternion.RotateTowards(tracerRigid.rotation,Quaternion.Euler(newEuler),5);
                tracerRigid.velocity = newVelocity;
                lastDistance = d;
                CrossProductUpdateToHead();
                CrossProductUpdateToTail();
            }
            yield return new WaitForFixedUpdate();
        }
        if (onReachedEvent != null)
            onReachedEvent();
        onReachedEvent = null;
        transform.localScale = Vector3.zero;
        tracerRigid.velocity = Vector3.zero;
        isDrawable = false;
        yield return null;
    }
    //public IEnumerator Trace(bool isTailToHead = true)
    //{
    //    Rigidbody tracerRigid = (isTailToHead) ? tailConnectedRigid : /*(headConnectedRigid)?headConnectedRigid:*/headObject.GetComponent<Rigidbody>();
    //    Vector3 tracerVelocity = Vector3.zero;
    //    tracerRigid.velocity = Vector3.zero;

    //    bool reached = false;
    //    NodeData currentNode;
    //    Vector3 dest=Vector3.zero;
    //    Vector3 startPoint;
        

    //    lastDistance=-1;
    //    while (!reached)
    //    {
    //        currentNode = (nodes.Count == 0) ? null : (isTailToHead) ? tailNode : headNode;
    //        dest = (nodes.Count>0)? currentNode.position:(isTailToHead)? headConnectedRigid.transform.TransformPoint(headConnectedOffset):tailConnectedRigid.transform.TransformPoint(tailConnectedOffset);
    //        startPoint= (isTailToHead) ? tailConnectedRigid.transform.TransformPoint(tailConnectedOffset) : headConnectedRigid.transform.TransformPoint(headConnectedOffset);
    //        float distanceBetweenDest = Vector3.Distance(tracerRigid.position, dest);
    //        if (distanceBetweenDest< traceDistance)
    //        {
    //            if (nodes.Count > 0)
    //            {
    //                nodes.Remove(currentNode);
    //                lastDistance = -1;
    //            }
    //            else
    //            {
    //                reached = true;
    //                break;
    //                //transform.position = dest;
    //            }
    //        }
    //        else
    //        {
    //            tracerVelocity = (dest - startPoint).normalized * movingSpeed;
    //            Vector3 newVelocity = Vector3.MoveTowards(tracerRigid.velocity, tracerVelocity, Time.deltaTime * 60);
    //            tracerRigid.velocity = newVelocity;
    //            Vector3 dir = (isTailToHead) ? (dest - startPoint).normalized : (startPoint - dest).normalized;
    //            Vector3 newEuler = Quaternion.LookRotation(dir).eulerAngles;
    //            tracerRigid.transform.eulerAngles = newEuler;
    //            float d = Vector3.Distance(tracerRigid.position, dest);
    //            /*if (lastDistance > 0)
    //            {
    //                tracerRigid.velocity += (dest - tracerRigid.position).normalized * 3 * Time.deltaTime;
    //            }
    //            lastDistance = d;*/
    //        }
    //        yield return null;
    //    }
    //    if (onReachedEvent!=null)
    //        onReachedEvent();
    //    onReachedEvent = null;
    //    transform.localScale = Vector3.zero;
    //    tracerRigid.velocity = Vector3.zero;
    //    isDrawable = false;
    //    yield return null;
    //}
}
