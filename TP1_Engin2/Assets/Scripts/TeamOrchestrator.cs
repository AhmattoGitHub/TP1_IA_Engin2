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

        //FindClosestWorker(collectible);
    }

    public void InitializeExploitationPhase()
    {
        AvailableCollectibles = KnownCollectibles;
        AvailableWorkersList = WorkersList;

        Debug.Log("Av. Collectibles" + AvailableCollectibles.Count);
        Debug.Log("Av. Workers" + AvailableWorkersList.Count);

        AssignAvailableWorkersToAvailableCollectibles();
    }

    public void AssignAvailableWorkersToAvailableCollectibles()
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
            //AvailableWorkersList.Remove(worker);
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
                        Debug.Log(worker.name + " not available anymore");
                        break;
                    }
                }
                //Debug.Log("Worker not found in av. list");
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
                        Debug.Log(collectible.name + " not available anymore");
                        break;
                    }
                }
                //Debug.Log("Collectible not found in av. list");
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

        Debug.Log(worker.name + " was assigned to " + closestCollectible.GetPosition());

        worker.SetAssignedCollectiblePosition(closestCollectible.GetPosition());
        AvailableCollectiblesToRemove.Add(closestCollectible);
        //AvailableCollectibles.Remove(closestCollectible);
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
