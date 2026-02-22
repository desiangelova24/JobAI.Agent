
using AutoMapper;
using JobAI.Core.DTOs;
using JobAI.Core.Interfaces;
using JobAI.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace JobAI.Core.Services
{
    public class JobService
    {
        private readonly IMapper _mapper;
        private readonly IJobRepository _repo;

        public JobService(IJobRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task ProcessAndSaveJob(RemoteJob job)
        {
            if (await _repo.ExistsAsync(job.ExternalId))
                return;
            await _repo.AddAsync(job);
        }
        public async Task<List<RemoteJob>> GetAllJobsAsync()
        {
            return await _repo.GetAllJobsAsync();
        }
        public async Task<List<JobDto>> GetJobsAsync(int? limit = null, bool onlyHighMatch = false)
        {
            var query = _repo.GetQueryable();
            if (onlyHighMatch)
            {
                //query = query.Where(j => j.WorkMode > 80);
            }

            if (limit.HasValue)
            {
                query = query.Take(limit.Value);
            }

            var jobs = await query.ToListAsync();
            return _mapper.Map<List<JobDto>>(jobs);
        }
        public async Task<bool> JobExistsAsync(string externalId)
        {
            return await _repo.ExistsAsync(externalId);
        }
    }
}
