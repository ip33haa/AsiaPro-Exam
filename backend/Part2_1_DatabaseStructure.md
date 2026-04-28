# Part 2.1 - Database Structure (HR / Timekeeping / Payroll - Back Pay)

## Tables

### 1. Members
| Field            | Type         | Description                        |
|------------------|--------------|------------------------------------|
| MemberId (PK)    | INT          | Unique identifier for the member   |
| FirstName        | VARCHAR(100) | Member's first name                |
| LastName         | VARCHAR(100) | Member's last name                 |
| DateHired        | DATE         | Date the member was hired          |
| Position         | VARCHAR(100) | Job title / position               |
| DepartmentId (FK)| INT          | Reference to Departments table     |
| MonthlySalary    | DECIMAL(18,2)| Current monthly salary             |
| Status           | VARCHAR(20)  | Active, Separated, Cleared         |

### 2. Departments
| Field            | Type         | Description                        |
|------------------|--------------|------------------------------------|
| DepartmentId (PK)| INT          | Unique identifier                  |
| DepartmentName   | VARCHAR(100) | Name of the department             |

### 3. Attendance
| Field            | Type         | Description                        |
|------------------|--------------|------------------------------------|
| AttendanceId (PK)| INT          | Unique identifier                  |
| MemberId (FK)    | INT          | Reference to Members table         |
| AttendanceDate   | DATE         | Date of attendance                 |
| TimeIn           | DATETIME     | Clock-in time                      |
| TimeOut          | DATETIME     | Clock-out time                     |
| LateMinutes      | INT          | Computed late minutes              |
| UndertimeMinutes | INT          | Computed undertime minutes         |
| WorkedHours      | DECIMAL(5,2) | Net worked hours                   |

### 4. SeparationClearance
| Field              | Type         | Description                        |
|--------------------|--------------|------------------------------------|
| ClearanceId (PK)   | INT          | Unique identifier                  |
| MemberId (FK)      | INT          | Reference to Members table         |
| SeparationDate     | DATE         | Date of separation                 |
| SeparationType     | VARCHAR(50)  | Resignation, Termination, etc.     |
| YearsOfService     | DECIMAL(5,2) | Computed years of service          |
| ClearanceStatus    | VARCHAR(20)  | Pending, Approved, Completed       |
| ProcessedDate      | DATE         | Date clearance was processed       |

### 5. Earnings
| Field              | Type         | Description                        |
|--------------------|--------------|------------------------------------|
| EarningId (PK)     | INT          | Unique identifier                  |
| ClearanceId (FK)   | INT          | Reference to SeparationClearance   |
| SeparationPay      | DECIMAL(18,2)| Based on DOLE labor law            |
| FinalSalary        | DECIMAL(18,2)| Based on last attendance rendered  |
| CooperativeShare   | DECIMAL(18,2)| Amount to be returned to member    |
| TotalEarnings      | DECIMAL(18,2)| Sum of all earnings                |

### 6. Deductions
| Field              | Type         | Description                        |
|--------------------|--------------|------------------------------------|
| DeductionId (PK)   | INT          | Unique identifier                  |
| ClearanceId (FK)   | INT          | Reference to SeparationClearance   |
| InventoryDeduction | DECIMAL(18,2)| Unreturned inventory items         |
| LoanBalance        | DECIMAL(18,2)| Outstanding loan balances          |
| RevolvingFund      | DECIMAL(18,2)| Revolving fund balance             |
| TotalDeductions    | DECIMAL(18,2)| Sum of all deductions              |

### 7. BackPay
| Field              | Type         | Description                        |
|--------------------|--------------|------------------------------------|
| BackPayId (PK)     | INT          | Unique identifier                  |
| ClearanceId (FK)   | INT          | Reference to SeparationClearance   |
| TotalEarnings      | DECIMAL(18,2)| From Earnings table                |
| TotalDeductions    | DECIMAL(18,2)| From Deductions table              |
| NetBackPay         | DECIMAL(18,2)| TotalEarnings - TotalDeductions    |
| ComputedDate       | DATE         | Date the back pay was computed     |
| ReleasedDate       | DATE         | Date the back pay was released     |
| Status             | VARCHAR(20)  | Pending, Approved, Released        |

### 8. Inventory
| Field              | Type         | Description                        |
|--------------------|--------------|------------------------------------|
| InventoryId (PK)   | INT          | Unique identifier                  |
| MemberId (FK)      | INT          | Reference to Members table         |
| ItemName           | VARCHAR(100) | Name of issued item                |
| DateIssued         | DATE         | Date item was issued               |
| DateReturned       | DATE         | Date item was returned (nullable)  |
| ItemValue          | DECIMAL(18,2)| Monetary value of the item         |
| IsReturned         | BIT          | Whether item has been returned     |

### 9. Loans
| Field              | Type         | Description                        |
|--------------------|--------------|------------------------------------|
| LoanId (PK)        | INT          | Unique identifier                  |
| MemberId (FK)      | INT          | Reference to Members table         |
| LoanType           | VARCHAR(50)  | Type of loan                       |
| LoanAmount         | DECIMAL(18,2)| Original loan amount               |
| OutstandingBalance | DECIMAL(18,2)| Remaining balance                  |
| MonthlyAmortization| DECIMAL(18,2)| Monthly payment amount             |

---

## Relationships

```
Departments (1) ──── (M) Members
Members (1) ──── (M) Attendance
Members (1) ──── (1) SeparationClearance
Members (1) ──── (M) Inventory
Members (1) ──── (M) Loans
SeparationClearance (1) ──── (1) Earnings
SeparationClearance (1) ──── (1) Deductions
SeparationClearance (1) ──── (1) BackPay
```

- A **Department** has many **Members**.
- A **Member** has many **Attendance** records.
- A **Member** has one **SeparationClearance** record (when being processed for separation).
- A **Member** may have many **Inventory** items issued.
- A **Member** may have many **Loans**.
- Each **SeparationClearance** has one **Earnings** record, one **Deductions** record, and one **BackPay** record.
