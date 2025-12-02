using AutoMapper;
using Kanini.LMP.Database.Entities.LoanProductEntities;
using Kanini.LMP.Database.Entities.LoanProductEntities.CommonLoanProductEntities;
using Kanini.LMP.Database.Entities.LoanProductEntities.HomeLoanEntities;
using Kanini.LMP.Database.Entities.LoanProductEntities.PersonalLoanEntities;
using Kanini.LMP.Database.Entities.LoanProductEntities.VehicleLoanEntities;
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

            #region 10. Common Loan Product nested DTOs
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

            CreateMap<EmploymentDetailsDTO, EmploymentDetails>().ReverseMap();
            CreateMap<FinancialInformationDTO, FinancialInformation>().ReverseMap();
            #endregion

            #region 11. Home-loan specific nested DTOs
            CreateMap<BuilderInformationDTO, BuilderInformation>().ReverseMap();
            CreateMap<HomeLoanDetailsDTO, HomeLoanDetails>().ReverseMap();
            CreateMap<PropertyDetailsDTO, PropertyDetails>().ReverseMap();
            #endregion

            #region 12. Vehicle-loan specific nested DTOs
            CreateMap<DealerInformationDTO, DealerInformation>().ReverseMap();
            CreateMap<VehicleInformationDTO, VehicleInformation>().ReverseMap();
            CreateMap<VehicleLoanDetailsDTO, VehicleLoanDetails>().ReverseMap();
            #endregion

            #region 13. Loan Product master
            CreateMap<LoanProductDTO, LoanProduct>().ReverseMap();
            CreateMap<LoanProductDto, LoanProduct>()
                .ForMember(d => d.LoanProductName, o => o.MapFrom(s => s.LoanProductName))
                .ForMember(d => d.LoanProductDescription, o => o.MapFrom(s => s.LoanProductDescription))
                .ForMember(d => d.IsActive, o => o.MapFrom(s => s.IsActive))
                .ReverseMap();
            #endregion
        }
    }
}
