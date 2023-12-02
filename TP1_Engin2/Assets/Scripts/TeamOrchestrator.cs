using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class TeamOrchestrator : MonoBehaviour
{
    private const float MIN_OBJECTS_DISTANCE = 0.1f; // remettre à 2.0f!!!!
    public List<Collectible> KnownCollectibles { get; private set; } = new List<Collectible>();
    public List<Collectible> AvailableCollectibles { get; private set; } = new List<Collectible>();
    public List<Collectible> AvailableCollectiblesToRemove { get; private set; } = new List<Collectible>();
    public List<Camp> Camps { get; private set; } = new List<Camp>();
    public List<Worker> WorkersList { get; private set; } = new List<Worker>();
    public List<Worker> AvailableWorkersList { get; private set; } = new List<Worker>();
    public List<Worker> AvailableWorkersToRemove { get; private set; } = new List<Worker>();

    [SerializeField]
    private TextMeshProUGUI m_scoreText;
    [SerializeField]
    private TextMeshProUGUI m_remainingTimeText;
    [SerializeField]
    private float m_timeScale = 1.0f;

    private float m_remainingTime;
    private int m_score = 0;

    public GameObject m_debugPrefab; //DEBUG

    public static TeamOrchestrator _Instance
    {
        get;
        private set;
    }

    private void Awake()
    {
        if (_Instance == null || _Instance == this)
        {
            _Instance = this;
            return;
        }
        Destroy(this);
    }

    private void Start()
    {
        m_remainingTime = MapGenerator.SimulationDuration.Value;

        Time.timeScale = m_timeScale;
    }

    private void Update()
    {
        m_remainingTime -= Time.deltaTime;
        m_remainingTimeText.text = "Remaining time: " + m_remainingTime.ToString("#.00");
    }

    private void FixedUpdate()
    {
        
        
        //foreach (var collectible in KnownCollectibles)
        //{
        //    float smallestDistance = 0;
        //    Worker idealWorker;
        //    
        //    foreach (var worker in WorkersList)
        //    {
        //        float distance = Vector2.Distance(collectible.transform.position, worker.transform.position);
        //
        //        if (distance < smallestDistance)
        //        {
        //            smallestDistance = distance;
        //            idealWorker = worker;
        //        }
        //    }
        //
        //    //Communicate to worker 
        //}



    }

    public float GetRemainingTime()
    {
        return m_remainingTime;
    }

    public void TryAddCollectible(Collectible collectible)
    {
        if (KnownCollectibles.Contains(collectible))
        {
            return;
        }

        KnownCollectibles.Add(collectible);
        //Debug.Log("Collectible added");

        //Instantiate(m_debugPrefab, new Vector3(collectible.GetPosition().x, collectible.GetPosition().y, 0), Quaternion.identity);
    }

    public void InitializeExploitationPhase()
    {
        AvailableCollectibles = KnownCollectibles;
        AvailableWorkersList = WorkersList;
        Debug.Log("Av. Collectibles" + AvailableCollectibles.Count);
        Debug.Log("Av. Workers" + AvailableWorkersList.Count);

        CalculateStrategyVariables();

        AssignAvailableWorkersToAvailableCollectibles();
    }

    private void CalculateStrategyVariables()
    {
        int campCost = MapGenerator.CampCost.Value;
        float workerCost = MapGenerator.WORKER_COST; //20
        //m_remainingTime
        int minimumWorkerAmountPerCamp = 0;
        float eachCampTotalRevenue = 0;
        float societyTotalRevenue = 0;

        float averageCollectiblesDistance = CalculateAverageCollectiblesDistance(); 
        Debug.Log("avCollectibDistance: " + averageCollectiblesDistance);

        float averageDepositTime = averageCollectiblesDistance / 5.0f; //approximated WorkerSpeed TO.IMPROVE    // In seconds
        Debug.Log("avDepositTime: " + averageDepositTime);

        float estimatedTotalDepositsPerWorker = m_remainingTime / averageDepositTime; // Per worker
        Debug.Log("estimatedTotalDepositsPerWorker: " + estimatedTotalDepositsPerWorker);


        minimumWorkerAmountPerCamp = Mathf.CeilToInt((float)campCost / estimatedTotalDepositsPerWorker);
        Debug.Log("minimumWorkers: " + minimumWorkerAmountPerCamp);

        eachCampTotalRevenue = (1 * minimumWorkerAmountPerCamp * estimatedTotalDepositsPerWorker) - campCost - (minimumWorkerAmountPerCamp * workerCost);
        Debug.Log("campTotalRevenue: " + eachCampTotalRevenue); //Considering no available workers

        int nbWorkers = 5;
        while (nbWorkers > 0)
        {
            societyTotalRevenue += (1 * minimumWorkerAmountPerCamp * estimatedTotalDepositsPerWorker) - campCost;
            nbWorkers -= minimumWorkerAmountPerCamp;
        }

        Debug.Log("using av. workers Income: " + societyTotalRevenue);
        //societyTotalRevenue += (1 * minimumWorkerAmountPerCamp * estimatedTotalDepositsPerWorker) - campCost - (minimumWorkerAmountPerCamp * workerCost);

    }

    private float CalculateAverageCollectiblesDistance()
    {
        float averageDistance = 0;
        
        foreach (var collectible in KnownCollectibles)
        {
            float smallestDistance = 1000;
            foreach (var coll in KnownCollectibles)
            {
                if (coll == collectible)
                {
                    continue;
                }
                
                float dist = Vector2.Distance(collectible.GetPosition(), coll.GetPosition());
                if (dist < smallestDistance)
                {
                    smallestDistance = dist;
                    //Debug.Log("smaller " + smallestDistance);
                }
            }

            if (collectible == KnownCollectibles[0])
            {
                averageDistance = smallestDistance;
            }
            else
            {
                averageDistance = (averageDistance + smallestDistance) / 2;
                //Debug.Log("average " + averageDistance);
            }

        }

        return averageDistance;
    }

    private void AssignAvailableWorkersToAvailableCollectibles()
    {
        //foreach (var collectible in AvailableCollectibles)
        //{
        //    if (AvailableWorkersList.Count == 0)
        //    {
        //        continue;
        //    }
        //    
        //    FindClosestAvailableWorker(collectible);
        //    AvailableCollectibles.Remove(collectible);
        //
        //}

        foreach (var worker in AvailableWorkersList)
        {
            if (AvailableCollectibles.Count == 0)
            {
                break;
            }

            FindClosestAvailableCollectible(worker);
            SetWorkerToThisCollectible(worker);

            AvailableWorkersToRemove.Add(worker);
        }

        RemoveItemsFromList();
    }

    private void RemoveItemsFromList()
    {
        if (AvailableWorkersToRemove.Count != 0)
        {
            foreach (var workerToRemove in AvailableWorkersToRemove)
            {
                foreach (var worker in AvailableWorkersList)
                {
                    if (workerToRemove == worker)
                    {
                        AvailableWorkersList.Remove(worker);
                        //Debug.Log(worker.name + " not available anymore");
                        break;
                    }
                }
            }
            AvailableWorkersToRemove.Clear();
        }

        if (AvailableCollectiblesToRemove.Count != 0)
        {
            foreach (var collectibleToRemove in AvailableCollectiblesToRemove)
            {
                foreach (var collectible in AvailableCollectibles)
                {
                    if (collectibleToRemove.GetPosition() == collectible.GetPosition())
                    {
                        AvailableCollectibles.Remove(collectible);
                        //Debug.Log(collectible.name + " not available anymore");
                        break;
                    }
                }
            }
            AvailableCollectiblesToRemove.Clear();
        }
    }

    //private void FindClosestAvailableWorker(Collectible collectible)
    //{
    //    Worker idealWorker = AvailableWorkersList[0];
    //    float smallestDistance = Vector2.Distance(collectible.transform.position, idealWorker.transform.position);
    //
    //    foreach (var worker in AvailableWorkersList)
    //    {
    //        float distance = Vector2.Distance(collectible.transform.position, worker.transform.position);
    //
    //        if (distance < smallestDistance)
    //        {
    //            smallestDistance = distance;
    //            idealWorker = worker;
    //        }
    //    }
    //
    //    SetWorkerToThisCollectible(idealWorker);
    //    AvailableWorkersList.Remove(idealWorker);
    //}

    private void FindClosestAvailableCollectible(Worker worker)
    {
        Collectible closestCollectible = AvailableCollectibles[0];
        float smallestDistance = Vector2.Distance(worker.transform.position, closestCollectible.transform.position);
    
        foreach (var collectible in AvailableCollectibles)
        {
            float distance = Vector2.Distance(worker.transform.position, collectible.transform.position);
    
            if (distance < smallestDistance)
            {
                smallestDistance = distance;
                closestCollectible = collectible;
            }
        }

        //Debug.Log(worker.name + " was assigned to " + closestCollectible.GetPosition());

        worker.SetAssignedCollectiblePosition(closestCollectible.GetPosition());
        AvailableCollectiblesToRemove.Add(closestCollectible);
    }

    private void SetWorkerToThisCollectible(Worker worker)
    {
        //worker.SetHasBeenAssignedToThisCollectibleBool(true);
        worker.SetIsAssignedBool(true);
    }

    public void GainResource(ECollectibleType collectibleType)
    {
        if (collectibleType == ECollectibleType.Regular)
        {
            m_score++;
        }
        if (collectibleType == ECollectibleType.Special)
        {
            m_score += 10;//TODO: Turn to const
        }

        m_scoreText.text = "Score: " + m_score.ToString();
    }

    public void OnGameEnded()
    {
        PrintTextFile();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    private void PrintTextFile()
    {
        string path = Application.persistentDataPath + "/Results.txt";
        File.AppendAllText(path, "Score of simulation with seed: " + MapGenerator.Seed +  ": " + m_score.ToString() + "\n");

#if UNITY_EDITOR
        UnityEditor.EditorUtility.RevealInFinder(path);
        UnityEditor.EditorUtility.OpenWithDefaultApp(path);
#endif
    }

    public bool CanPlaceObject(Vector2 coordinates)
    {
        foreach (var collectible in KnownCollectibles)
        {
            var collectibleLocation = new Vector2(collectible.transform.position.x, collectible.transform.position.y);
            if (Vector2.Distance(coordinates, collectibleLocation) < MIN_OBJECTS_DISTANCE)
            {
                return false;
            }
        }

        return true;
    }

    public void OnCampPlaced()
    {
        m_score -= MapGenerator.CampCost.Value;
    }

    public void OnWorkerCreated()
    {
        //TODO élèves. À vous de trouver quand utiliser cette méthode et l'utiliser.
        m_score -= MapGenerator.WORKER_COST;
    }
}
