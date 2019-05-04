using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace EasyRobot
{
    public class EasyRobotInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "EasyRobot";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return null;
            }
        }
        public override string Description
        {
            get
            {
                //Return a short string describing the purpose of this GHA library.
                return "provide a easy method of robot control and simulation";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("f62e8179-8bf5-4335-a20b-c5bd95839003");
            }
        }

        public override string AuthorName
        {
            get
            {
                //Return a string identifying you or your company.
                return "Ce Li";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "marchlice@outlook.com";
            }
        }
    }
}
