using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace EasyRobot
{
    public class EasyRobotTool : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the EasyRobotTool class.
        /// </summary>
        public EasyRobotTool()
          : base("Tool", "ERT",
              "EasyRobotTool",
              "EasyRobot", "EasyRobotTool")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Toolx", "Tx", "Toolx", GH_ParamAccess.item, 230);
            pManager.AddNumberParameter("Tooly", "Ty", "Toolx", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("Toolz", "Tz", "Toolx", GH_ParamAccess.item, 20);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("ToolData", "TD", "ToolData", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double Tx = 0;
            double Ty = 0;
            double Tz = 0;
            List<double> ToolData = new List<double>();

            if (!DA.GetData(0, ref Tx)) return;
            if (!DA.GetData(1, ref Ty)) return;
            if (!DA.GetData(2, ref Tz)) return;

            ToolData.Add(Tx);
            ToolData.Add(Ty);
            ToolData.Add(Tz);

            DA.SetDataList(0, ToolData);

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
            get { return new Guid("b35e98f2-4542-4df6-95cf-af1c799ad3f3"); }
        }
    }
}