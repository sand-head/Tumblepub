using AutoMapper;
using Tumblepub.Application.Aggregates;
using Tumblepub.Models;

namespace Tumblepub;

public class GraphQLProfile : Profile
{
    public GraphQLProfile()
    {
        CreateMap<Blog, BlogDto>();
        CreateMap<User, UserDto>();
        CreateMap<Post, PostDto>()
            .ForMember(dest => dest.Content, 
                opt => opt.MapFrom(src => GetStringFromPostContent(src.Content)));
    }

    private static string GetStringFromPostContent(PostContent content)
    {
        return content is PostContent.Internal markdown ? markdown.Content : string.Empty;
    }
}