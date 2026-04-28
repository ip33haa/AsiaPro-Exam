# Part 2.2 - Process Flow (Bill/Collect - Pakyaw System)

## Step-by-Step Process

### Phase 1: Record Daily Outputs
```
[Members Perform Work / Produce Output]
                    │
                    ▼
    [Record Daily Output per Member]
     - Date
     - SKU (product type)
     - Quantity produced
     - Day Type (Regular, Special Holiday, Legal Holiday)
```

### Phase 2: Compute Pakyaw Earnings
```
    [For Each Daily Output Record]
                    │
                    ▼
    [Get Piece Rate from SKU]
     - e.g., Sliced Banana = 5, Sliced Apple = 7
                    │
                    ▼
    [Get Day Type Multiplier]
     - Regular Day     = 1.0
     - Special Holiday = 1.3
     - Legal Holiday   = 2.0
                    │
                    ▼
    [Compute Line Amount]
     Line Amount = Quantity × Piece Rate × Day Type Multiplier
                    │
                    ▼
    [Sum All Line Amounts]
     Total Pakyaw Earnings = Σ (Line Amounts)
                    │
                    ▼
    Gross Payout = Total Pakyaw Earnings
```

### Phase 3: Compute Government Contributions
```
    [From Gross Payout, Compute:]
            │
    ┌───────┼───────────┐
    ▼       ▼           ▼
  [SSS]  [Pagibig]  [Philhealth]
    │       │           │
    └───────┼───────────┘
            ▼
    Total Gov't Contributions = SSS + Pagibig + Philhealth
```

### Phase 4: Generate Billing
```
    [Compute Billable Amount]
     Billable Amount = Gross Payout + SSS + Pagibig + Philhealth
                    │
                    ▼
    [Create Billing Record]
     - Client info
     - Billing period (date range)
     - Gross Payout
     - Gov't contributions breakdown
     - Total Billable Amount
     - Status = "Pending"
                    │
                    ▼
    [Send Billing to Client]
     - Status = "Sent"
```

### Phase 5: Collection Process
```
    [Client Receives Billing]
                    │
                    ▼
    [Client Makes Payment]
                    │
            ┌───────┴───────┐
            ▼               ▼
    [Full Payment]    [Partial / No Payment]
      │                     │
      ▼                     ▼
    [Record Collection]   [Record Partial Collection]
     - Amount = Billable    - Amount < Billable
     - Status = "Collected"   │
                              ▼
                    [Create Credit Note]
                     - Amount = Billable - Collected
                     - Reason (e.g., dispute, uncollectible)
                     - Billing Status = "Partial"
                              │
                              ▼
                    [Follow Up / Write Off]
                     - If resolved → Apply credit note, close
                     - If uncollectible → Write off
```

## Summary Flow Diagram
```
Record Daily Outputs (per member, per SKU, per day)
            │
            ▼
Compute Pakyaw Earnings
(Quantity × Piece Rate × Day Type Multiplier for each output)
            │
            ▼
Gross Payout = Sum of all Pakyaw Earnings
            │
            ▼
Compute Gov't Contributions (SSS + Pagibig + Philhealth)
            │
            ▼
Billable Amount = Gross Payout + Gov't Contributions
            │
            ▼
Generate Billing ──► Send to Client
            │
            ▼
Collection ──► Full? ──► Close Transaction
            │
            └──► Partial/None? ──► Create Credit Note ──► Follow Up
```
