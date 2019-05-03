using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace EasyRobot
{
    public class EasyRobotSim : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public EasyRobotSim()
          : base("MoveSimulation", "MovS",
              "RobotMoveSimulation",
              "EasyRobot", "Simulation")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("6AxisAngles","6Axis", "Data for Simulation", GH_ParamAccess.list);
            pManager.AddNumberParameter("Robot", "Robot", "RobotData", GH_ParamAccess.list);
            pManager.AddMeshParameter("ToolModel", "Tool", "Tool Mesh Model", GH_ParamAccess.item);
            pManager.AddNumberParameter("SimRatio", "Time", "Time ratio of Simulation", GH_ParamAccess.item, 0);
            pManager.AddTextParameter("Path", "Path", "Path", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("AxisAngles", "AAs", "AxisAngles", GH_ParamAccess.list);
            pManager.AddGeometryParameter("RobotArmLine", "RAL", "RobotArmLine", GH_ParamAccess.item);
            pManager.AddGeometryParameter("RobotArmModel", "RAM", "RobotArmModel", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            List<double> AllAxises = new List<double>();
            List<double> RobotData = new List<double>();
            Mesh ToolMeshModel = new Mesh();
            double SimRatio = 0;
            string Path = " ";
            List<string> stringwrite = new List<string>();

            if (!DA.GetDataList(0, AllAxises)) return;
            if (!DA.GetDataList(1, RobotData)) return;
            if (!DA.GetData(2, ref ToolMeshModel)) return;
            if (!DA.GetData(3, ref SimRatio)) return;
            if (!DA.GetData(4, ref Path)) return;

            double a2z = RobotData[0];
            double a2x = RobotData[1];
            double d23 = RobotData[2];
            double d34 = RobotData[3];
            double d45 = RobotData[4];
            double d56 = RobotData[5];

            double d35 = Math.Pow(d34 * d34 + d45 * d45, 0.5);
            double da = 180 * Math.Atan(d34 / d45) / Math.PI;

            List<double[]> AllAxisesD = new List<double[]>();
            for(int i =0; i < AllAxises.Count/6; i++)
            {
                double[] singlePair = new double[6];
                for(int j = 0; j < 6; j++)
                {
                    int indexOfSingle = i * 6 + j;
                    singlePair[j] = AllAxises[indexOfSingle];
                }
                AllAxisesD.Add(singlePair);

                string A1 = singlePair[0].ToString();
                string A2 = singlePair[1].ToString();
                string A3 = singlePair[2].ToString();
                string A4 = singlePair[3].ToString();
                string A5 = singlePair[4].ToString();
                string A6 = singlePair[5].ToString();
                string output = "PTP {E6AXIS: A1 " + A1 + ", A2 " + A2 + ", A3 " + A3 + ", A4 " + A4 + ", A5 " + A5 + ", A6 " + A6 + "}";
                stringwrite.Add(output);

                
            }


            int simIndex = (int)((AllAxisesD.Count - 1) * SimRatio);
            double[] UseAxises = AllAxisesD[simIndex];

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
            Point3d a3p = Point3d.Add(a2p, a2a3);
            Vector3d a3a4 = new Vector3d(0, 0, 25);
            Vector3d a4a5 = new Vector3d(420, 0, 0);
            a3a4.Rotate(UseA1, a1v);
            a3a4.Rotate(UseA2, a2v);
            a3a4.Rotate(UseA3, a2v);
            a4a5.Rotate(UseA1, a1v);
            a4a5.Rotate(UseA2, a2v);
            a4a5.Rotate(UseA3, a2v);

            Vector3d a3pv = new Vector3d(a3p);
            Axis34Model.Rotate(UseA1, a1v, a1p1);
            Axis34Model.Rotate(UseA2, a2v, a1p1);
            Axis34Model.Rotate(UseA3, a2v, a1p1);
            Axis34Model.Translate(a3pv);

            Point3d a4p = Point3d.Add(a3p, a3a4);
            Vector3d a4v = new Vector3d(1,0,0);
            a4v.Rotate(UseA1, a1v);
            a4v.Rotate(UseA2, a2v);
            a4v.Rotate(UseA3, a2v);

            Vector3d a4pv = new Vector3d(a4p);
            Axis45Model.Rotate(UseA1, a1v, a1p1);
            Axis45Model.Rotate(UseA2, a2v, a1p1);
            Axis45Model.Rotate(UseA3, a2v, a1p1);
            Axis45Model.Rotate(UseA4, a4v, a1p1);
            Axis45Model.Translate(a4pv);
            //---------------------------
            Point3d a5p = Point3d.Add(a4p, a4a5);
            Vector3d a5a6 = new Vector3d(d56, 0, 0);
            Vector3d a5v = new Vector3d(0, 1, 0);
            a5a6.Rotate(UseA1, a1v);
            a5a6.Rotate(UseA2, a2v);
            a5a6.Rotate(UseA3, a2v);
            a5a6.Rotate(UseA4, a4v);

            a5v.Rotate(UseA1, a1v);
            a5v.Rotate(UseA4, a4v);

            a5a6.Rotate(UseA5, a5v);

            
            Vector3d a5pv = new Vector3d(a5p);
            Axis56Model.Rotate(UseA1, a1v, a1p1);
            Axis56Model.Rotate(UseA2, a2v, a1p1);
            Axis56Model.Rotate(UseA3, a2v, a1p1);
            Axis56Model.Rotate(UseA4, a4v, a1p1);
            Axis56Model.Rotate(UseA5, a5v, a1p1);
            Axis56Model.Translate(a5pv);

            Point3d a6p = Point3d.Add(a5p, a5a6);
            Vector3d a6v = new Vector3d(1, 0, 0);
            Vector3d a6pv = new Vector3d(a6p);
            a6v.Rotate(UseA1, a1v);
            a6v.Rotate(UseA2, a2v);
            a6v.Rotate(UseA3, a2v);
            a6v.Rotate(UseA4, a4v);
            a6v.Rotate(UseA5, a5v);
            
            Axis6Model.Rotate(UseA1, a1v, a1p1);
            Axis6Model.Rotate(UseA2, a2v, a1p1);
            Axis6Model.Rotate(UseA3, a2v, a1p1);
            Axis6Model.Rotate(UseA4, a4v, a1p1);
            Axis6Model.Rotate(UseA5, a5v, a1p1);
            Axis6Model.Rotate(UseA6, a6v, a1p1);

            Axis6Model.Translate(a5pv);
            //---------------------------


            ToolMeshModel.Rotate(Math.PI / 2, Vector3d.YAxis, a1p1);
            ToolMeshModel.Rotate(UseA1, a1v, a1p1);
            ToolMeshModel.Rotate(UseA2, a2v, a1p1);
            ToolMeshModel.Rotate(UseA3, a2v, a1p1);
            ToolMeshModel.Rotate(UseA4, a4v, a1p1);
            ToolMeshModel.Rotate(UseA5, a5v, a1p1);
            ToolMeshModel.Rotate(UseA6, a6v, a1p1);
            ToolMeshModel.Translate(a6pv);

            //---------------------------

            List<Point3d> AxisesPos = new List<Point3d>();
            AxisesPos.Add(a1p1);
            AxisesPos.Add(a1p2);
            AxisesPos.Add(a2p);
            AxisesPos.Add(a3p);
            AxisesPos.Add(a4p);
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


            string[] finalExport = stringwrite.ToArray();
            System.IO.File.WriteAllLines(Path, finalExport);
            DA.SetDataList(0, UseAxises);
            DA.SetData(1, RobotArmLine);
            DA.SetDataList(2, RobotArms);








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
            get { return new Guid("14ca8c8b-199b-40c4-a5cc-8b7fdd4ebdb9"); }
        }
    }
}