using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCam : MonoBehaviour
{
    Transform playerTr;
    Transform tr;

    [SerializeField] float speed = 2f;
    [SerializeField] LayerMask camViewLayer;
    [SerializeField] float camWidth;
    void Start()
    {
        playerTr = GameObject.Find("Player").GetComponent<Transform>();
        camViewLayer = 1 << LayerMask.NameToLayer("CamView");
        camWidth = 2 * transform.GetComponent<Camera>().orthographicSize * transform.GetComponent<Camera>().aspect;
        tr = transform;
    }

    void Update()
    {
        var distance = new Vector3(playerTr.position.x - tr.position.x, 0, 0);
        if (ChkViewLayer(distance) == false)
        {
            transform.Translate(distance * speed * Time.deltaTime, Space.World);
        }
    }

    private bool ChkViewLayer(Vector3 distance)
    {
        if (ChkRay(tr.position + new Vector3(camWidth, 0, 0) + distance
            , Vector2.left, camWidth, camViewLayer))
            return true;
        if (ChkRay(tr.position + new Vector3(camWidth, 0, 0) + distance
            , Vector2.right, camWidth, camViewLayer))
            return true;

        return false;
    }
    bool ChkRay(Vector3 pos, Vector2 dir, float length, LayerMask layer)
    {
        Debug.Assert(layer != 0, "레이어 지정안됨");
        var hit = Physics2D.Raycast(pos, dir, length, layer);
        return hit.transform;
    }
}
