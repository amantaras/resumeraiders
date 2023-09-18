export class AuctionPayment {
    constructor(
        public idAuction: number,
        public name: string,
        public bidUser: string,
        public creditCardNo: string,
        public month: number,
        public year: number
      ) {  }
}
