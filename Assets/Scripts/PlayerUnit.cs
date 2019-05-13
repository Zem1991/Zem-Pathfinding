using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : MonoBehaviour
{
    private Pathfinder pathfinder;
    public Transform target;
    public List<PathNode> pathNodes = new List<PathNode>();
    public float pathSize;
    public float operations;
    public List<PathNode> openNodes = new List<PathNode>();

    // Start is called before the first frame update
    void Start()
    {
        pathfinder = new Pathfinder();
    }

    // Update is called once per frame
    void Update()
    {
        pathfinder.FindPath(transform.position, target.transform.position, out pathNodes, out pathSize, out operations);
        if (pathNodes != null)
        {
            Debug.Log("pathnodes = " + pathNodes.Count + " | size = " + pathSize + " | operations = " + operations);
        }
        else
        {
            Debug.Log("null pathnodes!");
        }
    }

    private void OnDrawGizmos()
    {
        if (pathNodes != null)
        {
            Gizmos.color = Color.magenta;
            foreach (PathNode item in pathNodes)
            {
                Gizmos.DrawSphere(item.gridNode.transform.position, .5F);
            }
        }
    }
}
