using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class AIHoverMotor : MonoBehaviour
{
	// Targeting
	public int currentTarget = -1;
	public List<GameObject> targetList;
	public Vector3 targetPoint;
    
    // Navigation
    public float aiTurnSpeed = .8f;
    public float aiReachPointRange = 350f;
    public float aiPointVariance = 15f;
    public float aiSpeedInput;
    public float aiMaxTurn = 20f;
	public float aiSpeedMod;

    public float speed = 270f;
    public float turnSpeed = 6f;
    public float smoothing = .2f;
    public float hoverForce = 65f;
    public float hoverHeight = 4.2f;
    public ParticleSystem burnerParticles;
    public Light headlight;
    //public float powerInput;
    public float turnInput;
    public Rigidbody carRigidbody;
    public bool accelerating = false;
    public bool reversing = false;
    public bool lightsOn = false;

    void Awake()
    {
        carRigidbody = this.GetComponent<Rigidbody>();
    }

    void Start()
    {
			targetList = GameObject.Find("LevelGenerator").GetComponent<LevelLayoutGenerator>().targetObjectList;
			//targetPoint = targetList[currentTarget].transform.position;
            //targetPoint.y = transform.position.y;
			aiSpeedMod = Random.Range(.8f, 1.1f);
			speed = speed * aiSpeedMod;
			//RandomiseAITarget();
    }

    void Update()
    {
        //Debug.Log(targetList);
            targetPoint.y = transform.position.y;
            if (currentTarget == -1)
            {
	            currentTarget = 1;
	            targetPoint = targetList[currentTarget].transform.position;
	            RandomiseAITarget();
	            accelerating = true;
            }
			//Debug.Log(transform.position);
            if (Vector3.Distance(transform.position, targetPoint) < aiReachPointRange)
            {
                currentTarget++;
				targetPoint = targetList[currentTarget].transform.position;
				RandomiseAITarget();
            }
			//targetPoint = targetList[currentTarget].transform.position;
			//RandomiseAITarget();
			
			Vector3 targetDirection = targetPoint - transform.position;
			float angle = Vector3.Angle(targetDirection, transform.forward);
			Vector3 localPosition = transform.InverseTransformPoint(targetPoint);
			if(localPosition.x < 0f) 
			{
				angle = -angle;
			}
			turnInput = Mathf.Clamp(angle / aiMaxTurn, -1f, 1f);

			//if(Mathf.Abs(angle) > aiMaxTurn)
			//{
			//	speed = aiSpeedInput * aiTurnSpeed;
			//}
			//accelerating = true;
			
    }
    void FixedUpdate()
    {
	    targetPoint = targetList[currentTarget].transform.position;
	    RandomiseAITarget();
	    Ray ray = new Ray(transform.position, -transform.up);
	    RaycastHit hit;

	    if (Physics.Raycast(ray, out hit, hoverHeight))
	    {
		    float proportionalHeight = (hoverHeight - hit.distance) / hoverHeight;
		    Vector3 appliedHoverForce = Vector3.up * proportionalHeight * hoverForce;
		    carRigidbody.AddForce(appliedHoverForce, ForceMode.Acceleration);
	    }
	    float tempInput = 0f;

	    float smoothedTurn = Mathf.Lerp(turnInput, tempInput, smoothing);

	    if (accelerating)
	    {
		    burnerParticles.Play();
		    carRigidbody.AddForce(transform.forward * speed, ForceMode.Acceleration);
		    reversing = false;
	    }
	    else if (reversing)
	    {
            
		    carRigidbody.AddForce(transform.forward * -speed * .5f, ForceMode.Acceleration);
	    }
	    else
	    {
		    burnerParticles.Stop();
	    }

	    carRigidbody.transform.Rotate(new Vector3(0f, smoothedTurn * turnSpeed, 0f));
    }
	public void RandomiseAITarget()
	{
		targetPoint = targetPoint + new Vector3(Random.Range(-aiPointVariance,aiPointVariance), 0f, Random.Range(-aiPointVariance,aiPointVariance));
	}

}