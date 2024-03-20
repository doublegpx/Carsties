﻿using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controller;

[ApiController]
[Route("api/auctions")]
public class AuctionController : ControllerBase
{
    private readonly AuctionDbContext _context;
    private readonly IMapper _mapper;

    public AuctionController(AuctionDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<AuctionDto>>> GetAllAcutions()
    {
        var auctions = await _context.Auctions
                            .Include(x => x.Item)
                            .OrderBy(x => x.Item.Make)
                            .ToListAsync();
        return _mapper.Map<List<AuctionDto>>(auctions);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuctionDto>> GetAcutionById(Guid id)
    {
        var auction = await _context.Auctions
                    .Include(x => x.Item)
                    .FirstOrDefaultAsync(x => x.Id == id);
        if (auction == null)
            return NotFound();

        return _mapper.Map<AuctionDto>(auction);

    }

    [HttpPost]
    public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto auctionDto)
    {
        var auction = _mapper.Map<Auction>(auctionDto);

        auction.Seller = "tester";

        _context.Auctions.Add(auction);

        var result = await _context.SaveChangesAsync() > 0;

        if (!result)
            return BadRequest("Could not save changes to DB");

        return CreatedAtAction(nameof(GetAcutionById), new { auction.Id }, _mapper.Map<AuctionDto>(auction));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAcution(Guid id, UpdateAuctionDto updateAuctionDto)
    {
        var auction = await _context.Auctions.Include(x => x.Item)
                            .FirstOrDefaultAsync(x => x.Id == id);
        if (auction == null)
            return NotFound();

        auction.Item.Make = updateAuctionDto.Make ?? auction.Item.Make;
        auction.Item.Model = updateAuctionDto.Model ?? auction.Item.Model;
        auction.Item.Color = updateAuctionDto.Color ?? auction.Item.Color;
        auction.Item.Mileage = updateAuctionDto.Mileage ?? auction.Item.Mileage;
        auction.Item.Year = updateAuctionDto.Year ?? auction.Item.Year;

        var result = await _context.SaveChangesAsync() > 0;

        if (!result)
            return BadRequest("Unable to Update data");

        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAuction(Guid id)
    {
        var auction = await _context.Auctions.FindAsync(id);

        if (auction == null)
            return NotFound();

        _context.Auctions.Remove(auction);

        var result = await _context.SaveChangesAsync() > 0;

        if (!result)
            return BadRequest("Unable to remove auction");

        return Ok();
    }
}
