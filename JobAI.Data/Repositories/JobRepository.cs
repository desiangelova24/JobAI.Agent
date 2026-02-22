
using JobAI.Core.Interfaces;
using JobAI.Core.Models;
using JobAI.Data.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobAI.Data.Repositories
{
    public class JobRepository : IJobRepository
    {
        private readonly AppDbContext _context;

        public JobRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<RemoteJob>> GetAllAsync() =>
            await _context.RemoteJobs.OrderByDescending(j => j.DateSaved).ToListAsync();

        public async Task AddAsync(RemoteJob job)
        {
            await _context.RemoteJobs.AddAsync(job);
            await _context.SaveChangesAsync();
        }
        public async Task<List<RemoteJob>> GetAllJobsAsync() => await _context.RemoteJobs.OrderByDescending(j => j.DateSaved).ToListAsync();    
        public async Task<bool> ExistsAsync(string externalId) =>
            await _context.RemoteJobs.AnyAsync(j => j.ExternalId == externalId);

        public IQueryable<RemoteJob> GetQueryable()
        {
            return _context.RemoteJobs.AsQueryable();
        }
    }
}
