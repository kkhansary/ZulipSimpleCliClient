Public Module Application

    Public Sub Main()
        Run()
        WaitHandle.WaitOne()
    End Sub

    Private Async Sub Run()
        Do
            Console.Write("-> ")
            Dim Command = Console.ReadLine()

            Select Case Command
                Case "Help"
                    Help()
                Case "Quit"
                    Exit Do
                Case Else
                    Console.WriteLine("Invalid command. Write Help for see commands.")
            End Select

            Console.WriteLine()
        Loop

        WaitHandle.Set()
    End Sub

    Private Sub Help()
        Console.WriteLine("Commands: ")
        Console.WriteLine("Quit")
    End Sub

    Private ReadOnly WaitHandle As Threading.EventWaitHandle = New Threading.EventWaitHandle(False, Threading.EventResetMode.ManualReset)
End Module
