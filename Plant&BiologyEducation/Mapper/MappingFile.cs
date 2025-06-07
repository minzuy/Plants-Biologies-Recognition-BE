using AutoMapper;
using Plant_BiologyEducation.Entity.DTO;
using Plant_BiologyEducation.Entity.Model;

namespace Plant_BiologyEducation.Mapper
{
    public class MappingFile : Profile
    {
        public MappingFile()
        {
            CreateMap<User, UserDTO>()
                .ForMember(dest => dest.Takings, opt => opt.MapFrom(src => src.Takings));

            // UserRequestDTO to User Entity
            CreateMap<UserRequestDTO, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // ID sẽ được set riêng
                .ForMember(dest => dest.CreatedTests, opt => opt.Ignore()) // Không cập nhật relationships qua DTO
                .ForMember(dest => dest.Takings, opt => opt.Ignore()); // Không cập nhật relationships qua DTO

            CreateMap<Test, TestDTO>()
                .ForMember(dest => dest.CreatorName, opt => opt.MapFrom(src => src.Creator.FullName))
                .ForMember(dest => dest.QuestionCount, opt => opt.MapFrom(src => src.Questions.Count))
                .ForMember(dest => dest.TakingCount, opt => opt.MapFrom(src => src.Takings.Count));

            // TestRequestDTO to Test Entity
            CreateMap<TestRequestDTO, Test>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // QUAN TRỌNG: Bỏ qua Id để EF tự tạo
                .ForMember(dest => dest.Creator, opt => opt.Ignore()) // Navigation property sẽ được EF tự load
                .ForMember(dest => dest.Questions, opt => opt.Ignore()) // Relationships
                .ForMember(dest => dest.Takings, opt => opt.Ignore()); // Relationships

            CreateMap<Question, QuestionDTO>()
                           .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                           .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                           .ForMember(dest => dest.Answer, opt => opt.MapFrom(src => src.Answer))
                           .ForMember(dest => dest.TestId, opt => opt.MapFrom(src => src.TestId));

            CreateMap<QuestionDTO, Question>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // ID sẽ được xử lý riêng
                .ForMember(dest => dest.Test, opt => opt.Ignore()); // Navigation property
        }
    }
}