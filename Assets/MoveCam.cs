using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCam : MonoBehaviour
{
    Transform playerTr;
    Transform tr;

    [SerializeField] float speed = 2f;
    [SerializeField] bool canFollow = true;
    void Start()
    { 
        playerTr = GameObject.Find("Player").GetComponent<Transform>();
        tr = transform;
    }

    void Update()
    {
        if (canFollow == true)
        {
            var distance = new Vector3(playerTr.position.x - tr.position.x, 0, 0);
            transform.Translate(distance * speed * Time.deltaTime, Space.World);
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log("¡¯¿‘");
        if (collision.CompareTag("CamViewCol"))
            canFollow = false;
        else
            canFollow = true;
    }
}
