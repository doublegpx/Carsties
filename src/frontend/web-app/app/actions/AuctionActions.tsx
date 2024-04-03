'use server'
import { Auction, PageResult } from "@/types";

export async function getData(query:string): Promise<PageResult<Auction>>{

    const result = await fetch(`http://localhost:6001/search${query}`);
    
    if(!result.ok) throw Error("fail to fetch data");
  
    return result.json();
  }