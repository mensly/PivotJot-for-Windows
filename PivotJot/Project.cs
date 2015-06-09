using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PivotJot
{
    public class Project
    {
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
