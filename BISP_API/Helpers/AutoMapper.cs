//using AutoMapper;
//using BISP_API.Models.Dto;

//public class AutoMapperProfile : Profile
//{
//    public AutoMapperProfile()
//    {
//        CreateMap<MessageDto, BISP_API.Models.Message>()
//            .ForMember(dest => dest.SenderId, opt => opt.MapFrom(src => src.SenderId))
//            .ForMember(dest => dest.SenderImage, opt => opt.MapFrom(src => src.SenderImage))
//            .ForMember(dest => dest.ReceiverId, opt => opt.MapFrom(src => src.ReceiverId))
//            .ForMember(dest => dest.MessageText, opt => opt.MapFrom(src => src.MessageText))
//            .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => DateTime.Now));
//    }
//}


