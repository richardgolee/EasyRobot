using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace EasyRobot
{
    public class EasyRobotFree : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public EasyRobotFree()
          : base("FreeCore", "FreC",
              "EasyRobotFreeCore",
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
            pManager.AddPlaneParameter("ToolPlane", "ToolP", "ToolPlane",GH_ParamAccess.item);
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
            Plane tool = Plane.WorldXY;

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
            double da = Math.Atan(d34 / d45);

            double Axis1 = 0;
            double Axis2 = 0;
            double Axis3 = 0;
            double Axis4 = 0;
            double Axis5 = 0;
            double Axis6 = 0;

            List<double[]> AllAxises = new List<double[]>();
            List<double> AllAxiesFlat = new List<double>();

            List<Plane> robPlanes = new List<Plane>();
            foreach(Plane tar in TarPls)
            {
                Transform now = Transform.PlaneToPlane(tool, tar);
                Plane rob = Plane.WorldXY;
                rob.Transform(now);
                robPlanes.Add(rob);
            }

            for(int i = 0; i < robPlanes.Count; i++)
            {
                Plane robNow = robPlanes[i];
                Point3d toolOrigin = robNow.Origin;
                Vector3d a5a6 = robNow.Normal;
                a5a6.Unitize();
                a5a6 = Vector3d.Multiply(d56, a5a6);

                Point3d a5p = Point3d.Add(toolOrigin, -a5a6);
                Point3d a6p = new Point3d(toolOrigin);

                Axis1 = Math.Atan(a5p.Y / a5p.X);

                double CalHorizontalLength = Math.Pow((a5p.X * a5p.X) + (a5p.Y * a5p.Y), 0.5) - a2x;
                double CalVerticalLength = a5p.Z - a2z;
                double SumLength = Math.Pow((CalHorizontalLength * CalHorizontalLength + CalVerticalLength * CalVerticalLength), 0.5);

                double cosA2i = (d23 * d23 + SumLength * SumLength - d35 * d35) / (2 * d23 * SumLength);
                double A2i = Math.Acos(cosA2i);
                double cosA2j = CalHorizontalLength / SumLength;
                double A2j = Math.Acos(cosA2j);
                if (CalVerticalLength < 0) { A2j = -A2j; }

                Axis2 = -(A2i + A2j);

                double cosA3i = (d23 * d23 + d35 * d35 - SumLength * SumLength) / (2 * d23 * d35);
                double A3i = Math.Acos(cosA3i);

                Axis3 =  (Math.PI - A3i) + da;

                Point3d a1p1 = new Point3d(0, 0, 0);
                Point3d a1p2 = new Point3d(0, 0, a2z);
                Vector3d a1a2 = new Vector3d(a2x, 0, 0);
                Vector3d a1v = new Vector3d(0, 0, 1);
                a1a2.Rotate(Axis1, a1v);

                //---------------------------
                Point3d a2p = Point3d.Add(a1p2, a1a2);
                Vector3d a2a3 = new Vector3d(d23, 0, 0);
                Vector3d a2v = new Vector3d(0, 1, 0);
                a2v.Rotate(Axis1, a1v);
                a2a3.Rotate(Axis1, a1v);
                a2a3.Rotate(Axis2, a2v);

                //--------------------------- 
                Point3d a3p = Point3d.Add(a2p, a2a3);
                Vector3d a3a4 = new Vector3d(0, 0, 25);
                Vector3d a4a5 = new Vector3d(420, 0, 0);
                a3a4.Rotate(Axis1, a1v);
                a3a4.Rotate(Axis2, a2v);
                a3a4.Rotate(Axis3, a2v);
                a4a5.Rotate(Axis1, a1v);
                a4a5.Rotate(Axis2, a2v);
                a4a5.Rotate(Axis3, a2v);

                Point3d a4p = Point3d.Add(a3p, a3a4);               
                Plane a5vPl = new Plane(a4p, a5p, a6p);
                Vector3d a5v = -a5vPl.Normal;
                Axis5 = Vector3d.VectorAngle(a5a6, a4a5,a5vPl);
                
                Vector3d a6vy = robNow.YAxis;
                Axis6 = Vector3d.VectorAngle(a5v, a6vy,robNow);

                Plane axis4pl = new Plane(a4p, a4a5);
                Axis4 = Vector3d.VectorAngle(a2v, a5v,axis4pl);

                Axis1 = Math.Round(Axis1 * 180 / Math.PI, 3);
                Axis2 = Math.Round(Axis2 * 180 / Math.PI, 3);
                Axis3 = Math.Round(Axis3 * 180 / Math.PI, 3);
                Axis4 = Math.Round(Axis4 * 180 / Math.PI, 3);
                Axis5 = Math.Round(Axis5 * 180 / Math.PI, 3);
                Axis6 = Math.Round(Axis6 * 180 / Math.PI, 3);

                double Axis4f = 0;
                double Axis5f = 0;
                double Axis6f = 0;
                if (i > 1) { 
                    double[] AxisesFormer = AllAxises[i - 1];
                     Axis4f = AxisesFormer[3];
                     Axis5f = AxisesFormer[4];
                     Axis6f = AxisesFormer[5];
                }

                if (Axis6 - Axis6f > 180) {
                        Axis6 = Math.Round(Axis6 - 360,3);
                }
                if (Axis5 - Axis5f > 180){
                        Axis5 = Math.Round(Axis5 - 360, 3);
                }
                if (Axis4 - Axis4f > 180){
                        Axis4 = Math.Round(Axis4 - 360, 3);
                }

                double[] Axises = new double[6];
                Axises[0] = Axis1;
                Axises[1] = Axis2;
                Axises[2] = Axis3;
                Axises[3] = Axis4;
                Axises[4] = Axis5;
                Axises[5] = Axis6;
                AllAxises.Add(Axises);

                for (int k = 0; k < 6; k++)
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
            get { return new Guid("9c3b47cd-a221-458e-96bb-b29bef1c6c2e"); }
        }
    }
}