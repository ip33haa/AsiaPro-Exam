Imports System

''' <summary>
''' Part 1 - Item 1: Late and Undertime Detection
''' Determines if an employee is late or has undertime based on:
'''   - Start time: 9:00 AM
'''   - End time: 6:00 PM
''' Returns late minutes and undertime minutes.
''' </summary>
Public Module LateUndertimeModule

    Public Structure AttendanceResult
        Public LateMinutes As Integer
        Public UndertimeMinutes As Integer
        Public IsLate As Boolean
        Public HasUndertime As Boolean
    End Structure

    ''' <summary>
    ''' Determines if an employee is late and/or has undertime.
    ''' </summary>
    ''' <param name="timeIn">The time the employee clocked in.</param>
    ''' <param name="timeOut">The time the employee clocked out.</param>
    ''' <returns>An AttendanceResult with late and undertime details.</returns>
    Public Function CheckLateAndUndertime(timeIn As DateTime, timeOut As DateTime) As AttendanceResult
        Dim result As New AttendanceResult()

        ' Define standard schedule
        Dim scheduledStart As New TimeSpan(9, 0, 0)   ' 9:00 AM
        Dim scheduledEnd As New TimeSpan(18, 0, 0)     ' 6:00 PM

        ' Validate inputs
        If timeOut <= timeIn Then
            Throw New ArgumentException("TimeOut must be later than TimeIn.")
        End If

        ' --- Late Calculation ---
        ' Employee is late if they clock in after 9:00 AM
        Dim actualIn As TimeSpan = timeIn.TimeOfDay
        If actualIn > scheduledStart Then
            Dim lateSpan As TimeSpan = actualIn - scheduledStart
            result.LateMinutes = CInt(Math.Floor(lateSpan.TotalMinutes))
            result.IsLate = True
        Else
            result.LateMinutes = 0
            result.IsLate = False
        End If

        ' --- Undertime Calculation ---
        ' Employee has undertime if they clock out before 6:00 PM
        Dim actualOut As TimeSpan = timeOut.TimeOfDay
        If actualOut < scheduledEnd Then
            Dim undertimeSpan As TimeSpan = scheduledEnd - actualOut
            result.UndertimeMinutes = CInt(Math.Floor(undertimeSpan.TotalMinutes))
            result.HasUndertime = True
        Else
            result.UndertimeMinutes = 0
            result.HasUndertime = False
        End If

        Return result
    End Function

    ''' <summary>
    ''' Demo / Test entry point showing various edge cases.
    ''' </summary>
    Public Sub Main()
        Console.WriteLine("=== Late and Undertime Detection ===")
        Console.WriteLine()

        ' Case 1: On time, full day
        Dim res1 = CheckLateAndUndertime(
            DateTime.Parse("2025-01-15 08:55:00"),
            DateTime.Parse("2025-01-15 18:05:00"))
        Console.WriteLine("Case 1 - Early In (8:55 AM), Late Out (6:05 PM):")
        Console.WriteLine($"  Late: {res1.IsLate} ({res1.LateMinutes} mins)")
        Console.WriteLine($"  Undertime: {res1.HasUndertime} ({res1.UndertimeMinutes} mins)")
        Console.WriteLine()

        ' Case 2: Late in, on-time out
        Dim res2 = CheckLateAndUndertime(
            DateTime.Parse("2025-01-15 09:30:00"),
            DateTime.Parse("2025-01-15 18:00:00"))
        Console.WriteLine("Case 2 - Late In (9:30 AM), On-Time Out (6:00 PM):")
        Console.WriteLine($"  Late: {res2.IsLate} ({res2.LateMinutes} mins)")
        Console.WriteLine($"  Undertime: {res2.HasUndertime} ({res2.UndertimeMinutes} mins)")
        Console.WriteLine()

        ' Case 3: On time in, early out (undertime)
        Dim res3 = CheckLateAndUndertime(
            DateTime.Parse("2025-01-15 09:00:00"),
            DateTime.Parse("2025-01-15 16:30:00"))
        Console.WriteLine("Case 3 - On-Time In (9:00 AM), Early Out (4:30 PM):")
        Console.WriteLine($"  Late: {res3.IsLate} ({res3.LateMinutes} mins)")
        Console.WriteLine($"  Undertime: {res3.HasUndertime} ({res3.UndertimeMinutes} mins)")
        Console.WriteLine()

        ' Case 4: Late in and early out (both)
        Dim res4 = CheckLateAndUndertime(
            DateTime.Parse("2025-01-15 10:15:00"),
            DateTime.Parse("2025-01-15 17:00:00"))
        Console.WriteLine("Case 4 - Late In (10:15 AM), Early Out (5:00 PM):")
        Console.WriteLine($"  Late: {res4.IsLate} ({res4.LateMinutes} mins)")
        Console.WriteLine($"  Undertime: {res4.HasUndertime} ({res4.UndertimeMinutes} mins)")
        Console.WriteLine()

        ' Case 5: Exactly on time (edge case)
        Dim res5 = CheckLateAndUndertime(
            DateTime.Parse("2025-01-15 09:00:00"),
            DateTime.Parse("2025-01-15 18:00:00"))
        Console.WriteLine("Case 5 - Exactly On-Time (9:00 AM - 6:00 PM):")
        Console.WriteLine($"  Late: {res5.IsLate} ({res5.LateMinutes} mins)")
        Console.WriteLine($"  Undertime: {res5.HasUndertime} ({res5.UndertimeMinutes} mins)")
        Console.WriteLine()

        ' Case 6: Edge case - TimeOut before TimeIn (should throw error)
        Console.WriteLine("Case 6 - Invalid: TimeOut before TimeIn:")
        Try
            Dim res6 = CheckLateAndUndertime(
                DateTime.Parse("2025-01-15 18:00:00"),
                DateTime.Parse("2025-01-15 09:00:00"))
        Catch ex As ArgumentException
            Console.WriteLine($"  Error caught: {ex.Message}")
        End Try
    End Sub

End Module
