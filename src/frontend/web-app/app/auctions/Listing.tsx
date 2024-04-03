'use client'
import React, { useEffect, useState } from 'react'
import { json } from 'stream/consumers';
import AuctionCard from './AuctionCard';
import { Auction, PageResult } from '@/types';
import AddPagination from '../components/AddPagination';
import { getData } from '@/app/actions/AuctionActions';
import Filters from './Filters';
import { useParamsStore } from '@/hooks/userParamsStore';
import { shallow } from 'zustand/shallow';
import queryString from 'query-string';
import EmptyFilter from '../components/EmptyFilter';


export default function Listing() {
    const [data, setData] = useState<PageResult<Auction>>();
    const params = useParamsStore(state => ({
        pageNumber: state.pageNumber,
        pageSize: state.pageSize,
        searchTerm: state.searchTerm,
        orderBy: state.orderBy,
        filterBy: state.filterBy,
    }),shallow);
    const setParams = useParamsStore(state => state.setParams);
    const url = queryString.stringifyUrl({url: '', query:params});

    function setPageNumber(pageNumber: number){
        setParams({pageNumber});
    }

//    const [auctions, setAuctions] = useState<Auction[]>([]);
//    const [pageCount, setPageCount] = useState(0);
//    const [pageNumber, setPageNumber] = useState(1);
//    const [pageSize, setPageSize] = useState(4);

    useEffect(() => {
        getData(url).then(data => {
            setData(data);
        })
    }, [url])
    
    if(!data) return <h3>Loading...</h3>
    
    return (
        <>
            <Filters/>
                {data.totalCount  === 0 ? (
                    <EmptyFilter showReset/>
                ): (
                <>
                    <div className='grid grid-cols-4 gap-6'>
                    {data.results.map(auction => (
                        <AuctionCard auction={auction} key={auction.id}/>
                        ))} 
                    </div>
                    <div className='flex justify-center mt-4'>
                        <AddPagination 
                            currentPage={params.pageNumber}
                            pageCount={data.pageCount}
                            pageChanged={setPageNumber}/>
                    </div>
                </>
                )}

        </>
  )
}
