export class BidDetail {
  constructor(
    public bidId: number,
    public auctionId: number,
    public bidAmount: number,
    public userId: string,
    public bidDate: Date,
    public userName: string
  ) {}
}
