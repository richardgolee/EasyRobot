using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace EasyRobot
{
    public class EasyRobotTargetTool : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the EasyRobotTargetTool class.
        /// </summary>
        public EasyRobotTargetTool()
          : base("FreeTool", "TarT",
              "EasyRobotTargetTool",
              "EasyRobot", "Tool")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPlaneParameter("Toolplane", "Tp", "Toolplane", GH_ParamAccess.item, Plane.WorldXY);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTransformParameter("ToolTransform", "Tt", "ToolTransform", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Plane origin = Plane.WorldXY;
            Plane Target = Plane.WorldXY;
            if (!DA.GetData(0, ref Target)) return;

            Transform TargetTransform = Transform.ChangeBasis(Target, origin);
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
            get { return new Guid("eff042fe-6966-4527-84d8-d3ca478ece45"); }
        }
    }
}