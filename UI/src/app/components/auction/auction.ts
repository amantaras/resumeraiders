export class Auction {
  constructor(
    public idAuction: number,
    public name: string,
    public description: string,
    public startingPrice: number,
    public bidPrice: number,
    public auctionDate: Date,
    public status: string,
    public activeInHours: number,
    public userId: String,
    public isActive: boolean,
    public image: String,
    public userName: String,
    public bidUser: String,
    public isPaymentMade: boolean
  ) {}
}
