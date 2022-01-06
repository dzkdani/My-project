using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [Header("Ball Status")]
    public bool IsPickedUp;
    public bool IsPassing;
    public float speed;

    private Vector3 targetPass;

    private void Start() {
        IsPickedUp = false;
        IsPassing = false;
    }

    private void Update() {
        if (IsPassing)
        {
            PassingBall();
        }

        if (IsPickedUp)
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }

    public void CanPass(Vector3 _targetPass)
    {
        IsPassing = true;
        targetPass = _targetPass;
    }

    private void PassingBall()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPass, speed * Time.deltaTime);

        PassedBall();
    }

    private void PassedBall()
    {
        if (transform.position == targetPass)
        {
            IsPassing = false;
            transform.position = targetPass + Vector3.up;
        }
    }
    
    private void OnCollisionEnter(Collision other) {
        if (other.collider.CompareTag("Field"))
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }
}
