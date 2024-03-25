using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Model;

namespace SearchService.Consumers;

public class AuctionCreatedConsumer : IConsumer<AuctionCreated>
{
    private readonly IMapper _mapper;

    public AuctionCreatedConsumer(IMapper mapper)
    {
        _mapper = mapper;
    }
    public async Task Consume(ConsumeContext<AuctionCreated> context)
    {
        Console.WriteLine("Consuming action required: " + context.Message.Id);

        var item = _mapper.Map<Item>(context.Message);

        if (item.Model == "Foo")
            throw new ArgumentException("Can't accept model name Foo");

        await item.SaveAsync();
    }
}
