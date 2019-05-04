using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace EasyRobot
{
    public class EasyRobotLock : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public EasyRobotLock()
          : base("LockCore", "LocC",
              "EasyRobotLockCore",
              "EasyRobot", "Core")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPlaneParameter("TargetPlanes", "TPls", "targetPlanes", GH_ParamAccess.list);
            pManager.AddNumberParameter("Robot", "Robot", "RobotData", GH_ParamAccess.list);
            pManager.AddPointParameter("Tool", "Tool", "ToolPoint", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("AllAxises", "AllAxises", "AllAxises", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Plane> TarPls = new List<Plane>();
            List<double> RobotData = new List<double>();
            Point3d tool = Point3d.Origin;


            if (!DA.GetDataList(0, TarPls)) return;
            if (!DA.GetDataList(1, RobotData)) return;
            if (!DA.GetData(2, ref tool)) return;

            double a2z = RobotData[0];
            double a2x = RobotData[1];
            double d23 = RobotData[2];
            double d34 = RobotData[3];
            double d45 = RobotData[4];
            double d56 = RobotData[5];

            double d35 = Math.Pow(d34 * d34 + d45 * d45, 0.5);
            double da = 180*Math.Atan(d34 / d45)/Math.PI;

            double Tx = tool.X;
            double Ty = 0;
            double Tz = tool.Z;

            double Axis1 = 0;
            double Axis2 = 0;
            double Axis3 = 0;
            double Axis4 = 0;
            double Axis5 = 0;
            double Axis6 = 0;

            List<double[]> AllAxises = new List<double[]>();
            List<double> AllAxiesFlat = new List<double>();


            for (int i = 0; i < TarPls.Count; i++)
            {
                Plane pl = TarPls[i];
                Point3d pltar = pl.Origin;
                Vector3d plf = pl.Normal;
                plf.Unitize();
                Vector3d toolf = new Vector3d(-plf.X * Tx, -plf.Y * Tx, -plf.Z * Tx);
                Point3d toolOrigin = Point3d.Add(pltar, toolf);

                double px = toolOrigin.X;
                double py = toolOrigin.Y;
                double pz = toolOrigin.Z;

                Axis1 = (180 * Math.Atan(py / px) / Math.PI);

                Plane armpl = new Plane(py, -px, 0, 0);
                Point3d proTool = armpl.ClosestPoint(pltar);
                double refDis = armpl.DistanceTo(pltar);
                Axis6 = -180 * Math.Asin(refDis / Tx) / Math.PI;
                

                Plane Falan = new Plane(pltar, proTool, toolOrigin);
                Vector3d a5a6vec = Falan.Normal;
                a5a6vec.Unitize();
                Vector3d a5tool = Vector3d.Multiply(d56 + Tz, a5a6vec);
                if (Axis6 < 0)
                {
                    a5tool.Reverse();
                }
                double a5World = 180*Vector3d.VectorAngle(a5tool, Vector3d.ZAxis)/Math.PI;
                if (a5tool.X > 0)
                {
                    a5World = -a5World;
                }



                Point3d a5Real = Point3d.Add(toolOrigin, a5tool);


                double CalHorizontalLength = Math.Pow((a5Real.X * a5Real.X)+(a5Real.Y*a5Real.Y), 0.5) - a2x;
                double CalVerticalLength = a5Real.Z - a2z;
                double SumLength = Math.Pow((CalHorizontalLength * CalHorizontalLength + CalVerticalLength * CalVerticalLength), 0.5);

                double cosA2i = (d23 * d23 + SumLength * SumLength - d35 * d35) / (2 * d23 * SumLength);
                double A2i = Math.Acos(cosA2i);
                double cosA2j = CalHorizontalLength / SumLength;
                double A2j = Math.Acos(cosA2j);
                if (CalVerticalLength < 0) { A2j = -A2j; }

                Axis2 = 180 * -(A2i + A2j) / Math.PI;

                double cosA3i = (d23 * d23 + d35 * d35 - SumLength * SumLength) / (2 * d23 * d35);
                double A3i = Math.Acos(cosA3i);

                Axis3 = 180 * (Math.PI - A3i) / Math.PI;

                Axis4 = 0;

                Axis3 = Axis3 + da;
                Axis5 = (0-Axis3-Axis2)+(90-a5World);

                Axis1 = Math.Round(Axis1, 3);
                Axis2 = Math.Round(Axis2, 3);
                Axis3 = Math.Round(Axis3, 3);
                Axis4 = Math.Round(Axis4, 3);
                Axis5 = Math.Round(Axis5, 3);
                Axis6 = Math.Round(Axis6, 3);

                double[] Axises = new double[6];
                Axises[0] = Axis1;
                Axises[1] = Axis2;
                Axises[2] = Axis3;
                Axises[3] = Axis4;
                Axises[4] = Axis5;
                Axises[5] = Axis6;
                AllAxises.Add(Axises);
                for(int k = 0;k< 6; k++)
                {
                    AllAxiesFlat.Add(Axises[k]);
                }

            }


            DA.SetDataList(0, AllAxiesFlat);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("aa3b106d-6fa7-4598-be99-d67c3819825b"); }
        }
    }
}