using AutoMapper;
using Kanini.LMP.Database.Entities;
using Kanini.LMP.Database.EntitiesDto.PaymentTransaction;

namespace Kanini.LMP.Application.Mappings
{
    public class PaymentTransactionProfile : Profile
    {
        public PaymentTransactionProfile()
        {

            CreateMap<PaymentTransactionCreateDTO, PaymentTransaction>();
            CreateMap<PaymentTransactionDTO, PaymentTransaction>().ReverseMap();
            CreateMap<PaymentTransaction, PaymentTransactionResponseDTO>();
            //CreateMap<PaymentHistoryDTO, PaymentHistory>().ReverseMap();
            //CreateMap<PaymentSummaryDTO, PaymentSummary>().ReverseMap();
            //CreateMap<LoanAccountSummaryDTO, LoanAccountSummary>().ReverseMap();
        }
    }
}
