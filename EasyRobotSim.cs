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
            pManager.AddNumberParameter("RobotData", "RData", "RobotData", GH_ParamAccess.list);
            pManager.AddGeometryParameter("RobotModel", "RModel", "RobotMeshModel", GH_ParamAccess.list);
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
            List<GeometryBase> RobotModel = new List<GeometryBase>();
            Mesh ToolMeshModel = new Mesh();
            double SimRatio = 0;
            string Path = " ";
            List<string> stringwrite = new List<string>();

            if (!DA.GetDataList(0, AllAxises)) return;
            if (!DA.GetDataList(1, RobotData)) return;
            if (!DA.GetDataList(2, RobotModel)) return;
            if (!DA.GetData(3, ref ToolMeshModel)) return;
            if (!DA.GetData(4, ref SimRatio)) return;
            if (!DA.GetData(5, ref Path)) return;

            double d01 = RobotData[0];
            double d12 = RobotData[1];
            double d23 = RobotData[2];
            double d34 = RobotData[3];
            double d45 = RobotData[4];
            double d56 = RobotData[5];

            string iniA1 = AllAxises[0].ToString();
            string iniA2 = AllAxises[1].ToString();
            string iniA3 = AllAxises[2].ToString();
            string iniA4 = AllAxises[3].ToString();
            string iniA5 = AllAxises[4].ToString();
            string iniA6 = AllAxises[5].ToString();

            int RobotBase = 1;
            int RobotTool = 1;
            int RobotVel = 15;
            int RobotACC = 100;
            int RobotAPO = 50;
            int RobotPTP = 3;

            double RobotVelCp = 0.25;
            double RobotApocdis = 1.5;
            double RobotAdvance = 3;

            stringwrite.Add(
@"&ACCESS RVP
&REL 1
&PARAM TEMPLATE = C:\KRC\Roboter\Template\vorgabe
&PARAM EDITMASK = *
DEF KUKAProg()

;FOLD INI By Cobra(EasyRobot)
;FOLD BASISTECH INI
GLOBAL INTERRUPT DECL 3 WHEN $STOPMESS == TRUE DO IR_STOPM()
INTERRUPT ON 3
BAS (#INITMOV,0 )
;ENDFOLD (BASISTECH INI)
;ENDFOLD (INI)");
            stringwrite.Add("");
            stringwrite.Add(";FOLD STARTPOSITION - BASE IS "+RobotBase+ ", TOOL IS " + RobotTool + ", POSITION IS A1 " + iniA1+ ", A2 " + iniA2 + ", A3 " + iniA3 + ", A4 " + iniA4 + ", A5 " + iniA5 + ", A6 " + iniA6);
            stringwrite.Add("$BWDSTART = FALSE");
            stringwrite.Add("PDAT_ACT = {VEL "+RobotVel+ ",ACC " + RobotACC + ",APO_DIST " + RobotAPO + "}");
            stringwrite.Add("FDAT_ACT = {TOOL_NO " + RobotTool + ",BASE_NO " + RobotBase + ",IPO_FRAME #BASE}");
            stringwrite.Add("BAS (#PTP_PARAMS," + RobotPTP + ")");
            stringwrite.Add("PTP  {A1 " + iniA1 + ", A2 " + iniA2 + ", A3 " + iniA3 + ", A4 " + iniA4 + ", A5 " + iniA5 + ", A6 " + iniA6+"}");
            stringwrite.Add(";ENDFOLD\n");

            stringwrite.Add(";FOLD LIN SPEED IS " + RobotVelCp + " m/sec, INTERPOLATION SETTINGS IN FOLD");
            stringwrite.Add("$VEL.CP=" + RobotVelCp);
            stringwrite.Add("$APO.CDIS=" + RobotApocdis);
            stringwrite.Add("$ADVANCE=" + RobotAdvance);
            stringwrite.Add(";ENDFOLD\n");

            stringwrite.Add(";FOLD COMMANDS IN FOLD. SELECT EDIT/FOLDS/OPEN ALL FOLDS TO DISPLAY");
            stringwrite.Add("BAS(#VEL_PTP," + RobotPTP + ")");

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
            stringwrite.Add(";ENDFOLD");
            stringwrite.Add("END");


            int simIndex = (int)((AllAxisesD.Count - 1) * SimRatio);
            double[] UseAxises = AllAxisesD[simIndex];

            double UseA1 = -UseAxises[0] * Math.PI / 180;
            double UseA2 = UseAxises[1] * Math.PI / 180;
            double UseA3 = UseAxises[2] * Math.PI / 180;
            double UseA4 = -UseAxises[3] * Math.PI / 180;
            double UseA5 = UseAxises[4] * Math.PI / 180;
            double UseA6 = -UseAxises[5] * Math.PI / 180;

            GeometryBase Axis1Model = RobotModel[0];
            GeometryBase Axis12Model = RobotModel[1];
            GeometryBase Axis23Model = RobotModel[2];
            GeometryBase Axis34Model = RobotModel[3];
            GeometryBase Axis45Model = RobotModel[4];
            GeometryBase Axis56Model = RobotModel[5];
            GeometryBase Axis6Model = RobotModel[6];


            Point3d a1p1 = new Point3d(0, 0, 0);
            Point3d a1p2 = new Point3d(0, 0, d01);
            Vector3d a1a2 = new Vector3d(d12, 0, 0);
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
                return Properties.Resources.simulation;
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