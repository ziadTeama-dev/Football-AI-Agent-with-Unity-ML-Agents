using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;


public class score_goal : Agent
{
    [Header("References")]
    [SerializeField] private Transform enemy;
    [SerializeField] private Transform goal;
    [SerializeField] private Transform myGoal;
    [SerializeField] private Transform ball;
    [SerializeField] private ParticleSystem dust;
    [SerializeField] private ParticleSystem skull;


    [Header("Movement")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float rotationSpeed = 200f;

    [Header("Dash")]
    [SerializeField] private float dashForce = 8f;
    [SerializeField] private float dashCooldown = 1.5f;

    [Header("Game Logic")]
    [SerializeField] private Goal_Is_hit goalIsHit;
    [SerializeField] private Team myTeam;

    public enum Team { Red, Blue }

    private Rigidbody rb;
    private Rigidbody ballRb;

    private Vector3 startPos;
    private Vector3 ballStartLocalPos;

    private float lastDashTime = -999f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        ballRb = ball.GetComponent<Rigidbody>();

        startPos = transform.localPosition;

        ballStartLocalPos = ball.localPosition;
    }

    public override void OnEpisodeBegin()
    {
        // Reset Agent
        transform.localPosition = startPos;
        // transform.rotation = startRot;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Reset Ball
        ball.localPosition = new Vector3(
            Random.Range(-1f, 1f),
            ballStartLocalPos.y,
            Random.Range(-1.5f, 0.5f)
        );
        ballRb.velocity = Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;

        // Reset Goals
        goalIsHit.is_hit_blue = false;
        goalIsHit.is_hit_red = false;
        goalIsHit.isHit = false;

        lastDashTime = -999f;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Agent-space observations
        // Ball
        sensor.AddObservation(ball.localPosition - transform.localPosition);
        sensor.AddObservation(transform.InverseTransformDirection(ballRb.velocity));

        // Enemy
        sensor.AddObservation(enemy.localPosition - transform.localPosition);
        sensor.AddObservation(transform.InverseTransformDirection(enemy.GetComponent<Rigidbody>().velocity));

        // Goals
        sensor.AddObservation(goal.localPosition - transform.localPosition);
        sensor.AddObservation(myGoal.localPosition - transform.localPosition);

        // Self
        sensor.AddObservation(transform.InverseTransformDirection(rb.velocity));
        sensor.AddObservation(transform.forward);
        sensor.AddObservation(transform.localRotation.y/360f);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];
        float rotate = actions.ContinuousActions[2];
        int dash = actions.DiscreteActions[0];
        // int not_dash = actions.DiscreteActions[1];


        // Movement
        Vector3 move = transform.forward * moveZ + transform.right * moveX;
        rb.MovePosition(rb.position + move * speed * Time.deltaTime);
        // adding some effects
        if (rb.velocity.magnitude>=.2f)
        {
            Instantiate(dust,transform.position,Quaternion.identity);

            
        }


        // Rotation
        transform.Rotate(Vector3.up, rotate * rotationSpeed * Time.deltaTime);

        // DASH
        if (dash ==1 && Time.time - lastDashTime > dashCooldown)
        {
            rb.AddForce(transform.forward * dashForce, ForceMode.VelocityChange);
            lastDashTime = Time.time;


            AddReward(-0.05f); // dash
        }


        // Time penalty
        AddReward(-0.005f);

        // Distance to ball penalty
        float distToBall = Vector3.Distance(transform.position, ball.position);
        AddReward(-Mathf.Clamp(distToBall, 0f, 5f) * 0.0003f);

        // Ball moving toward goal reward
        if (ballRb.velocity.magnitude > 0.1f)
        {
            Vector3 ballToGoal = (goal.position - ball.position).normalized;
            float align = Vector3.Dot(ballRb.velocity.normalized, ballToGoal);
            AddReward(align * 0.005f);
        }

        // Look-at-ball reward
        Vector3 dirToBall = (ball.position - transform.position).normalized;
        float lookDot = Vector3.Dot(transform.forward, dirToBall);
        AddReward(Mathf.Max(0f, lookDot) * 0.01f);

        // Goal logic
        if (myTeam == Team.Red && goalIsHit.is_hit_blue)
            Win();
        else if (myTeam == Team.Blue && goalIsHit.is_hit_red)
            Win();
    }

    void Win()
    {
        AddReward(7f);

        if (enemy.TryGetComponent(out score_goal enemyAgent))
        {
            enemyAgent.AddReward(-5f);
            enemyAgent.EndEpisode();
        }

        EndEpisode();
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var cont = actionsOut.ContinuousActions;
        var discrete = actionsOut.DiscreteActions;

        cont[0] = Input.GetAxis("Horizontal");
        cont[1] = Input.GetAxis("Vertical");
        cont[2] = Input.GetAxis("Mouse X");
        discrete[0] = Input.GetKey(KeyCode.LeftShift) ? 1 : 0; // Dash



    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("ball"))
        {
            AddReward(0.07f);

        }
        
        if (collision.collider.CompareTag("enemy") && rb.velocity.magnitude > .4f)
        {
            Instantiate(skull,collision.collider.transform.position+new Vector3(0f,1.2f,0f),Quaternion.identity);
        }

    }
}
