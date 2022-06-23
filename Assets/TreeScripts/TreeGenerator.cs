using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.TreeScripts
{
    static class TreeGenerator
    {
        private const float coast = 1.0f;
        private const float height = 0.5f;
        private const float solidFactor = 0.75f;
        private const float coeff = 0.9f;

        private const float outerRadius = 10f;   //¬нешний радиус шестиугольника
        private const float innerRadius = outerRadius * 0.866025404f;    //¬нутренний радиус шестиугольника

        private enum HexDirection
        {
            NE, E, SE, SW, W, NW
        }

        private static Vector3[] corners = {
        new Vector3(0f, 0f, outerRadius),
        new Vector3(innerRadius, 0f, 0.5f * outerRadius),
        new Vector3(innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(0f, 0f, -outerRadius),
        new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(-innerRadius, 0f, 0.5f * outerRadius),
        new Vector3(0f, 0f, outerRadius)
        };

        private static Vector3 GetFirstSolidCorner(HexDirection direction, TreeBranchStruct tree)
        {
            return corners[(int)direction] * solidFactor;
        }

        private static Vector3 GetSecondSolidCorner(HexDirection direction, TreeBranchStruct tree)
        {
            return corners[(int)direction + 1] * solidFactor;
        }

        private static void Triangulate(TreeBranchStruct tree, HexDirection direction)
        {
            //Vector3 v1 = tree.Position + GetFirstSolidCorner(direction, tree);
            //Vector3 v2 = tree.Position + GetSecondSolidCorner(direction, tree);
            //tree.layer.Add(v1);
            //tree.layer.Add(v2);
        }

        private static Vector3 SetPosition(Vector3 direction, TreeBranchStruct tree)
        {

            Vector3 newPosition = tree.Position;
            newPosition.x += direction.x + UnityEngine.Random.Range(-0.1f, 0.1f);
            newPosition.z += direction.z + UnityEngine.Random.Range(-0.1f, 0.1f);
            newPosition.y += height;

            return newPosition;
        }

        private static void GenerateBranch(ref float bank, ref List<TreeBranchStruct> tree, TreeBranchStruct branch)
        {
            float chance = UnityEngine.Random.Range(0.0f, 1.0f);
            if (chance >= 0.6f)
            {
                Vector3 newPosition;
                newPosition = branch.Position;
                newPosition.x += UnityEngine.Random.Range(-0.5f, 0.5f);
                newPosition.z += UnityEngine.Random.Range(-0.5f, 0.5f);
                newPosition.y += height;

                Vector3 direction = (newPosition - branch.Position) * UnityEngine.Random.Range(-1.5f, 1.5f);
                branch.OptionalBranch = new TreeBranchStruct(SetPosition(direction, branch), direction, 0, branch, null, null);
                tree.Add(branch.OptionalBranch);
               
                direction *= -1;
                branch.MainBranch = new TreeBranchStruct(SetPosition(direction, branch), direction, 0, branch, null, null);
                tree.Add(branch.MainBranch);

                bank -= coast * 2;
            }
            else
            {
                branch.MainBranch = new TreeBranchStruct(SetPosition(branch.Direction, branch), branch.Direction, 0, branch, null, null);
                tree.Add(branch.MainBranch);
                bank -= coast;
            }
        }

        public static List<TreeBranchStruct> GenrateTree()
        {
            Vector3 center; center.x = 0f; center.y = 0f; center.z = 0f;

            List<TreeBranchStruct> tree = new List<TreeBranchStruct> ();
            TreeBranchStruct treePoint = new TreeBranchStruct(center,new Vector3(), 1, null, null, null);


            Vector3 newPosition = treePoint.Position;
            newPosition.x = UnityEngine.Random.Range(treePoint.Position.x, 0.1f);
            newPosition.z = UnityEngine.Random.Range(treePoint.Position.z, 0.1f);
            newPosition.y += height;

            tree.Add(treePoint);

            treePoint.MainBranch = new TreeBranchStruct(newPosition, newPosition - treePoint.Position, treePoint.Coeff * coeff, treePoint, null, null);

            treePoint.Direction = treePoint.MainBranch.Position - treePoint.Position;
            treePoint.MainBranch.Direction = treePoint.Direction;
            tree.Add (treePoint.MainBranch);        //Инициализация первого и второго элемента дерева(Направление)

            float bank = UnityEngine.Random.Range(10.0f, 15.0f);      
            while(bank > 0)
            {
                List<TreeBranchStruct> tmpBranches = new List<TreeBranchStruct>();
                foreach (var branch in tree)
                {
                    if(branch.MainBranch == null)
                        GenerateBranch(ref bank, ref tmpBranches, branch);
                }
                foreach (var branch in tmpBranches)
                {
                    tree.Add(branch);

                }

                tmpBranches.RemoveAll(branch => branch.MainBranch == null);

            }

            foreach(var branch in tree)
            {
                if(branch.MainBranch != null)
                {
                    if (branch.OptionalBranch != null)
                    {
                        if (branch.OptionalBranch.MainBranch == null)
                        {
                            branch.OptionalBranch.Coeff = 0.1f;
                            branch.MainBranch.Coeff = (branch.Coeff - 0.1f) * coeff;
                        }
                        else
                        {
                            if(branch.OptionalBranch != null)
                            {
                                branch.OptionalBranch.Coeff = (branch.Coeff / 2) * coeff;
                                branch.MainBranch.Coeff = (branch.Coeff / 2) * coeff;
                            }
                            else
                                branch.MainBranch.Coeff = branch.Coeff * coeff;
                        }
                    }
                    else
                        branch.MainBranch.Coeff = branch.Coeff * coeff;

                }


                for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
                {
                    Triangulate(branch, d);
                }
            }


            return tree;
        }
        
        
    }
}
