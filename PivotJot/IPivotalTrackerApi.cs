using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PivotJot
{
    public interface IPivotalTrackerApi
    {
        [Get("/services/v5/projects")]
        Task<List<Project>> GetProjects([Header("X-TrackerToken")] string token);

        [Post("/services/v5/projects/{id}/stories")]
        Task PostStory([Header("X-TrackerToken")] string token, [AliasAs("id")] int projectId, [Body] Story story);

        [Get("/services/v5/me")]
        Task<User> Authorize([Header("Authorization")] string authorization);
    }
}
