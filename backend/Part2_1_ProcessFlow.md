# Part 2.1 - Process Flow (HR / Timekeeping / Payroll - Back Pay)

## Step-by-Step: Member Separation and Clearance Process

### Phase 1: Initiate Separation
```
[Member Submits Resignation / HR Initiates Termination]
                        │
                        ▼
        [HR Creates Separation Record]
         - Record separation date
         - Record separation type
         - Compute years of service
         - Set clearance status = "Pending"
```

### Phase 2: Process Clearance
```
        [Clearance Process Begins]
                        │
            ┌───────────┼───────────┐
            ▼           ▼           ▼
     [Check Inventory] [Check Loans] [Check Revolving Fund]
      - List all items  - List all    - Check if member
        issued to         outstanding    has revolving
        member            loans          fund balance
      - Identify        - Sum up
        unreturned        remaining
        balances          balances
            │           │           │
            └───────────┼───────────┘
                        ▼
              [All Departments Clear]
               - Clearance status = "Approved"
```

### Phase 3: Compute Earnings
```
        [Compute Earnings]
                │
        ┌───────┼───────────────┐
        ▼       ▼               ▼
  [Separation  [Final         [Cooperative
     Pay]       Salary]        Share]
     │          │               │
     │  Based on last           │
     │  attendance              │
     │  rendered                │
     │          │               │
     └──────────┼───────────────┘
                ▼
        Total Earnings =
        Separation Pay + Final Salary + Cooperative Share
```

#### Separation Pay Computation (DOLE Labor Law):
- **Authorized Causes (Redundancy, Retrenchment, etc.):**
  - Separation Pay = Monthly Salary × Years of Service
  - Minimum: 1 month salary or 1/2 month per year of service (whichever is higher, depending on cause)
- **Just Causes (Employee fault):**
  - Generally no separation pay unless company policy states otherwise

#### Final Salary Computation:
- Based on actual days/hours worked in the last pay period
- Uses attendance records (TimeIn/TimeOut) to compute worked hours
- Applies daily rate × days worked (or hourly rate × hours worked)

#### Cooperative Share:
- Fixed amount recorded in member's cooperative account
- Returned in full to the member upon separation

### Phase 4: Compute Deductions
```
        [Compute Deductions]
                │
        ┌───────┼───────────────┐
        ▼       ▼               ▼
  [Inventory  [Loan           [Revolving
   Deduction]  Balance]        Fund]
     │          │               │
  Sum of all  Sum of all       Balance
  unreturned  outstanding      amount
  item values loan balances
     │          │               │
     └──────────┼───────────────┘
                ▼
        Total Deductions =
        Inventory Deduction + Loan Balance + Revolving Fund
```

### Phase 5: Compute Final Back Pay
```
        [Compute Back Pay]
                │
                ▼
        Net Back Pay = Total Earnings - Total Deductions
                │
                ▼
        [Record BackPay Entry]
         - Status = "Pending" (for approval)
                │
                ▼
        [Management Approves]
         - Status = "Approved"
                │
                ▼
        [Finance Releases Payment]
         - Status = "Released"
         - Record release date
```

## Summary Flow Diagram
```
Member Separation
       │
       ▼
Initiate Clearance ──► Check Inventory ──┐
                   ──► Check Loans ──────┤
                   ──► Check Rev. Fund ──┘
                                         │
                                         ▼
                              Compute Earnings
                              (Sep Pay + Final Salary + Coop Share)
                                         │
                                         ▼
                              Compute Deductions
                              (Inventory + Loans + Rev. Fund)
                                         │
                                         ▼
                              Net Back Pay = Earnings - Deductions
                                         │
                                         ▼
                              Approve ──► Release Payment
```
