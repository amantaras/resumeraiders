export class Job {
  constructor(
    public JobID: Number,
    public JobTitle: String,
    public Description: String,
    public Location: Number,
    public Salary: Number,
    public CompanyID: Number,
    public CategoryID: Number,
    public ContractType: String,
    public Benefits: String,
    public Tags: String
  ) {}
}
