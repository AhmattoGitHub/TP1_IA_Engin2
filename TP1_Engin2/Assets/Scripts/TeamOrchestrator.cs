using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class TeamOrchestrator : MonoBehaviour
{
    private const float MIN_OBJECTS_DISTANCE = 0.1f; // remettre à 2.0f!!!!
    public List<Collectible> KnownCollectibles { get; private set; } = new List<Collectible>();
    public List<Camp> Camps { get; private set; } = new List<Camp>();
    public List<Worker> WorkersList { get; private set; } = new List<Worker>();

    [SerializeField]
    private TextMeshProUGUI m_scoreText;
    [SerializeField]
    private TextMeshProUGUI m_remainingTimeText;
    [SerializeField]
    private float m_timeScale = 1.0f;

    private float m_remainingTime;
    private int m_score = 0;

    [field: Header("SEARCH GRID")]
    [SerializeField]
    private GameObject m_gridMarker = null;
    private int m_distanceBetweenPoints = 6;
    public List<SearchGridCell> SearchGridCells { get; set; } = new List<SearchGridCell>();    



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

    public void TryAddCollectible(Collectible collectible)
    {
        if (KnownCollectibles.Contains(collectible))
        {
            return;
        }

        KnownCollectibles.Add(collectible);
        Debug.Log("Collectible added");

        FindClosestWorker(collectible);
    }

    private void FindClosestWorker(Collectible collectible)
    {
        Worker idealWorker = WorkersList[0];
        float smallestDistance = Vector2.Distance(collectible.transform.position, idealWorker.transform.position);

        foreach (var worker in WorkersList)
        {
            float distance = Vector2.Distance(collectible.transform.position, worker.transform.position);

            if (distance < smallestDistance)
            {
                smallestDistance = distance;
                idealWorker = worker;
            }
        }

        SetWorkerToThisCollectible(idealWorker); //check there is at least one worker
    }

    private void SetWorkerToThisCollectible(Worker worker)
    {
        worker.SetHasBeenAssignedToThisCollectibleBool(true);
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

        Debug.Log("New score = " + m_score);
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

    public void GenerateSearchGrid(int mapDimensionValue)
    {
        Debug.Log("Map dimension value in Search grid : " + mapDimensionValue);

        float pointsRatio = (mapDimensionValue / m_distanceBetweenPoints) + 1;
        float gridCenterOffset = (mapDimensionValue / m_distanceBetweenPoints) * m_distanceBetweenPoints / 2f;

        for (int x = 0; x < pointsRatio; x++)
        {
            for (int y = 0; y < pointsRatio; y++)
            {
                float xPosition = x * m_distanceBetweenPoints - gridCenterOffset;
                float yPosition = y * m_distanceBetweenPoints - gridCenterOffset;

                Vector2 gridPosition = new Vector2(xPosition, yPosition);
                SearchGridCell cell = new SearchGridCell(gridPosition);

                SearchGridCells.Add(cell);
                //Debug.Log("New grid position done");
            }
        }
    }

    public void ShowSearchGrid()
    {
        foreach (SearchGridCell gridCell in SearchGridCells)
        {
            Instantiate(m_gridMarker, new Vector3(gridCell.gridPosition.x, gridCell.gridPosition.y, 0.0f), Quaternion.identity);
        }
    }
}

public struct SearchGridCell
{
    public Vector2 gridPosition;
    public bool gridCellAssignedForSearch;
    public bool positionSearched;    

    public SearchGridCell(Vector2 position)
    {
        gridPosition = position;
        gridCellAssignedForSearch = false;
        positionSearched = false;
    }
}



