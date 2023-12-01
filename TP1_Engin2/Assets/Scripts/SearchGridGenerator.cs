using System.Collections.Generic;
using UnityEngine;

public class SearchGridGenerator : MonoBehaviour
{
    [SerializeField]
    private MapGenerator m_mapGenerator = null;
    [SerializeField]
    private GameObject m_gridMarker = null;    
    private int m_distanceBetweenPoints = 6;
    public List<GridCell> GridCells { get; private set; } = new List<GridCell>();
    public struct GridCell
    {
        public Vector2 position;
        public bool positionSearched;

        public GridCell(Vector2 pos)
        {
            position = pos;
            positionSearched = false;
        }
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
                GridCell cell = new GridCell(gridPosition);

                GridCells.Add(cell);

                Debug.Log("New grid position done");
            }
        }        
    }

    public void ShowSearchGrid()
    {
        foreach (GridCell cell in GridCells)
        {
            Instantiate(m_gridMarker, new Vector3(cell.position.x, cell.position.y, 0.0f), Quaternion.identity);

        }
    }


}





//public static SearchGridGenerator _Instance
//{
//    get;
//    private set;
//}
//
//
//private void Awake()
//{
//    if (_Instance == null || _Instance == this)
//    {
//        _Instance = this;
//        return;
//    }
//    Destroy(this);
//}



