using JobAI.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobAI.Core.Interfaces
{
    public interface IJobRepository
    {
        IQueryable<RemoteJob> GetQueryable();
        Task<List<RemoteJob>> GetAllAsync();
        Task AddAsync(RemoteJob job);
        Task<bool> ExistsAsync(string externalId);
        Task<List<RemoteJob>> GetAllJobsAsync();
    }
}
