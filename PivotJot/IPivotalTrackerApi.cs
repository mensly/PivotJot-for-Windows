using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PivotJot
{
    interface IPivotalTrackerApi
    {
        [Get("/services/v5/projects")]
        Task<List<Project>> GetProjects([Header("X-TrackerToken")] string token);
    }
}
