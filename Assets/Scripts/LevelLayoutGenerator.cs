using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class LevelLayoutGenerator : MonoBehaviour
{
	private bool firstChunkCreated = false;
	public GameObject userTracker;
	public TextMeshProUGUI AI_DeathText;
	public GameObject yellowTracker;
	public GameObject blackTracker;
	public GameObject purpleTracker;
    public LevelChunkData[] levelChunkData;
    public LevelChunkData firstChunk;
	public List<GameObject> targetObjectList;
    private LevelChunkData previousChunk;
    public Vector3 spawnOrigin;
    private Vector3 spawnPosition;
    public int chunksToSpawn = 10;
	
	public int timeTrack;
    public bool firstTimeTrigger = false;
    public bool secondTimeTrigger = false;
    public bool thirdTimeTrigger = false;
    
	void OnEnable()
    {
        TriggerExit.OnChunkExited += PickAndSpawnChunk;
    }

    private void OnDisable()
    {
        TriggerExit.OnChunkExited -= PickAndSpawnChunk;
    }

 	void Start()
    {
        previousChunk = firstChunk;
        for (int i = 0; i < chunksToSpawn; i++)
        {
            PickAndSpawnChunk();
        }
		timeTrack = 0;
		userTracker = GameObject.Find("HoverCar");
		yellowTracker = GameObject.Find("Yellow - AI HoverCar");
		blackTracker = GameObject.Find("Black - AI HoverCar");
		purpleTracker = GameObject.Find("Purple - AI HoverCar");
		AI_DeathText = userTracker.transform.GetChild(0).transform.GetChild(3).transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
		AI_DeathText.SetText("");

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            PickAndSpawnChunk();
        }
		CheckForFalls();
		TimedElimination();
		CheckIfSolo();
    }

   
    
    LevelChunkData PickNextChunk()
    {
        List<LevelChunkData> allowedChunkList = new List<LevelChunkData>();
        LevelChunkData nextChunk = null;
		
        LevelChunkData.Direction nextRequiredDirection = LevelChunkData.Direction.North;

        switch (previousChunk.exitDirection)
        {
            case LevelChunkData.Direction.North:
                nextRequiredDirection = LevelChunkData.Direction.South;
                spawnPosition = spawnPosition + new Vector3(0f, 0, previousChunk.chunkSize.y);
                break;

            case LevelChunkData.Direction.East:
                nextRequiredDirection = LevelChunkData.Direction.West;
                spawnPosition = spawnPosition + new Vector3(previousChunk.chunkSize.x, 0, 0);
                break;
            
            case LevelChunkData.Direction.South:
                nextRequiredDirection = LevelChunkData.Direction.North;
                spawnPosition = spawnPosition + new Vector3(0, 0, -previousChunk.chunkSize.y);
                break;
            
            case LevelChunkData.Direction.West:
                nextRequiredDirection = LevelChunkData.Direction.East;
                spawnPosition = spawnPosition + new Vector3(-previousChunk.chunkSize.x, 0, 0);
                break;

            default:
                break;
        }

        for (int i = 0; i < levelChunkData.Length; i++)
        {
            if(levelChunkData[i].entryDirection == nextRequiredDirection)
            {
                allowedChunkList.Add(levelChunkData[i]);
            }
        }
        if(firstChunkCreated == false)
		{
		firstChunkCreated = true;
		nextChunk = allowedChunkList[1];
        return nextChunk;
		}
		else
		{
        nextChunk = allowedChunkList[Random.Range(0, allowedChunkList.Count)];
        return nextChunk;
		}
    }

    void PickAndSpawnChunk()
    {
        LevelChunkData chunkToSpawn = PickNextChunk();

        GameObject objectFromChunk = chunkToSpawn.levelChunks[Random.Range(0, chunkToSpawn.levelChunks.Length)];
		previousChunk = chunkToSpawn;
        GameObject instantiatedObj = Instantiate(objectFromChunk, spawnPosition + spawnOrigin, Quaternion.identity);
		targetObjectList.Add(instantiatedObj.transform.Find("Target (4)").gameObject);
		targetObjectList.Add(instantiatedObj.transform.Find("Target (3)").gameObject);
		targetObjectList.Add(instantiatedObj.transform.Find("Target (2)").gameObject);
        targetObjectList.Add(instantiatedObj.transform.Find("Target (1)").gameObject);
		targetObjectList.Add(instantiatedObj.transform.Find("Target").gameObject);
    }

    public void UpdateSpawnOrigin(Vector3 originDelta)
    {
        spawnOrigin = spawnOrigin + originDelta;
    }
	
	public void TriggerGameOver(string reason) 
	{
			if(reason == "fall" && !userTracker.transform.GetChild(0).transform.GetChild(1).gameObject.activeSelf && !userTracker.transform.GetChild(0).transform.GetChild(2).gameObject.activeSelf)
			{
            	userTracker.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
			}
			else if(reason == "last")
			{
            	userTracker.transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(true);
			}
			Destroy(userTracker.GetComponent("UserHoverMotor"));
			userTracker.GetComponent<Rigidbody>().useGravity = false;
	}

	public void DisableAI(GameObject AI_Player) 
	{
			Destroy(AI_Player.GetComponent("UserHoverMotor"));
			AI_Player.SetActive(false);
	}
	
	public GameObject selectLast()
	{
		GameObject[] hoverCars;
        hoverCars = GameObject.FindGameObjectsWithTag("Car");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject car in hoverCars)
        {
            Vector3 diff = car.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = car;
                distance = curDistance;
            }
        }
        return closest;
	}

	public void CheckForFalls()
	{
	if (userTracker.transform.position.y < 70)
        {
			TriggerGameOver("fall");
        }
	if (yellowTracker.activeSelf == true && yellowTracker.transform.position.y < 70)
        {
			StartCoroutine(AI_Eliminated(yellowTracker.name, "fall"));
			DisableAI(yellowTracker);
        }
	if (blackTracker.activeSelf == true && blackTracker.transform.position.y < 70)
        {
			StartCoroutine(AI_Eliminated(blackTracker.name, "fall"));
			DisableAI(blackTracker);
        }
	if (purpleTracker.activeSelf == true && purpleTracker.transform.position.y < 70)
        {
			StartCoroutine(AI_Eliminated(purpleTracker.name, "fall"));
			DisableAI(purpleTracker);
        }
	}
	
	public void CheckIfSolo()
	{
		if (yellowTracker.activeSelf == false && blackTracker.activeSelf == false && purpleTracker.activeSelf == false)
		{
           	userTracker.transform.GetChild(0).transform.GetChild(2).gameObject.SetActive(true);
		}
	}
	public void TimedElimination()
	{
		timeTrack = (int)Time.timeSinceLevelLoad;
		Debug.Log(timeTrack);
		if(timeTrack == 90 && firstTimeTrigger == false)
		{
			firstTimeTrigger = true;
			GameObject last = selectLast();
			if(last.name == "HoverCar")
			{
				TriggerGameOver("last");
			}
			else
			{
				last.SetActive(false);
				StartCoroutine(AI_Eliminated(last.name, "last"));
			}
		}
		if(timeTrack == 180 && secondTimeTrigger == false)
		{
			secondTimeTrigger = true;
			GameObject last = selectLast();
			if(last.name == "HoverCar")
			{
				TriggerGameOver("last");
			}
			else
			{
				last.SetActive(false);
				StartCoroutine(AI_Eliminated(last.name, "last"));
			}
		}
		if(timeTrack == 270 && thirdTimeTrigger == false)
		{
			thirdTimeTrigger = true;
			GameObject last = selectLast();
			if(last.name == "HoverCar")
			{	
				TriggerGameOver("last");
			}
			else
			{
				last.SetActive(false);
			    StartCoroutine(AI_Eliminated(last.name, "last"));
			}
		}
	}
	
	public IEnumerator AI_Eliminated(string name, string reason)
	{
		AI_DeathText.SetText($"{name} has been eliminated.\nReason: {reason}");
		Debug.Log(AI_DeathText.text);
		yield return new WaitForSeconds(3f);
		AI_DeathText.SetText("");
	}

}
