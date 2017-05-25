Public Class Client

    Public Async Function Run() As Task
        Do
            Console.Write("-> ")
            Dim Command = Console.ReadLine()

            Select Case Command
                Case "Help"
                    Help()
                Case "LogIn"
                    Await LogIn()
                Case "Quit"
                    Exit Do
                Case Else
                    Console.WriteLine("Invalid command. Write Help for see commands.")
            End Select

            Console.WriteLine()
        Loop
    End Function

    Private Sub Help()
        Console.WriteLine("Commands: ")
        Console.Write("LogIn" & ControlChars.Tab)
        Console.WriteLine("Quit")
    End Sub

    Private Async Function LogIn() As Task(Of Boolean)
        Client = New Zulip.Client(Address)

        Console.Write("UserName: ")
        Dim UserName = Console.ReadLine()
        Console.Write("Password: ")
        Dim Password = Console.ReadLine()
        Try
            Await Client.LoginAsync(Zulip.LoginData.CreateByPassword(UserName, Password))
            Console.WriteLine("Loged in.")
            Return True
        Catch ex As Exception
            Console.WriteLine(ex.Message)
            Return False
        End Try
    End Function

    Private Client As Zulip.Client
    Private ReadOnly Address As String = "https://chat.zulip.org/"

End Class
