using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PivotJot
{
    public class Project
    {
        public static readonly Project PLACEHOLDER_LOGOUT = new Project(-1) { Name = "Logout" };
        public static readonly Project PLACEHOLDER_EMPTY = new Project(-1) { Name = "No projects" };
        public int ProjectId { get; private set; }
        public String Name { get; set; }
        public List<String> Tags { get; set; }
        public Project(int id)
        {
            ProjectId = id;
            Tags = new List<string>();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
