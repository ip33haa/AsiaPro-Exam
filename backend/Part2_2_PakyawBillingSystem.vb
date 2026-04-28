Imports System
Imports System.Collections.Generic

''' <summary>
''' Part 2.2 - Item 10: Pakyaw Billing and Collection System
''' Computes pakyaw earnings, generates billing, and handles collection.
''' </summary>
Public Module PakyawBillingSystemModule

    ' ========== Data Models ==========

    Public Class SKU
        Public Property SKUId As Integer
        Public Property SKUName As String
        Public Property PieceRate As Decimal
    End Class

    Public Class DayType
        Public Property DayTypeId As Integer
        Public Property DayTypeName As String
        Public Property Multiplier As Decimal
    End Class

    Public Class DailyOutput
        Public Property OutputDate As Date
        Public Property DayTypeName As String
        Public Property SKUName As String
        Public Property Quantity As Integer
    End Class

    Public Class OutputLineResult
        Public Property OutputDate As Date
        Public Property DayTypeName As String
        Public Property SKUName As String
        Public Property Quantity As Integer
        Public Property PieceRate As Decimal
        Public Property Multiplier As Decimal
        Public Property LineAmount As Decimal
    End Class

    Public Class GovernmentContributions
        Public Property SSS As Decimal
        Public Property Pagibig As Decimal
        Public Property Philhealth As Decimal

        Public ReadOnly Property Total As Decimal
            Get
                Return SSS + Pagibig + Philhealth
            End Get
        End Property
    End Class

    Public Class BillingResult
        Public Property GrossPayout As Decimal
        Public Property Contributions As GovernmentContributions
        Public Property BillableAmount As Decimal
        Public Property LineDetails As List(Of OutputLineResult)
    End Class

    Public Enum CollectionStatus
        Collected
        Partial
        Uncollected
    End Enum

    Public Class CollectionResult
        Public Property BillableAmount As Decimal
        Public Property AmountCollected As Decimal
        Public Property RemainingBalance As Decimal
        Public Property Status As CollectionStatus
        Public Property CreditNoteAmount As Decimal
        Public Property CreditNoteReason As String
    End Class

    ' ========== Lookup Helpers ==========

    ''' <summary>
    ''' Returns the piece rate for a given SKU name.
    ''' </summary>
    Public Function GetPieceRate(skuName As String, skuList As List(Of SKU)) As Decimal
        For Each s In skuList
            If s.SKUName.ToUpper() = skuName.ToUpper() Then
                Return s.PieceRate
            End If
        Next
        Throw New ArgumentException($"SKU '{skuName}' not found in the SKU list.")
    End Function

    ''' <summary>
    ''' Returns the multiplier for a given day type name.
    ''' </summary>
    Public Function GetMultiplier(dayTypeName As String, dayTypeList As List(Of DayType)) As Decimal
        For Each d In dayTypeList
            If d.DayTypeName.ToUpper() = dayTypeName.ToUpper() Then
                Return d.Multiplier
            End If
        Next
        Throw New ArgumentException($"Day Type '{dayTypeName}' not found.")
    End Function

    ' ========== Core Functions ==========

    ''' <summary>
    ''' Computes the total pakyaw earnings from a list of daily outputs.
    ''' Returns line-by-line details and the total gross payout.
    ''' </summary>
    Public Function ComputePakyawEarnings(outputs As List(Of DailyOutput),
                                           skuList As List(Of SKU),
                                           dayTypeList As List(Of DayType)) As BillingResult
        Dim result As New BillingResult()
        result.LineDetails = New List(Of OutputLineResult)()
        Dim totalPakyaw As Decimal = 0D

        For Each output In outputs
            Dim pieceRate = GetPieceRate(output.SKUName, skuList)
            Dim multiplier = GetMultiplier(output.DayTypeName, dayTypeList)

            Dim lineAmount As Decimal = output.Quantity * pieceRate * multiplier

            Dim lineResult As New OutputLineResult()
            lineResult.OutputDate = output.OutputDate
            lineResult.DayTypeName = output.DayTypeName
            lineResult.SKUName = output.SKUName
            lineResult.Quantity = output.Quantity
            lineResult.PieceRate = pieceRate
            lineResult.Multiplier = multiplier
            lineResult.LineAmount = lineAmount

            result.LineDetails.Add(lineResult)
            totalPakyaw += lineAmount
        Next

        result.GrossPayout = totalPakyaw
        Return result
    End Function

    ''' <summary>
    ''' Computes government contributions based on gross payout.
    ''' Uses sample fixed rates for demonstration.
    ''' </summary>
    Public Function ComputeGovernmentContributions(grossPayout As Decimal) As GovernmentContributions
        Dim contrib As New GovernmentContributions()

        ' Sample contribution computation
        ' In production, these would come from official rate tables
        contrib.SSS = 500D
        contrib.Pagibig = 200D
        contrib.Philhealth = 250D

        Return contrib
    End Function

    ''' <summary>
    ''' Generates the full billing including gross payout, contributions, and billable amount.
    ''' </summary>
    Public Function GenerateBilling(outputs As List(Of DailyOutput),
                                     skuList As List(Of SKU),
                                     dayTypeList As List(Of DayType)) As BillingResult

        Dim result = ComputePakyawEarnings(outputs, skuList, dayTypeList)
        Dim contributions = ComputeGovernmentContributions(result.GrossPayout)

        result.Contributions = contributions
        result.BillableAmount = result.GrossPayout + contributions.Total

        Return result
    End Function

    ''' <summary>
    ''' Processes a collection against a billing.
    ''' If full payment: close transaction.
    ''' If partial or zero: create credit note.
    ''' </summary>
    Public Function ProcessCollection(billableAmount As Decimal,
                                       amountCollected As Decimal) As CollectionResult
        If amountCollected < 0 Then
            Throw New ArgumentException("Amount collected cannot be negative.")
        End If

        Dim result As New CollectionResult()
        result.BillableAmount = billableAmount
        result.AmountCollected = amountCollected
        result.RemainingBalance = billableAmount - amountCollected

        If amountCollected >= billableAmount Then
            result.Status = CollectionStatus.Collected
            result.CreditNoteAmount = 0D
            result.CreditNoteReason = ""
            result.RemainingBalance = 0D
        ElseIf amountCollected > 0 Then
            result.Status = CollectionStatus.Partial
            result.CreditNoteAmount = billableAmount - amountCollected
            result.CreditNoteReason = "Partial payment received"
        Else
            result.Status = CollectionStatus.Uncollected
            result.CreditNoteAmount = billableAmount
            result.CreditNoteReason = "No payment received - uncollectible"
        End If

        Return result
    End Function

    ' ========== Display Helpers ==========

    Public Sub DisplayBilling(billing As BillingResult)
        Console.WriteLine("PAKYAW EARNINGS (Line Details):")
        Console.WriteLine(String.Format("{0,-12} {1,-18} {2,-16} {3,6} {4,6} {5,5} {6,12}",
            "Date", "Day Type", "SKU", "Qty", "Rate", "Mult", "Amount"))
        Console.WriteLine(New String("-"c, 85))

        For Each line In billing.LineDetails
            Console.WriteLine(String.Format("{0,-12} {1,-18} {2,-16} {3,6} {4,6} {5,5} {6,12:N2}",
                line.OutputDate.ToString("MMM dd"),
                line.DayTypeName,
                line.SKUName,
                line.Quantity,
                line.PieceRate,
                line.Multiplier,
                line.LineAmount))
        Next

        Console.WriteLine(New String("-"c, 85))
        Console.WriteLine(String.Format("{0,73} {1,12:N2}", "Gross Payout:", billing.GrossPayout))
        Console.WriteLine()
        Console.WriteLine("GOVERNMENT CONTRIBUTIONS:")
        Console.WriteLine($"  SSS:        {billing.Contributions.SSS:N2}")
        Console.WriteLine($"  Pagibig:    {billing.Contributions.Pagibig:N2}")
        Console.WriteLine($"  Philhealth: {billing.Contributions.Philhealth:N2}")
        Console.WriteLine($"  ─────────────────────────")
        Console.WriteLine($"  Total:      {billing.Contributions.Total:N2}")
        Console.WriteLine()
        Console.WriteLine(New String("="c, 85))
        Console.WriteLine($"  BILLABLE AMOUNT: {billing.BillableAmount:N2}")
        Console.WriteLine(New String("="c, 85))
    End Sub

    Public Sub DisplayCollection(collection As CollectionResult)
        Console.WriteLine($"  Billable Amount:  {collection.BillableAmount:N2}")
        Console.WriteLine($"  Amount Collected: {collection.AmountCollected:N2}")
        Console.WriteLine($"  Remaining:        {collection.RemainingBalance:N2}")
        Console.WriteLine($"  Status:           {collection.Status}")

        If collection.CreditNoteAmount > 0 Then
            Console.WriteLine()
            Console.WriteLine("  CREDIT NOTE GENERATED:")
            Console.WriteLine($"    Amount: {collection.CreditNoteAmount:N2}")
            Console.WriteLine($"    Reason: {collection.CreditNoteReason}")
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

    ' ========== Interactive Main ==========

    Public Sub Main()
        Console.WriteLine("=============================================")
        Console.WriteLine("  PAKYAW BILLING AND COLLECTION SYSTEM")
        Console.WriteLine("=============================================")
        Console.WriteLine()

        ' --- Step 1: Setup SKUs ---
        Console.WriteLine("--- Step 1: Setup SKUs (Products & Piece Rates) ---")
        Dim skuList As New List(Of SKU)
        Dim skuCount = ReadInteger("How many SKUs (products)? ")
        For i As Integer = 1 To skuCount
            Console.WriteLine($"  SKU #{i}:")
            Dim s As New SKU()
            s.SKUId = i
            Console.Write("    SKU Name: ")
            s.SKUName = Console.ReadLine()
            s.PieceRate = ReadDecimal("    Piece Rate: ")
            skuList.Add(s)
        Next
        Console.WriteLine()

        ' --- Day Types (predefined) ---
        Console.WriteLine("--- Day Type Multipliers (predefined) ---")
        Dim dayTypeList As New List(Of DayType)
        dayTypeList.Add(New DayType() With {.DayTypeId = 1, .DayTypeName = "Regular Day", .Multiplier = 1.0D})
        dayTypeList.Add(New DayType() With {.DayTypeId = 2, .DayTypeName = "Special Holiday", .Multiplier = 1.3D})
        dayTypeList.Add(New DayType() With {.DayTypeId = 3, .DayTypeName = "Legal Holiday", .Multiplier = 2.0D})
        Console.WriteLine("  1 - Regular Day      (x1.0)")
        Console.WriteLine("  2 - Special Holiday  (x1.3)")
        Console.WriteLine("  3 - Legal Holiday    (x2.0)")
        Console.WriteLine()

        ' --- Step 2: Enter Daily Outputs ---
        Console.WriteLine("--- Step 2: Enter Daily Outputs ---")
        Dim outputs As New List(Of DailyOutput)
        Dim outputCount = ReadInteger("How many output entries? ")
        For i As Integer = 1 To outputCount
            Console.WriteLine($"  Output Entry #{i}:")
            Dim o As New DailyOutput()
            o.OutputDate = ReadDate("    Date (yyyy-MM-dd): ")
            Console.WriteLine("    Day Type Options: 1=Regular Day, 2=Special Holiday, 3=Legal Holiday")
            Dim dayTypeChoice = ReadInteger("    Day Type (1/2/3): ")
            Select Case dayTypeChoice
                Case 1 : o.DayTypeName = "Regular Day"
                Case 2 : o.DayTypeName = "Special Holiday"
                Case 3 : o.DayTypeName = "Legal Holiday"
                Case Else
                    Console.WriteLine("    Invalid choice, defaulting to Regular Day.")
                    o.DayTypeName = "Regular Day"
            End Select
            Console.Write("    SKU Name: ")
            o.SKUName = Console.ReadLine()
            o.Quantity = ReadInteger("    Quantity (output): ")
            outputs.Add(o)
        Next
        Console.WriteLine()

        ' --- Step 3: Generate Billing ---
        Console.WriteLine("=============================================")
        Console.WriteLine("         BILLING COMPUTATION RESULT")
        Console.WriteLine("=============================================")
        Console.WriteLine()

        Try
            Dim billing = GenerateBilling(outputs, skuList, dayTypeList)
            DisplayBilling(billing)
            Console.WriteLine()

            ' --- Step 4: Government Contributions ---
            Console.WriteLine("--- Government Contributions (enter actual amounts) ---")
            Console.Write("  Enter SSS Amount: ")
            Dim sssInput = Console.ReadLine()
            Console.Write("  Enter Pagibig Amount: ")
            Dim pagibigInput = Console.ReadLine()
            Console.Write("  Enter Philhealth Amount: ")
            Dim philhealthInput = Console.ReadLine()

            Dim sssVal As Decimal = 0D
            Dim pagibigVal As Decimal = 0D
            Dim philhealthVal As Decimal = 0D
            Decimal.TryParse(sssInput, sssVal)
            Decimal.TryParse(pagibigInput, pagibigVal)
            Decimal.TryParse(philhealthInput, philhealthVal)

            ' Override with user-entered contributions
            billing.Contributions.SSS = sssVal
            billing.Contributions.Pagibig = pagibigVal
            billing.Contributions.Philhealth = philhealthVal
            billing.BillableAmount = billing.GrossPayout + billing.Contributions.Total

            Console.WriteLine()
            Console.WriteLine("--- Updated Billing ---")
            Console.WriteLine($"  Gross Payout:    {billing.GrossPayout:N2}")
            Console.WriteLine($"  SSS:             {billing.Contributions.SSS:N2}")
            Console.WriteLine($"  Pagibig:         {billing.Contributions.Pagibig:N2}")
            Console.WriteLine($"  Philhealth:      {billing.Contributions.Philhealth:N2}")
            Console.WriteLine($"  ─────────────────────────")
            Console.WriteLine($"  BILLABLE AMOUNT: {billing.BillableAmount:N2}")
            Console.WriteLine()

            ' --- Step 5: Collection ---
            Console.WriteLine("=============================================")
            Console.WriteLine("           COLLECTION PROCESS")
            Console.WriteLine("=============================================")
            Console.WriteLine()
            Console.WriteLine($"  Billable Amount: {billing.BillableAmount:N2}")
            Dim amountCollected = ReadDecimal("  Enter Amount Collected from Client: ")
            Console.WriteLine()

            Dim collection = ProcessCollection(billing.BillableAmount, amountCollected)

            Console.WriteLine("--- Collection Result ---")
            DisplayCollection(collection)

        Catch ex As ArgumentException
            Console.WriteLine($"  Error: {ex.Message}")
        End Try

        Console.WriteLine()
        Console.WriteLine("Press any key to exit...")
        Console.ReadKey()
    End Sub

End Module
