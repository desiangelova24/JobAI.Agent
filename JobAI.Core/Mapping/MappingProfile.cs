using AutoMapper;
using JobAI.Core.DTOs;
using JobAI.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobAI.Core.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RemoteJob, JobDto>();
        }
    }
}
