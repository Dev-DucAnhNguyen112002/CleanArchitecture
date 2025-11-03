//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using ISAT.Domain.Entities;

//namespace ISAT.Application.Features.Users.Models;
//public class FileDto
//{
//    public Guid Id { get; set; }
//    public string Name { get; set; } = null!;
//    public string OriginalName { get; set; } = null!;
//    public string? FileExtension { get; set; }
//    public string? MimeType { get; set; }
//    public long FileSize { get; set; }
//    public string StoragePath { get; set; } = null!;
//}
//public record FolderWithFilesDto
//{
//    public Guid Id { get; set; }
//    public string Name { get; set; } = null!;
//    public string? Description { get; set; }
//    public string Path { get; set; } = null!;
//    public bool IsRoot { get; set; }
//    public List<FileDto> Files { get; set; } = new();
//}
//public class FolderProfile : Profile
//{
//    public FolderProfile()
//    {
//        CreateMap<Folder, FolderWithFilesDto>()
//            .ForMember(dest => dest.Files,
//                       opt => opt.MapFrom(src => src.Files.Where(f => !f.IsDeleted)));

//        CreateMap<File, FileDto>();
//    }
//}
