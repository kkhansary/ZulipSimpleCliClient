Public Class Client

    Public Async Function Run() As Task
        Me.InitializeCommands()
        Await Me.RunInteractiveConsole()
    End Function

    Private Async Function RunInteractiveConsole() As Task
        Do
            Console.Write(Me.Client?.UserName & "> ")
            Dim CommandString = Console.ReadLine().Split({" "c}, StringSplitOptions.RemoveEmptyEntries)

            If CommandString.Length = 0 Then
                Continue Do
            End If

            Dim CommandName As String = Nothing
            If Not Me.AliasesNames.TryGetValue(CommandString(0), CommandName) Then
                Console.WriteLine("Command not found. Write ""Help"" to see commands list.")
                Console.WriteLine()
                Continue Do
            End If

            Dim Command = Me.Commands.Item(CommandName)

            If CommandString.Length - 1 > Command.ParametersDescriptions.Count Then
                Console.WriteLine("Invalid usage.")
                Me.Help(Command.Name, Description.OnlyUsage)
                Console.WriteLine()
                Continue Do
            End If

            Dim Args = New List(Of String)()
            For I = 1 To CommandString.Length - 1
                Args.Add(CommandString(I))
            Next
            For I = CommandString.Length - 1 To Command.ParametersDescriptions.Count - 1
                If Not Command.ParametersDescriptions(I).IsOptional Then
                    Console.WriteLine("Invalid usage.")
                    Me.Help(Command.Name, Description.OnlyUsage)
                    Console.WriteLine()
                    Continue Do
                End If
                Args.Add(Command.ParametersDescriptions(I).DefaultValue)
            Next

            Await DirectCast(Command.Method.Invoke(Me, Args.ToArray()), Task)
            Console.WriteLine()
        Loop
    End Function

    Private Sub InitializeCommands()
        For Each Method In Me.GetType().GetMethods(Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Public Or Reflection.BindingFlags.Instance Or Reflection.BindingFlags.Static)
            Dim Attributes = Method.GetCustomAttributes(GetType(CommandAttribute), False)
            If Attributes.Length = 0 Then
                Continue For
            End If

            If Method.IsStatic Then
                Throw New InvalidOperationException("A method with a command attribute cannot be shared.")
            End If
            If Method.ReturnType <> GetType(Task) Then
                Throw New InvalidOperationException("A method with a command attribute must return a task.")
            End If

            Dim Command = DirectCast(Attributes(0), CommandAttribute)
            Command.Name = Method.Name
            Command.Method = Method

            Dim Parameters = Method.GetParameters()
            Dim ParametersDescriptions = New List(Of ParameterDescriptionAttribute)()
            Dim ParameterAttributes = Method.GetCustomAttributes(GetType(ParameterDescriptionAttribute), False) _
                                            .Cast(Of ParameterDescriptionAttribute)()
            For Each P In Parameters
                Dim Par = ParameterAttributes.First(Function(A) A.Name = P.Name)
                Par.IsOptional = P.IsOptional
                If Par.IsOptional Then
                    Par.DefaultValue = DirectCast(P.DefaultValue, String)
                End If
                ParametersDescriptions.Add(Par)
            Next

            Command.SetParametersDescriptions(ParametersDescriptions)

            Attributes = Method.GetCustomAttributes(GetType(CommandAliasAttribute), False)
            Dim Aliases = New List(Of String)()
            For Each [Alias] As CommandAliasAttribute In Attributes
                Me.AliasesNames.Add([Alias].Alias, Command.Name)
                Aliases.Add([Alias].Alias)
            Next
            Me.AliasesNames.Add(Command.Name, Command.Name)

            Command.SetAliases(Aliases)
            Me.Commands.Add(Command.Name, Command)
        Next
    End Sub

    <Command(Description:="Set server address.")>
    <ParameterDescription("Address", "Server address")>
    <CommandAlias("SetAddress")>
    <CommandAlias("Address")>
    <CommandAlias("Addr")>
    <CommandAlias("A")>
    Private Function SetServerAddress(Address As String) As Task
        Me.Client = Nothing
        Me.Address = Address
        Return Task.FromResult(Of Object)(Nothing)
    End Function

    <Command(Description:="Logs in to an account.")>
    <ParameterDescription("UserName", "")>
    <ParameterDescription("Password", "")>
    <CommandAlias("L")>
    Private Async Function LogIn(UserName As String, Password As String) As Task
        If Me.Address Is Nothing Then
            Console.WriteLine("You should set server address first.")
            Exit Function
        End If

        Me.Client = New Zulip.Client(Me.Address)
        Try
            Await Me.Client.LoginAsync(Zulip.LoginData.CreateByPassword(UserName, Password))
            Console.WriteLine("Logged in.")
        Catch Ex As Exception
            Console.WriteLine(Ex.Message)
        End Try
    End Function

    <Command(Description:="")>
    <ParameterDescription("Command", "")>
    <ParameterDescription("SubCommand1", "")>
    <ParameterDescription("SubCommand2", "")>
    <CommandAlias("U")>
    Private Async Function Users(Command As String, Optional SubCommand1 As String = Nothing, Optional SubCommand2 As String = Nothing) As Task
        If Me.Client Is Nothing Then
            Console.WriteLine("You should log in first.")
            Exit Function
        End If

        Select Case Command.ToLower()
            Case "show"
                If SubCommand2 IsNot Nothing Then
                    Console.WriteLine("Invalid usage. Show has only one sub-command.")
                    Exit Function
                End If

                If SubCommand1 Is Nothing Then
                    Await Me.UsersShow()
                End If
                Await Me.UsersShow(SubCommand1)
            Case "information", "info"
                Await Me.UserInformation(SubCommand1, SubCommand2)
            Case Else
                Console.WriteLine("Invalid parameters.")
                Me.Help("Users", Description.OnlyParam)
        End Select
    End Function

    <Command(Description:="")>
    <ParameterDescription("Type", "")>
    <CommandAlias("US")>
    Private Async Function UsersShow(Optional ByVal Type As String = "All") As Task
        If Me.Client Is Nothing Then
            Console.WriteLine("You should log in first.")
            Exit Function
        End If

        Dim NamePadLength = 20
        Dim EmailPadLength = 30

        Dim ShowBots = False
        Dim ShowAdmins = False
        Dim ShowOtherUsers = False

        Select Case Type.ToLower()
            Case "all"
                ShowBots = True
                ShowAdmins = True
                ShowOtherUsers = True
            Case "bot"
                ShowBots = True
            Case "admin", "admins"
                ShowAdmins = True
            Case "user", "users"
                ShowAdmins = True
                ShowOtherUsers = True
            Case Else
                Console.WriteLine("Invalid parameters.")
                Me.Help("UsersShow", Description.OnlyParam)
                Exit Function
        End Select

        Await Me.Client.Users.RetrieveAsync()

        For I = 1 To Me.Client.Users.Value.Count
            Dim User = Me.Client.Users.Value.ItemAt(I - 1)

            Dim PadLength = CInt(Math.Log10(Me.Client.Users.Value.Count)) + 1
            If User.IsBot Then
                If Not ShowBots Then
                    Continue For
                End If
                Console.Write((CStr(I) & ".").PadLeft(PadLength) & "   Bot     ")
            ElseIf User.IsAdmin Then
                If Not ShowAdmins Then
                    Continue For
                End If
                Console.Write((CStr(I) & ".").PadLeft(PadLength) & "   Admin   ")
            Else
                If Not ShowOtherUsers Then
                    Continue For
                End If
                Console.Write((CStr(I) & ".").PadLeft(PadLength) & "   User    ")
            End If

            Console.Write(User.FullName.PadRight(NamePadLength) & "   " & User.EmailAddress.PadRight(EmailPadLength))

            If User.IsActive Then
                Console.Write("Active     ")
            Else
                Console.Write("Inactive   ")
            End If
            If User.IsFrozen Then
                Console.Write("Frozen")
            End If
            Console.WriteLine()
        Next
    End Function

    <Command(Description:="")>
    <ParameterDescription("By", "")>
    <ParameterDescription("Key", "")>
    <CommandAlias("UserInfo")>
    <CommandAlias("UI")>
    Private Async Function UserInformation(ByVal By As String, ByVal Key As String) As Task
        If Me.Client Is Nothing Then
            Console.WriteLine("You should log in first.")
            Exit Function
        End If

        If By Is Nothing Or Key Is Nothing Then
            Console.WriteLine("Invalid usage.")
            Me.Help("UserInformation", Description.OnlyUsage)
            Exit Function
        End If

        Await Me.Client.Users.RetrieveAsync()

        Dim User As Zulip.User

        Select Case By.ToLower()
            Case "byindex", "index", "i"
                Dim Index = CInt(Key)
                If Index > Me.Client.Users.Value.Count Or Index < 1 Then
                    Console.WriteLine("User not found.")
                    Exit Function
                End If
                User = Me.Client.Users.Value.ItemAt(Index - 1)
            Case "byemail", "email", "e"
                If Not Me.Client.Users.Value.TryGetValueByEmail(Key, User) Then
                    Console.WriteLine("User not found.")
                    Exit Function
                End If
            Case "byfullname", "byname", "fullname", "name", "n"
                If Not Me.Client.Users.Value.TryGetValueByFullName(Key, User) Then
                    Console.WriteLine("User not found.")
                    Exit Function
                End If
            Case Else
                Console.WriteLine("Invalid parameters.")
                Me.Help("UserInformation", Description.OnlyParam)
        End Select

        Dim PadLength = 24
        Console.WriteLine("Full Name:".PadRight(PadLength) & "   " & User.FullName)
        If User.IsBot Then
            Console.WriteLine("Bot Owner Email Address:".PadRight(PadLength) & "   " & User.BotOwnerEmailAddress)
            Console.Write("Type:".PadRight(PadLength) & "   Bot     ")
        Else
            Console.WriteLine("Email Address:".PadRight(PadLength) & "   " & User.EmailAddress)
            If User.IsAdmin Then
                Console.Write("Type:".PadRight(PadLength) & "   Admin   ")
            Else
                Console.Write("Type:".PadRight(PadLength) & "   User    ")
            End If
        End If

        If User.IsActive Then
            Console.Write("Active   ")
        Else
            Console.Write("Inactive   ")
        End If
        If User.IsFrozen Then
            Console.Write("Frozen")
        End If
        Console.WriteLine()
        Console.WriteLine("Avatar Url".PadRight(PadLength) & "   " & User.AvatarUrl)
    End Function

    <Command(Description:="")>
    <ParameterDescription("CommandName", "")>
    <CommandAlias("H")>
    Private Function Help(Optional ByVal CommandName As String = Nothing) As Task
        If CommandName Is Nothing Then
            Console.WriteLine("Commands:")
            Dim PadLength = Me.Commands.Max(Function(Cmd) Cmd.Key.Length)
            For Each Cmd In Me.Commands
                Console.WriteLine("   " & Cmd.Key.PadRight(PadLength) & "   " & Cmd.Value.Description)
            Next
        Else
            If Not Me.AliasesNames.TryGetValue(CommandName, CommandName) Then
                Console.WriteLine("Command not found. Write ""Help"" to see commands list.")

                Return Task.FromResult(Of Object)(Nothing)
            End If

            Me.Help(CommandName, Description.Full)
        End If

        Return Task.FromResult(Of Object)(Nothing)
    End Function

    Enum Description

        Full
        OnlyUsage
        OnlyParam

    End Enum

    Private Sub Help(ByVal CommandName As String, ByVal Mode As Description)
        Dim Command = Me.Commands.Item(CommandName)

        If Mode = Description.Full Or Mode = Description.OnlyUsage Then
            Console.WriteLine("Usage:")
            Console.Write("   " & Command.Name)
            For Each PD In Command.ParametersDescriptions
                If PD.IsOptional Then
                    Console.Write($" [<{PD.Name}>]")
                Else
                    Console.Write($" <{PD.Name}>")
                End If
            Next
            Console.WriteLine()
        End If

        If Mode = Description.Full Then
            Console.WriteLine("Aliases:")
            Console.Write("  ")
            For Each [Alias] In Command.Aliases
                Console.Write(" " & [Alias])
            Next
            Console.WriteLine()
            Console.WriteLine()
        End If

        If Mode = Description.Full Or Mode = Description.OnlyParam Then
            Console.WriteLine("Parameters:")
            Dim PadLength = Command.ParametersDescriptions.Max(Function(PD) PD.Description.Length)
            For Each PD In Command.ParametersDescriptions
                Console.WriteLine("   " & PD.Name.PadRight(PadLength) & "   " & PD.Description)
            Next
        End If
    End Sub

    Private ReadOnly AliasesNames As Dictionary(Of String, String) = New Dictionary(Of String, String)(StringComparer.InvariantCultureIgnoreCase)
    Private ReadOnly Commands As Dictionary(Of String, CommandAttribute) = New Dictionary(Of String, CommandAttribute)()

    Private Client As Zulip.Client
    Private Address As String

End Class
