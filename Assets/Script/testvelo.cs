using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testvelo : MonoBehaviour
{
    Rigidbody2D rigid;
    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    [SerializeField] float veloY = 500f;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            var velo = rigid.velocity;
            velo.y = veloY;
            rigid.velocity = velo;
        }
    }
}
