Imports System
Imports System.Collections.Generic

''' <summary>
''' Part 2.1 - Item 6: Back Pay Computation System
''' HR / Timekeeping / Payroll
''' Computes the final back pay of a separated member based on:
'''   Earnings: Separation Pay, Final Salary, Cooperative Share
'''   Deductions: Unreturned Inventory, Loan Balances, Revolving Fund
''' </summary>
Public Module BackPaySystemModule

    ' ========== Data Models ==========

    Public Class Member
        Public Property MemberId As Integer
        Public Property FirstName As String
        Public Property LastName As String
        Public Property DateHired As Date
        Public Property Position As String
        Public Property MonthlySalary As Decimal
        Public Property Status As String

        Public ReadOnly Property FullName As String
            Get
                Return $"{FirstName} {LastName}"
            End Get
        End Property
    End Class

    Public Class SeparationClearance
        Public Property ClearanceId As Integer
        Public Property MemberId As Integer
        Public Property SeparationDate As Date
        Public Property SeparationType As String
        Public Property ClearanceStatus As String
    End Class

    Public Class InventoryItem
        Public Property InventoryId As Integer
        Public Property MemberId As Integer
        Public Property ItemName As String
        Public Property ItemValue As Decimal
        Public Property IsReturned As Boolean
    End Class

    Public Class Loan
        Public Property LoanId As Integer
        Public Property MemberId As Integer
        Public Property LoanType As String
        Public Property OutstandingBalance As Decimal
    End Class

    Public Class BackPayEarnings
        Public Property SeparationPay As Decimal
        Public Property FinalSalary As Decimal
        Public Property CooperativeShare As Decimal

        Public ReadOnly Property TotalEarnings As Decimal
            Get
                Return SeparationPay + FinalSalary + CooperativeShare
            End Get
        End Property
    End Class

    Public Class BackPayDeductions
        Public Property InventoryDeduction As Decimal
        Public Property LoanBalance As Decimal
        Public Property RevolvingFund As Decimal

        Public ReadOnly Property TotalDeductions As Decimal
            Get
                Return InventoryDeduction + LoanBalance + RevolvingFund
            End Get
        End Property
    End Class

    Public Class BackPayResult
        Public Property MemberName As String
        Public Property Earnings As BackPayEarnings
        Public Property Deductions As BackPayDeductions
        Public Property NetBackPay As Decimal
        Public Property HasBalanceDue As Boolean
    End Class

    ' ========== Core Functions ==========

    ''' <summary>
    ''' Computes years of service, rounding up fractions of 6 months or more per DOLE rules.
    ''' </summary>
    Public Function ComputeYearsOfService(dateHired As Date, separationDate As Date) As Integer
        If separationDate < dateHired Then
            Throw New ArgumentException("Separation date cannot be earlier than date hired.")
        End If

        Dim totalMonths As Integer = ((separationDate.Year - dateHired.Year) * 12) +
                                      (separationDate.Month - dateHired.Month)
        Dim years As Integer = totalMonths \ 12
        Dim remainingMonths As Integer = totalMonths Mod 12

        ' Per DOLE: fractions of at least 6 months count as 1 whole year
        If remainingMonths >= 6 Then
            years += 1
        End If

        Return years
    End Function

    ''' <summary>
    ''' Computes separation pay based on DOLE labor law.
    ''' For authorized causes: Monthly Salary x Years of Service.
    ''' For just causes: 0 (unless company policy dictates otherwise).
    ''' </summary>
    Public Function ComputeSeparationPay(monthlySalary As Decimal, yearsOfService As Integer,
                                          separationType As String) As Decimal
        Select Case separationType.ToUpper()
            Case "RESIGNATION"
                Return 0D
            Case "JUST CAUSE"
                Return 0D
            Case "REDUNDANCY", "RETRENCHMENT", "CLOSURE", "AUTHORIZED CAUSE"
                Return monthlySalary * yearsOfService
            Case Else
                Return monthlySalary * yearsOfService
        End Select
    End Function

    ''' <summary>
    ''' Computes the total value of unreturned inventory items.
    ''' </summary>
    Public Function ComputeInventoryDeduction(items As List(Of InventoryItem)) As Decimal
        Dim total As Decimal = 0D
        For Each item In items
            If Not item.IsReturned Then
                total += item.ItemValue
            End If
        Next
        Return total
    End Function

    ''' <summary>
    ''' Computes the total outstanding loan balance.
    ''' </summary>
    Public Function ComputeLoanBalance(loans As List(Of Loan)) As Decimal
        Dim total As Decimal = 0D
        For Each loan In loans
            total += loan.OutstandingBalance
        Next
        Return total
    End Function

    ''' <summary>
    ''' Computes the full back pay for a member.
    ''' </summary>
    Public Function ComputeBackPay(member As Member,
                                    clearance As SeparationClearance,
                                    finalSalary As Decimal,
                                    cooperativeShare As Decimal,
                                    inventoryItems As List(Of InventoryItem),
                                    loans As List(Of Loan),
                                    revolvingFund As Decimal) As BackPayResult

        ' Validate clearance
        If clearance.ClearanceStatus <> "Approved" Then
            Throw New InvalidOperationException(
                $"Cannot compute back pay. Clearance status is '{clearance.ClearanceStatus}'. Must be 'Approved'.")
        End If

        ' Compute years of service
        Dim yearsOfService = ComputeYearsOfService(member.DateHired, clearance.SeparationDate)

        ' Compute earnings
        Dim earnings As New BackPayEarnings()
        earnings.SeparationPay = ComputeSeparationPay(member.MonthlySalary, yearsOfService,
                                                       clearance.SeparationType)
        earnings.FinalSalary = finalSalary
        earnings.CooperativeShare = cooperativeShare

        ' Compute deductions
        Dim deductions As New BackPayDeductions()
        deductions.InventoryDeduction = ComputeInventoryDeduction(inventoryItems)
        deductions.LoanBalance = ComputeLoanBalance(loans)
        deductions.RevolvingFund = revolvingFund

        ' Compute net back pay
        Dim netBackPay As Decimal = earnings.TotalEarnings - deductions.TotalDeductions

        Dim result As New BackPayResult()
        result.MemberName = member.FullName
        result.Earnings = earnings
        result.Deductions = deductions
        result.NetBackPay = netBackPay
        result.HasBalanceDue = (netBackPay < 0)

        Return result
    End Function

    ''' <summary>
    ''' Displays the back pay computation result.
    ''' </summary>
    Public Sub DisplayResult(result As BackPayResult)
        Console.WriteLine($"Back Pay Computation for: {result.MemberName}")
        Console.WriteLine(New String("="c, 50))
        Console.WriteLine()
        Console.WriteLine("EARNINGS:")
        Console.WriteLine($"  Separation Pay:     {result.Earnings.SeparationPay:N2}")
        Console.WriteLine($"  Final Salary:       {result.Earnings.FinalSalary:N2}")
        Console.WriteLine($"  Cooperative Share:  {result.Earnings.CooperativeShare:N2}")
        Console.WriteLine($"  ─────────────────────────────")
        Console.WriteLine($"  Total Earnings:     {result.Earnings.TotalEarnings:N2}")
        Console.WriteLine()
        Console.WriteLine("DEDUCTIONS:")
        Console.WriteLine($"  Inventory Deduction: {result.Deductions.InventoryDeduction:N2}")
        Console.WriteLine($"  Loan Balance:        {result.Deductions.LoanBalance:N2}")
        Console.WriteLine($"  Revolving Fund:      {result.Deductions.RevolvingFund:N2}")
        Console.WriteLine($"  ─────────────────────────────")
        Console.WriteLine($"  Total Deductions:    {result.Deductions.TotalDeductions:N2}")
        Console.WriteLine()
        Console.WriteLine(New String("="c, 50))

        If result.HasBalanceDue Then
            Console.WriteLine($"  BALANCE DUE (member owes): {Math.Abs(result.NetBackPay):N2}")
        Else
            Console.WriteLine($"  NET BACK PAY: {result.NetBackPay:N2}")
        End If
    End Sub

    ' ========== Demo / Test ==========

    Public Sub Main()
        Console.WriteLine("=== Back Pay Computation System ===")
        Console.WriteLine()

        ' --- Sample Data (from exam) ---
        Dim member As New Member()
        member.MemberId = 1
        member.FirstName = "Juan"
        member.LastName = "Dela Cruz"
        member.DateHired = New Date(2020, 3, 15)
        member.Position = "Production Worker"
        member.MonthlySalary = 12500D
        member.Status = "Separated"

        Dim clearance As New SeparationClearance()
        clearance.ClearanceId = 1
        clearance.MemberId = 1
        clearance.SeparationDate = New Date(2025, 1, 15)
        clearance.SeparationType = "Authorized Cause"
        clearance.ClearanceStatus = "Approved"

        ' Inventory items
        Dim inventoryItems As New List(Of InventoryItem)
        inventoryItems.Add(New InventoryItem() With {
            .InventoryId = 1, .MemberId = 1, .ItemName = "Laptop",
            .ItemValue = 2000D, .IsReturned = False
        })
        inventoryItems.Add(New InventoryItem() With {
            .InventoryId = 2, .MemberId = 1, .ItemName = "ID Badge",
            .ItemValue = 500D, .IsReturned = True
        })
        inventoryItems.Add(New InventoryItem() With {
            .InventoryId = 3, .MemberId = 1, .ItemName = "Uniform",
            .ItemValue = 500D, .IsReturned = False
        })

        ' Loans
        Dim loans As New List(Of Loan)
        loans.Add(New Loan() With {
            .LoanId = 1, .MemberId = 1, .LoanType = "Salary Loan",
            .OutstandingBalance = 5000D
        })
        loans.Add(New Loan() With {
            .LoanId = 2, .MemberId = 1, .LoanType = "Emergency Loan",
            .OutstandingBalance = 3000D
        })

        ' Use sample values from exam for direct computation
        Console.WriteLine("--- Using Exam Sample Values ---")
        Console.WriteLine()

        Dim sampleResult As New BackPayResult()
        Dim sampleEarnings As New BackPayEarnings()
        sampleEarnings.SeparationPay = 50000D
        sampleEarnings.FinalSalary = 10000D
        sampleEarnings.CooperativeShare = 5000D

        Dim sampleDeductions As New BackPayDeductions()
        sampleDeductions.InventoryDeduction = 3000D
        sampleDeductions.LoanBalance = 8000D
        sampleDeductions.RevolvingFund = 2000D

        sampleResult.MemberName = "Sample Member"
        sampleResult.Earnings = sampleEarnings
        sampleResult.Deductions = sampleDeductions
        sampleResult.NetBackPay = sampleEarnings.TotalEarnings - sampleDeductions.TotalDeductions
        sampleResult.HasBalanceDue = (sampleResult.NetBackPay < 0)

        DisplayResult(sampleResult)
        Console.WriteLine()

        ' --- Dynamic computation using member data ---
        Console.WriteLine("--- Dynamic Computation (Member: Juan Dela Cruz) ---")
        Console.WriteLine()

        Dim dynamicResult = ComputeBackPay(member, clearance, 10000D, 5000D,
                                            inventoryItems, loans, 2000D)
        DisplayResult(dynamicResult)
        Console.WriteLine()

        ' --- Edge Case: Deductions exceed earnings ---
        Console.WriteLine("--- Edge Case: Deductions Exceed Earnings ---")
        Console.WriteLine()

        Dim edgeResult As New BackPayResult()
        Dim edgeEarnings As New BackPayEarnings()
        edgeEarnings.SeparationPay = 5000D
        edgeEarnings.FinalSalary = 2000D
        edgeEarnings.CooperativeShare = 1000D

        Dim edgeDeductions As New BackPayDeductions()
        edgeDeductions.InventoryDeduction = 5000D
        edgeDeductions.LoanBalance = 8000D
        edgeDeductions.RevolvingFund = 3000D

        edgeResult.MemberName = "Edge Case Member"
        edgeResult.Earnings = edgeEarnings
        edgeResult.Deductions = edgeDeductions
        edgeResult.NetBackPay = edgeEarnings.TotalEarnings - edgeDeductions.TotalDeductions
        edgeResult.HasBalanceDue = (edgeResult.NetBackPay < 0)

        DisplayResult(edgeResult)
        Console.WriteLine()

        ' --- Edge Case: Pending clearance (should throw error) ---
        Console.WriteLine("--- Edge Case: Pending Clearance ---")
        Try
            Dim pendingClearance As New SeparationClearance()
            pendingClearance.ClearanceId = 2
            pendingClearance.MemberId = 1
            pendingClearance.SeparationDate = New Date(2025, 1, 15)
            pendingClearance.SeparationType = "Authorized Cause"
            pendingClearance.ClearanceStatus = "Pending"

            Dim pendingResult = ComputeBackPay(member, pendingClearance, 10000D, 5000D,
                                                inventoryItems, loans, 2000D)
        Catch ex As InvalidOperationException
            Console.WriteLine($"  Error caught: {ex.Message}")
        End Try
    End Sub

End Module
