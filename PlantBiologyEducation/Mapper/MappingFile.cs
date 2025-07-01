using AutoMapper;
using Plant_BiologyEducation.Entity.DTO.AccessHistories;
using Plant_BiologyEducation.Entity.DTO.Book;
using Plant_BiologyEducation.Entity.DTO.Chapter;
using Plant_BiologyEducation.Entity.DTO.Lesson;
using Plant_BiologyEducation.Entity.DTO.P_B_A;
using Plant_BiologyEducation.Entity.DTO.User;
using Plant_BiologyEducation.Entity.Model;
using PlantBiologyEducation.Entity.DTO.User;
using PlantBiologyEducation.Entity.Model;

namespace Plant_BiologyEducation.Mapper
{
    public class MappingFile : Profile
    {
        public MappingFile()
        {
            // User mappings
            CreateMap<User, UserDTO>();
            CreateMap<AdminRequestDTO, User>()
                .ForMember(dest => dest.User_Id, opt => opt.Ignore());
            CreateMap<UserRequestDTO, User>()
                .ForMember(dest => dest.User_Id, opt => opt.Ignore());

            // Lesson mappings
            CreateMap<LessonDTO, Lesson>();
            CreateMap<LessonRequestDTO, Lesson>()
                .ForMember(dest => dest.Lesson_Id, opt => opt.Ignore())
                .ForMember(dest => dest.Chapter_Id, opt => opt.Ignore());
            CreateMap<Lesson, LessonRequestDTO>();
            // Lesson ↔ LessonDTO mapping
            CreateMap<Lesson, LessonDTO>()
                .ForMember(dest => dest.Plant_Biology_Animal, opt => opt.MapFrom(src => src.RelatedSpecies));

            // Chapter mappings
            CreateMap<Chapter, ChapterDTO>()
                .ForMember(dest => dest.Book_Id, opt => opt.MapFrom(src => src.Book.Book_Id))
                .ForMember(dest => dest.Lessons, opt => opt.MapFrom(src => src.Lessons));
            CreateMap<ChapterDTO, Chapter>();
            CreateMap<ChapterRequestDTO, Chapter>()
                .ForMember(dest => dest.Chapter_Id, opt => opt.Ignore());

            // Book mappings
            CreateMap<Book, BookDTO>()
                .ForMember(dest => dest.Chapters, opt => opt.MapFrom(src => src.Chapters));

            CreateMap<BookDTO, Book>();
            CreateMap<BookRequestDTO, Book>()
                .ForMember(dest => dest.Book_Id, opt => opt.Ignore());

            // PBA (Plant_Biology_Animals) mappings
            CreateMap<Plant_Biology_Animals, P_B_A_DTO>()
                .ForMember(dest => dest.Lesson_Id, opt => opt.MapFrom(src => src.LessonId));

            CreateMap<P_B_A_RequestDTO, Plant_Biology_Animals>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.LessonId, opt => opt.MapFrom(src => src.Lesson_Id));

           ;

            CreateMap<AccessBook, AccessBookDTO>().ReverseMap();


        }
    }
}
