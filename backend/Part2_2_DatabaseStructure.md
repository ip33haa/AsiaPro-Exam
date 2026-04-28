# Part 2.2 - Database Structure (Bill/Collect - Pakyaw System)

## Tables

### 1. Members
| Field            | Type         | Description                        |
|------------------|--------------|------------------------------------|
| MemberId (PK)    | INT          | Unique identifier for the member   |
| FirstName        | VARCHAR(100) | Member's first name                |
| LastName         | VARCHAR(100) | Member's last name                 |
| Status           | VARCHAR(20)  | Active, Inactive                   |

### 2. Clients
| Field            | Type         | Description                        |
|------------------|--------------|------------------------------------|
| ClientId (PK)    | INT          | Unique identifier for the client   |
| ClientName       | VARCHAR(200) | Name of the client company         |
| ContactPerson    | VARCHAR(100) | Contact person                     |
| ContactNumber    | VARCHAR(20)  | Phone number                       |
| Address          | VARCHAR(500) | Client address                     |

### 3. SKUs (Stock Keeping Units / Product Items)
| Field            | Type         | Description                        |
|------------------|--------------|------------------------------------|
| SKUId (PK)       | INT          | Unique identifier                  |
| SKUName          | VARCHAR(100) | Product name (e.g., Sliced Banana) |
| PieceRate        | DECIMAL(18,2)| Rate per unit of output            |

### 4. DayTypes
| Field            | Type         | Description                        |
|------------------|--------------|------------------------------------|
| DayTypeId (PK)   | INT          | Unique identifier                  |
| DayTypeName      | VARCHAR(50)  | Regular Day, Special Holiday, etc. |
| Multiplier       | DECIMAL(5,2) | Rate multiplier (1.0, 1.3, 2.0)   |

### 5. DailyOutputs
| Field            | Type         | Description                        |
|------------------|--------------|------------------------------------|
| OutputId (PK)    | INT          | Unique identifier                  |
| MemberId (FK)    | INT          | Reference to Members table         |
| ClientId (FK)    | INT          | Reference to Clients table         |
| SKUId (FK)       | INT          | Reference to SKUs table            |
| DayTypeId (FK)   | INT          | Reference to DayTypes table        |
| OutputDate       | DATE         | Date of output                     |
| Quantity         | INT          | Number of units produced           |

### 6. GovernmentContributions
| Field            | Type         | Description                        |
|------------------|--------------|------------------------------------|
| ContributionId (PK)| INT        | Unique identifier                  |
| BillingId (FK)   | INT          | Reference to Billings table        |
| SSS              | DECIMAL(18,2)| SSS contribution                   |
| Pagibig          | DECIMAL(18,2)| Pag-IBIG contribution              |
| Philhealth       | DECIMAL(18,2)| PhilHealth contribution            |
| TotalContributions| DECIMAL(18,2)| Sum of all contributions           |

### 7. Billings
| Field            | Type         | Description                        |
|------------------|--------------|------------------------------------|
| BillingId (PK)   | INT          | Unique identifier                  |
| ClientId (FK)    | INT          | Reference to Clients table         |
| BillingPeriodStart| DATE        | Start of billing period            |
| BillingPeriodEnd | DATE         | End of billing period              |
| GrossPayout      | DECIMAL(18,2)| Total pakyaw earnings              |
| SSSAmount        | DECIMAL(18,2)| SSS contribution                   |
| PagibigAmount    | DECIMAL(18,2)| Pag-IBIG contribution              |
| PhilhealthAmount | DECIMAL(18,2)| PhilHealth contribution            |
| BillableAmount   | DECIMAL(18,2)| Gross + Gov't contributions        |
| BillingDate      | DATE         | Date billing was generated         |
| Status           | VARCHAR(20)  | Pending, Sent, Collected, Partial  |

### 8. Collections
| Field            | Type         | Description                        |
|------------------|--------------|------------------------------------|
| CollectionId (PK)| INT          | Unique identifier                  |
| BillingId (FK)   | INT          | Reference to Billings table        |
| AmountCollected  | DECIMAL(18,2)| Amount actually collected          |
| CollectionDate   | DATE         | Date of collection                 |
| PaymentMethod    | VARCHAR(50)  | Check, Bank Transfer, Cash         |
| ReferenceNumber  | VARCHAR(100) | Payment reference number           |

### 9. CreditNotes
| Field            | Type         | Description                        |
|------------------|--------------|------------------------------------|
| CreditNoteId (PK)| INT         | Unique identifier                  |
| BillingId (FK)   | INT          | Reference to Billings table        |
| Amount           | DECIMAL(18,2)| Uncollected / partial amount       |
| Reason           | VARCHAR(500) | Reason for credit note             |
| CreatedDate      | DATE         | Date credit note was created       |
| Status           | VARCHAR(20)  | Open, Applied, Closed              |

---

## Relationships

```
Members (1) ──── (M) DailyOutputs
Clients (1) ──── (M) DailyOutputs
Clients (1) ──── (M) Billings
SKUs (1) ──── (M) DailyOutputs
DayTypes (1) ──── (M) DailyOutputs
Billings (1) ──── (1) GovernmentContributions
Billings (1) ──── (M) Collections
Billings (1) ──── (M) CreditNotes
```

- A **Member** can have many **DailyOutputs** (multiple outputs per day across different SKUs).
- A **Client** has many **DailyOutputs** and many **Billings**.
- Each **SKU** can appear in many **DailyOutputs**.
- Each **DayType** can be assigned to many **DailyOutputs**.
- Each **Billing** has one **GovernmentContributions** record.
- A **Billing** can have multiple **Collections** (partial payments).
- A **Billing** can have multiple **CreditNotes** (for uncollectible amounts).
