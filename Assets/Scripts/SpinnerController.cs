using UnityEngine;
using System.Collections.Generic;

public class SpinnerController : MonoBehaviour
{
    public List<GameObject> segments;
    public List<int> segmentTypes; // Stores the type assigned to each segment
    public List<Material> mats;

    public List<int> maxSegmentsActive; // Max allowed for each material type
    public List<int> currentCounts;    // Tracks how many are currently used

    private int ticks;

    private bool isSpinning;

    [SerializeField] private float spinPower;
    [SerializeField] private float stopPower;
    [SerializeField] private float currentSpinForce;

    //Result
    public GetWheelResult gwr;
    private bool resultFound;
    public int segmentTypeSelected;
    private bool canGetResult;

    public bool confirmButtonActive;

    public bool canSpin = true;

    //Pointer Movement
    [SerializeField] private PointerController pointerController;

    private void Start()
    {
        NewRound();
    }

    public void PrepWheel()
    {
        if (PlayerStats.spinsRemaining > 0)
        {
            GenerateWheel();
            pointerController.canMove = false;
            confirmButtonActive = false;
            Invoke("SpinWheel", 1f);
        }
        else
        {
            NewRound();
        }
    }

    public void GenerateWheel()
    {
        currentCounts = new List<int>(new int[mats.Count]);
        
        if(segmentTypes.Count != segments.Count) 
            segmentTypes = new List<int>(new int[segments.Count]);

        for (int i = 0; i < segments.Count; i++)
        {
            int chosenType = -1;

            if (i != 0 && ticks < 15 && Random.Range(0, 100) >= 10)
            {
                int prevType = segmentTypes[i - 1];
                if (currentCounts[prevType] < maxSegmentsActive[prevType])
                {
                    chosenType = prevType;
                    ticks++;
                }
            }

            if (chosenType == -1)
            {
                ticks = 0;
                List<int> validTypes = new List<int>();
                for (int t = 0; t < mats.Count; t++)
                {
                    if (currentCounts[t] < maxSegmentsActive[t]) 
                        validTypes.Add(t);
                }

                if (validTypes.Count > 0)
                {
                    chosenType = validTypes[Random.Range(0, validTypes.Count)];
                }
                else
                {
                    chosenType = 0;
                }
            }

            segmentTypes[i] = chosenType;
            segments[i].GetComponent<MeshRenderer>().material = mats[chosenType];
            currentCounts[chosenType]++;
        }
    }

    public void SpinWheel()
    {
        float spinningTime = Random.Range(3f,5f);
        isSpinning = true;
        currentSpinForce = Random.Range(spinPower-1f, spinPower+3f);
        resultFound = false;
        canGetResult = true;
        PlayerStats.spinsRemaining -= 1;
    }

    private void FixedUpdate()
    {
        if (isSpinning)
        {
            transform.Rotate(transform.rotation.x + currentSpinForce, 0, 0);

            if (currentSpinForce > 0)
                currentSpinForce -= stopPower*Time.deltaTime;
        }

        if (currentSpinForce < 0)
        {
            isSpinning = false;
            currentSpinForce = 0;
        }

        if (currentSpinForce == 0 && !resultFound && canGetResult)
        {
            GetResult();
        }
    }

    private void GetResult()
    {
        if (!resultFound)
            segmentTypeSelected = segmentTypes[gwr.result];
        resultFound = true;
        canGetResult = false;

        if (!PlayerStats.insanityEnabled) //Not Insane
        {
            switch (segmentTypeSelected)
            {
                case 0: //Money Segment - +1-+5 chips (R0)
                    PlayerStats.chips += Random.Range(1,5);
                    break;
                case 1: //Safe Segment - Nothing (R0)
                    Debug.Log("Safe Segment"); //No insanity diff
                    break;
                case 2: //Mystery Segment - Minigame (R0)
                    canSpin = false;
                    break;
                case 3: //Death Segment - Lose Chips (R0)
                    PlayerStats.chips -= 3;
                    break;
                case 4: //Insanity Segment - Insanity Mode (R5+) R10 = 100%
                    PlayerStats.insanityEnabled = true; //Once this segment has been picked some segments change
                    break;
                case 5: //Pit Segment - (R5+ Insanity)
                    //Can't appear outside of Insanity Mode
                    break;
                case 6: //Witching Hour (R10+ Insanity)
                    //Can't appear outside of Insanity Mode
                    break;
                case 7: //Freedom (R10+) 10%
                    Debug.Log("Fr3Ed0M..."); //Gives the player to end the run
                    break;
            }
        }
        else //Insane
        {
            switch (segmentTypeSelected)
            {
                case 0: //Money Segment - +1-+5 chips (R0)
                    PlayerStats.chips += Random.Range(1,5);
                    break;
                case 1: //Safe Segment - Nothing (R0)
                    Debug.Log("Safe Segment"); //No insanity diff
                    break;
                case 2: //Mystery Segment - Minigame (R0)
                    Debug.Log("Mystery Segment"); //insanity: Analog horror type minigames
                    break;
                case 3: //Death Segment - Lose Chips (R0)
                    Debug.Log("Death Segment"); //insanity: Dodge clockman's gunshots
                    break;
                case 4: //Insanity Segment - Insanity Mode (R5+) R10 = 100%
                    //Can't appear in Insanity Mode
                    break;
                case 5: //Pit Segment - (R5+ Insanity)
                    Debug.Log("The Pit Segment"); //Drops the player into a pit (quick time events)
                    break;
                case 6: //Witching Hour (R10+ Insanity)
                    Debug.Log("Witching Hour"); //Forces the player into a labrynth where they must hide
                    break;
                case 7: //Freedom (R10+) 10%
                    Debug.Log("Fr3Ed0M..."); //Gives the player to end the run
                    break;
            }
        }

        if (PlayerStats.spinsRemaining > 0)
        {
            if (canSpin)
            {
                Invoke("GenerateWheel", 1f);
                pointerController.canMove = false;
                confirmButtonActive = false;
                Invoke("SpinWheel", 2f);
            }
        }
        else if (PlayerStats.spinsRemaining == 0)
        {
            Shop();
        }
    }

    public void NewRound()
    {
        PlayerStats.spinsRemaining = 5;
        PlayerStats.currentRound += 1;
        confirmButtonActive = true;
        pointerController.canMove = true;
    }

    private void Shop()
    {
        NewRound();
    }
}