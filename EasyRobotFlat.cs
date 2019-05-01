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
          : base("EasyRobotFlat", "ERB",
              "EasyRobotFlatRoutine",
              "EasyRobot", "EasyRobotCalculation")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("TargetPoints", "TPs", "targetpoints", GH_ParamAccess.list);
            pManager.AddNumberParameter("Robot", "Robot", "RobotData", GH_ParamAccess.list);
            pManager.AddNumberParameter("Tool", "Tool", "ToolData", GH_ParamAccess.list);
            pManager.AddTextParameter("Path", "Path", "Path", GH_ParamAccess.item);
            pManager.AddNumberParameter("Simulation", "Sim", "Simulation", GH_ParamAccess.item,0);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("AxisAngles", "AAs", "AxisAngles", GH_ParamAccess.list);
            pManager.AddGeometryParameter("RobotArmLine", "RAL", "RobotArmLine", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Point3d> TarPts = new List<Point3d>();
            List<double> RobotData = new List<double>();
            List<double> ToolData = new List<double>();
            List<string> stringwrite = new List<string>();
            string Path = " ";
            double sim = 0;

            if (!DA.GetDataList(0, TarPts)) return;
            if (!DA.GetDataList(1, RobotData)) return;
            if (!DA.GetDataList(2, ToolData)) return;
            if (!DA.GetData(3, ref Path)) return;
            if (!DA.GetData(4, ref sim)) return;

            double a2z = RobotData[0];
            double a2x = RobotData[1];
            double d23 = RobotData[2];
            double d35 = RobotData[3];
            double d56 = RobotData[4];
            double da = RobotData[5];

            double Tx = ToolData[0];
            double Ty = ToolData[1];
            double Tz = ToolData[2];

            double Axis1 = 0;
            double Axis2 = 0;
            double Axis3 = 0;
            double Axis4 = 0;
            double Axis5 = 0;
            double Axis6 = 0;

            List<double[]> AllAxises= new List<double[]>();
            


            for (int i = 0; i < TarPts.Count; i++) {
                Point3d pt = TarPts[i];
                double px = pt.X;
                double py = pt.Y;
                double pz = pt.Z;

                double CalHorizontalLength = Math.Pow((px * px + py * py - Ty * Ty), 0.5) - a2x - Tz - d56;
                Axis1 = (180 * Math.Atan(py / px) / Math.PI) - (180 * Math.Atan(Ty / CalHorizontalLength) / Math.PI);

                double CalVerticalLength = pz - a2z + Tx;
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

                string A1 = Axis1.ToString();
                string A2 = Axis2.ToString();
                string A3 = Axis3.ToString();
                string A4 = Axis4.ToString();
                string A5 = Axis5.ToString();
                string A6 = Axis6.ToString();
                string output = "PTP {E6AXIS: A1 "+A1+ ", A2 " + A2 + ", A3 " + A3 + ", A4 " + A4 + ", A5 " + A5 + ", A6 " + A6+"}";
                stringwrite.Add(output);
            }

            int simIndex = (int)((AllAxises.Count-1) * sim);
            double[] UseAxises = AllAxises[simIndex];

            double UseA1 = UseAxises[0]*Math.PI / 180;
            double UseA2 = UseAxises[1]*Math.PI / 180;
            double UseA3 = UseAxises[2]*Math.PI / 180;
            double UseA4 = UseAxises[3]*Math.PI / 180;
            double UseA5 = UseAxises[4]*Math.PI / 180;
            double UseA6 = UseAxises[5]*Math.PI / 180;

            Point3d a1p1 = new Point3d(0, 0, 0);
            Point3d a1p2 = new Point3d(0, 0, a2z);
            Vector3d a1a2 = new Vector3d(a2x, 0, 0);
            Vector3d a1v = new Vector3d(0, 0, 1);
            a1a2.Rotate(UseA1, a1v);
            Point3d a2p = Point3d.Add(a1p2, a1a2);
            Vector3d a2a3 = new Vector3d(d23,0,0);
            Vector3d a2v = new Vector3d(0, 1, 0);
            a2v.Rotate(UseA1, a1v);
            a2a3.Rotate(UseA1, a1v);
            a2a3.Rotate(UseA2, a2v);
            Point3d a3p1 = Point3d.Add(a2p, a2a3);
            Vector3d a3p1p2 = new Vector3d(0, 0, 35);
            Vector3d a3a5 = new Vector3d(420, 0, 0);
            a3p1p2.Rotate(UseA1, a1v);
            a3p1p2.Rotate(UseA2, a2v);
            a3p1p2.Rotate(UseA3, a2v);
            a3a5.Rotate(UseA1, a1v);
            a3a5.Rotate(UseA2, a2v);
            a3a5.Rotate(UseA3, a2v);
            Point3d a3p2 = Point3d.Add(a3p1, a3p1p2);
            Point3d a5 = Point3d.Add(a3p2, a3a5);
            Vector3d a5a6 = new Vector3d(d56, 0, 0);
            a5a6.Rotate(UseA1, a1v);
            a5a6.Rotate(UseA2, a2v);
            a5a6.Rotate(UseA3, a2v);
            a5a6.Rotate(UseA5, a2v);
            Point3d a6 = Point3d.Add(a5, a5a6);

            List<Point3d> AxisesPos = new List<Point3d>();
            AxisesPos.Add(a1p1);
            AxisesPos.Add(a1p2);
            AxisesPos.Add(a2p);
            AxisesPos.Add(a3p1);
            AxisesPos.Add(a3p2);
            AxisesPos.Add(a5);
            AxisesPos.Add(a6);

            Polyline RobotArmLine = new Polyline(AxisesPos);

            string[] finalExport = stringwrite.ToArray();
            System.IO.File.WriteAllLines(Path, finalExport);
            DA.SetDataList(0, UseAxises);
            DA.SetData(1, RobotArmLine);

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
            get { return new Guid("3f6afe52-f0f7-4cb0-89ef-3d5aa3b72815"); }
        }
    }
}