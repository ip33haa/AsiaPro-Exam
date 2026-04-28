Imports System

''' <summary>
''' Part 1 - Item 2: Total Worked Hours Computation
''' Computes the total worked hours of an employee.
''' Requirements:
'''   - Accept TimeIn and TimeOut
'''   - If total hours > 5, deduct 1 hour for lunch
'''   - Validate input (TimeOut must be later than TimeIn)
'''   - Return result rounded to 2 decimal places
''' </summary>
Public Module WorkedHoursModule

    ''' <summary>
    ''' Computes total worked hours with automatic lunch deduction.
    ''' </summary>
    ''' <param name="timeIn">The time the employee clocked in.</param>
    ''' <param name="timeOut">The time the employee clocked out.</param>
    ''' <returns>Total worked hours rounded to 2 decimal places.</returns>
    Public Function ComputeWorkedHours(timeIn As DateTime, timeOut As DateTime) As Double
        ' Validate: TimeOut must be later than TimeIn
        If timeOut <= timeIn Then
            Throw New ArgumentException("TimeOut must be later than TimeIn.")
        End If

        ' Calculate raw hours worked
        Dim totalSpan As TimeSpan = timeOut - timeIn
        Dim totalHours As Double = totalSpan.TotalHours

        ' Deduct 1 hour for lunch if total hours exceed 5
        Dim lunchDeduction As Double = 0
        If totalHours > 5 Then
            lunchDeduction = 1.0
        End If

        Dim netWorkedHours As Double = totalHours - lunchDeduction

        ' Round to 2 decimal places
        Return Math.Round(netWorkedHours, 2)
    End Function

    ''' <summary>
    ''' Demo / Test entry point showing various scenarios.
    ''' </summary>
    Public Sub Main()
        Console.WriteLine("=== Total Worked Hours Computation ===")
        Console.WriteLine()

        ' Case 1: Full day (9 AM - 6 PM) = 9 hours - 1 lunch = 8 hours
        Dim hours1 = ComputeWorkedHours(
            DateTime.Parse("2025-01-15 09:00:00"),
            DateTime.Parse("2025-01-15 18:00:00"))
        Console.WriteLine("Case 1 - Full Day (9:00 AM - 6:00 PM):")
        Console.WriteLine($"  Worked Hours: {hours1}")
        Console.WriteLine()

        ' Case 2: Half day (9 AM - 1 PM) = 4 hours, no lunch deduction (<=5)
        Dim hours2 = ComputeWorkedHours(
            DateTime.Parse("2025-01-15 09:00:00"),
            DateTime.Parse("2025-01-15 13:00:00"))
        Console.WriteLine("Case 2 - Half Day (9:00 AM - 1:00 PM):")
        Console.WriteLine($"  Worked Hours: {hours2}")
        Console.WriteLine()

        ' Case 3: Exactly 5 hours (9 AM - 2 PM) = 5 hours, no lunch deduction
        Dim hours3 = ComputeWorkedHours(
            DateTime.Parse("2025-01-15 09:00:00"),
            DateTime.Parse("2025-01-15 14:00:00"))
        Console.WriteLine("Case 3 - Exactly 5 Hours (9:00 AM - 2:00 PM):")
        Console.WriteLine($"  Worked Hours: {hours3}")
        Console.WriteLine()

        ' Case 4: Just over 5 hours (9 AM - 2:15 PM) = 5.25 - 1 = 4.25
        Dim hours4 = ComputeWorkedHours(
            DateTime.Parse("2025-01-15 09:00:00"),
            DateTime.Parse("2025-01-15 14:15:00"))
        Console.WriteLine("Case 4 - 5 Hours 15 Min (9:00 AM - 2:15 PM):")
        Console.WriteLine($"  Worked Hours: {hours4}")
        Console.WriteLine()

        ' Case 5: Late in, overtime out (10:30 AM - 7:30 PM) = 9 - 1 = 8
        Dim hours5 = ComputeWorkedHours(
            DateTime.Parse("2025-01-15 10:30:00"),
            DateTime.Parse("2025-01-15 19:30:00"))
        Console.WriteLine("Case 5 - Late In + OT (10:30 AM - 7:30 PM):")
        Console.WriteLine($"  Worked Hours: {hours5}")
        Console.WriteLine()

        ' Case 6: Short shift with fractional hours (9 AM - 11:45 AM) = 2.75
        Dim hours6 = ComputeWorkedHours(
            DateTime.Parse("2025-01-15 09:00:00"),
            DateTime.Parse("2025-01-15 11:45:00"))
        Console.WriteLine("Case 6 - Short Shift (9:00 AM - 11:45 AM):")
        Console.WriteLine($"  Worked Hours: {hours6}")
        Console.WriteLine()

        ' Case 7: Invalid input - TimeOut before TimeIn
        Console.WriteLine("Case 7 - Invalid: TimeOut before TimeIn:")
        Try
            Dim hours7 = ComputeWorkedHours(
                DateTime.Parse("2025-01-15 18:00:00"),
                DateTime.Parse("2025-01-15 09:00:00"))
        Catch ex As ArgumentException
            Console.WriteLine($"  Error caught: {ex.Message}")
        End Try
        Console.WriteLine()

        ' Case 8: Invalid input - Same TimeIn and TimeOut
        Console.WriteLine("Case 8 - Invalid: Same TimeIn and TimeOut:")
        Try
            Dim hours8 = ComputeWorkedHours(
                DateTime.Parse("2025-01-15 09:00:00"),
                DateTime.Parse("2025-01-15 09:00:00"))
        Catch ex As ArgumentException
            Console.WriteLine($"  Error caught: {ex.Message}")
        End Try
    End Sub

End Module
