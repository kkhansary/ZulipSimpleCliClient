Public Class Client

    Public Async Function Run() As Task
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

            Dim Command = DirectCast(Attributes(0), CommandAttribute)
            Command.Method = Method

            Attributes = Method.GetCustomAttributes(GetType(ParameterDescriptionAttribute), False)
            Command.ParametersDescriptions = Attributes.Cast(Of ParameterDescriptionAttribute)().ToArray()

            Commands.Add(Method.Name, Command)

            Attributes = Method.GetCustomAttributes(GetType(CommandAliasAttribute), False)
            For Each [Alias] As CommandAliasAttribute In Attributes
                AliasesNames.Add([Alias].Alias, Method.Name)
            Next
            AliasesNames.Add(Method.Name, Method.Name)
        Next

        Do
            Console.Write(Client?.UserName & "> ")
            Dim CommandString = Console.ReadLine().Split({" "c}, StringSplitOptions.RemoveEmptyEntries)
            Dim Command As CommandAttribute = Nothing
            If Not Commands.TryGetValue(CommandString(0), Command) Then
                Console.WriteLine("Command not found.")
                Continue Do
            End If
            Await DirectCast(Command.Method.Invoke(Me, CommandString.Skip(1).ToArray()), Task)
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

    <Command(Description:="")>
    <ParameterDescription("CommandName", "")>
    <CommandAlias("H")>
    Private Sub Help(Optional ByVal CommandName As String = Nothing)
        If CommandName Is Nothing Then
            Console.WriteLine("Commands:")
            Dim PadLength = Commands.Max(Function(Cmd) Cmd.Key.Length) + 3
            For Each Cmd In Commands
                Console.WriteLine(Cmd.Key.PadRight(PadLength) & Cmd.Value.Description)
            Next
        Else
            Dim Command As CommandAttribute = Nothing
            If Not Commands.TryGetValue(CommandName, Command) Then
                Console.WriteLine("Command not found. Write ""Help"" to see commands list.")
                Exit Sub
            End If

            Console.WriteLine(CommandName & ":")
            Dim PadLength = Command.ParametersDescriptions.Max(Function(PD) PD.Description.Length) + 3
            For Each PD In Command.ParametersDescriptions
                Console.WriteLine(PD.Name.PadRight(PadLength) & PD.Description)
            Next
        End If
    End Sub

    Private ReadOnly AliasesNames As Dictionary(Of String, String) = New Dictionary(Of String, String)()
    Private ReadOnly Commands As Dictionary(Of String, CommandAttribute) = New Dictionary(Of String, CommandAttribute)(StringComparer.InvariantCultureIgnoreCase)

    Private Client As Zulip.Client
    Private Address As String

End Class
