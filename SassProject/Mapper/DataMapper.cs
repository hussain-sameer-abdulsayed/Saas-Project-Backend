using AutoMapper;
using SassProject.Dtos.CategoryDto;
using SassProject.Dtos.ProductRepo;
using SassProject.Models;

namespace SassProject.SassMapper
{
    public class DataMapper : Profile
    {
        public DataMapper()
        {
            CreateMap<Category,CreateCategoryDto>().ReverseMap();
            CreateMap<Category,UpdateCategoryDto>().ReverseMap();


            CreateMap<Product, CreateProductDto>().ReverseMap();
            CreateMap<Product, UpdateProductDto>().ReverseMap();


        }
    }
}
