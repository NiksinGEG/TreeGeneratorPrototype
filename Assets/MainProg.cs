using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.TreeScripts;


//[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MainProg: MonoBehaviour
{
    public Mesh treeMesh;
    List<Vector3> vertices;
    List<int> triangles;

    private List<TreeBranchStruct> tree = new List<TreeBranchStruct>();

    private void OnDrawGizmos()
    {
        if (tree == null)
            return;
        foreach(var branch in tree)
        {
            Gizmos.color = Color.red;
            if (branch.MainBranch != null)
            {
                Gizmos.DrawSphere(branch.Position, 0.05f);
                Gizmos.DrawLine(branch.Position, branch.MainBranch.Position);
            }

            if(branch.OptionalBranch != null)
            {
                Gizmos.DrawLine(branch.Position, branch.OptionalBranch.Position);
            }
            if (branch.MainBranch == null)
                Gizmos.DrawSphere(branch.Position, 0.05f);

            foreach(var verticle in branch.layer)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawSphere(verticle, 0.02f);
            }
            
        }

    }


    // Start is called before the first frame update
    [ContextMenu("Create Log")]
    public void Awake()
    {
        System.Random rand = new System.Random();
        UnityEngine.Random.InitState(rand.Next(1, 3000000));

        GetComponent<MeshFilter>().mesh = treeMesh = new Mesh();//GetComponent<Mesh>();
        
        tree = TreeGenerator.GenrateTree();
        
        //Триангуляция
        treeMesh.Clear();
        vertices = new List<Vector3>();
        triangles = new List<int>();
        Triangulate(tree[0], 65);
        treeMesh.vertices = vertices.ToArray();
        treeMesh.triangles = triangles.ToArray();
        treeMesh.RecalculateNormals();
    }
    
    private void Triangulate(TreeBranchStruct root, int poligons)
    {
        for (int i = 0; i < poligons; i++)
        {
            var v1 = GetVertice(i, poligons, root);
            var v2 = GetVertice(i + 1, poligons, root);
            if (root.MainBranch != null)
            {
                var v3 = GetVertice(i, poligons, root.MainBranch);
                var v4 = GetVertice(i + 1, poligons, root.MainBranch);
                AddQuad(v1, v2, v3, v4);
            }
            if(root.OptionalBranch != null)
            {
                var v5 = GetVertice(i, poligons, root.OptionalBranch);
                var v6 = GetVertice(i + 1, poligons, root.OptionalBranch);
                AddQuad(v1, v2, v5, v6);
            }
        }
        if(root.MainBranch != null)
            Triangulate(root.MainBranch, poligons);
        if (root.OptionalBranch != null)
            Triangulate(root.OptionalBranch, poligons);
    }

    private void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
    {
        var index = vertices.Count;
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        vertices.Add(v4);
        triangles.Add(index);
        triangles.Add(index + 2);
        triangles.Add(index + 1);
        triangles.Add(index + 1);
        triangles.Add(index + 2);
        triangles.Add(index + 3);
    }

    private Vector3 GetVertice(int number, int totalNum, TreeBranchStruct point)
    {
        Vector3 res = new Vector3();
        res.y = point.Position.y;
        res.x = point.Position.x + point.Coeff * Mathf.Sin(2 * Mathf.PI / totalNum * number);
        res.z = point.Position.z + point.Coeff * Mathf.Cos(2 * Mathf.PI / totalNum * number);
        return res;
    }
}
