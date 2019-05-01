using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace EasyRobot
{
    public class EasyRobotConstructor : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public EasyRobotConstructor()
          : base("Robot", "ERC",
              "EasyRobotConstructor",
              "EasyRobot", "EasyRobotConstructor")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Axis2PointHorizontalOffset", "A2Z", "Axis2PointHorizontalOffset", GH_ParamAccess.item,400);
            pManager.AddNumberParameter("Axis2PointVerticalOffset", "A2X", "Axis2PointVerticalOffset", GH_ParamAccess.item,25);
            pManager.AddNumberParameter("DistanceBetweenAxis23", "D23", "DistanceBetweenAxis2AndAxis3", GH_ParamAccess.item,455);
            pManager.AddNumberParameter("DistanceBetweenAxis35", "D35", "DistanceBetweenAxis3AndAxis5", GH_ParamAccess.item,421.46);
            pManager.AddNumberParameter("DistanceBetweenAxis56", "D56", "DistanceBetweenAxis5AndAxis6", GH_ParamAccess.item,80);
            pManager.AddNumberParameter("DeltaAngle", "DA", "DeltaAngleOfAxis35", GH_ParamAccess.item,4.76);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("RobotData", "RD", "RobotData", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double a2z = 0;
            double a2x = 0;
            double d23 = 0;
            double d35 = 0;
            double d56 = 0;
            double da = 0;
            List<double> RobotData= new List<double>();

            if (!DA.GetData(0, ref a2z)) return;
            if (!DA.GetData(1, ref a2x)) return;
            if (!DA.GetData(2, ref d23)) return;
            if (!DA.GetData(3, ref d35)) return;
            if (!DA.GetData(4, ref d56)) return;
            if (!DA.GetData(5, ref da)) return;

            RobotData.Add(a2z);
            RobotData.Add(a2x);
            RobotData.Add(d23);
            RobotData.Add(d35);
            RobotData.Add(d56);
            RobotData.Add(da);

            DA.SetDataList(0, RobotData);

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
            get { return new Guid("0cae49ea-94a2-4d25-b07d-5946766fb87c"); }
        }
    }
}