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
        public static readonly Project PLACEHOLDER_LOGOUT = new Project("Logout");
        public static readonly Project PLACEHOLDER_EMPTY = new Project("No projects");
        public static readonly Project PLACEHOLDER_ERROR = new Project("Error loading list, click to retry");
        private const int PLACEHOLDER_ID = int.MinValue;

        [JsonProperty("id")]
        public int ProjectId { get; private set; }
        [JsonProperty("name")]
        public String Name { get; set; }
        public bool IsPlaceholder { get { return ProjectId == PLACEHOLDER_ID; } }

        private Project(string placeholderName)
        {
            ProjectId = PLACEHOLDER_ID;
            Name = placeholderName;
        }

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
