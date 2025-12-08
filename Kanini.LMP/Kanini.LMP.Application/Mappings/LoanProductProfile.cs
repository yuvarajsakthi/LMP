using AutoMapper;
using Kanini.LMP.Database.Entities.LoanProductEntities;
using Kanini.LMP.Database.EntitiesDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Application.Mappings
{
    public class LoanProductProfile : Profile
    {
        public LoanProductProfile()
        {
            CreateMap<LoanProductDto, LoanProduct>().ReverseMap();
        }
    }
}
