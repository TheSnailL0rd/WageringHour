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

    private void Start()
    {
        GenerateWheel();
        SpinWheel();
    }

    public void GenerateWheel()
    {
        // Initialize counters to match the number of materials
        currentCounts = new List<int>(new int[mats.Count]);
        
        if(segmentTypes.Count != segments.Count) 
            segmentTypes = new List<int>(new int[segments.Count]);

        for (int i = 0; i < segments.Count; i++)
        {
            int chosenType = -1;

            // 1. Logic for "Following" (Clumping)
            // Check if the PREVIOUS color still has room left before repeating it
            if (i != 0 && ticks < 15 && Random.Range(0, 100) >= 10)
            {
                int prevType = segmentTypes[i - 1];
                if (currentCounts[prevType] < maxSegmentsActive[prevType])
                {
                    chosenType = prevType;
                    ticks++;
                }
            }

            // 2. Fallback to Random if no clumping happened or color was full
            if (chosenType == -1)
            {
                ticks = 0;
                // Get a list of only colors that are NOT at their limit
                List<int> validTypes = new List<int>();
                for (int t = 0; t < mats.Count; t++)
                {
                    if (currentCounts[t] < maxSegmentsActive[t]) 
                        validTypes.Add(t);
                }

                // Pick a random available color
                if (validTypes.Count > 0)
                {
                    chosenType = validTypes[Random.Range(0, validTypes.Count)];
                }
                else
                {
                    // Safety: all colors are full, default to the first one
                    chosenType = 0;
                }
            }

            // 3. Apply the results
            segmentTypes[i] = chosenType;
            segments[i].GetComponent<MeshRenderer>().material = mats[chosenType];
            currentCounts[chosenType]++;
        }
    }

    public void SpinWheel()
    {
        float spinningTime = Random.Range(3f,5f);
        isSpinning = true;
        currentSpinForce = spinPower;
        resultFound = false;
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

        if (currentSpinForce == 0 && !resultFound)
            GetResult();
    }

    private void GetResult()
    {
        if (!resultFound)
            segmentTypeSelected = segmentTypes[gwr.result];
        resultFound = true;

        switch (segmentTypeSelected)
        {
            case 0: //Money Segment
                Debug.Log("Money Segment");
                break;
            case 1: //Safe Segment
                Debug.Log("Safe Segment");
                break;
            case 2: //Mystery Segment
                Debug.Log("Mystery Segment");
                break;
            case 3: //Death Segment
                Debug.Log("Death Segment");
                break;
        }
    }
}