using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCam : MonoBehaviour
{
    MoveCam m_instance;
    public MoveCam Instance
    {
        get
        {
            if (m_instance == null)
                m_instance = this;
            return m_instance;
        }
    }
    void Awake()
    {
        m_instance = null;
    }

    Player player;
    Transform tr;

    [SerializeField] float speed = 5f;
    [SerializeField] LayerMask camViewLayer;
    [SerializeField] float camWidthHalf;


    void Start()
    {
        player = Player.Instance;
        camViewLayer = 1 << LayerMask.NameToLayer("CamView");
        camWidthHalf = transform.GetComponent<Camera>().orthographicSize * transform.GetComponent<Camera>().aspect;
        tr = transform;
    }

    void Update()
    {
        FollowPlayer();
        WiggleScreen();
    }

    void FollowPlayer()
    {
        if (player != null)
        {
            var moveValue = speed * Time.deltaTime
                * new Vector3(player.transform.position.x - tr.position.x, 0, 0);

            if (ChkViewLayer(moveValue) == false)
                transform.Translate(moveValue, Space.World);
        }
    }
    public void WiggleScreen()
    {

    }

    private bool ChkViewLayer(Vector3 value)
    {
        if (value.x > 0)
        {
            if (ChkRay(tr.position + value
                , Vector2.right, camWidthHalf, camViewLayer))
                return true;
        }
        else
        {
            if (ChkRay(tr.position + value
                , Vector2.left, camWidthHalf, camViewLayer))
                return true;
        }

        return false;
    }
    bool ChkRay(Vector3 pos, Vector2 dir, float length, LayerMask layer)
    {
        Debug.Assert(layer != 0, "레이어 지정안됨");
        var hit = Physics2D.Raycast(pos, dir, length, layer);
        return hit.transform;
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawRay(tr.position + new Vector3(0, 1, 0), Vector2.left * camWidthHalf);
    //    Gizmos.DrawRay(tr.position, Vector2.right * camWidthHalf);
    //}
}
