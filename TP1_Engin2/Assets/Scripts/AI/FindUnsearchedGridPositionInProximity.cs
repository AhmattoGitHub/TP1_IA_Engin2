using UnityEngine;
using MBT;
using System.Collections.Generic;
using System.Linq;

[AddComponentMenu("")]
[MBTNode(name = "Engin2/Find Unsearched Grid Position In Proximity")]
public class FindUnsearchedGridPositionInProximity : Leaf
{
    private Vector2 m_currentPosition = new Vector2();
    private int m_numberOfPositionsToCheckAround = 9;
    public TransformReference m_agentTransform = new TransformReference();
    public Vector2Reference m_targetPosition2D = new Vector2Reference(VarRefMode.DisableConstant);

    public override void OnEnter()
    {
        m_currentPosition = new Vector2(m_agentTransform.Value.position.x, m_agentTransform.Value.position.y);
    }

    public override NodeResult Execute()
    {
        List<SearchGridCell> nearestGridPositions = new List<SearchGridCell>();

        List<SearchGridCell> searchGridCells = TeamOrchestrator._Instance.SearchGridCells;
        int count = searchGridCells.Count;

        nearestGridPositions = GetNearestGridPositions(m_currentPosition, m_numberOfPositionsToCheckAround);

        foreach (SearchGridCell gridCell in nearestGridPositions)
        {
            if (!gridCell.positionSearched && !gridCell.gridCellAssignedForSearch)
            {
                m_targetPosition2D.Value = gridCell.gridPosition;
                
                break;
            }
        }

        for (int i = 0; i < count; i++)
        {
            SearchGridCell gridCell = searchGridCells[i];

            if (!gridCell.positionSearched && !gridCell.gridCellAssignedForSearch)
            {                
                gridCell.gridCellAssignedForSearch = true;
                break;
            }
        }

        return NodeResult.success;
    }

    private List<SearchGridCell> GetNearestGridPositions(Vector2 currentPosition, int numberOfPositions)
    {
        List<SearchGridCell> sortedGridPositions = new List<SearchGridCell>(TeamOrchestrator._Instance.SearchGridCells);
        List<SearchGridCell> nearestPositions = new List<SearchGridCell>();

        sortedGridPositions = sortedGridPositions.OrderBy(gridCell => Vector2.Distance(m_currentPosition, gridCell.gridPosition)).ToList();
        nearestPositions = sortedGridPositions.Take(numberOfPositions).ToList();
        
        return nearestPositions;
    }
}


//List<SearchGridCell> sortedPositions = TeamOrchestrator._Instance.SearchGridCells.OrderBy(cell => Vector2.Distance(currentPosition, cell.position)).ToList();
//sortingPositionsList = sortingPositionsList.OrderBy(gridCell => Vector2.Distance(m_currentPosition, gridCell.gridPosition)).ToList();
