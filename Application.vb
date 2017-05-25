Public Class Application

    Private Sub New()
        Throw New NotSupportedException()
    End Sub

    Public Shared Sub Main()
        Run()
        WaitHandle.WaitOne()
    End Sub

    Private Shared Async Sub Run()
        Dim Client = New Client()
        Await Client.Run()
        WaitHandle.Set()
    End Sub

    Private Shared ReadOnly WaitHandle As Threading.EventWaitHandle = New Threading.EventWaitHandle(False, Threading.EventResetMode.ManualReset)

End Class
