# Part 2.2 - Computation Logic & Edge Cases (Bill/Collect - Pakyaw System)

## Computation Logic

### 1. Pakyaw Computation
```
For each daily output:
    Line Amount = Quantity × Piece Rate × Day Type Multiplier

Total Pakyaw = Σ (all Line Amounts)
Gross Payout = Total Pakyaw
```

### 2. Day Type Multipliers
| Day Type         | Multiplier |
|------------------|------------|
| Regular Day      | 1.0        |
| Special Holiday  | 1.3        |
| Legal Holiday    | 2.0        |

### 3. Piece Rates (from sample)
| SKU            | Piece Rate |
|----------------|------------|
| Sliced Banana  | 5          |
| Sliced Apple   | 7          |

### 4. Billable Amount Formula
```
Billable Amount = Gross Payout + SSS + Pagibig + Philhealth
```

---

## Sample Computation

Using the provided sample data:

| Date       | Day Type      | SKU           | Output | Rate | Multiplier | Line Amount |
|------------|---------------|---------------|--------|------|------------|-------------|
| Jan 12     | Legal Holiday | Sliced Banana | 150    | 5    | 2.0        | 1,500.00    |
| Jan 13     | Regular Day   | Sliced Apple  | 200    | 7    | 1.0        | 1,400.00    |
| Jan 14     | Regular Day   | Sliced Apple  | 100    | 7    | 1.0        |   700.00    |
| Jan 15     | Regular Day   | Sliced Banana | 40     | 5    | 1.0        |   200.00    |
| Jan 15     | Regular Day   | Sliced Apple  | 50     | 7    | 1.0        |   350.00    |
| Jan 16     | Regular Day   | Sliced Banana | 90     | 5    | 1.0        |   450.00    |
| Jan 17     | Regular Day   | Sliced Banana | 100    | 5    | 1.0        |   500.00    |
| Jan 18     | Regular Day   | Sliced Banana | 130    | 5    | 1.0        |   650.00    |

### Step-by-Step:

**Line-by-line calculation:**
1. Jan 12: 150 × 5 × 2.0 = **1,500.00**
2. Jan 13: 200 × 7 × 1.0 = **1,400.00**
3. Jan 14: 100 × 7 × 1.0 = **700.00**
4. Jan 15: 40 × 5 × 1.0 = **200.00**
5. Jan 15: 50 × 7 × 1.0 = **350.00**
6. Jan 16: 90 × 5 × 1.0 = **450.00**
7. Jan 17: 100 × 5 × 1.0 = **500.00**
8. Jan 18: 130 × 5 × 1.0 = **650.00**

**Total Pakyaw Earnings (Gross Payout):**
```
= 1,500 + 1,400 + 700 + 200 + 350 + 450 + 500 + 650
= 5,750.00
```

**Government Contributions (sample rates):**
```
SSS         = 500.00   (based on SSS contribution table)
Pagibig     = 200.00   (based on Pag-IBIG schedule)
Philhealth  = 250.00   (based on PhilHealth rate)
```

**Billable Amount:**
```
Billable Amount = Gross Payout + SSS + Pagibig + Philhealth
Billable Amount = 5,750.00 + 500.00 + 200.00 + 250.00
Billable Amount = 6,700.00
```

**The total Billable Amount to the client is ₱6,700.00**

---

## Edge Cases

### 1. Zero Output on a Given Day
- **Scenario:** Member worked on a day but produced 0 output.
- **Handling:** Line Amount = 0 × Rate × Multiplier = 0. Include the record for tracking but it contributes ₱0 to pakyaw.

### 2. Multiple SKUs on the Same Day
- **Scenario:** Member produces different products on the same day (e.g., Jan 15 has both Sliced Banana and Sliced Apple).
- **Handling:** Each output is a separate line item. Compute each independently and sum them all.

### 3. Unknown or New SKU
- **Scenario:** A new product is introduced with no defined piece rate.
- **Handling:** System should reject the output entry until a piece rate is defined for the SKU. Validate SKU existence before recording output.

### 4. Holiday Falls on Weekend
- **Scenario:** Legal holiday or special holiday falls on a non-working day.
- **Handling:** If member actually worked and produced output, apply the appropriate holiday multiplier. If no output was recorded, no computation needed.

### 5. Partial Collection
- **Scenario:** Client pays only a portion of the billable amount.
- **Handling:**
  - Record the partial collection amount.
  - Create a Credit Note for the remaining unpaid balance.
  - Billing status = "Partial".
  - System tracks outstanding balance for follow-up.

### 6. Full Non-Payment (Uncollectible)
- **Scenario:** Client does not pay at all.
- **Handling:**
  - Collection amount = 0.
  - Create a Credit Note for the full billable amount.
  - Billing status = "Partial" or "Uncollected".
  - Trigger escalation or write-off process.

### 7. Overpayment by Client
- **Scenario:** Client pays more than the billable amount.
- **Handling:** Record the full amount collected. The excess should be flagged and can be applied as advance payment to the next billing cycle or refunded.

### 8. Negative Output (Defective / Rejected)
- **Scenario:** Some output is rejected due to quality issues.
- **Handling:** System should allow adjustments. Rejected output should be deducted from the total quantity before computing pakyaw. Record the adjustment with reason.

### 9. Government Contribution Rate Changes
- **Scenario:** SSS, Pagibig, or PhilHealth rates change mid-period.
- **Handling:** Use the rates effective at the time of the billing period end date. Store rate tables with effective dates.

### 10. Duplicate Output Entry
- **Scenario:** Same member, same date, same SKU entered twice.
- **Handling:** System should validate and warn for potential duplicates. Allow override if intentional (e.g., different batches on the same day).
