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
            pManager.AddNumberParameter("Tool", "Tool", "ToolData", GH_ParamAccess.list);
            //pManager.AddTextParameter("Path", "Path", "Path", GH_ParamAccess.item);
            //pManager.AddNumberParameter("Simulation", "Sim", "Simulation", GH_ParamAccess.item, 0);
            //pManager.AddMeshParameter("ToolModel","TMo","ToolMeshModel",GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            /*
            pManager.AddTextParameter("AxisAngles", "AAs", "AxisAngles", GH_ParamAccess.list);
            pManager.AddGeometryParameter("RobotArmLine", "RAL", "RobotArmLine", GH_ParamAccess.item);
            pManager.AddGeometryParameter("RobotArmModel", "RAM", "RobotArmModel", GH_ParamAccess.list);
            */
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
            List<double> ToolData = new List<double>();
            //List<string> stringwrite = new List<string>();
            //string Path = " ";
            //double sim = 0;
           // Mesh ToolMeshModel = new Mesh();

            if (!DA.GetDataList(0, TarPls)) return;
            if (!DA.GetDataList(1, RobotData)) return;
            if (!DA.GetDataList(2, ToolData)) return;
            //if (!DA.GetData(3, ref Path)) return;
            //if (!DA.GetData(4, ref sim)) return;
            //if (!DA.GetData(5, ref ToolMeshModel)) return;

            /*
            double a2z = RobotData[0];
            double a2x = RobotData[1];
            double d23 = RobotData[2];
            double d35 = RobotData[3];
            double d56 = RobotData[4];
            double da = RobotData[5];
            */
            double a2z = RobotData[0];
            double a2x = RobotData[1];
            double d23 = RobotData[2];
            double d34 = RobotData[3];
            double d45 = RobotData[4];
            double d56 = RobotData[5];

            double d35 = Math.Pow(d34 * d34 + d45 * d45, 0.5);
            double da = 180*Math.Atan(d34 / d45)/Math.PI;

            double Tx = ToolData[0];
            double Ty = ToolData[1];
            double Tz = ToolData[2];

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

                Axis4 = 45;

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

                /*
                string A1 = Axis1.ToString();
                string A2 = Axis2.ToString();
                string A3 = Axis3.ToString();
                string A4 = Axis4.ToString();
                string A5 = Axis5.ToString();
                string A6 = Axis6.ToString();
                string output = "PTP {E6AXIS: A1 " + A1 + ", A2 " + A2 + ", A3 " + A3 + ", A4 " + A4 + ", A5 " + A5 + ", A6 " + A6 + "}";
                stringwrite.Add(output);
                */
            }

            /*
            int simIndex = (int)((AllAxises.Count - 1) * sim);
            double[] UseAxises = AllAxises[simIndex];

            double UseA1 = UseAxises[0] * Math.PI / 180;
            double UseA2 = UseAxises[1] * Math.PI / 180;
            double UseA3 = UseAxises[2] * Math.PI / 180;
            double UseA4 = UseAxises[3] * Math.PI / 180;
            double UseA5 = UseAxises[4] * Math.PI / 180;
            double UseA6 = UseAxises[5] * Math.PI / 180;

            string Axis1ModelString = Properties.Resources.axis1;
            string Axis12ModelString = Properties.Resources.axis12;
            string Axis23ModelString = Properties.Resources.axis23;
            string Axis34ModelString = Properties.Resources.axis34;
            string Axis45ModelString = Properties.Resources.axis45;
            string Axis56ModelString = Properties.Resources.axis56;
            string Axis6ModelString = Properties.Resources.axis6;

            GeometryBase Axis1Model = GH_Convert.ByteArrayToCommonObject<GeometryBase>(System.Convert.FromBase64String(Axis1ModelString));
            GeometryBase Axis12Model = GH_Convert.ByteArrayToCommonObject<GeometryBase>(System.Convert.FromBase64String(Axis12ModelString));
            GeometryBase Axis23Model = GH_Convert.ByteArrayToCommonObject<GeometryBase>(System.Convert.FromBase64String(Axis23ModelString));
            GeometryBase Axis34Model = GH_Convert.ByteArrayToCommonObject<GeometryBase>(System.Convert.FromBase64String(Axis34ModelString));
            GeometryBase Axis45Model = GH_Convert.ByteArrayToCommonObject<GeometryBase>(System.Convert.FromBase64String(Axis45ModelString));
            GeometryBase Axis56Model = GH_Convert.ByteArrayToCommonObject<GeometryBase>(System.Convert.FromBase64String(Axis56ModelString));
            GeometryBase Axis6Model = GH_Convert.ByteArrayToCommonObject<GeometryBase>(System.Convert.FromBase64String(Axis6ModelString));


            Point3d a1p1 = new Point3d(0, 0, 0);
            Point3d a1p2 = new Point3d(0, 0, a2z);
            Vector3d a1a2 = new Vector3d(a2x, 0, 0);
            Vector3d a1v = new Vector3d(0, 0, 1);
            a1a2.Rotate(UseA1, a1v);
            Axis12Model.Rotate(UseA1, a1v, a1p1);
            //---------------------------
            Point3d a2p = Point3d.Add(a1p2, a1a2);
            Vector3d a2a3 = new Vector3d(d23, 0, 0);
            Vector3d a2v = new Vector3d(0, 1, 0);
            a2v.Rotate(UseA1, a1v);
            a2a3.Rotate(UseA1, a1v);
            a2a3.Rotate(UseA2, a2v);

            Vector3d a2pv = new Vector3d(a2p);
            Axis23Model.Rotate(UseA1, a1v, a1p1);
            Axis23Model.Rotate(UseA2, a2v, a1p1);
            Axis23Model.Translate(a2pv);
            //--------------------------- 
            Point3d a3p1 = Point3d.Add(a2p, a2a3);
            Vector3d a3p1p2 = new Vector3d(0, 0, 35);
            Vector3d a3a5 = new Vector3d(420, 0, 0);
            a3p1p2.Rotate(UseA1, a1v);
            a3p1p2.Rotate(UseA2, a2v);
            a3p1p2.Rotate(UseA3, a2v);
            a3a5.Rotate(UseA1, a1v);
            a3a5.Rotate(UseA2, a2v);
            a3a5.Rotate(UseA3, a2v);

            Vector3d a3p1v = new Vector3d(a3p1);
            Axis34Model.Rotate(UseA1, a1v, a1p1);
            Axis34Model.Rotate(UseA2, a2v, a1p1);
            Axis34Model.Rotate(UseA3, a2v, a1p1);
            Axis34Model.Translate(a3p1v);

            Axis45Model.Rotate(UseA1, a1v, a1p1);
            Axis45Model.Rotate(UseA2, a2v, a1p1);
            Axis45Model.Rotate(UseA3, a2v, a1p1);
            Axis45Model.Translate(a3p1v);
            //---------------------------
            Point3d a3p2 = Point3d.Add(a3p1, a3p1p2);
            Point3d a5p = Point3d.Add(a3p2, a3a5);
            Vector3d a5a6 = new Vector3d(d56, 0, 0);
            a5a6.Rotate(UseA1, a1v);
            a5a6.Rotate(UseA2, a2v);
            a5a6.Rotate(UseA3, a2v);
            a5a6.Rotate(UseA5, a2v);
            Point3d a6p = Point3d.Add(a5p, a5a6);

            Vector3d a5pv = new Vector3d(a5p);
            Axis56Model.Rotate(UseA1, a1v, a1p1);
            Axis56Model.Rotate(UseA2, a2v, a1p1);
            Axis56Model.Rotate(UseA3, a2v, a1p1);
            Axis56Model.Rotate(UseA5, a2v, a1p1);
            Axis56Model.Translate(a5pv);

            Axis6Model.Rotate(UseA1, a1v, a1p1);
            Axis6Model.Rotate(UseA2, a2v, a1p1);
            Axis6Model.Rotate(UseA3, a2v, a1p1);
            Axis6Model.Rotate(UseA5, a2v, a1p1);
            Axis6Model.Translate(a5pv);
            //---------------------------
            Vector3d a6v = new Vector3d(1, 0, 0);
            Vector3d a6pv = new Vector3d(a6p);
            a6v.Rotate(UseA1, a1v);
            a6v.Rotate(UseA2, a2v);
            a6v.Rotate(UseA3, a2v);
            a6v.Rotate(UseA5, a2v);

            ToolMeshModel.Rotate(Math.PI / 2, Vector3d.YAxis, a1p1);
            ToolMeshModel.Rotate(UseA1, a1v, a1p1);
            ToolMeshModel.Rotate(UseA2, a2v, a1p1);
            ToolMeshModel.Rotate(UseA3, a2v, a1p1);
            ToolMeshModel.Rotate(UseA5, a2v, a1p1);
            ToolMeshModel.Rotate(UseA6, a6v, a1p1);
            ToolMeshModel.Translate(a6pv);

            //---------------------------

            List<Point3d> AxisesPos = new List<Point3d>();
            AxisesPos.Add(a1p1);
            AxisesPos.Add(a1p2);
            AxisesPos.Add(a2p);
            AxisesPos.Add(a3p1);
            AxisesPos.Add(a3p2);
            AxisesPos.Add(a5p);
            AxisesPos.Add(a6p);

            Polyline RobotArmLine = new Polyline(AxisesPos);

            List<GeometryBase> RobotArms = new List<GeometryBase>();
            RobotArms.Add(Axis1Model);
            RobotArms.Add(Axis12Model);
            RobotArms.Add(Axis23Model);
            RobotArms.Add(Axis34Model);
            RobotArms.Add(Axis45Model);
            RobotArms.Add(Axis56Model);
            RobotArms.Add(Axis6Model);
            RobotArms.Add(ToolMeshModel);
            */

            /*
            Grasshopper.DataTree<double> TreeAxises = new Grasshopper.DataTree<double>();
            
            for(int i = 0; i < AllAxises.Count; i++)
            {
                double[] SingleAxises = AllAxises[i];
                for (int j = 0; j < 6; j++)
                {
                    double nowAxis = SingleAxises[j];
                    Grasshopper.Kernel.Data.GH_Path path = new Grasshopper.Kernel.Data.GH_Path(i,j);
                    TreeAxises.Add(nowAxis, path);
                }
            }
            */
            
            /*
            string[] finalExport = stringwrite.ToArray();
            System.IO.File.WriteAllLines(Path, finalExport);
            */

            /*
            DA.SetDataList(0, UseAxises);
            DA.SetData(1, RobotArmLine);
            DA.SetDataList(2, RobotArms);
            */
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