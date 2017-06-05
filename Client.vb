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

    <Command(Description:="Set server address.")>
    <ParameterDescription("Address", "Server address")>
    <CommandAlias("SetAddress")>
    <CommandAlias("Address")>
    <CommandAlias("Addr")>
    <CommandAlias("A")>
    Private Function SetServerAddress(Address As String) As Task
        Me.Address = Address
        Return Task.FromResult(Of Object)(Nothing)
    End Function

    Private Sub Help()
        Console.WriteLine("Commands: ")
        Console.Write("LogIn" & ControlChars.Tab)
        Console.WriteLine("Quit")
    End Sub


    <Command(Description:="Logs in to an account.")>
    <ParameterDescription("UserName", "")>
    <ParameterDescription("Password", "")>
    <CommandAlias("L")>
    Private Async Function LogIn(UserName As String, Password As String) As Task
        Client = New Zulip.Client(Address)
        Try
            Await Client.LoginAsync(Zulip.LoginData.CreateByPassword(UserName, Password))
            Console.WriteLine("Logged in.")
        Catch Ex As Exception
            Console.WriteLine(Ex.Message)
        End Try
    End Function

    Private Client As Zulip.Client
    Private Address As String

End Class
