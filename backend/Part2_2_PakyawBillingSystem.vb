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

    ' ========== Demo / Test ==========

    Public Sub Main()
        Console.WriteLine("=== Pakyaw Billing and Collection System ===")
        Console.WriteLine()

        ' --- Setup: SKUs ---
        Dim skuList As New List(Of SKU)
        skuList.Add(New SKU() With {.SKUId = 1, .SKUName = "Sliced Banana", .PieceRate = 5D})
        skuList.Add(New SKU() With {.SKUId = 2, .SKUName = "Sliced Apple", .PieceRate = 7D})

        ' --- Setup: Day Types ---
        Dim dayTypeList As New List(Of DayType)
        dayTypeList.Add(New DayType() With {.DayTypeId = 1, .DayTypeName = "Regular Day", .Multiplier = 1.0D})
        dayTypeList.Add(New DayType() With {.DayTypeId = 2, .DayTypeName = "Special Holiday", .Multiplier = 1.3D})
        dayTypeList.Add(New DayType() With {.DayTypeId = 3, .DayTypeName = "Legal Holiday", .Multiplier = 2.0D})

        ' --- Setup: Daily Outputs (from exam sample) ---
        Dim outputs As New List(Of DailyOutput)
        outputs.Add(New DailyOutput() With {
            .OutputDate = New Date(2025, 1, 12), .DayTypeName = "Legal Holiday",
            .SKUName = "Sliced Banana", .Quantity = 150})
        outputs.Add(New DailyOutput() With {
            .OutputDate = New Date(2025, 1, 13), .DayTypeName = "Regular Day",
            .SKUName = "Sliced Apple", .Quantity = 200})
        outputs.Add(New DailyOutput() With {
            .OutputDate = New Date(2025, 1, 14), .DayTypeName = "Regular Day",
            .SKUName = "Sliced Apple", .Quantity = 100})
        outputs.Add(New DailyOutput() With {
            .OutputDate = New Date(2025, 1, 15), .DayTypeName = "Regular Day",
            .SKUName = "Sliced Banana", .Quantity = 40})
        outputs.Add(New DailyOutput() With {
            .OutputDate = New Date(2025, 1, 15), .DayTypeName = "Regular Day",
            .SKUName = "Sliced Apple", .Quantity = 50})
        outputs.Add(New DailyOutput() With {
            .OutputDate = New Date(2025, 1, 16), .DayTypeName = "Regular Day",
            .SKUName = "Sliced Banana", .Quantity = 90})
        outputs.Add(New DailyOutput() With {
            .OutputDate = New Date(2025, 1, 17), .DayTypeName = "Regular Day",
            .SKUName = "Sliced Banana", .Quantity = 100})
        outputs.Add(New DailyOutput() With {
            .OutputDate = New Date(2025, 1, 18), .DayTypeName = "Regular Day",
            .SKUName = "Sliced Banana", .Quantity = 130})

        ' --- Generate Billing ---
        Console.WriteLine("--- Billing Computation (Exam Sample Data) ---")
        Console.WriteLine()
        Dim billing = GenerateBilling(outputs, skuList, dayTypeList)
        DisplayBilling(billing)
        Console.WriteLine()

        ' --- Collection: Full Payment ---
        Console.WriteLine("--- Collection Case 1: Full Payment ---")
        Dim fullCollection = ProcessCollection(billing.BillableAmount, billing.BillableAmount)
        DisplayCollection(fullCollection)
        Console.WriteLine()

        ' --- Collection: Partial Payment ---
        Console.WriteLine("--- Collection Case 2: Partial Payment (4,000 of 6,700) ---")
        Dim partialCollection = ProcessCollection(billing.BillableAmount, 4000D)
        DisplayCollection(partialCollection)
        Console.WriteLine()

        ' --- Collection: No Payment ---
        Console.WriteLine("--- Collection Case 3: No Payment ---")
        Dim noCollection = ProcessCollection(billing.BillableAmount, 0D)
        DisplayCollection(noCollection)
        Console.WriteLine()

        ' --- Edge Case: Unknown SKU ---
        Console.WriteLine("--- Edge Case: Unknown SKU ---")
        Try
            Dim badOutputs As New List(Of DailyOutput)
            badOutputs.Add(New DailyOutput() With {
                .OutputDate = New Date(2025, 1, 20), .DayTypeName = "Regular Day",
                .SKUName = "Sliced Mango", .Quantity = 100})
            Dim badBilling = GenerateBilling(badOutputs, skuList, dayTypeList)
        Catch ex As ArgumentException
            Console.WriteLine($"  Error caught: {ex.Message}")
        End Try
        Console.WriteLine()

        ' --- Edge Case: Zero Output ---
        Console.WriteLine("--- Edge Case: Zero Output ---")
        Dim zeroOutputs As New List(Of DailyOutput)
        zeroOutputs.Add(New DailyOutput() With {
            .OutputDate = New Date(2025, 1, 20), .DayTypeName = "Regular Day",
            .SKUName = "Sliced Banana", .Quantity = 0})
        Dim zeroBilling = GenerateBilling(zeroOutputs, skuList, dayTypeList)
        Console.WriteLine($"  Gross Payout for zero output: {zeroBilling.GrossPayout:N2}")
        Console.WriteLine($"  Billable Amount: {zeroBilling.BillableAmount:N2}")
    End Sub

End Module
