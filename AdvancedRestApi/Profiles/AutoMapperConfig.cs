using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace AdvancedRestApi.Profiles
{
    using AdvancedRestApi.DTOs;
    using AdvancedRestApi.Models;

    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<User, UserDTO>().ReverseMap();
        }
    }
}