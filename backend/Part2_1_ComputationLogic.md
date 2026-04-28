# Part 2.1 - Computation Logic & Edge Cases (HR / Back Pay)

## Computation Logic

### Formula
```
Net Back Pay = Total Earnings - Total Deductions
```

### Earnings Breakdown
| Component        | Description                                    |
|------------------|------------------------------------------------|
| Separation Pay   | Based on DOLE labor law (monthly salary × years of service) |
| Final Salary     | Pro-rated salary based on last attendance rendered |
| Cooperative Share | Member's cooperative contribution to be returned |

```
Total Earnings = Separation Pay + Final Salary + Cooperative Share
```

### Deductions Breakdown
| Component            | Description                              |
|----------------------|------------------------------------------|
| Inventory Deduction  | Total value of unreturned company items   |
| Loan Balance         | Sum of all outstanding loan balances      |
| Revolving Fund       | Remaining revolving fund balance (if any) |

```
Total Deductions = Inventory Deduction + Loan Balance + Revolving Fund
```

---

## Sample Computation

Using the provided sample values:

### Earnings
| Component        | Amount      |
|------------------|-------------|
| Separation Pay   | 50,000.00   |
| Final Salary     | 10,000.00   |
| Cooperative Share |  5,000.00   |
| **Total Earnings** | **65,000.00** |

### Deductions
| Component            | Amount      |
|----------------------|-------------|
| Inventory Deduction  |  3,000.00   |
| Loan Balance         |  8,000.00   |
| Revolving Fund       |  2,000.00   |
| **Total Deductions** | **13,000.00** |

### Net Back Pay
```
Net Back Pay = Total Earnings - Total Deductions
Net Back Pay = 65,000.00 - 13,000.00
Net Back Pay = 52,000.00
```

**The member will receive ₱52,000.00 as final back pay.**

---

## Edge Cases

### 1. Deductions Exceed Earnings
- **Scenario:** Total deductions are greater than total earnings.
- **Handling:** Net Back Pay becomes negative. The member owes the company.
- **Action:** System should flag this and generate a "Balance Due" notice instead of a payment. The member must settle the remaining balance before clearance is completed.

### 2. Zero Separation Pay (Just Cause Termination)
- **Scenario:** Member was terminated for just cause (e.g., serious misconduct).
- **Handling:** Separation Pay = 0. Only Final Salary and Cooperative Share apply as earnings.
- **Formula:** `Net Back Pay = (0 + Final Salary + Cooperative Share) - Total Deductions`

### 3. No Outstanding Deductions
- **Scenario:** Member has no unreturned inventory, no loans, no revolving fund.
- **Handling:** Total Deductions = 0. Member receives full earnings.
- **Formula:** `Net Back Pay = Total Earnings - 0 = Total Earnings`

### 4. Partial Inventory Return
- **Scenario:** Member returned some items but not all.
- **Handling:** Only the value of unreturned items is deducted.
- **Computation:** Sum item values where `IsReturned = False`.

### 5. Member Has Not Rendered Last Attendance
- **Scenario:** Member did not report for work in the last pay period (e.g., AWOL).
- **Handling:** Final Salary = 0. Only Separation Pay and Cooperative Share apply.

### 6. Fractional Years of Service
- **Scenario:** Member worked 3 years and 7 months.
- **Handling:** Per DOLE guidelines, fractions of at least 6 months are counted as 1 whole year.
- **Example:** 3 years 7 months = 4 years for separation pay computation.

### 7. Multiple Outstanding Loans
- **Scenario:** Member has more than one active loan.
- **Handling:** Sum all outstanding balances across all loan types.

### 8. No Cooperative Share
- **Scenario:** Member did not participate in the cooperative.
- **Handling:** Cooperative Share = 0. Only Separation Pay and Final Salary apply.

### 9. Negative Revolving Fund (Overpayment)
- **Scenario:** Company overpaid into revolving fund.
- **Handling:** Revolving fund becomes an earning (returned to member), not a deduction.
- **System should detect and adjust accordingly.**

### 10. Clearance Not Yet Complete
- **Scenario:** Not all departments have cleared the member.
- **Handling:** System should block back pay computation until all clearance steps are "Approved." Show pending departments to the user.
