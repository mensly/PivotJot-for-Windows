using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PivotJot
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Project
    {
        public static readonly Project PLACEHOLDER_LOGOUT = new Project(-1) { Name = "Logout" };
        public static readonly Project PLACEHOLDER_EMPTY = new Project(-1) { Name = "No projects" };

        [JsonProperty("id")]
        public int ProjectId { get; private set; }
        [JsonProperty("name")]
        public String Name { get; set; }
        public Project(int id)
        {
            ProjectId = id;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
