using AutoMapper;
using Plant_BiologyEducation.Entity.DTO.Book;
using Plant_BiologyEducation.Entity.DTO.Chapter;
using Plant_BiologyEducation.Entity.DTO.Lesson;
using Plant_BiologyEducation.Entity.DTO.Management;
using Plant_BiologyEducation.Entity.DTO.User;
using Plant_BiologyEducation.Entity.Model;

namespace Plant_BiologyEducation.Mapper
{
    public class MappingFile : Profile
    {
        public MappingFile()
        {
            // User mappings
            CreateMap<User, UserDTO>();
            CreateMap<UserRequestDTO, User>()
                .ForMember(dest => dest.User_Id, opt => opt.Ignore());

            // Lesson mappings
            CreateMap<Lesson, LessonDTO>();
            CreateMap<LessonDTO, Lesson>();
            CreateMap<LessonRequestDTO, Lesson>()
                .ForMember(dest => dest.Lesson_Id, opt => opt.Ignore())
                .ForMember(dest => dest.Chapter_Id, opt => opt.Ignore());
            CreateMap<Lesson, LessonRequestDTO>();

            // Chapter mappings
            CreateMap<Chapter, ChapterDTO>()
                .ForMember(dest => dest.Book_Id, opt => opt.MapFrom(src => src.Book.Book_Id))
                .ForMember(dest => dest.Lessons, opt => opt.MapFrom(src => src.Lessons));
            CreateMap<ChapterDTO, Chapter>();
            CreateMap<ChapterRequestDTO, Chapter>()
                .ForMember(dest => dest.Chapter_Id, opt => opt.Ignore());

            // Book mappings
            CreateMap<Book, BookDTO>();
            CreateMap<BookDTO, Book>();
            CreateMap<BookRequestDTO, Book>()
                .ForMember(dest => dest.Book_Id, opt => opt.Ignore());

            // ManageBook mappings
            CreateMap<ManageBook, ManageBookRequestDTO>().ReverseMap();
            CreateMap<ManageBook, ManageBookDTO>()
                .ForMember(dest => dest.User_Id, opt => opt.MapFrom(src => src.User_Id))
                .ForMember(dest => dest.Book_Id, opt => opt.MapFrom(src => src.Book_Id))
                .ForMember(dest => dest.UpdatedDate, opt => opt.MapFrom(src => src.UpdatedDate));
            CreateMap<ManageBookDTO, ManageBook>()
                .ForMember(dest => dest.User_Id, opt => opt.MapFrom(src => src.User_Id))
                .ForMember(dest => dest.Book_Id, opt => opt.MapFrom(src => src.Book_Id))
                .ForMember(dest => dest.UpdatedDate, opt => opt.MapFrom(src => src.UpdatedDate))
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Book, opt => opt.Ignore());

            // ManageLesson mappings
            CreateMap<ManageLesson, ManageLessonDTO>()
                .ForMember(dest => dest.User_Id, opt => opt.MapFrom(src => src.User_Id))
                .ForMember(dest => dest.Lesson_Id, opt => opt.MapFrom(src => src.Lesson_Id))
                .ForMember(dest => dest.UpdatedDate, opt => opt.MapFrom(src => src.UpdatedDate));
            CreateMap<ManageLessonDTO, ManageLesson>()
                .ForMember(dest => dest.User_Id, opt => opt.MapFrom(src => src.User_Id))
                .ForMember(dest => dest.Lesson_Id, opt => opt.MapFrom(src => src.Lesson_Id))
                .ForMember(dest => dest.UpdatedDate, opt => opt.MapFrom(src => src.UpdatedDate))
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Lesson, opt => opt.Ignore());

            // ManageChapter mappings
            CreateMap<ManageChapter, ManageChapterDTO>()
                .ForMember(dest => dest.User_Id, opt => opt.MapFrom(src => src.User_Id))
                .ForMember(dest => dest.Chapter_Id, opt => opt.MapFrom(src => src.Chapter_Id))
                .ForMember(dest => dest.UpdatedDate, opt => opt.MapFrom(src => src.UpdatedDate));
            CreateMap<ManageChapterDTO, ManageChapter>()
                .ForMember(dest => dest.User_Id, opt => opt.MapFrom(src => src.User_Id))
                .ForMember(dest => dest.Chapter_Id, opt => opt.MapFrom(src => src.Chapter_Id))
                .ForMember(dest => dest.UpdatedDate, opt => opt.MapFrom(src => src.UpdatedDate))
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Chapter, opt => opt.Ignore());
        }
    }
}
