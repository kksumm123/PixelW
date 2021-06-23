using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCam : MonoBehaviour
{
    Transform playerTr;
    Transform tr;

    [SerializeField] float speed = 5f;
    [SerializeField] LayerMask camViewLayer;
    [SerializeField] float camWidthHalf;
    void Start()
    {
        playerTr = GameObject.Find("Player").GetComponent<Transform>();
        camViewLayer = 1 << LayerMask.NameToLayer("CamView");
        camWidthHalf = transform.GetComponent<Camera>().orthographicSize * transform.GetComponent<Camera>().aspect;
        tr = transform;
    }

    void Update()
    {
        var moveValue = 
            new Vector3(playerTr.position.x - tr.position.x, 0, 0) * speed * Time.deltaTime;
        if (ChkViewLayer(moveValue) == false)
        {
            transform.Translate(moveValue, Space.World);
        }
    }

    private bool ChkViewLayer(Vector3 value)
    {
        if (ChkRay(tr.position + value
            , Vector2.left, camWidthHalf, camViewLayer))
            return true;
        if (ChkRay(tr.position + value
            , Vector2.right, camWidthHalf, camViewLayer))
            return true;

        return false;
    }
    bool ChkRay(Vector3 pos, Vector2 dir, float length, LayerMask layer)
    {
        Debug.Assert(layer != 0, "레이어 지정안됨");
        var hit = Physics2D.Raycast(pos, dir, length, layer);
        return hit.transform;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(tr.position + new Vector3(0, 1, 0), Vector2.left * camWidthHalf);
        Gizmos.DrawRay(tr.position, Vector2.right * camWidthHalf);
    }
}
