using AutoMapper;
using Kanini.LMP.Database.Entities.LoanProductEntities;
using Kanini.LMP.Database.Entities.LoanProductEntities.CommonLoanProductEntities;
using Kanini.LMP.Database.EntitiesDto.LoanProductEntitiesDto;
using Kanini.LMP.Database.EntitiesDto.LoanProductEntitiesDto.CommonLoanProductEntiesDto;
using Kanini.LMP.Database.EntitiesDto.LoanProductEntitiesDto.CommonLoanProductEntiesDto.AddressInformation;
using Kanini.LMP.Database.EntitiesDto.LoanProductEntitiesDto.CommonLoanProductEntiesDto.PersonalDetails;
using Kanini.LMP.Database.EntitiesDto.LoanProductEntitiesDto.HomeLoanEntitiesDto;
using Kanini.LMP.Database.EntitiesDto.LoanProductEntitiesDto.PersonalLoanEntitiesDto;
using Kanini.LMP.Database.EntitiesDto.LoanProductEntitiesDto.VehicleLoanEntitiesDto;
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

            CreateMap<LoanDetailsDTO, LoanDetails>().ReverseMap();
            CreateMap<DeclarationDTO, Declaration>().ReverseMap();
            CreateMap<DocumentUploadDTO, DocumentUpload>()
                 .ForMember(d => d.DocumentData,
                            o => o.MapFrom(s => s.DocumentDataBase64 == null ? null : Convert.FromBase64String(s.DocumentDataBase64)));
            CreateMap<DocumentUpload, DocumentUploadDTO>()
                 .ForMember(d => d.DocumentDataBase64,
                            o => o.MapFrom(s => s.DocumentData == null ? null : Convert.ToBase64String(s.DocumentData)));

            CreateMap<FamilyEmergencyDetailsDTO, FamilyEmergencyDetails>().ReverseMap();

            CreateMap<PersonalDetailsCreateDTO, PersonalDetails>();
            CreateMap<PersonalDetailsDTO, PersonalDetails>().ReverseMap();
            CreateMap<PersonalDetailsUpdateDTO, PersonalDetails>();
            CreateMap<PersonalDetails, PersonalDetailsResponseDTO>();

            CreateMap<AddressInformationCreateDTO, AddressInformation>();
            CreateMap<AddressInformationDTO, AddressInformation>().ReverseMap();
            CreateMap<AddressInformationUpdateDTO, AddressInformation>();
            CreateMap<AddressInformation, AddressInformationResponseDTO>();

            CreateMap<LoanProductDTO, LoanProduct>().ReverseMap();
            CreateMap<LoanProductDto, LoanProduct>().ReverseMap();
        }
    }
}
