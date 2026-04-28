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

    ' ========== Input Helpers ==========

    Private Function ReadDecimal(prompt As String) As Decimal
        Dim value As Decimal
        Do
            Console.Write(prompt)
            Dim input = Console.ReadLine()
            If Decimal.TryParse(input, value) Then
                Return value
            End If
            Console.WriteLine("  Invalid input. Please enter a valid number.")
        Loop
    End Function

    Private Function ReadInteger(prompt As String) As Integer
        Dim value As Integer
        Do
            Console.Write(prompt)
            Dim input = Console.ReadLine()
            If Integer.TryParse(input, value) Then
                Return value
            End If
            Console.WriteLine("  Invalid input. Please enter a valid whole number.")
        Loop
    End Function

    Private Function ReadDate(prompt As String) As Date
        Dim value As Date
        Do
            Console.Write(prompt)
            Dim input = Console.ReadLine()
            If Date.TryParse(input, value) Then
                Return value
            End If
            Console.WriteLine("  Invalid input. Please enter a valid date (e.g., 2025-01-15).")
        Loop
    End Function

    Private Function ReadYesNo(prompt As String) As Boolean
        Do
            Console.Write(prompt & " (Y/N): ")
            Dim input = Console.ReadLine().Trim().ToUpper()
            If input = "Y" Then Return True
            If input = "N" Then Return False
            Console.WriteLine("  Please enter Y or N.")
        Loop
    End Function

    ' ========== Interactive Main ==========

    Public Sub Main()
        Console.WriteLine("=============================================")
        Console.WriteLine("     BACK PAY COMPUTATION SYSTEM")
        Console.WriteLine("=============================================")
        Console.WriteLine()

        ' --- Member Information ---
        Console.WriteLine("--- Member Information ---")
        Dim member As New Member()
        Console.Write("Enter Member First Name: ")
        member.FirstName = Console.ReadLine()
        Console.Write("Enter Member Last Name: ")
        member.LastName = Console.ReadLine()
        member.DateHired = ReadDate("Enter Date Hired (yyyy-MM-dd): ")
        Console.Write("Enter Position: ")
        member.Position = Console.ReadLine()
        member.MonthlySalary = ReadDecimal("Enter Monthly Salary: ")
        Console.WriteLine()

        ' --- Separation Details ---
        Console.WriteLine("--- Separation Details ---")
        Dim clearance As New SeparationClearance()
        clearance.SeparationDate = ReadDate("Enter Separation Date (yyyy-MM-dd): ")
        Console.Write("Enter Separation Type (Authorized Cause / Resignation / Just Cause): ")
        clearance.SeparationType = Console.ReadLine()
        clearance.ClearanceStatus = "Approved"
        Console.WriteLine()

        ' --- Earnings ---
        Console.WriteLine("--- Earnings ---")
        Dim earnings As New BackPayEarnings()
        earnings.SeparationPay = ReadDecimal("Enter Separation Pay: ")
        earnings.FinalSalary = ReadDecimal("Enter Final Salary (last pay): ")
        earnings.CooperativeShare = ReadDecimal("Enter Cooperative Share: ")
        Console.WriteLine()

        ' --- Deductions ---
        Console.WriteLine("--- Deductions ---")

        ' Inventory items
        Dim inventoryItems As New List(Of InventoryItem)
        Dim itemCount = ReadInteger("How many unreturned inventory items? ")
        For i As Integer = 1 To itemCount
            Console.WriteLine($"  Inventory Item #{i}:")
            Dim item As New InventoryItem()
            Console.Write($"    Item Name: ")
            item.ItemName = Console.ReadLine()
            item.ItemValue = ReadDecimal($"    Item Value: ")
            item.IsReturned = False
            inventoryItems.Add(item)
        Next
        Dim inventoryDeduction = ComputeInventoryDeduction(inventoryItems)
        Console.WriteLine()

        ' Loans
        Dim loans As New List(Of Loan)
        Dim loanCount = ReadInteger("How many outstanding loans? ")
        For i As Integer = 1 To loanCount
            Console.WriteLine($"  Loan #{i}:")
            Dim ln As New Loan()
            Console.Write($"    Loan Type: ")
            ln.LoanType = Console.ReadLine()
            ln.OutstandingBalance = ReadDecimal($"    Outstanding Balance: ")
            loans.Add(ln)
        Next
        Dim loanBalance = ComputeLoanBalance(loans)
        Console.WriteLine()

        Dim revolvingFund = ReadDecimal("Enter Revolving Fund Balance: ")
        Console.WriteLine()

        ' --- Build Deductions ---
        Dim deductions As New BackPayDeductions()
        deductions.InventoryDeduction = inventoryDeduction
        deductions.LoanBalance = loanBalance
        deductions.RevolvingFund = revolvingFund

        ' --- Compute Net Back Pay ---
        Dim netBackPay As Decimal = earnings.TotalEarnings - deductions.TotalDeductions

        Dim result As New BackPayResult()
        result.MemberName = member.FullName
        result.Earnings = earnings
        result.Deductions = deductions
        result.NetBackPay = netBackPay
        result.HasBalanceDue = (netBackPay < 0)

        ' --- Display Result ---
        Console.WriteLine()
        Console.WriteLine("=============================================")
        Console.WriteLine("            COMPUTATION RESULT")
        Console.WriteLine("=============================================")
        Console.WriteLine()
        DisplayResult(result)
        Console.WriteLine()

        Console.WriteLine("Press any key to exit...")
        Console.ReadKey()
    End Sub

End Module
