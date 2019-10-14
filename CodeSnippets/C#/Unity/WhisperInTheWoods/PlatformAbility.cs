using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlatformAbility : MonoBehaviour
{
    private float totalWaitTime;
    //private bool platformSpawned;
    private Transform spawnPosition;
    private bool platformAlive;
    private int platformChoice;
    private float platformOffset;

    //controller support for xbox, ps4, and keyboard
    private string activateAbility = "Fire1";//"SpawnAbility";
    private string leftAndRight;
    private string upAndDown;
    [SerializeField]
    private float inputSensitivity = 0.4f;

    [Header("No Touchy")] //Imma touch it -- BTM
    public GameObject[] attachedPlatforms;
    public GameObject[] spawnablePlatforms;
    //public float spawnResetTime;

    // Use this for initialization
    void Start ()
    {
        //platformSpawned = false;
        platformAlive = false;
        totalWaitTime = 0;
        platformChoice = 0;
        platformOffset = 0.0f;

        float jF = gameObject.GetComponent<PlayerController>().jumpPower;
        float playerHeight = gameObject.transform.lossyScale.y / 2.0f; //Only works if collider size is 1:1
        float platformHeight = 0.25f; //Could change, but not during gameplay

        float time = jF / 9.8f;
        float jumpHeight = (jF * time) - (0.5f * 9.8f * time * time);
        float platLength = playerHeight + jumpHeight + platformHeight;

        attachedPlatforms[0].transform.localPosition = new Vector2(-platLength, 0.0f);
        attachedPlatforms[1].transform.localPosition = new Vector2(0.0f, platLength);
        attachedPlatforms[2].transform.localPosition = new Vector2(platLength, 0.0f);
        attachedPlatforms[3].transform.localPosition = new Vector2(0.0f, -platLength);
    }
	
	// Update is called once per frame
	void Update ()
    {
        DetectControls();
        //SpawnAbilityResetTime();

        //if (platformSpawned)
           // return;

        Controller();
    }

    void DetectControls()
    {
        string[] names = Input.GetJoystickNames();

        foreach(string name in names)
        {
            if (!string.IsNullOrEmpty(name))
            {
                if (name.ToUpper().Contains("XBOX"))
                {
                    leftAndRight = "Horz1";//"XBOXRSXAxis";
                    upAndDown = "Vert1";//"XBOXRSYAxis";
                }
                else if (name.ToUpper().Contains("WIRELESS"))
                {
                    leftAndRight = "Horz1";//"PS4RSXAxis";
                    upAndDown = "Vert1";//"PS4RSYAxis";
                }
                else
                {
                    leftAndRight = "Horz1";//"PS4RSXAxis";
                    upAndDown = "Vert1";//"PS4RSYAxis";
                }
            }
        }
    }

    void Controller()
    {
        if (Input.GetButton(activateAbility) /*&& !platformSpawned*/)
        {
            float test = Input.GetAxis(leftAndRight);
            float test2 = Input.GetAxis(upAndDown);

            if (Input.GetAxis(leftAndRight) /*==*/ <=  -1 * inputSensitivity)
            {
                ActivatePlatform((int)PlatformActivateNumber.Left);
                DeactivatePlatform((int)PlatformActivateNumber.Down);
                DeactivatePlatform((int)PlatformActivateNumber.Right);
                DeactivatePlatform((int)PlatformActivateNumber.Up);
            }
            else if (Input.GetAxis(leftAndRight) /*==*/ >= 1 * inputSensitivity)
            {
                ActivatePlatform((int)PlatformActivateNumber.Right);
                DeactivatePlatform((int)PlatformActivateNumber.Down);
                DeactivatePlatform((int)PlatformActivateNumber.Left);
                DeactivatePlatform((int)PlatformActivateNumber.Up);
            }
            else if (Input.GetAxis(upAndDown) /*==*/ <= 1 * -1 * inputSensitivity)//take out -1 when input mapping is fixed
            {
                ActivatePlatform((int)PlatformActivateNumber.Up);
                DeactivatePlatform((int)PlatformActivateNumber.Down);
                DeactivatePlatform((int)PlatformActivateNumber.Left);
                DeactivatePlatform((int)PlatformActivateNumber.Right);
            }
            else if (Input.GetAxis(upAndDown) /*==*/ >= -1 * -1 * inputSensitivity) // take out -1 when input mapping is fixed
            {
                ActivatePlatform((int)PlatformActivateNumber.Down);
                DeactivatePlatform((int)PlatformActivateNumber.Up);
                DeactivatePlatform((int)PlatformActivateNumber.Left);
                DeactivatePlatform((int)PlatformActivateNumber.Right);
            }
            else
            {
                ActivatePlatform((int)PlatformActivateNumber.Down);
                DeactivatePlatform((int)PlatformActivateNumber.Left);
                DeactivatePlatform((int)PlatformActivateNumber.Up);
                DeactivatePlatform((int)PlatformActivateNumber.Right);
            }

            if (!(Input.GetAxis(activateAbility) > 0) && platformAlive)
            {
                FalseObjectRelease();
            }
        }
        else if (Input.GetButtonUp(activateAbility))
        {
            ReleasePlatform(platformChoice);
        }
    }

    void ActivatePlatform(int platformNumber)
    {
        attachedPlatforms[platformNumber].SetActive(true);
        platformChoice = platformNumber;
        platformAlive = true;
    }

    void DeactivatePlatform(int platformNumber)
    {
        attachedPlatforms[platformNumber].SetActive(false);
        platformAlive = false;
    }

    void ReleasePlatform(int platformNumber)
    {
        DestoryPreviousPlatform();
        CheckPlatformHeight(platformNumber);

        Instantiate(spawnablePlatforms[platformNumber], spawnPosition.position, spawnPosition.rotation);
        attachedPlatforms[platformNumber].SetActive(false);
        //platformSpawned = true;
        platformAlive = false;
    }

    void CheckPlatformHeight(int platformNumber)
    {
        float offset = 0;

        if (!PlayerController.grounded)
            offset = platformOffset;
        else
            offset = 0;

        spawnPosition = new GameObject().transform;
        spawnPosition.position = new Vector3(attachedPlatforms[platformNumber].transform.position.x,
                    attachedPlatforms[platformNumber].transform.position.y - offset,
                    attachedPlatforms[platformNumber].transform.position.z);
        spawnPosition.rotation = attachedPlatforms[platformNumber].transform.rotation;
    }

    void DestoryPreviousPlatform()
    {
        for (int i = 0; i < spawnablePlatforms.Length; i++)
        {
            string spawnedPlatformName = spawnablePlatforms[i].name + "(Clone)";
            GameObject previousSpawnedPlatform = GameObject.Find(spawnedPlatformName);

            if (previousSpawnedPlatform != null)
            {
                GameObject previousSpawnTransform = GameObject.Find("New Game Object");
                Destroy(previousSpawnedPlatform);
                Destroy(previousSpawnTransform);
            }
        }
    }

    void FalseObjectRelease()
    {
        //Turn off all active states of platforms not summoned properly
        for(int i = 0; i < attachedPlatforms.Length; i++)
        {
            attachedPlatforms[i].SetActive(false);
        }
    }

    /*void SpawnAbilityResetTime()
    {
        if (platformSpawned)
        {
            totalWaitTime += Time.deltaTime;

            if(totalWaitTime >= spawnResetTime)
            {
                platformSpawned = false;
            }
        }
    }*/
    
    enum PlatformActivateNumber
    {
        Left,
        Up,
        Right,
        Down
    }
}
