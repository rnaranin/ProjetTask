using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using TaskManagerApi.DTOs;
using TaskManagerApi.Models;

namespace TaskManagerApi.Mappings
{
    public class TaskProfile : Profile
    {
        public TaskProfile()
        {
           CreateMap<TaskItem, TaskItemDto>()
                .ForMember(dest => dest.IsLate, opt =>
                    opt.MapFrom(src => !src.IsDone && src.DueDate < DateTime.Now));                         
            CreateMap<TaskItemCreateDto,TaskItem>();
            CreateMap<TaskItemUpdateDto,TaskItem>();      
        }
    }
}