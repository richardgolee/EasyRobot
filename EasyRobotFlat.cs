using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace EasyRobot
{
    public class EasyRobotFlat : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public EasyRobotFlat()
          : base("FlatCore", "FlaC",
              "EasyRobotFlatCore",
              "EasyRobot", "Core")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("TargetPoints", "TPs", "targetpoints", GH_ParamAccess.list);
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
            List<Point3d> TarPts = new List<Point3d>();
            List<double> RobotData = new List<double>();
            Point3d tool = Point3d.Origin;

            if (!DA.GetDataList(0, TarPts)) return;
            if (!DA.GetDataList(1, RobotData)) return;
            if (!DA.GetData(2, ref tool)) return;

            double d01 = RobotData[0];
            double d12 = RobotData[1];
            double d23 = RobotData[2];
            double d34 = RobotData[3];
            double d45 = RobotData[4];
            double d56 = RobotData[5];

            double d35 = Math.Pow(d34 * d34 + d45 * d45, 0.5);
            double da = 180 * Math.Atan(d34 / d45) / Math.PI;

            double Tx = tool.X;
            double Ty = tool.Y;
            double Tz = tool.Z;

            double Axis1 = 0;
            double Axis2 = 0;
            double Axis3 = 0;
            double Axis4 = 0;
            double Axis5 = 0;
            double Axis6 = 0;

            List<double[]> AllAxises= new List<double[]>();
            List<double> AllAxiesFlat = new List<double>();



            for (int i = 0; i < TarPts.Count; i++) {
                Point3d pt = TarPts[i];
                double px = pt.X;
                double py = pt.Y;
                double pz = pt.Z;

                double CalHorizontalLength = Math.Pow((px * px + py * py - Ty * Ty), 0.5) - d12 - Tz - d56;
                Axis1 = ((180 * Math.Atan(py / px) / Math.PI) - (180 * Math.Atan(Ty / (CalHorizontalLength+d12+Tz+d56)) / Math.PI));

                double CalVerticalLength = pz - d01 + Tx;
                double SumLength = Math.Pow((CalHorizontalLength * CalHorizontalLength + CalVerticalLength * CalVerticalLength), 0.5);

                double cosA2i = (d23 * d23 + SumLength * SumLength - d35 * d35) / (2 * d23 * SumLength);
                double A2i = Math.Acos(cosA2i);
                double cosA2j = CalHorizontalLength / SumLength;
                double A2j = Math.Acos(cosA2j);
                if (CalVerticalLength < 0) { A2j = -A2j; }

                Axis2 = 180*-(A2i + A2j)/Math.PI;

                double cosA3i = (d23 * d23 + d35 * d35 - SumLength * SumLength) / (2 * d23 * d35);
                double A3i = Math.Acos(cosA3i);

                Axis3 = 180 * (Math.PI - A3i) / Math.PI;

                double cosA5i = (d35 * d35 + SumLength * SumLength - d23 * d23) / (2 * d35 * SumLength);
                double A5i = Math.Acos(cosA5i);

                Axis5 = 180 * (-(A5i - A2j)) / Math.PI;

                Axis4 = 0;
                Axis6 = 0;

                Axis3 = Axis3 + da;
                Axis5 = Axis5 - da;

                Axis1 = -Math.Round(Axis1, 3);
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
                return Properties.Resources.flatcore;
                //return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("3f6afe52-f0f7-4cb0-89ef-3d5aa3b72815"); }
        }
    }
}