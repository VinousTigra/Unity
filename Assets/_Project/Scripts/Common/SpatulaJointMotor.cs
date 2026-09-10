using UnityEngine;

public class SpatulaJointMotor : MonoBehaviour
{
    [SerializeField] private HingeJoint hinge;
    [SerializeField] private float speed = 30f;
    [SerializeField] private float switchTime = 2f;

    private float timer;
    private bool reverse;

    private void Awake()
    {
        if (hinge == null)
            hinge = GetComponent<HingeJoint>();

        SetMotorDirection();
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= switchTime)
        {
            timer = 0f;
            reverse = !reverse;
            SetMotorDirection();
        }
    }

    private void SetMotorDirection()
    {
        JointMotor motor = hinge.motor;
        motor.targetVelocity = reverse ? -speed : speed;
        motor.force = 10f;
        motor.freeSpin = false;

        hinge.motor = motor;
    }
}