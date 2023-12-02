using UnityEngine;
using MBT;
using System.Collections.Generic;



[AddComponentMenu("")]
[MBTNode(name = "Engin2/Find Unsearched Grid Position In Proximity")]
public class FindUnsearchedGridPositionInProximity : Leaf
{
    private Vector2 m_currentPosition = new Vector2();
    private TeamOrchestrator m_teamOrchestrator = null;
    private Dictionary<Vector2Int, SearchGridCell> m_searchGridCellDictionary = null;
    public TransformReference m_agentTransform = new TransformReference();
    public Vector2Reference m_targetPosition2D = new Vector2Reference(VarRefMode.DisableConstant);


    public override void OnEnter()
    {
        m_currentPosition = new Vector2(m_agentTransform.Value.position.x, m_agentTransform.Value.position.y);
        m_teamOrchestrator = TeamOrchestrator._Instance;
        m_searchGridCellDictionary = m_teamOrchestrator.SearchGridCellsDictionary;
    }

    public override NodeResult Execute()
    {
        Vector2Int dictionaryKey = Vector2Int.zero;

        dictionaryKey = FindNearestUnassignedSearchGridCell();

        if (m_teamOrchestrator.TestingThing == 3)
        {
            Debug.Log("Test");
        }

        if (m_teamOrchestrator.SearchGridCellsDictionary.ContainsKey(dictionaryKey))
        {
            SearchGridCell cellToUpdate = m_teamOrchestrator.SearchGridCellsDictionary[dictionaryKey];

            //m_teamOrchestrator.SearchGridCellsDictionary[dictionaryKey].GridCellAssignedForSearch = true;

            cellToUpdate.GridCellAssignedForSearch = true;
        }
        else 
        {
            
        }

        m_targetPosition2D.Value = dictionaryKey;

        m_teamOrchestrator.TestingThing++;

        

        //throw new System.NotImplementedException();
        return NodeResult.success;
    }



    private Vector2Int FindNearestUnassignedSearchGridCell()
    {        
        float minDistance = float.MaxValue;
        Vector2Int nearestPosition = Vector2Int.zero;        

        foreach (var key in m_teamOrchestrator.SearchGridCellsDictionary)
        {
            Vector2Int gridPosition = key.Key;
            SearchGridCell gridCell = key.Value;
            
            float distance = Vector2.Distance(m_currentPosition, gridPosition);
            
            if (distance < minDistance 
                && !gridCell.GridCellAssignedForSearch 
                && !gridCell.PositionSearched)
            {                
                minDistance = distance;
                nearestPosition = gridPosition;                
            }
        }

        return nearestPosition;
    }



}


//List<SearchGridCell> sortedPositions = TeamOrchestrator._Instance.SearchGridCells.OrderBy(cell => Vector2.Distance(currentPosition, cell.position)).ToList();
//sortingPositionsList = sortingPositionsList.OrderBy(gridCell => Vector2.Distance(m_currentPosition, gridCell.gridPosition)).ToList();







//public override NodeResult Execute()
//{
//    List<SearchGridCell> nearestGridPositions = new List<SearchGridCell>();
//
//    List<SearchGridCell> searchGridCells = TeamOrchestrator._Instance.SearchGridCells;
//    int count = searchGridCells.Count;
//
//    nearestGridPositions = GetNearestGridPositions(m_currentPosition, m_numberOfPositionsToCheckAround);
//
//    foreach (SearchGridCell gridCell in nearestGridPositions)
//    {
//        if (!gridCell.positionSearched && !gridCell.gridCellAssignedForSearch)
//        {
//            m_targetPosition2D.Value = gridCell.gridPosition;
//            
//            break;
//        }
//    }
//
//    for (int i = 0; i < count; i++)
//    {
//        SearchGridCell gridCell = searchGridCells[i];
//
//        if (!gridCell.positionSearched && !gridCell.gridCellAssignedForSearch)
//        {                
//            gridCell.gridCellAssignedForSearch = true;
//            break;
//        }
//    }
//
//    return NodeResult.success;
//}
//
//private List<SearchGridCell> GetNearestGridPositions(Vector2 currentPosition, int numberOfPositions)
//{
//    List<SearchGridCell> sortedGridPositions = new List<SearchGridCell>(TeamOrchestrator._Instance.SearchGridCells);
//    List<SearchGridCell> nearestPositions = new List<SearchGridCell>();
//
//    sortedGridPositions = sortedGridPositions.OrderBy(gridCell => Vector2.Distance(m_currentPosition, gridCell.gridPosition)).ToList();
//    nearestPositions = sortedGridPositions.Take(numberOfPositions).ToList();
//    
//    return nearestPositions;
//}








