using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class TreeBranchStruct
{
    public Vector3 Position { get; set; }
    public Vector3 Direction { get; set; }
    public float Coeff { get; set; }            //Херня для установки толщины ветви
    public TreeBranchStruct ParentBranch { get; set; }
    public TreeBranchStruct MainBranch { get; set; }
    public TreeBranchStruct OptionalBranch { get; set; }
    public List<Vector3> layer = new List <Vector3>(); 

    public TreeBranchStruct(Vector3 position, Vector3 direction, float coeff, TreeBranchStruct parentBranch, TreeBranchStruct mainBranch, TreeBranchStruct optionalBranch)
    {
        this.Position = position;
        this.Direction = direction;
        this.Coeff = coeff;
        this.ParentBranch = parentBranch;
        this.MainBranch = mainBranch;
        this.OptionalBranch = optionalBranch;
    }
}
