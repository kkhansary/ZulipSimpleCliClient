Public Class Client

    Public Async Function Run() As Task
        Dim AliasesNames = New Dictionary(Of String, String)
        Dim Commands = New Dictionary(Of String, (Reflection.MethodInfo, String, List(Of String)))

        For Each Method In Me.GetType().GetMethods()
            Dim Attributes = Method.GetCustomAttributes(GetType(CommandAttribute), False)
            If Attributes.Length = 0 Then
                Continue For
            End If

            If (Method.Attributes And Reflection.MethodAttributes.Static) = Reflection.MethodAttributes.Static Then
                Throw New Exception("A command attributed method must be shared.")
            End If
            If Method.ReturnType <> GetType(Task) Then
                Throw New Exception("A command attributed method must return a task.")
            End If

            Dim CommandDescription = DirectCast(Attributes(0), CommandAttribute).Description
            Dim ParametersDescription = New List(Of String)
            Attributes = Method.GetCustomAttributes(GetType(ParameterDescriptionAttribute), False)
            For Each ParameterDescription As ParameterDescriptionAttribute In Attributes
                ParametersDescription.Add(ParameterDescription.Description)
            Next

            Commands.Add(Method.Name, (Method, CommandDescription, ParametersDescription))

            Attributes = Method.GetCustomAttributes(GetType(CommandAliasAttribute), False)
            For Each [Alias] As CommandAliasAttribute In Attributes
                AliasesNames.Add([Alias].Alias, Method.Name)
            Next
            AliasesNames.Add(Method.Name, Method.Name)
        Next

        Do
            Console.Write(Client?.UserName & "> ")
            Dim CommandString = Console.ReadLine().Split({" "c}, StringSplitOptions.RemoveEmptyEntries)
            Dim Command As (Reflection.MethodInfo, String, List(Of String)) = Nothing
            If Not Commands.TryGetValue(CommandString(0), Command) Then
                Console.WriteLine("Command not found.")
            End If
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
