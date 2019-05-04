using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace EasyRobot
{
    public class KUKA_KR6_R900 : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the KUKA_KR6_R900 class.
        /// </summary>
        public KUKA_KR6_R900()
          : base("KUKA_KR6_R900", "KUKA_KR6_R900",
              "KUKA_KR6_R900",
              "EasyRobot", "Robot")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Data", "Data", "RobotData", GH_ParamAccess.list);
            pManager.AddGeometryParameter("Model", "Model", "RobotMeshModel", GH_ParamAccess.list);
            pManager.HideParameter(1);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double d01 = 400;
            double d12 = 25;
            double d23 = 455;
            double d34 = 25;
            double d45 = 420;
            double d56 = 90;
            List<double> RobotData = new List<double>();
            List<GeometryBase> RobotModel = new List<GeometryBase>();

            RobotData.Add(d01);
            RobotData.Add(d12);
            RobotData.Add(d23);
            RobotData.Add(d34);
            RobotData.Add(d45);
            RobotData.Add(d56);

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

            RobotModel.Add(Axis1Model);
            RobotModel.Add(Axis12Model);
            RobotModel.Add(Axis23Model);
            RobotModel.Add(Axis34Model);
            RobotModel.Add(Axis45Model);
            RobotModel.Add(Axis56Model);
            RobotModel.Add(Axis6Model);

            DA.SetDataList(0, RobotData);
            DA.SetDataList(1, RobotModel);
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
                return Properties.Resources.kr6r900;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("a6d125e8-7d9b-4952-a150-a92b1ffff817"); }
        }
    }
}