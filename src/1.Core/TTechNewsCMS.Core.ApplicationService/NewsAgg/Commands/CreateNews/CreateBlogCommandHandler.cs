using NewCms.Core.Contracts.NewsAgg.Commands.CreateBlog;
using NewCms.Core.Contracts.NewsAgg.DAL;
using TTechNewsCMS.Core.Domain.NewsAgg.Entities;
using Zamin.Core.ApplicationServices.Commands;
using Zamin.Core.Contracts.ApplicationServices.Commands;
using Zamin.Core.Domain.ValueObjects;
using Zamin.Core.RequestResponse.Commands;
using Zamin.Utilities;

namespace NewCms.Core.ApplicationService.NewsAgg.Commands.CreateNews
{
    public class CreateBlogCommandHandler : CommandHandler<CreateNewsCommand>
    {

        private readonly INewsCommandRepository _newsCommandRepository;

        public CreateBlogCommandHandler(ZaminServices zaminServices, INewsCommandRepository blogCommandRepository) : base(zaminServices)
        {
            _newsCommandRepository = blogCommandRepository;
        }

        public override async Task<CommandResult> Handle(CreateNewsCommand request)
        {
            News news = new(request.BusunessId, request.Title, request.Description, request.Body,
                request.KeywordsId.Select(c => new NewsKeyword
                {
                    KeywordBusinessId = BusinessId.FromGuid(c)
                }).ToList());
            _newsCommandRepository.Insert(news);
            try
            {
                await _newsCommandRepository.CommitAsync();

            }
            catch (Exception ex)
            {

                throw;
            }
            return await OkAsync();

        }
    }
}
